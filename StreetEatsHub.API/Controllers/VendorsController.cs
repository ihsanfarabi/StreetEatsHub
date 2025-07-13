using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreetEatsHub.API.DTOs;
using StreetEatsHub.API.Services;
using System.Security.Claims;

namespace StreetEatsHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendorsController : ControllerBase
    {
        private readonly IVendorService _vendorService;

        public VendorsController(IVendorService vendorService)
        {
            _vendorService = vendorService;
        }

        /// <summary>
        /// Get all vendors (public endpoint)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllVendors()
        {
            var vendors = await _vendorService.GetAllVendorsAsync();
            return Ok(vendors);
        }

        /// <summary>
        /// Get vendor by ID with menu (public endpoint)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVendor(int id)
        {
            var vendor = await _vendorService.GetVendorByIdAsync(id);

            if (vendor == null)
            {
                return NotFound(new { message = "Vendor not found" });
            }

            return Ok(vendor);
        }

        /// <summary>
        /// Update vendor open/closed status (requires authentication)
        /// </summary>
        [HttpPut("{id}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto statusDto)
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

            var success = await _vendorService.UpdateVendorStatusAsync(id, userId, statusDto.IsOpen);

            if (!success)
            {
                return NotFound(new { message = "Vendor not found or access denied" });
            }

            return Ok(new { message = "Status updated successfully", isOpen = statusDto.IsOpen });
        }



        /// <summary>
        /// Get open vendors only (public endpoint)
        /// </summary>
        [HttpGet("open")]
        public async Task<IActionResult> GetOpenVendors()
        {
            var vendors = await _vendorService.GetAllVendorsAsync();
            var openVendors = vendors.Where(v => v.IsOpen).ToList();
            return Ok(openVendors);
        }
    }
}