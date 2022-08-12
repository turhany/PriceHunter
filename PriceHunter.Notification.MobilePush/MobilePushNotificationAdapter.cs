namespace PriceHunter.Notification.MobilePush
{
    public class MobilePushNotificationAdapter : INotificationAdapter
    {
        public void Notify(string message)
        {
            Console.WriteLine($"{nameof(MobilePushNotificationAdapter)} : {message}");
        }
    }
}
