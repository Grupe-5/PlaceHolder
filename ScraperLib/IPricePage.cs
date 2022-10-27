namespace ScraperLib
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