using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Text;
using VendingDataExtractor.Consumer;
using VendingDataExtractor.Interfaces;
using VendingDataExtractor.Misc;
using VendingDataExtractor.Structures;
using VendingDataExtractor.Subscribers;

namespace VendingDataExtractor
{
    class Program
    {
        static void Main()
        {

            var appSettingsReader   = new AppConfigSettingsReader();
            var logger              = new FileLogger(appSettingsReader);
            var logManager          = new LogManager(logger);
            var settingsReader      = new SettingsReader(appSettingsReader);
            var failedFileList      = new Dictionary<string, string>();
            var accountAuth         = new Auth(settingsReader.GetApiIDProperty(), settingsReader.GetApiKeyProperty());
            var unleashedSender     = new UnleashedDataSender(accountAuth, appSettingsReader, logger);
            var sender              = new DataSender(unleashedSender);
            var csvParser           = new CsvParser(logger);
            var parserObject        = new Parser(csvParser);
            var dataKeeper          = new DataLogger(new FileDataLogger(settingsReader.GetGetLogFileDirectoryProperty()));
            var vendingMapper       = new VendingProductMapper(appSettingsReader.GetMappingsFileLocation());
            var mappingManager      = new ProductMappingManager(vendingMapper);
            var sessionSettings     = settingsReader.GetSessionSettings();
            string ifFailMessage;
            var processedFileCount  = 0;
            Console.WriteLine(SolutionConstants.CONST_STATUS_STARTINGSESSION);
            // We need validate that we have all the correct settings first
            if (!sessionSettings.IsValidSession())
            {
                StartExiting(appSettingsReader, logger, SolutionConstants.CONST_ERROR_NOTENOUGHINFO);
            }
            
            // We need to now get the file to process.
            var ftpClient = new FtpFileRetreiver(settingsReader.GetProtocolConnectionProperty(), settingsReader.GetProtocolPortProperty().ToString(CultureInfo.InvariantCulture), settingsReader.GetProtocolGetDirectoryProperty(), settingsReader.GetProtocolUsernameProperty(), Encoding.UTF8.GetString(Convert.FromBase64String(settingsReader.GetProtocolPasswordProperty())), settingsReader.GetGetFilePrefixProperty());
            var retreiver = new FileRetreiver(ftpClient);

            Console.WriteLine(SolutionConstants.CONST_STATUS_RETREIVINGFILES);

            // Base Class for all return objects
            Result resultBaseClass;

            if (!((resultBaseClass = retreiver.GetFileList(logger)).Status.Success))
            {
                StartExiting(appSettingsReader, logger, SolutionConstants.CONST_ERROR_NOFILESFOUND);
            }

            Console.WriteLine(SolutionConstants.CONST_STATUS_FILESTOPROCESS , ((FtpFileListResult)resultBaseClass).Items.Count);

            foreach (var file in ((FtpFileListResult)resultBaseClass).Items.Where(retreiver.IsTargetFile))
            {
                ifFailMessage = String.Format(SolutionConstants.CONST_ERROR_FILELISTEMPTY, file);
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine(SolutionConstants.CONST_STATUS_PROCESSING, file);
                resultBaseClass = retreiver.GetFileData(file, logger);
                if (!String.IsNullOrEmpty(((FtpFileResult) resultBaseClass).File.Trim()))
                {
                    Console.WriteLine(SolutionConstants.CONST_STATUS_SAVING);
                    if (!String.IsNullOrEmpty(settingsReader.GetSaveDirectoryProperty()))
                    {
                        SaveToFile(appSettingsReader, file, ((FtpFileResult) resultBaseClass).File);
                    }
                    ifFailMessage = SolutionConstants.CONST_ERROR_MAPPING;
                    Console.WriteLine(SolutionConstants.CONST_STATUS_PARSING);
                    if ((resultBaseClass = parserObject.ParseText(((FtpFileResult) resultBaseClass).File)).Status.Success)
                    {
                        ifFailMessage = SolutionConstants.CONST_ERROR_MAPPING;
                        Console.WriteLine(SolutionConstants.CONST_STATUS_MAPPING);
                        if ((resultBaseClass =(ProductsSoldResult)mappingManager.GetProductMappings(((VendingDataResult) resultBaseClass).Items,logger)).Status.Success)
                        {
                            var storedData = Utilities.DeepClone((ProductsSoldResult) resultBaseClass);
                            ifFailMessage = SolutionConstants.CONST_ERROR_SENDING;
                            Console.WriteLine(SolutionConstants.CONST_STATUS_SENDING);
                            if ((resultBaseClass = sender.DoSendData(((ProductsSoldResult)resultBaseClass).Items)).Status.Success)
                            {
                                Console.WriteLine(SolutionConstants.CONST_SUCCESS_SENDING);
                                if (!(dataKeeper.LogData(storedData.Items, GetVendingFileDate(file), logger)).Status.Success)
                                {
                                    logManager.DoLogEntry(String.Format(SolutionConstants.CONST_ERROR_ERRORWHENDATALOGGING, file));
                                }

                                if (!retreiver.MoveFile(file,settingsReader.GetProtocolCompletedDirectoryProperty() + file, logger))
                                {
                                    logManager.DoLogEntry(SolutionConstants.CONST_ERROR_REMOTEMOVE);
                                    Console.WriteLine(SolutionConstants.CONST_ERROR_REMOTEMOVE);
                                }

                                processedFileCount++;
                            }
                        }
                        
                    }
                }

                LogFailedFile(file, ifFailMessage, logManager, logger, ref failedFileList, resultBaseClass, retreiver, settingsReader.GetProtocolErrorDirectoryProperty());

            }

            if (processedFileCount == 0)
                StartExiting(appSettingsReader, logger, SolutionConstants.CONST_ERROR_NOFILESFOUND);

            if (failedFileList.Count <= 0) return;

            ConstructFileListToReport(failedFileList, appSettingsReader, logger);

        }

