using PointMarket.API.DTOs;

namespace PointMarket.API.Services
{
    public interface IOtpCacheService
    {
        void StoreOtp(string phoneNumber, string otp, UserRegistrationDto? registrationData = null);
        string? GetOtp(string phoneNumber);
        UserRegistrationDto? GetRegistrationData(string phoneNumber);
        void RemoveOtp(string phoneNumber);
    }
}
