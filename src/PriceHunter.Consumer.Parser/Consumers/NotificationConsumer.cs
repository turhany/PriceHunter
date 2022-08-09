using MassTransit;
using PriceHunter.Contract.Consumer.Parser;

namespace PriceHunter.Consumer.Parser.Consumers
{
    internal class ParserConsumer : IConsumer<SendParserCommand>
    {
        public Task Consume(ConsumeContext<SendParserCommand> context)
        {
            Console.WriteLine($"Parser Consumer - ProductId {context.Message.ProductId} - {DateTime.Now}");

            return Task.CompletedTask;
        }
    }
}
