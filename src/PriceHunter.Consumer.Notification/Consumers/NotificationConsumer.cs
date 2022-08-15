using MassTransit;
using PriceHunter.Contract.Consumer.Notification;

namespace PriceHunter.Consumer.Notification.Consumers
{
    public class NotificationConsumer : IConsumer<SendNotificationCommand>
    {
        public Task Consume(ConsumeContext<SendNotificationCommand> context)
        {
            Console.WriteLine($"Notification Consumer - ProductId {context.Message.ProductId} - {DateTime.Now}");

            return Task.CompletedTask;
        }
    }
}
