namespace PriceHunter.Common.Options
{
    public class RabbitMqOption
    {
        public string RabbitMqUri { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        
        public string ParserQueue { get; set; }
        public string NotificationQueue { get; set; }
    }
}