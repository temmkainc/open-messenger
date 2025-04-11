using MessengerBackend.Data;
using MessengerBackend.Models;
using Microsoft.AspNetCore.Mvc;
using MessengerBackend.Repositories;

namespace MessengerBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            await _userRepository.AddUserAsync(user);
            return Ok(user);
        }
    }
}
