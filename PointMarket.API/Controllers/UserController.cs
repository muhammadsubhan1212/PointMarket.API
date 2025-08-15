using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PointMarket.API.DTOs;
using PointMarket.API.Services;
using System.Security.Claims;

namespace PointMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Step 1: Register user and request OTP
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto registrationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.RegisterUserAsync(registrationDto);

            if (result.Success)
            {
                return Ok(new
                {
                    success = true,
                    message = result.Message,
                    // In development, return OTP. In production, remove this line
                    otpCode = result.OtpCode
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.Message
            });
        }

        /// <summary>
        /// Step 2: Verify OTP and complete registration
        /// </summary>
        [HttpPost("verify-registration")]
        public async Task<IActionResult> VerifyRegistration([FromBody] OtpVerificationDto otpDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.VerifyOtpAndCompleteRegistrationAsync(otpDto);

            if (result.Success)
            {
                return Ok(new
                {
                    success = true,
                    message = result.Message,
                    user = result.User,
                    token = result.Token
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.Message
            });
        }

        /// <summary>
        /// Login Step 1: Request OTP for existing user
        /// </summary>
        [HttpPost("login/request-otp")]
        public async Task<IActionResult> RequestLoginOtp([FromBody] OtpRequestDto otpRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.RequestLoginOtpAsync(otpRequestDto);

            if (result.Success)
            {
                return Ok(new
                {
                    success = true,
                    message = result.Message,
                    // In development, return OTP. In production, remove this line
                    otpCode = result.OtpCode
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.Message
            });
        }

        /// <summary>
        /// Login Step 2: Verify OTP and login
        /// </summary>
        [HttpPost("login/verify-otp")]
        public async Task<IActionResult> VerifyLoginOtp([FromBody] OtpVerificationDto otpDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.VerifyLoginOtpAsync(otpDto);

            if (result.Success)
            {
                return Ok(new
                {
                    success = true,
                    message = result.Message,
                    user = result.User,
                    token = result.Token
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.Message
            });
        }

        /// <summary>
        /// Get current user profile (requires authentication)
        /// </summary>
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { success = false, message = "Invalid token" });
            }

            var user = await _userService.GetUserByUserIdAsync(userId);
            
            if (user == null)
            {
                return NotFound(new { success = false, message = "User not found" });
            }

            return Ok(new
            {
                success = true,
                user = user
            });
        }

        /// <summary>
        /// Get user by ID (requires authentication)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            
            if (user == null)
            {
                return NotFound(new { success = false, message = "User not found" });
            }

            return Ok(new
            {
                success = true,
                user = user
            });
        }
    }
}
