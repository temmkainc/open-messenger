using MessengerBackend.Data;
using MessengerBackend.Models;
using Microsoft.AspNetCore.Mvc;
using MessengerBackend.Repositories;
using Microsoft.AspNetCore.Identity;
using MessengerBackend.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MessengerBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _hasher;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _hasher = new PasswordHasher<User>();
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

            return Ok(new { user.Id, user.Username, user.Email });
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
