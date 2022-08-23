namespace PriceHunter.Contract.App.User
{
    [Serializable]
    public class ProfileFileContract
    {
        public string FileName { get; set; }
        public byte[] FileData { get; set; }
    }
}
