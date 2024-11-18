using System.ComponentModel.DataAnnotations;

namespace SocialMedia.UserManagement.Data.DTOs
{
    public class AuthRegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string Fullname { get; set; }

        [Required]
        [Url]
        public string ProfilePictureUrl { get; set; }
    }
}
