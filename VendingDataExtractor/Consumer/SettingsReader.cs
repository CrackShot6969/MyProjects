using VendingDataExtractor.Interfaces;
using VendingDataExtractor.Subscribers;

namespace VendingDataExtractor.Consumer
{
    public class SettingsReader
    {
        private readonly ISettingsReader _reader;

        public SettingsReader(ISettingsReader iReader)
        {
            this._reader = iReader;
        }

        public string GetFileLocationProperty()
        {
            return this._reader.GetFileLocation();
        }

        public string GetProtocolConnectionProperty()
        {
            return this._reader.GetProtocolConnectionString();
        }

        public string GetContactProperty()
        {
            return this._reader.GetContact();
        }

        public string GetContactMessageProperty()
        {
            return this._reader.GetContactMessage();
        }

        public string GetAccountUsernameProperty()
        {
            return this._reader.GetAccountPassword();
        }

        public string GetAccountPasswordProperty()
        {
            return this._reader.GetAccountPassword();
        }

        public string GetProtocolUsernameProperty()
        {
            return this._reader.GetProtocolUsername();
        }

        public string GetProtocolPasswordProperty()
        {
            return this._reader.GetProtocolPassword();
        }

        public int GetProtocolPortProperty()
        {
            return this._reader.GetProtocolPort();
        }

        public string GetSaveDirectoryProperty()
        {
            return this._reader.GetSaveDirectory();
        }

        public string GetGetFilePrefixProperty()
        {
            return this._reader.GetGetFilePrefix();
        }

        public string GetGetLogFileDirectoryProperty()
        {
            return this._reader.GetLogFileDirectory();
        }

        public string GetProtocolGetDirectoryProperty()
        {
            return this._reader.GetProtocolGetDirectory();
        }

        public string GetCompleteDirectoryProperty()
        {
            return this._reader.GetCompleteDirectory();
        }

        public string GetDataDestinationEndpointProperty()
        {
            return this._reader.GetDataDestinationEndpoint();
        }

        public string GetTaxRateProperty()
        {
            return this._reader.GetTaxRate();
        }

        public string GetMappingsFileLocationProperty()
        {
            return this._reader.GetMappingsFileLocation();
        }

        public string GetCustomerCodeProperty()
        {
            return this._reader.GetMappingsFileLocation();
        }

        public string GetOrderIterationProperty()
        {
            return this._reader.GetOrderIteration();
        }

        public string GetOrderIterationFileLocationProperty()
        {
            return this._reader.GetOrderIterationFileLocation();
        }

        public bool DoOrderIterationAction()
        {
            return this._reader.DoOrderIteration();
        }

        public string GetApiIDProperty()
        {
            return this._reader.GetApiId();
        }

        public string GetApiKeyProperty()
        {
            return this._reader.GetApiKey();
        }

        public string GetProtocolCompletedDirectoryProperty()
        {
            return this._reader.GetProtocolCompletedDirectory();
        }

        public string GetProtocolErrorDirectoryProperty()
        {
            return this._reader.GetProtocolErrorDirectory();
        }

        public SessionSettings GetSessionSettings()
        {
            return this._reader.GetDataRetreivalSettings();
        }
    }
}
