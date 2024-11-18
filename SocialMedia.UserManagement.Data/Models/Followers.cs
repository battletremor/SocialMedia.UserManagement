using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialMedia.UserManagement.Data.Models
{
    public class Followers
    {
        [Key]
        public Guid FollowId { get; set; }

        [Required]
        [ForeignKey("Users")]
        public Guid FollowerId { get; set; } // User who is following

        [Required]
        [ForeignKey("Users")]
        public Guid FollowingId { get; set; } // User being followed

        public DateTime FollowedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Users? Follower { get; set; }
        public virtual Users? Following { get; set; }
    }
}
