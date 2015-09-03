using VendingDataExtractor.Interfaces;

namespace VendingDataExtractor.Consumer
{
    public class Notifier 
    {

        INotifier<string> Proxy { get; set; }

        public Notifier(INotifier<string> sender)
        {
            this.Proxy = sender;
        }

        public bool NotifyRecipient(ISettingsReader reader, string message)
        {
            return this.Proxy.Notify(message);
        }
    }
}
