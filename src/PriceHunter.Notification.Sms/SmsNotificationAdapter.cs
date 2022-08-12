namespace PriceHunter.Notification.Sms
{
    public class SmsNotificationAdapter : INotificationAdapter
    {
        public void Notify(string message)
        {
            Console.WriteLine($"{nameof(SmsNotificationAdapter)} : {message}");
        }
    }
}
