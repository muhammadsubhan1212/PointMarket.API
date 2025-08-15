using System.Collections.Concurrent;
using PointMarket.API.DTOs;

namespace PointMarket.API.Services
{
    public class OtpCacheService : IOtpCacheService
    {
        private readonly ConcurrentDictionary<string, string> _otpStorage = new();
        private readonly ConcurrentDictionary<string, UserRegistrationDto> _registrationStorage = new();

        public void StoreOtp(string phoneNumber, string otp, UserRegistrationDto? registrationData = null)
        {
            _otpStorage[phoneNumber] = otp;
            if (registrationData != null)
            {
                _registrationStorage[phoneNumber] = registrationData;
            }
        }

        public string? GetOtp(string phoneNumber)
        {
            _otpStorage.TryGetValue(phoneNumber, out var otp);
            return otp;
        }

        public UserRegistrationDto? GetRegistrationData(string phoneNumber)
        {
            _registrationStorage.TryGetValue(phoneNumber, out var data);
            return data;
        }

        public void RemoveOtp(string phoneNumber)
        {
            _otpStorage.TryRemove(phoneNumber, out _);
            _registrationStorage.TryRemove(phoneNumber, out _);
        }
    }
}
