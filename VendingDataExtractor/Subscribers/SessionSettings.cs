using System;
using VendingDataExtractor.Interfaces;

namespace VendingDataExtractor.Subscribers
{
    public class SessionSettings : ISessionSettings
    {
        public string ProtocolConnectionString { get; set; }
        public string FileLocation { get; set; }
        public bool UseProtocol { get; set; }

        public SessionSettings(String connectionString, String fileLocation)
        {
            this.ProtocolConnectionString = connectionString;
            this.FileLocation = fileLocation;
        }

        public bool IsValidSession()
        {
            return (this.ProtocolConnectionString != "" && this.FileLocation != "");
        }
    }
}