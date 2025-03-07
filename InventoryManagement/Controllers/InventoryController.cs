using InventoryManagement.Data;
using InventoryManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace InventoryManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryContext _context;
        private readonly ILogger<InventoryController> _logger;

        // Inject ILogger
        public InventoryController(InventoryContext context, ILogger<InventoryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> GetItems()
        {
            _logger.LogInformation("Fetching all items at {Time}", DateTime.UtcNow);

            try
            {
                var items = await _context.Items.Include(i => i.Category).Include(i => i.Supplier).ToListAsync();
                _logger.LogInformation("Successfully fetched {ItemCount} items at {Time}", items.Count, DateTime.UtcNow);
                return items;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching items at {Time}", DateTime.UtcNow);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the items.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> GetItem(int id)
        {
            _logger.LogInformation("Fetching item with Id: {ItemId} at {Time}", id, DateTime.UtcNow);

            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                _logger.LogWarning("Item with Id: {ItemId} not found at {Time}", id, DateTime.UtcNow);
                return NotFound();
            }

            _logger.LogInformation("Successfully fetched item with Id: {ItemId} at {Time}", id, DateTime.UtcNow);
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<Item>> CreateItem(Item item)
        {
            _logger.LogInformation("Creating new item with CategoryId: {CategoryId}, SupplierId: {SupplierId} at {Time}", item.CategoryId, item.SupplierId, DateTime.UtcNow);

            var category = await _context.Categories.FindAsync(item.CategoryId);
            if (category == null)
            {
                _logger.LogWarning("Invalid CategoryId: {CategoryId} at {Time}", item.CategoryId, DateTime.UtcNow);
                return BadRequest("Invalid CategoryId.");
            }

            var supplier = await _context.Suppliers.FindAsync(item.SupplierId);
            if (supplier == null)
            {
                _logger.LogWarning("Invalid SupplierId: {SupplierId} at {Time}", item.SupplierId, DateTime.UtcNow);
                return BadRequest("Invalid SupplierId.");
            }

            item.Category = category;
            item.Supplier = supplier;

            _context.Items.Add(item);

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully created item with Id: {ItemId} at {Time}", item.Id, DateTime.UtcNow);
                return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while saving the item at {Time}", DateTime.UtcNow);
                return BadRequest($"Error while saving the item: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, Item item)
        {
            _logger.LogInformation("Updating item with Id: {ItemId} at {Time}", id, DateTime.UtcNow);

            var category = await _context.Categories.FindAsync(item.CategoryId);
            if (category == null)
            {
                _logger.LogWarning("Invalid CategoryId: {CategoryId} at {Time}", item.CategoryId, DateTime.UtcNow);
                return BadRequest("Invalid CategoryId");
            }

            var supplier = await _context.Suppliers.FindAsync(item.SupplierId);
            if (supplier == null)
            {
                _logger.LogWarning("Invalid SupplierId: {SupplierId} at {Time}", item.SupplierId, DateTime.UtcNow);
                return BadRequest("Invalid SupplierId");
            }

            item.Category = category;
            item.Supplier = supplier;

            _context.Entry(item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully updated item with Id: {ItemId} at {Time}", id, DateTime.UtcNow);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Items.Any(e => e.Id == id))
                {
                    _logger.LogWarning("Item with Id: {ItemId} not found during update at {Time}", id, DateTime.UtcNow);
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            _logger.LogInformation("Deleting item with Id: {ItemId} at {Time}", id, DateTime.UtcNow);

            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                _logger.LogWarning("Item with Id: {ItemId} not found during delete at {Time}", id, DateTime.UtcNow);
                return NotFound();
            }

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully deleted item with Id: {ItemId} at {Time}", id, DateTime.UtcNow);
            return NoContent();
        }
    }
}
