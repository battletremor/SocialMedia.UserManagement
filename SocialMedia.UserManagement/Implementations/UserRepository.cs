using DTO.UserManagement.DBContexts;
using Microsoft.EntityFrameworkCore;
using SocialMedia.UserManagement.Data.Models;

namespace SocialMedia.UserManagement.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DTOContext _dbContext;

        public UserRepository(DTOContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Users> CreateUserAsync(Users user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<Users> GetUserByIdAsync(Guid userId)
        {
            return await _dbContext.Users.FindAsync(userId);
        }

        public async Task<Users> GetUserByUsernameAsync(string username)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<IEnumerable<Users>> GetAllUsersAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<Users> UpdateUserAsync(Users user)
        {
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null)
                return false;

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Users> GetUserByEmailAsync(string email)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        // Follow Management
        public async Task FollowUserAsync(UserFollows follow)
        {
            await _dbContext.UserFollows.AddAsync(follow);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> UnfollowUserAsync(Guid userId, Guid followeeId)
        {
            var follow = await _dbContext.UserFollows
                .FirstOrDefaultAsync(f => f.FollowerId == userId && f.FolloweeId == followeeId);

            if (follow == null) return false;

            _dbContext.UserFollows.Remove(follow);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Users>> GetFollowersAsync(Guid userId)
        {
            var followerIds = await _dbContext.UserFollows
                .Where(f => f.FolloweeId == userId)
                .Select(f => f.FollowerId)
                .ToListAsync();

            return await _dbContext.Users
                .Where(u => followerIds.Contains(u.UserId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Users>> GetFollowingAsync(Guid userId)
        {
            var followeeIds = await _dbContext.UserFollows
                .Where(f => f.FollowerId == userId)
                .Select(f => f.FolloweeId)
                .ToListAsync();

            return await _dbContext.Users
                .Where(u => followeeIds.Contains(u.UserId))
                .ToListAsync();
        }

        // Account Updates
        public async Task UpdatePasswordAsync(Guid userId, string newPasswordHash)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null) throw new KeyNotFoundException("User not found");

            user.PasswordHash = newPasswordHash;
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateEmailAsync(Guid userId, string newEmail)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null) throw new KeyNotFoundException("User not found");

            user.Email = newEmail;
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<UserFollows> GetFollowRelationshipAsync(Guid followerId, Guid followeeId)
        {
            return await _dbContext.UserFollows
                .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FolloweeId == followeeId);
        }

        public async Task<bool> UserExistsAsync(Guid userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            return user != null;
        }

        public async Task<IEnumerable<Users>> SearchUsers(string username, int records = 10)
        {
            // Search for users whose username contains the search term (case-insensitive)
            var users = await _dbContext.Users
                .Where(u => EF.Functions.Like(u.Username, $"%{username}%"))
                .Take(records)
                .ToListAsync();

            return users;
        }

        //public async Task<string> GeneratePasswordResetTokenAsync(Guid userId)
        //{
        //    var user = await _dbContext.Users.FindAsync(userId);
        //    if (user == null)
        //    {
        //        return null;
        //    }

        //    // Generate a unique reset token (e.g., GUID or JWT)
        //    var resetToken = Guid.NewGuid().ToString();

        //    // Store token in the database with expiration date (if needed)
        //    var tokenRecord = new PasswordResetToken
        //    {
        //        UserId = userId,
        //        Token = resetToken,
        //        ExpirationDate = DateTime.UtcNow.AddHours(1)  // Token expires in 1 hour
        //    };
        //    _dbContext.PasswordResetTokens.Add(tokenRecord);
        //    await _dbContext.SaveChangesAsync();

        //    return resetToken;
        //}

        //public async Task<bool> ValidatePasswordResetTokenAsync(Guid userId, string token)
        //{
        //    var tokenRecord = await _dbContext.PasswordResetTokens
        //        .FirstOrDefaultAsync(t => t.UserId == userId && t.Token == token);

        //    if (tokenRecord == null || tokenRecord.ExpirationDate < DateTime.UtcNow)
        //    {
        //        return false; // Token is either invalid or expired
        //    }

        //    return true;
        //}

        //public async Task<bool> ResetPasswordAsync(Guid userId, string newPassword)
        //{
        //    var user = await _dbContext.Users.FindAsync(userId);
        //    if (user == null)
        //    {
        //        return false;
        //    }

        //    // Hash the new password (use a password hashing mechanism like ASP.NET Core Identity's PasswordHasher)
        //    user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
        //    await _dbContext.SaveChangesAsync();

        //    return true;
        //}

    }
}
