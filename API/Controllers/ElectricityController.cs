using API.Data;
using Common;
using Microsoft.AspNetCore.Mvc;

namespace trackingapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElectricityController : ControllerBase
    {
        private readonly DayPricesDbContext _context;
        private readonly IFetcher _fetcher;
        public ElectricityController(DayPricesDbContext context, IFetcher fetcher) { _context = context; _fetcher = fetcher; }

        [HttpGet("{date}")] /* Get unit by it's date */
        [ProducesResponseType(typeof(DayPrices), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById(String date)
        {
            if (!DateTime.TryParse(date, out DateTime dateVal))
            {
                return BadRequest("Date must be supplied in this format: yyyy-MM-dd");
            }

            var key = dateVal.Date.DaysSinceUnixEpoch();
            var prices = await _fetcher.GetDayPricesAsync(dateVal);
            /* For now DB things aren't fully setup, so just skip this setup
            var prices = await _context.DayPrices.FindAsync(key);
            if (prices != null)
            {
                await _context.DayPrices.AddAsync(prices);
            }
            */

            return prices == null ? NotFound() : Ok(prices);
        }
    }
}
