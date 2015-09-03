using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using VendingDataExtractor.Interfaces;
using VendingDataExtractor.Misc;
using VendingDataExtractor.Structures;

namespace VendingDataExtractor.Subscribers
{
    public class FileDataLogger : IDataLogger
    {
        private readonly string _filePath;

        public FileDataLogger(string filePath)
        {
            this._filePath = filePath;
        }

        public Result LogData(object dataToLog, string DateOfEntry,  ILogger<string> errorLogger)
        {
            var returnObject = new Result();
            
            try
            {
                if (Directory.Exists(this._filePath) && dataToLog is List<SoldGenericProduct>)
                {
                    var targetFileName = "DataLog_" + DateOfEntry + ".txt";
                    var existingLogObjects = GetCurrentDayExistingLogData(Utilities.PathAddBackslash(this._filePath) + targetFileName, errorLogger);
                    dataToLog = StripExistingEntries((List<SoldGenericProduct>) dataToLog, existingLogObjects);
                    existingLogObjects.AddRange((List<SoldGenericProduct>)dataToLog);
                    existingLogObjects = ProcessUnorderedData(existingLogObjects);
                    File.WriteAllText(Utilities.PathAddBackslash(this._filePath) + targetFileName, Utilities.XMLSerialize(existingLogObjects));
                    returnObject.SetStatus(new Status("", true));
                }
            }
            catch (Exception e)
            {
                returnObject.SetStatus(new Status(SolutionConstants.CONST_ERROR_ERRORWHENDATALOGGING + e, false));
                errorLogger.LogEntry(SolutionConstants.CONST_ERROR_ERRORWHENDATALOGGING + e);
            }
            return returnObject;
        }

        private static List<SoldGenericProduct> StripExistingEntries(IEnumerable<SoldGenericProduct> objectsToStrip, List<SoldGenericProduct> compareSource )
        {
            return objectsToStrip.Where(entry => !compareSource.Exists(source => source.Equals(entry))).ToList();
        }



        private static List<SoldGenericProduct> ProcessUnorderedData(IEnumerable<SoldGenericProduct> listToProcess)
        {
            return listToProcess.GroupBy(x => x.ProductCode).Select(g => new SoldGenericProduct{ProductCode = g.Key, 
                                                                                                QuantitySold = g.Sum(x => Convert.ToInt16(x.QuantitySold)).ToString(CultureInfo.InvariantCulture),
                                                                                                Cost = g.First(x => true).Cost,
                                                                                                DateSold = g.First(x => true).DateSold,
                                                                                                Description = g.First(x => true).Description,
                                                                                                Price = g.First(x => true).Price}).ToList();
        }

        private static List<SoldGenericProduct> GetCurrentDayExistingLogData(string filePath, ILogger<string> errorLogger)
        {
            var theseObjects = new List<SoldGenericProduct>();
            if (File.Exists(filePath))
            {
                theseObjects = (List<SoldGenericProduct>)Utilities.DeserializeObject(theseObjects.GetType(), filePath, errorLogger);
            }
            return theseObjects;
        }


    }
}