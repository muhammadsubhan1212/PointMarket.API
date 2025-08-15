using PointMarket.API.DTOs;
using PointMarket.API.Models;
using PointMarket.API.Repositories;
using PointMarket.API.Helpers;

namespace PointMarket.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IOtpCacheService _otpCache;

        public UserService(IUserRepository userRepository, IOtpCacheService otpCache)
        {
            _userRepository = userRepository;
            _otpCache = otpCache;
        }

        public async Task<(bool Success, string Message, string? OtpCode)> RegisterUserAsync(UserRegistrationDto registrationDto)
        {
            try
            {
                // Check if user already exists
                if (await _userRepository.ExistsAsync(registrationDto.Email, registrationDto.PhoneNumber))
                {
                    return (false, "User with this email or phone number already exists", null);
                }

                // Generate dummy OTP
                var otpCode = OtpHelper.GenerateOtp();
                
                // Store OTP and registration data in cache
                _otpCache.StoreOtp(registrationDto.PhoneNumber, otpCode, registrationDto);

                return (true, "OTP sent successfully", otpCode);
            }
            catch (Exception ex)
            {
                return (false, $"Registration failed: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message, UserResponseDto? User, string? Token)> VerifyOtpAndCompleteRegistrationAsync(OtpVerificationDto otpDto)
        {
            try
            {
                // Get OTP from cache
                var storedOtp = _otpCache.GetOtp(otpDto.PhoneNumber);
                if (storedOtp == null)
                {
                    return (false, "OTP not found or expired", null, null);
                }

                if (!OtpHelper.VerifyOtp(otpDto.OtpCode, storedOtp))
                {
                    return (false, "Invalid OTP", null, null);
                }

                // Get registration data from cache
                var registrationData = _otpCache.GetRegistrationData(otpDto.PhoneNumber);
                if (registrationData == null)
                {
                    return (false, "Registration data not found", null, null);
                }

                // Create user with actual registration data
                var user = new User
                {
                    UserID = Guid.NewGuid().ToString(),
                    FirstName = registrationData.FirstName,
                    LastName = registrationData.LastName,
                    Email = registrationData.Email,
                    PhoneNumber = registrationData.PhoneNumber,
                    IsPhoneNumberVerified = true,
                    UserType = registrationData.UserType,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                var createdUser = await _userRepository.CreateAsync(user);

                // Remove from cache
                _otpCache.RemoveOtp(otpDto.PhoneNumber);

                // Generate JWT token
                var token = JwtHelper.GenerateToken(createdUser.UserID, createdUser.Email, createdUser.UserType);

                // Update last login
                createdUser.LastLoginDate = DateTime.UtcNow;
                await _userRepository.UpdateAsync(createdUser);

                var userResponse = MapToUserResponse(createdUser);
                return (true, "Registration completed successfully", userResponse, token);
            }
            catch (Exception ex)
            {
                return (false, $"OTP verification failed: {ex.Message}", null, null);
            }
        }

        public async Task<(bool Success, string Message, string? OtpCode)> RequestLoginOtpAsync(OtpRequestDto otpRequestDto)
        {
            try
            {
                var user = await _userRepository.GetByPhoneNumberAsync(otpRequestDto.PhoneNumber);
                if (user == null)
                {
                    return (false, "User not found with this phone number", null);
                }

                if (!user.IsActive)
                {
                    return (false, "User account is inactive", null);
                }

                var otpCode = OtpHelper.GenerateOtp();
                
                // Store OTP in cache (no registration data needed for login)
                _otpCache.StoreOtp(otpRequestDto.PhoneNumber, otpCode);

                return (true, "Login OTP sent successfully", otpCode);
            }
            catch (Exception ex)
            {
                return (false, $"Login OTP request failed: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message, UserResponseDto? User, string? Token)> VerifyLoginOtpAsync(OtpVerificationDto otpDto)
        {
            try
            {
                // Get OTP from cache
                var storedOtp = _otpCache.GetOtp(otpDto.PhoneNumber);
                if (storedOtp == null)
                {
                    return (false, "OTP not found or expired", null, null);
                }

                if (!OtpHelper.VerifyOtp(otpDto.OtpCode, storedOtp))
                {
                    return (false, "Invalid OTP", null, null);
                }

                var user = await _userRepository.GetByPhoneNumberAsync(otpDto.PhoneNumber);
                if (user == null)
                {
                    return (false, "User not found", null, null);
                }

                // Remove OTP from cache
                _otpCache.RemoveOtp(otpDto.PhoneNumber);

                var token = JwtHelper.GenerateToken(user.UserID, user.Email, user.UserType);

                user.LastLoginDate = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);

                var userResponse = MapToUserResponse(user);
                return (true, "Login successful", userResponse, token);
            }
            catch (Exception ex)
            {
                return (false, $"Login failed: {ex.Message}", null, null);
            }
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user != null ? MapToUserResponse(user) : null;
        }

        public async Task<UserResponseDto?> GetUserByUserIdAsync(string userId)
        {
            var user = await _userRepository.GetByUserIdAsync(userId);
            return user != null ? MapToUserResponse(user) : null;
        }

        private UserResponseDto MapToUserResponse(User user)
        {
            return new UserResponseDto
            {
                ID = user.ID,
                UserID = user.UserID,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                IsEmailVerified = user.IsEmailVerified,
                PhoneNumber = user.PhoneNumber,
                IsPhoneNumberVerified = user.IsPhoneNumberVerified,
                UserType = user.UserType,
                IsActive = user.IsActive,
                CreatedDate = user.CreatedDate,
                LastLoginDate = user.LastLoginDate
            };
        }
    }
}
