// Models/ApplicationDbContext.cs
using Microsoft.Crm.Sdk.Messages;
using System.Data.Entity;

namespace UserManagement.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("DefaultConnection")
        {

        }
        public DbSet<UserDetail> UserDetails { get; set; }
        public DbSet<PasswordDetail> PasswordDetails { get; set; }
    }

}
