using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using VendingDataExtractor.Interfaces;

namespace VendingDataExtractor.Subscribers
{
    public class AppConfigSettingsReader : ISettingsReader
    {

        public string GetProtocolConnectionString()
        {
            return ConfigurationManager.AppSettings["protocolConnectionString"].Trim();
        }

        public string GetFileLocation()
        {
            return ConfigurationManager.AppSettings["csvFileLocation"].Trim();
        }

        public string GetContact()
        {
            return ConfigurationManager.AppSettings["contact"].Trim();
        }

        public string GetContactMessage()
        {
            return ConfigurationManager.AppSettings["contactMessage"].Trim();
        }

        public string GetAccountUsername()
        {
            return ConfigurationManager.AppSettings["accountUsername"].Trim();
        }

        public string GetAccountPassword()
        {
            return ConfigurationManager.AppSettings["accountPassword"].Trim();
        }

        public string GetProtocolUsername()
        {
            return ConfigurationManager.AppSettings["protocolUsername"].Trim();
        }

        public string GetProtocolPassword()
        {
            return ConfigurationManager.AppSettings["protocolPassword"].Trim();
        }

        public int GetProtocolPort()
        {
            return String.IsNullOrEmpty(ConfigurationManager.AppSettings["protocolPort"].Trim()) ? -1 : Convert.ToInt32(ConfigurationManager.AppSettings["protocolPort"].Trim());
        }

        public string GetSaveDirectory()
        {
            return ConfigurationManager.AppSettings["saveDirectory"].Trim();
        }

        public string GetGetFilePrefix()
        {
            return ConfigurationManager.AppSettings["getFilePrefix"].Trim();
        }

        public string GetLogFileDirectory()
        {
            return ConfigurationManager.AppSettings["logFileDirectory"].Trim();
        }

        public string GetProtocolGetDirectory()
        {
            return ConfigurationManager.AppSettings["protocolGetDirectory"].Trim();
        }

        public string GetCompleteDirectory()
        {
            return ConfigurationManager.AppSettings["completeDirectory"].Trim();
        }

        public string GetDataDestinationEndpoint()
        {
            return ConfigurationManager.AppSettings["dataDestinationEnpoint"].Trim();
        }

        public string GetTaxRate()
        {
            return ConfigurationManager.AppSettings["taxRate"].Trim();
        }

        public string GetMappingsFileLocation()
        {
            return ConfigurationManager.AppSettings["mappingsFileLocation"].Trim();
        }

        public string GetCustomerCode()
        {
            return ConfigurationManager.AppSettings["customerCode"].Trim();
        }

        public string GetOrderIterationFileLocation()
        {
            return ConfigurationManager.AppSettings["orderIterationFileLocation"].Trim();
        }

        public string GetOrderIteration()
        {
            var fileData = File.ReadAllText(this.GetOrderIterationFileLocation()).Trim();
            return fileData == "" ? "1" : fileData ;
        }

        public string GetApiId()
        {
            return ConfigurationManager.AppSettings["apiID"].Trim();
        }

        public string GetApiKey()
        {
            return ConfigurationManager.AppSettings["apiKey"].Trim();
        }

        public bool DoOrderIteration()
        {
            bool result;
            try
            {
                var thisIteration = Convert.ToInt32(this.GetOrderIteration());
                thisIteration++;
                File.WriteAllText(this.GetOrderIterationFileLocation(), (thisIteration).ToString(CultureInfo.InvariantCulture));
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        public string GetProtocolCompletedDirectory()
        {
            return ConfigurationManager.AppSettings["protocolCompleteDirectory"].Trim();
        }

        public string GetProtocolErrorDirectory()
        {
            return ConfigurationManager.AppSettings["protocolErrorDirectory"].Trim();
        }

        public SessionSettings GetDataRetreivalSettings()
        {
            return new SessionSettings(this.GetProtocolConnectionString(), this.GetFileLocation());
        }


    }
}