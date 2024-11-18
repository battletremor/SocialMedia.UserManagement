using SocialMedia.UserManagement.Data.Models;

namespace SocialMedia.UserManagement.Data.Repositories
{
    public interface IUserRepository
    {
        //User
        Task<Users> CreateUserAsync(Users user);
        Task<Users> GetUserByIdAsync(Guid userId);
        Task<Users> GetUserByEmailAsync(string email);
        Task<Users> GetUserByUsernameAsync(string username);
        Task<IEnumerable<Users>> GetAllUsersAsync();
        Task<Users> UpdateUserAsync(Users user);
        Task<bool> DeleteUserAsync(Guid userId);
        Task<IEnumerable<Users>> SearchUsers(string username, int records);

        //Follow
        Task FollowUserAsync(UserFollows follow);
        Task<bool> UnfollowUserAsync(Guid userId, Guid followeeId);
        Task<IEnumerable<Users>> GetFollowersAsync(Guid userId);
        Task<IEnumerable<Users>> GetFollowingAsync(Guid userId);
        Task<UserFollows> GetFollowRelationshipAsync(Guid followerId, Guid followeeId);
        Task<bool> UserExistsAsync(Guid userId);

        //Account
        Task UpdatePasswordAsync(Guid userId, string newPasswordHash);
        Task UpdateEmailAsync(Guid userId, string newEmail);
        //Task<string> GeneratePasswordResetTokenAsync(Guid userId);
        //Task<bool> ValidatePasswordResetTokenAsync(Guid userId, string token);
        //Task<bool> ResetPasswordAsync(Guid userId, string newPassword);
    }
}
