using System.ComponentModel.DataAnnotations;

namespace SocialMedia.UserManagement.Data.DTO
{
    public class UserCreateDto
    {
        [Required]
        [DataType("varchar(50)")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType("varchar(100)")]
        public string Fullname { get; set; }

        [DataType("text")]
        public string? Bio { get; set; }

        [Required]
        [Url]
        public string ProfilePictureUrl { get; set; }
    }
}
