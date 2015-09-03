using VendingDataExtractor.Interfaces;

namespace VendingDataExtractor.Consumer
{
    public class LogManager
    {
        public ILogger<string> Logger;

        public LogManager(ILogger<string> logger)
        {
            this.Logger = logger;
        }

        public bool DoLogEntry(string message)
        {
            return this.Logger.LogEntry(message);
        }
    }
}