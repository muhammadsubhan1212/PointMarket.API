using PointMarket.API.DTOs;
using PointMarket.API.Models;

namespace PointMarket.API.Services
{
    public interface IUserService
    {
        Task<(bool Success, string Message, string? OtpCode)> RegisterUserAsync(UserRegistrationDto registrationDto);
        Task<(bool Success, string Message, UserResponseDto? User, string? Token)> VerifyOtpAndCompleteRegistrationAsync(OtpVerificationDto otpDto);
        Task<(bool Success, string Message, string? OtpCode)> RequestLoginOtpAsync(OtpRequestDto otpRequestDto);
        Task<(bool Success, string Message, UserResponseDto? User, string? Token)> VerifyLoginOtpAsync(OtpVerificationDto otpDto);
        Task<UserResponseDto?> GetUserByIdAsync(int id);
        Task<UserResponseDto?> GetUserByUserIdAsync(string userId);
    }
}