        static void ConstructFileListToReport(Dictionary<string, string> failedFileList, ISettingsReader appSettingsReader, ILogger<string> logger   )
        {
            var failedList = String.Empty;
            failedFileList.ToList().ForEach(i => failedList += i.Key + ":" + i.Value + Environment.NewLine);
            Notify(appSettingsReader, logger, SolutionConstants.CONST_STATUS_EMAILHEADER + Environment.NewLine + failedList);
        }

        static void Notify(ISettingsReader appSettingsReader, ILogger<string> logger, string message)
        {
            var notifyFailure = new Notifier(new EmailNotifier(appSettingsReader));
            if (!notifyFailure.NotifyRecipient(appSettingsReader, message))
            {
                logger.LogEntry(message);
            }
        }

        static void LogFailedFile(string fileName, string errorIntro, LogManager logManager, ILogger<string> logger, ref Dictionary<string, string> failedFileList, Result currentResult, FileRetreiver retreiver, string errorDirectory)
        {
            if (!currentResult.Status.Success)
            {
                Console.WriteLine(errorIntro + currentResult.Status.Message);
                failedFileList.Add(fileName, currentResult.Status.Message);
                logManager.DoLogEntry(currentResult.Status.Message);
                retreiver.MoveFile(fileName, errorDirectory + fileName, logger);
            }
        }

        static void StartExiting(ISettingsReader appSettingsReader, ILogger<string> logger, string message)
        {
            Notify(appSettingsReader, logger, message);

            Environment.Exit(1);
        }

        static void SaveToFile(ISettingsReader appSettingsReader, string fileName, string fileContents)
        {
            File.WriteAllText(FtpFileRetreiver.IncludePathDelimeter(appSettingsReader.GetSaveDirectory()) + fileName, fileContents);
        }

        static string GetVendingFileDate(string fileName)
        {
            var dateTemp = fileName.Split(new[] { "_" }, StringSplitOptions.None);
            string Result = DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy");
            if (dateTemp.Length >= 2)
            {
                dateTemp[2] = dateTemp[2].Insert(4, "-").Insert(7, "-");
                Result = Convert.ToDateTime(dateTemp[2]).ToString("dd-MM-yyyy");
            }
            return Result;
        }

    }
}
