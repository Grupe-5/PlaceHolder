using PuppeteerSharp;

namespace GP3.Scraper
{
    public interface IPriceBrowser
    {
        Task<IPage> CreatePageAsync();
    }
}