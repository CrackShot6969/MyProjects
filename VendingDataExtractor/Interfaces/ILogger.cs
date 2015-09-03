namespace VendingDataExtractor.Interfaces
{
    public interface ILogger<in T>
    {
        bool LogEntry(T message);
    }
}