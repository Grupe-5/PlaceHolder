using PuppeteerSharp;

namespace ScraperLib
{
    public interface IPriceBrowser
    {
        Task<IPage> CreatePageAsync();
    }
}