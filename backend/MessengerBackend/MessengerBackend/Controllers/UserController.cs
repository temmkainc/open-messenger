using MessengerBackend.Data;
using MessengerBackend.Models;
using Microsoft.AspNetCore.Mvc;
using MessengerBackend.Repositories;
using Microsoft.AspNetCore.Identity;
using MessengerBackend.DTOs;
using Microsoft.EntityFrameworkCore;
using MessengerBackend.Services;

namespace MessengerBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _hasher;
        private readonly ITokenService _tokenService;

        public UserController(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _hasher = new PasswordHasher<User>();
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return BadRequest("Email is already in use.");
            }

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
            };

            user.PasswordHash = _hasher.HashPassword(user, dto.Password);

            await _userRepository.AddUserAsync(user);

            return Ok(new { user.Id, user.Username, user.Email });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LogDto dto)
        {
            var user = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            var passwordVerificationResult = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if (passwordVerificationResult != PasswordVerificationResult.Success)
            {
                return Unauthorized("Invalid email or password.");
            }

            var accessToken = _tokenService.CreateAccessToken(user);
            var refreshToken = _tokenService.CreateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateUserAsync(user);


            return Ok(new { accessToken, refreshToken, user.Id, user.Username, user.Email });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
        {
            var user = await _userRepository.GetUserByIdAsync(dto.UserId);
            if (user == null || user.RefreshToken != dto.RefreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                return Unauthorized("Invalid or expired refresh token.");
            }

            var accessToken = _tokenService.CreateAccessToken(user);
            var refreshToken = _tokenService.CreateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateUserAsync(user);

            return Ok(new
            {
                accessToken,
                refreshToken
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutDto logoutRequest)
        {
            var user = await _userRepository.GetUserByIdAsync(logoutRequest.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Clear the refresh token and its expiry time
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            await _userRepository.UpdateUserAsync(user);

            return Ok(new { message = "User logged out successfully." });
        }




        [HttpDelete("deleteAccount/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            await _userRepository.DeleteUserAsync(userId);

            return Ok("User deleted successfully.");
        }
    }
}
