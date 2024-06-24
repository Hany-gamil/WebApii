using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace WebApii_2.Models
    
{
    public class applicationDbContext:IdentityDbContext<applicationUser>
    {
        public applicationDbContext(DbContextOptions<applicationDbContext>options):base(options)
        {

        }
        public DbSet<student> students { get; set; }

    }
}
