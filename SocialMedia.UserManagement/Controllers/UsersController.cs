using Serilog;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.UserManagement.Data.Models;
using SocialMedia.UserManagement.Data.Repositories;
using SocialMedia.UserManagement.Data.DTO;
using SocialMedia.UserManagement.Data.DTOs;

namespace SocialMedia.UserManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [Obsolete("Use /auth/register")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto userDto)
        {
            try
            {
                // Hash the password
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

                // Map DTO to entity
                var user = new Users
                {
                    UserId = Guid.NewGuid(),
                    Username = userDto.Username,
                    Email = userDto.Email,
                    PasswordHash = hashedPassword,
                    Fullname = userDto.Fullname,
                    Bio = userDto.Bio,
                    ProfilePictureUrl = userDto.ProfilePictureUrl
                };

                var createdUser = await _userRepository.CreateUserAsync(user);
                return CreatedAtAction(nameof(GetUserById), new { userId = createdUser.UserId }, createdUser);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while creating user");
                return BadRequest("Error while creating user");
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                    return NotFound();
                var userResponse = new UserDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    Fullname = user.Fullname,
                    Bio = user.Bio,
                    ProfilePictureUrl = user.ProfilePictureUrl
                };
                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while getting user info for: {userId}");
                return BadRequest($"Error while getting user info for: {userId}");
            }
        }

        [Obsolete("use /users/search")]
        [HttpGet("username/{username}")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            try
            {
                var user = await _userRepository.GetUserByUsernameAsync(username);
                if (user == null)
                    return NotFound();

                var userResponse = new UserDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    Fullname = user.Fullname,
                    Bio = user.Bio,
                    ProfilePictureUrl = user.ProfilePictureUrl
                };
                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while getting user info for: {username}");
                return BadRequest($"Error while getting user info for: {username}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userRepository.GetAllUsersAsync();

                var userResponses = new List<UserDto>();
                foreach (var user in users)
                {
                    userResponses.Add(new UserDto
                    {
                        UserId = user.UserId,
                        Username = user.Username,
                        Email = user.Email,
                        Fullname = user.Fullname,
                        Bio = user.Bio,
                        ProfilePictureUrl = user.ProfilePictureUrl
                    });
                }

                return Ok(userResponses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while getting users");
                return BadRequest("Error while getting users");
            }
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UserUpdateDto userDto)
        {
            try
            {
                var existingUser = await _userRepository.GetUserByIdAsync(userId);
                if (existingUser == null)
                    return NotFound();

                existingUser.Username = userDto.Username;
                existingUser.Fullname = userDto.Fullname;
                existingUser.Bio = userDto.Bio;
                existingUser.ProfilePictureUrl = userDto.ProfilePictureUrl;

                var updatedUser = await _userRepository.UpdateUserAsync(existingUser);

                var userResponse = new UserDto
                {
                    UserId = updatedUser.UserId,
                    Username = updatedUser.Username,
                    Email = updatedUser.Email,
                    Fullname = updatedUser.Fullname,
                    Bio = updatedUser.Bio,
                    ProfilePictureUrl = updatedUser.ProfilePictureUrl
                };

                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while updating user info for: {userId}");
                return BadRequest($"Error while updating user info for: {userId}");
            }
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            try
            {
                var isDeleted = await _userRepository.DeleteUserAsync(userId);
                if (!isDeleted)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while deleting user info for: {userId}");
                return BadRequest($"Error while deleting user info for: {userId}");
            }
        }

        [HttpPost("{userId}/follow")]
        public async Task<IActionResult> FollowUser(Guid userId, [FromBody] Guid followeeId)
        {
            try
            {
                if(followeeId == userId)
                {
                    return BadRequest("User followeeId can't be same as userId");
                }

                // Check if the user exists
                var userExists = await _userRepository.UserExistsAsync(userId);
                if (!userExists)
                {
                    return NotFound("User not found.");
                }

                // Check if the followee exists (optional, if you want to verify that the followee exists as well)
                var followeeExists = await _userRepository.UserExistsAsync(followeeId);
                if (!followeeExists)
                {
                    return NotFound("Followee not found.");
                }

                // Check if the relationship already exists
                var existingFollow = await _userRepository.GetFollowRelationshipAsync(userId, followeeId);
                if (existingFollow != null)
                {
                    return BadRequest("You are already following this user.");
                }

                // Create a new follow relationship
                var follow = new UserFollows
                {
                    Id = Guid.NewGuid(),
                    FollowerId = userId,
                    FolloweeId = followeeId
                };

                await _userRepository.FollowUserAsync(follow);
                return Ok("Followed successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while following user: {userId}");
                return BadRequest($"Error while following user: {userId}");
            }
        }


        [HttpPost("{userId}/unfollow")]
        public async Task<IActionResult> UnfollowUser(Guid userId, [FromBody] Guid followeeId)
        {
            try
            {
                var result = await _userRepository.UnfollowUserAsync(userId, followeeId);
                if (!result)
                    return NotFound("Follow relationship not found.");

                return Ok("Unfollowed successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while unfollowing user info for: {userId}");
                return BadRequest($"Error while unfollowing user info for: {userId}");
            }
        }

        [HttpGet("{userId}/followers")]
        public async Task<IActionResult> GetFollowers(Guid userId)
        {
            try
            {
                var followers = await _userRepository.GetFollowersAsync(userId);
                return Ok(followers);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while getting followers for: {userId}");
                return BadRequest($"Error while getting followers for: {userId}");
            }
        }

        [HttpGet("{userId}/following")]
        public async Task<IActionResult> GetFollowing(Guid userId)
        {
            try
            {
                var following = await _userRepository.GetFollowingAsync(userId);
                return Ok(following);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while getting following for: {userId}");
                return BadRequest($"Error while getting following for: {userId}");
            }
        }

        [HttpPut("{userId}/change-password")]
        public async Task<IActionResult> ChangePassword(Guid userId, [FromBody] string newPassword)
        {
            try
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                await _userRepository.UpdatePasswordAsync(userId, hashedPassword);
                return Ok("Password changed successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while changing password for: {userId}");
                return BadRequest($"Error while changing password for: {userId}");
            }
        }

        [HttpPut("{userId}/update-email")]
        public async Task<IActionResult> UpdateEmail(Guid userId, [FromBody] string newEmail)
        {
            try
            {
                await _userRepository.UpdateEmailAsync(userId, newEmail);
                return Ok("Email updated successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while updating email for: {userId}");
                return BadRequest($"Error while updating email for: {userId}");
            }
        }

        [HttpGet("search/{username}")]
        public async Task<IActionResult> SearchUser(string username,int records)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    return BadRequest("Minimum one character is required to search");
                }

                var users = await _userRepository.SearchUsers(username, records);
                if (users == null)
                    return NotFound();

                var userResponses = new List<UserDto>();
                foreach (var user in users)
                {
                    userResponses.Add(new UserDto
                    {
                        UserId = user.UserId,
                        Username = user.Username,
                        Email = user.Email,
                        Fullname = user.Fullname,
                        Bio = user.Bio,
                        ProfilePictureUrl = user.ProfilePictureUrl
                    });
                }

                return Ok(userResponses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error while searching users for query: {username}");
                return BadRequest($"Error while searching users for query: {username}");
            }
        }

        //[HttpPost("{userId}/reset-password")]
        //public async Task<IActionResult> ResetPassword(Guid userId)
        //{
        //    try
        //    {
        //        // Check if the user exists
        //        var user = await _userRepository.GetUserByIdAsync(userId);
        //        if (user == null)
        //        {
        //            return NotFound("User not found.");
        //        }

        //        // Generate a password reset token
        //        var resetToken = await _userRepository.GeneratePasswordResetTokenAsync(userId);

        //        // Send the reset token to the user's email
        //        var resetLink = Url.Action("ConfirmPasswordReset", "Users", new { userId = user.UserId, token = resetToken }, Request.Scheme);

        //        // Send email with the link (you would need a service to send emails, e.g., using SMTP, SendGrid, etc.)
        //        await _emailService.SendPasswordResetEmailAsync(user.Email, resetLink);

        //        return Ok("Password reset link sent to the user’s email.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex, "Error occurred during password reset request.");
        //        return StatusCode(500, "An error occurred while processing your request.");
        //    }
        //}

        //[HttpPost("{userId}/confirm-reset-password")]
        //public async Task<IActionResult> ConfirmPasswordReset(Guid userId, [FromQuery] string token, [FromBody] ResetPasswordModel model)
        //{
        //    try
        //    {
        //        // Validate the input parameters
        //        if (string.IsNullOrWhiteSpace(token) || model.NewPassword == null || model.NewPassword.Length < 6)
        //        {
        //            return BadRequest("Invalid token or password.");
        //        }

        //        // Check if the user exists
        //        var user = await _userRepository.GetUserByIdAsync(userId);
        //        if (user == null)
        //        {
        //            return NotFound("User not found.");
        //        }

        //        // Verify the token
        //        var isTokenValid = await _userRepository.ValidatePasswordResetTokenAsync(userId, token);
        //        if (!isTokenValid)
        //        {
        //            return BadRequest("Invalid or expired reset token.");
        //        }

        //        // Reset the user's password
        //        var resetResult = await _userRepository.ResetPasswordAsync(userId, model.NewPassword);
        //        if (!resetResult)
        //        {
        //            return StatusCode(500, "An error occurred while resetting the password.");
        //        }

        //        return Ok("Password has been reset successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex, "Error occurred while confirming password reset.");
        //        return StatusCode(500, "An error occurred while processing your request.");
        //    }
        //}


    }
}
