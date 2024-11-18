using System;

namespace SocialMedia.UserManagement.Data.DTOs
{
    public class UserDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }
        public string? Bio { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
}
