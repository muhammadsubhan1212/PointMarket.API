using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PointMarket.API.Models
{
    public class User
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(36)]
        public string UserID { get; set; } = Guid.NewGuid().ToString();

        [StringLength(50)]
        public string? Username { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public bool IsEmailVerified { get; set; } = false;

        [StringLength(15)]
        public string? PhoneNumber { get; set; }

        public bool IsPhoneNumberVerified { get; set; } = false;

        [StringLength(255)]
        public string? PasswordHash { get; set; }

        [Required]
        [StringLength(20)]
        public string UserType { get; set; } = "Customer";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; }

        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        public int? UpdatedBy { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        [ForeignKey("CreatedBy")]
        public virtual User? CreatedByUser { get; set; }

        [ForeignKey("UpdatedBy")]
        public virtual User? UpdatedByUser { get; set; }

        public virtual ICollection<User> CreatedUsers { get; set; } = new List<User>();
        public virtual ICollection<User> UpdatedUsers { get; set; } = new List<User>();
    }
}
