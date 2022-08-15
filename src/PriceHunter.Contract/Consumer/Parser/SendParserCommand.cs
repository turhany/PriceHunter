namespace PriceHunter.Contract.Consumer.Parser
{
    public class SendParserCommand
    {
        public Guid ProductId { get; set; }
        public Guid SupplierId { get; set; }
        public string Url { get; set; }
        public int EnumMapping { get; set; }
        public DateTime RequestTime { get; set; }
    }
}
