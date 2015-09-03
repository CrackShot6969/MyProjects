using System;
using System.IO;
using VendingDataExtractor.Interfaces;

namespace VendingDataExtractor.Subscribers
{
    public class FileLogger : ILogger<string>
    {
        public ISettingsReader Reader;

        public FileLogger(ISettingsReader reader)
        {
            this.Reader = reader;
        }

        public bool LogEntry(string message)
        {
            var filePath = Reader.GetLogFileDirectory() + @"\VendingDataExtractorLog-" + DateTime.Now.ToString("dd.MM.yyyy") + ".txt";
            var successfulEntry = File.Exists(filePath) ? AppendText(message, filePath) : CreateNew(message, filePath);
            return successfulEntry;

        }

        private static bool AppendText(string messageText, string filePath)
        {
            bool success;
            try
            {
                using (var sw = File.AppendText(filePath))
                {
                    sw.WriteLine("===================================================");
                    sw.WriteLine(DateTime.Now.ToString("hh:mm:ss"));
                    sw.Write    (messageText);
                    sw.WriteLine();
                    sw.WriteLine("End of Entry");
                    sw.WriteLine("===================================================");
                    sw.WriteLine();
                    success = true;
                }
            }
            catch
            {
                success = false;
            }

            return success;
        }

        private static bool CreateNew(string messageText, string filePath)
        {
            bool success;
            try
            {
                using (StreamWriter sw = File.CreateText(filePath))
                {
                    sw.WriteLine("***************************************************");
                    sw.WriteLine("Log started at " + DateTime.Now.ToString("hh:mm:ss"));
                    sw.WriteLine("***************************************************");
                    sw.WriteLine();
                    success = AppendText(messageText, filePath);
                }
            }
            catch 
            {
                success = false;
            }
            return success;
        }
    }
}