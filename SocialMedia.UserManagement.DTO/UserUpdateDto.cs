using System.ComponentModel.DataAnnotations;

namespace SocialMedia.UserManagement.Data.DTO
{
    public class UserUpdateDto
    {
        [Required]
        [DataType("varchar(50)")]
        public string Username { get; set; }

        [DataType("varchar(100)")]
        public string Fullname { get; set; }

        [DataType("text")]
        public string? Bio { get; set; }

        [Required]
        [Url]
        public string ProfilePictureUrl { get; set; }
    }
}
