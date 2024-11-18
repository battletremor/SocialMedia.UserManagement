using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialMedia.UserManagement.Data.Models
{
    public class Sessions
    {
        [Key]
        public Guid SessionId { get; set; }

        [Required]
        [ForeignKey("Users")]
        public Guid UserId { get; set; }

        [Required]
        [DataType("text")]
        public string RefreshToken { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual Users User { get; set; }
    }
}
