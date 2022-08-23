namespace PriceHunter.Contract.Service.User
{
    public class ProfileFileContractServiceRequest
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public byte[] FileData { get; set; }
    }
}
