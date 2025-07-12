using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StreetEatsHub.API.Models
{
    public class MenuItem
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        public bool IsAvailable { get; set; } = true;

        [MaxLength(50)]
        public string Category { get; set; } = "General";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key
        public int VendorId { get; set; }

        // Navigation property
        public virtual Vendor Vendor { get; set; } = null!;
    }
}