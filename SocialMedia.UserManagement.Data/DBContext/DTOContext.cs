using Microsoft.EntityFrameworkCore;
using SocialMedia.UserManagement.Data.Models;

namespace DTO.UserManagement.DBContexts
{
    public class DTOContext : DbContext
    {
        public DTOContext(DbContextOptions<DTOContext> options) : base(options)
        {

        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Followers> Followers { get; set; }
        public DbSet<Sessions> Sessions { get; set; }
        public DbSet<UserFollows> UserFollows { get; set; }


    }//class
}//namespace
