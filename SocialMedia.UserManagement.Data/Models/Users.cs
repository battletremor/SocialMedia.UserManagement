using System;
using System.ComponentModel.DataAnnotations;

namespace SocialMedia.UserManagement.Data.Models
{
    public class Users
    {
        [Key]
        public Guid UserId { get; set; } 

        [Required]
        [DataType("varchar(50)")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType("text")]
        public string PasswordHash { get; set; }

        [DataType("varchar(100)")]
        public string Fullname { get; set; }

        [DataType("text")]
        public string? Bio { get; set; }

        [DataType("varchar(255)")]
        public string? ProfilePictureUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
