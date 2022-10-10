using API.Data;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace trackingapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElectricityController : ControllerBase
    {
        private readonly IssueDbContext _context;       //Allow controller to access the database

        public ElectricityController(IssueDbContext context) => _context = context;



        [HttpGet] //All created units
        public async Task<IEnumerable<Issue>> Get()
            => await _context.Issues.ToListAsync();

        [HttpGet("{id}")] //Get unit by it's id
        [ProducesResponseType(typeof(Issue), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var issue = await _context.Issues.FindAsync(id);
            return issue == null ? NotFound() : Ok(issue);
        }

        [HttpPost] //Create new unit
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(Issue issue)
        {
            await _context.Issues.AddAsync(issue);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = issue.Id }, issue);
        }

        [HttpPut("{id}")] //Modify selected (by id) unit
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, Issue issue)
        {
            if (id != issue.Id) return BadRequest();

            _context.Entry(issue).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")] //Delete selected (by id) unit
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var issueToDelete = await _context.Issues.FindAsync(id);
            if (issueToDelete == null) return NotFound();

            _context.Issues.Remove(issueToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
