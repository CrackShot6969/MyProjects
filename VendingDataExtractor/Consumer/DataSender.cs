using VendingDataExtractor.Interfaces;
using VendingDataExtractor.Structures;

namespace VendingDataExtractor.Consumer
{
    public class DataSender
    {
        private readonly IDataSender _proxy;

        public DataSender(IDataSender proxy)
        {
            this._proxy = proxy;
        }

        public Result DoSendData(object dataToSend)
        {
            var success = new Result();
            var connectionObj = _proxy.GetConnection();
            if (connectionObj != null)
            {
                if (_proxy.OpenConnection(connectionObj))
                {
                    if (_proxy.SendData(dataToSend))
                    {
                        success.Status.setStatus("", true);
                        _proxy.CloseConnection(connectionObj);
                    }
                    else
                    {
                        success.Status.setStatus("Unable to send data.", false);
                    }
                }
                else
                {
                    success.Status.setStatus("Unable to connect to destination", false);
                }
            }
            else
            {
                success.Status.setStatus("Unable to retreive connection object", false); 
            }

            return success;
        }

    }
}
