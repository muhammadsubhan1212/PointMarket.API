using System.ComponentModel.DataAnnotations;

namespace PointMarket.API.DTOs
{
    public class OtpVerificationDto
    {
        [Required]
        [Phone]
        [StringLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(6)]
        public string OtpCode { get; set; } = string.Empty;
    }
}
