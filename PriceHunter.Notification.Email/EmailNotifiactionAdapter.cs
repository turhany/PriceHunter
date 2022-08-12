namespace PriceHunter.Notification.Email
{
    public class EmailNotifiactionAdapter : INotificationAdapter
    {
        public void Notify(string message)
        {
            Console.WriteLine($"{nameof(EmailNotifiactionAdapter)} : {message}");
        }
    }
}
