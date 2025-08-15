namespace PointMarket.API.DTOs
{
    public class UserResponseDto
    {
        public int ID { get; set; }
        public string UserID { get; set; } = string.Empty;
        public string? Username { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsEmailVerified { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsPhoneNumberVerified { get; set; }
        public string UserType { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }
}
