using System;
using System.Collections.Generic;
using VendingDataExtractor.Interfaces;
using VendingDataExtractor.Structures;

namespace VendingDataExtractor.Subscribers
{
    public class UnleashedDataSender : IDataSender
    {
        private object _connectionObj;
        private readonly Auth _auth;
        private readonly ISettingsReader _reader;
        private readonly ILogger<string> _logger;

        public UnleashedDataSender(Auth auth, ISettingsReader reader, ILogger<string> logger)
        {
            this._auth = auth;
            this._reader = reader;
            this._logger = logger;
        }

        public object GetConnection()
        {
            return _reader.GetDataDestinationEndpoint(); 
        }

        public bool OpenConnection(object connectionObject)
        {
            // Seeing as we will be using a WebClient there is no means to "open a connection" persay.
            this._connectionObj = connectionObject;
            return true;
        }

        public bool SendData(object data)
        {
            bool result = false;

            if (this._connectionObj == null) return false;
            var helper = new UnleashedRestHelper(this._connectionObj as String, Method.Get, ApiEndpoint.Customers, this._auth, this._logger);
            var list = data as List<SoldGenericProduct>;
            if (list == null) return false;
            var customer = new SalesOrderCustomer
            {
                CustomerCode = _reader.GetCustomerCode(),
                Guid = "00000000-0000-0000-0000-000000000000"
            };
            bool success = helper.PostSalesOrder(list, new List<SalesOrderCustomer> { customer }, this._reader.GetOrderIteration(), Convert.ToDecimal(this._reader.GetTaxRate()));
            if (success)
            {
                this._reader.DoOrderIteration();
                result = true;
            }
            else
            {
                this._logger.LogEntry("Error posting sales order (sendData())");
            }
            return result;
        }

        public void CloseConnection(object connectionObject)
        {
            
        }
    }
}