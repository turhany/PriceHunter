namespace PriceHunter.Web.Data.Pager
{
    public class PagedList<T>
    {
        public List<T> Data { get; set; }
        public Page PageInfo { get; set; }
    }
}