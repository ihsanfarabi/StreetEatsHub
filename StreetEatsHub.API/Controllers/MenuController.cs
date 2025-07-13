using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreetEatsHub.API.DTOs;
using StreetEatsHub.API.Services;
using System.Security.Claims;

namespace StreetEatsHub.API.Controllers
{
    [ApiController]
    [Route("api/vendors/{vendorId}/menu")]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        /// <summary>
        /// Get all menu items for a vendor (public)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetMenu(int vendorId)
        {
            var menuItems = await _menuService.GetMenuAsync(vendorId);
            return Ok(menuItems);
        }

        /// <summary>
        /// Get available menu items only (public)
        /// </summary>
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableMenu(int vendorId)
        {
            var menuItems = await _menuService.GetMenuAsync(vendorId);
            var availableItems = menuItems.Where(m => m.IsAvailable).ToList();
            return Ok(availableItems);
        }

        /// <summary>
        /// Get menu categories (public)
        /// </summary>
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories(int vendorId)
        {
            var categories = await _menuService.GetMenuCategoriesAsync(vendorId);
            return Ok(categories);
        }

        /// <summary>
        /// Get specific menu item (public)
        /// </summary>
        [HttpGet("{menuItemId}")]
        public async Task<IActionResult> GetMenuItem(int vendorId, int menuItemId)
        {
            var menuItem = await _menuService.GetMenuItemAsync(vendorId, menuItemId);

            if (menuItem == null)
            {
                return NotFound(new { message = "Menu item not found" });
            }

            return Ok(menuItem);
        }

        /// <summary>
        /// Create new menu item (requires authentication)
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateMenuItem(int vendorId, [FromBody] CreateMenuItemDto menuItemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var menuItem = await _menuService.CreateMenuItemAsync(vendorId, userId, menuItemDto);

            if (menuItem == null)
            {
                return NotFound(new { message = "Vendor not found or access denied" });
            }

            return CreatedAtAction(nameof(GetMenuItem),
                new { vendorId, menuItemId = menuItem.Id }, menuItem);
        }

        /// <summary>
        /// Update menu item (requires authentication)
        /// </summary>
        [HttpPut("{menuItemId}")]
        [Authorize]
        public async Task<IActionResult> UpdateMenuItem(int vendorId, int menuItemId, [FromBody] UpdateMenuItemDto menuItemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var menuItem = await _menuService.UpdateMenuItemAsync(vendorId, userId, menuItemId, menuItemDto);

            if (menuItem == null)
            {
                return NotFound(new { message = "Menu item not found or access denied" });
            }

            return Ok(menuItem);
        }

        /// <summary>
        /// Delete menu item (requires authentication)
        /// </summary>
        [HttpDelete("{menuItemId}")]
        [Authorize]
        public async Task<IActionResult> DeleteMenuItem(int vendorId, int menuItemId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _menuService.DeleteMenuItemAsync(vendorId, userId, menuItemId);

            if (!success)
            {
                return NotFound(new { message = "Menu item not found or access denied" });
            }

            return NoContent();
        }

        /// <summary>
        /// Toggle menu item availability (requires authentication)
        /// </summary>
        [HttpPatch("{menuItemId}/availability")]
        [Authorize]
        public async Task<IActionResult> ToggleAvailability(int vendorId, int menuItemId, [FromBody] ToggleAvailabilityDto toggleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _menuService.ToggleMenuItemAvailabilityAsync(vendorId, userId, menuItemId, toggleDto.IsAvailable);

            if (!success)
            {
                return NotFound(new { message = "Menu item not found or access denied" });
            }

            return Ok(new { message = "Availability updated successfully", isAvailable = toggleDto.IsAvailable });
        }

        /// <summary>
        /// Batch toggle menu items availability (requires authentication)
        /// </summary>
        [HttpPatch("batch/availability")]
        [Authorize]
        public async Task<IActionResult> BatchToggleAvailability(int vendorId, [FromBody] BatchMenuOperationDto batchDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _menuService.BatchToggleAvailabilityAsync(vendorId, userId, batchDto.MenuItemIds, batchDto.IsAvailable);

            if (!success)
            {
                return NotFound(new { message = "Vendor not found or access denied" });
            }

            return Ok(new
            {
                message = "Batch availability update successful",
                updatedItemsCount = batchDto.MenuItemIds.Count,
                isAvailable = batchDto.IsAvailable
            });
        }

        /// <summary>
        /// Replace entire menu (requires authentication)
        /// </summary>
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> ReplaceMenu(int vendorId, [FromBody] UpdateMenuDto menuDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _menuService.ReplaceMenuAsync(vendorId, userId, menuDto.MenuItems);

            if (!success)
            {
                return NotFound(new { message = "Vendor not found or access denied" });
            }

            return Ok(new { message = "Menu replaced successfully" });
        }
    }
}