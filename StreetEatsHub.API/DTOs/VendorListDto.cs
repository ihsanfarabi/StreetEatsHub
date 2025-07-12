using System.ComponentModel.DataAnnotations;

namespace StreetEatsHub.API.DTOs
{
    // For displaying vendor in public list
    public class VendorListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public bool IsOpen { get; set; }
        public DateTime LastUpdated { get; set; }
        public string WhatsAppNumber { get; set; } = string.Empty;
    }

    // For displaying individual vendor with menu
    public class VendorDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public bool IsOpen { get; set; }
        public DateTime LastUpdated { get; set; }
        public string WhatsAppNumber { get; set; } = string.Empty;
        public List<MenuItemDto> MenuItems { get; set; } = new();
    }

    // For vendor registration
    public class RegisterVendorDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Location { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Specialty { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string WhatsAppNumber { get; set; } = string.Empty;
    }

    // For vendor login
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    // For updating vendor status
    public class UpdateStatusDto
    {
        [Required]
        public bool IsOpen { get; set; }
    }

    // For authentication response
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public VendorListDto Vendor { get; set; } = new();
    }
}