using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StreetEatsHub.API.Data;
using StreetEatsHub.API.DTOs;
using StreetEatsHub.API.Models;

namespace StreetEatsHub.API.Services
{
    public interface IMenuService
    {
        Task<List<MenuItemDto>> GetMenuAsync(int vendorId);
        Task<MenuItemDto?> GetMenuItemAsync(int vendorId, int menuItemId);
        Task<MenuItemDto?> CreateMenuItemAsync(int vendorId, string userId, CreateMenuItemDto menuItemDto);
        Task<MenuItemDto?> UpdateMenuItemAsync(int vendorId, string userId, int menuItemId, UpdateMenuItemDto menuItemDto);
        Task<bool> DeleteMenuItemAsync(int vendorId, string userId, int menuItemId);
        Task<bool> ToggleMenuItemAvailabilityAsync(int vendorId, string userId, int menuItemId, bool isAvailable);
        Task<bool> BatchToggleAvailabilityAsync(int vendorId, string userId, List<int> menuItemIds, bool isAvailable);
        Task<List<string>> GetMenuCategoriesAsync(int vendorId);
        Task<bool> ReplaceMenuAsync(int vendorId, string userId, List<CreateMenuItemDto> menuItems);
    }

    public class MenuService : IMenuService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public MenuService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<MenuItemDto>> GetMenuAsync(int vendorId)
        {
            var menuItems = await _context.MenuItems
                .Where(m => m.VendorId == vendorId)
                .OrderBy(m => m.Category)
                .ThenBy(m => m.Name)
                .ToListAsync();

            return _mapper.Map<List<MenuItemDto>>(menuItems);
        }

        public async Task<MenuItemDto?> GetMenuItemAsync(int vendorId, int menuItemId)
        {
            var menuItem = await _context.MenuItems
                .FirstOrDefaultAsync(m => m.Id == menuItemId && m.VendorId == vendorId);

            if (menuItem == null)
                return null;

            return _mapper.Map<MenuItemDto>(menuItem);
        }

        public async Task<MenuItemDto?> CreateMenuItemAsync(int vendorId, string userId, CreateMenuItemDto menuItemDto)
        {
            // Verify vendor ownership
            if (!await IsVendorOwnerAsync(vendorId, userId))
                return null;

            var menuItem = _mapper.Map<MenuItem>(menuItemDto);
            menuItem.VendorId = vendorId;

            _context.MenuItems.Add(menuItem);
            await UpdateVendorTimestampAsync(vendorId);
            await _context.SaveChangesAsync();

            return _mapper.Map<MenuItemDto>(menuItem);
        }

        public async Task<MenuItemDto?> UpdateMenuItemAsync(int vendorId, string userId, int menuItemId, UpdateMenuItemDto menuItemDto)
        {
            // Verify vendor ownership
            if (!await IsVendorOwnerAsync(vendorId, userId))
                return null;

            var menuItem = await _context.MenuItems
                .FirstOrDefaultAsync(m => m.Id == menuItemId && m.VendorId == vendorId);

            if (menuItem == null)
                return null;

            // Update fields
            menuItem.Name = menuItemDto.Name;
            menuItem.Price = menuItemDto.Price;
            menuItem.IsAvailable = menuItemDto.IsAvailable;
            menuItem.Category = menuItemDto.Category;
            menuItem.UpdatedAt = DateTime.UtcNow;

            await UpdateVendorTimestampAsync(vendorId);
            await _context.SaveChangesAsync();

            return _mapper.Map<MenuItemDto>(menuItem);
        }

        public async Task<bool> DeleteMenuItemAsync(int vendorId, string userId, int menuItemId)
        {
            // Verify vendor ownership
            if (!await IsVendorOwnerAsync(vendorId, userId))
                return false;

            var menuItem = await _context.MenuItems
                .FirstOrDefaultAsync(m => m.Id == menuItemId && m.VendorId == vendorId);

            if (menuItem == null)
                return false;

            _context.MenuItems.Remove(menuItem);
            await UpdateVendorTimestampAsync(vendorId);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ToggleMenuItemAvailabilityAsync(int vendorId, string userId, int menuItemId, bool isAvailable)
        {
            // Verify vendor ownership
            if (!await IsVendorOwnerAsync(vendorId, userId))
                return false;

            var menuItem = await _context.MenuItems
                .FirstOrDefaultAsync(m => m.Id == menuItemId && m.VendorId == vendorId);

            if (menuItem == null)
                return false;

            menuItem.IsAvailable = isAvailable;
            menuItem.UpdatedAt = DateTime.UtcNow;

            await UpdateVendorTimestampAsync(vendorId);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> BatchToggleAvailabilityAsync(int vendorId, string userId, List<int> menuItemIds, bool isAvailable)
        {
            // Verify vendor ownership
            if (!await IsVendorOwnerAsync(vendorId, userId))
                return false;

            var menuItems = await _context.MenuItems
                .Where(m => menuItemIds.Contains(m.Id) && m.VendorId == vendorId)
                .ToListAsync();

            if (!menuItems.Any())
                return false;

            foreach (var item in menuItems)
            {
                item.IsAvailable = isAvailable;
                item.UpdatedAt = DateTime.UtcNow;
            }

            await UpdateVendorTimestampAsync(vendorId);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<string>> GetMenuCategoriesAsync(int vendorId)
        {
            var categories = await _context.MenuItems
                .Where(m => m.VendorId == vendorId)
                .Select(m => m.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            return categories;
        }

        public async Task<bool> ReplaceMenuAsync(int vendorId, string userId, List<CreateMenuItemDto> menuItems)
        {
            // Verify vendor ownership
            if (!await IsVendorOwnerAsync(vendorId, userId))
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
            await UpdateVendorTimestampAsync(vendorId);
            await _context.SaveChangesAsync();

            return true;
        }

        private async Task<bool> IsVendorOwnerAsync(int vendorId, string userId)
        {
            return await _context.Vendors
                .AnyAsync(v => v.Id == vendorId && v.UserId == userId);
        }

        private async Task UpdateVendorTimestampAsync(int vendorId)
        {
            var vendor = await _context.Vendors.FindAsync(vendorId);
            if (vendor != null)
            {
                vendor.LastUpdated = DateTime.UtcNow;
            }
        }
    }
}