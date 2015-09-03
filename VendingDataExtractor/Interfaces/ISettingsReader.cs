using VendingDataExtractor.Subscribers;

namespace VendingDataExtractor.Interfaces
{
    public interface ISettingsReader
    {
        string GetFileLocation();
        string GetContact();
        string GetContactMessage();
        string GetAccountUsername();
        string GetAccountPassword();
        string GetProtocolConnectionString();
        string GetProtocolUsername();
        string GetProtocolPassword();
        string GetSaveDirectory();
        string GetGetFilePrefix();
        string GetLogFileDirectory();
        string GetProtocolGetDirectory();
        string GetCompleteDirectory();
        string GetDataDestinationEndpoint();
        string GetTaxRate();
        string GetMappingsFileLocation();
        string GetCustomerCode();
        string GetOrderIterationFileLocation();
        string GetOrderIteration();
        string GetApiId();
        string GetApiKey();
        string GetProtocolCompletedDirectory();
        string GetProtocolErrorDirectory();
        bool DoOrderIteration();
        int GetProtocolPort();

        SessionSettings GetDataRetreivalSettings();
    }
}