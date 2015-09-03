using System.Runtime.InteropServices;
using VendingDataExtractor.Structures;

namespace VendingDataExtractor.Interfaces
{
    public interface IDataLogger
    {

        Result LogData(object dataToLog, string DateOfEntry, ILogger<string> errorLogger);

    }
}