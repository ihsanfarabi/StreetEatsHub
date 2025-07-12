using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StreetEatsHub.API.Data;
using StreetEatsHub.API.DTOs;
using StreetEatsHub.API.Models;

namespace StreetEatsHub.API.Services
{
    public interface IVendorService
    {
        Task<List<VendorListDto>> GetAllVendorsAsync();
        Task<VendorDetailDto?> GetVendorByIdAsync(int id);
        Task<bool> UpdateVendorStatusAsync(int vendorId, string userId, bool isOpen);
        Task<List<MenuItemDto>> GetVendorMenuAsync(int vendorId);
        Task<bool> UpdateVendorMenuAsync(int vendorId, string userId, List<CreateMenuItemDto> menuItems);
    }

    public class VendorService : IVendorService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public VendorService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<VendorListDto>> GetAllVendorsAsync()
        {
            var vendors = await _context.Vendors
                .OrderByDescending(v => v.IsOpen)
                .ThenBy(v => v.Name)
                .ToListAsync();

            return _mapper.Map<List<VendorListDto>>(vendors);
        }

        public async Task<VendorDetailDto?> GetVendorByIdAsync(int id)
        {
            var vendor = await _context.Vendors
                .Include(v => v.MenuItems.Where(m => m.IsAvailable))
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vendor == null)
                return null;

            return _mapper.Map<VendorDetailDto>(vendor);
        }

        public async Task<bool> UpdateVendorStatusAsync(int vendorId, string userId, bool isOpen)
        {
            var vendor = await _context.Vendors
                .FirstOrDefaultAsync(v => v.Id == vendorId && v.UserId == userId);

            if (vendor == null)
                return false;

            vendor.IsOpen = isOpen;
            vendor.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<MenuItemDto>> GetVendorMenuAsync(int vendorId)
        {
            var menuItems = await _context.MenuItems
                .Where(m => m.VendorId == vendorId)
                .OrderBy(m => m.Category)
                .ThenBy(m => m.Name)
                .ToListAsync();

            return _mapper.Map<List<MenuItemDto>>(menuItems);
        }

        public async Task<bool> UpdateVendorMenuAsync(int vendorId, string userId, List<CreateMenuItemDto> menuItems)
        {
            // Verify vendor ownership
            var vendor = await _context.Vendors
                .FirstOrDefaultAsync(v => v.Id == vendorId && v.UserId == userId);

            if (vendor == null)
                return false;

            // Remove existing menu items
            var existingItems = await _context.MenuItems
                .Where(m => m.VendorId == vendorId)
                .ToListAsync();

            _context.MenuItems.RemoveRange(existingItems);

            // Add new menu items
            var newMenuItems = _mapper.Map<List<MenuItem>>(menuItems);
            foreach (var item in newMenuItems)
            {
                item.VendorId = vendorId;
            }

            _context.MenuItems.AddRange(newMenuItems);

            // Update vendor timestamp
            vendor.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}