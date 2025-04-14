using MessengerBackend.Data;
using MessengerBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace MessengerBackend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddUserAsync(User user)
        {
           await _context.Users.AddAsync(user);
           await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            var userInDb = await _context.Users.FindAsync(id);

            if (userInDb == null)
            {
                throw new KeyNotFoundException("User was not found.");
            }

            _context.Users.Remove(userInDb);
            await _context.SaveChangesAsync();
        }


        public Task<IEnumerable<User>> GetAllAsync()
        {
            throw new NotImplementedException();
        }


        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }
    }
}
