using AutoMapper;
using StreetEatsHub.API.DTOs;
using StreetEatsHub.API.Models;

namespace StreetEatsHub.API.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Vendor mappings
            CreateMap<Vendor, VendorListDto>();
            CreateMap<Vendor, VendorDetailDto>();
            CreateMap<RegisterVendorDto, Vendor>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.MenuItems, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => DateTime.UtcNow));

            // MenuItem mappings
            CreateMap<MenuItem, MenuItemDto>();
            CreateMap<CreateMenuItemDto, MenuItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.VendorId, opt => opt.Ignore())
                .ForMember(dest => dest.Vendor, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<UpdateMenuItemDto, MenuItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.VendorId, opt => opt.Ignore())
                .ForMember(dest => dest.Vendor, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}