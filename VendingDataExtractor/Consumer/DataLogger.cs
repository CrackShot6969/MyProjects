using VendingDataExtractor.Interfaces;
using VendingDataExtractor.Structures;

namespace VendingDataExtractor.Consumer
{
    public class DataLogger
    {
        private readonly IDataLogger _logger;

        public DataLogger(IDataLogger logger)
        {
            this._logger = logger;
        }

        public Result LogData( object dataToLog, string dateOfEntry, ILogger<string> errorLogger)
        {
            return this._logger.LogData(dataToLog, dateOfEntry, errorLogger);
        }
    }
}
