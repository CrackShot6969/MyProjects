namespace VendingDataExtractor.Interfaces
{
    public interface INotifier<in T>
    {
        bool Notify(T message);
    }
}