namespace PriceHunter.Web.Data.User.Api
{
    [Serializable]
    public class UserProfileImageUpdateApiModel
    {
        public string FileName { get; set; }
        public byte[] FileData { get; set; }
    }
}
