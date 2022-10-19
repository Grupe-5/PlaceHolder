using API.Data;
using Common;
using Microsoft.AspNetCore.Mvc;

namespace trackingapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DayPricesController : ControllerBase
    {
        private readonly DayPricesDbContext _context;
        public DayPricesController(DayPricesDbContext context) { _context = context; }

        [HttpGet("{date}")]
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
            var prices = await _context.DayPrices.FindAsync(key);
            if (prices != null)
            {
                await _context.DayPrices.AddAsync(prices);
            }

            return prices == null ? NotFound() : Ok(prices);
        }
    }
}
