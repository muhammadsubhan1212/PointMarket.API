using System.ComponentModel.DataAnnotations;

namespace PointMarket.API.DTOs
{
    public class OtpRequestDto
    {
        [Required]
        [Phone]
        [StringLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
