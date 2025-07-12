using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace StreetEatsHub.API.Models
{
    public class Vendor
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Location { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Specialty { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string WhatsAppNumber { get; set; } = string.Empty;

        public bool IsOpen { get; set; } = false;

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key to Identity User
        [Required]
        public string UserId { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

        // Navigation property to Identity User
        public virtual IdentityUser User { get; set; } = null!;
    }
}