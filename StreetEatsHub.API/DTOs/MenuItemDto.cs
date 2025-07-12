using System.ComponentModel.DataAnnotations;

namespace StreetEatsHub.API.DTOs
{
    // For displaying menu items
    public class MenuItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public string Category { get; set; } = string.Empty;
    }

    // For creating/updating menu items
    public class CreateMenuItemDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 999999.99)]
        public decimal Price { get; set; }

        public bool IsAvailable { get; set; } = true;

        [MaxLength(50)]
        public string Category { get; set; } = "General";
    }

    // For updating entire menu
    public class UpdateMenuDto
    {
        public List<CreateMenuItemDto> MenuItems { get; set; } = new();
    }

    // For toggling item availability
    public class ToggleAvailabilityDto
    {
        [Required]
        public bool IsAvailable { get; set; }
    }
}