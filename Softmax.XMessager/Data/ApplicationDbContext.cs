using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Softmax.XMessager.Data.Entities;
using Softmax.XMessager.Models;
//using Softmax.XMessager.Data.Entities;


namespace Softmax.XMessager.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Gateway> Gateways { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Request> Requests { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

           

        }
    }
}
