using Microsoft.EntityFrameworkCore;
using PointMarket.API.Models;
using PointMarket.API.Data;

namespace PointMarket.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Where(u => !u.IsDeleted)
                .FirstOrDefaultAsync(u => u.ID == id);
        }

        public async Task<User?> GetByUserIdAsync(string userId)
        {
            return await _context.Users
                .Where(u => !u.IsDeleted)
                .FirstOrDefaultAsync(u => u.UserID == userId);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .Where(u => !u.IsDeleted)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.Users
                .Where(u => !u.IsDeleted)
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .Where(u => !u.IsDeleted)
                .ToListAsync();
        }

        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            user.UpdatedDate = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await GetByIdAsync(id);
            if (user == null) return false;

            user.IsDeleted = true;
            user.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(string email, string phoneNumber)
        {
            return await _context.Users
                .Where(u => !u.IsDeleted)
                .AnyAsync(u => u.Email == email || u.PhoneNumber == phoneNumber);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
