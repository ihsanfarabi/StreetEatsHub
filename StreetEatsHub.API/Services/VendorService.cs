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
                .Include(v => v.MenuItems)
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
    }
}