namespace VendingDataExtractor.Interfaces
{
    public interface IDataSender
    {
        object GetConnection();
        bool OpenConnection(object connectionObject);
        bool SendData(object data);
        void CloseConnection(object connectionObject);
    }
}