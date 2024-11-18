using System;
using System.ComponentModel.DataAnnotations;

namespace SocialMedia.UserManagement.Data.Models
{
    public class UserFollows
    {
        [Key]
        public Guid Id { get; set; }
        public Guid FollowerId { get; set; }
        public Guid FolloweeId { get; set; }
    }
}
