namespace GP3.Scraper
{
    public struct PageData
    {
        public string[] tableHead;
        public string[] tableBody;
    }

    public interface IPricePage
    {
        Task<PageData> GetPageDataAsync(DateTime date);
    }
}