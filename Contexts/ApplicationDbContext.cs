using jwt_security_token_handler_asymmetric.Models;
using Microsoft.EntityFrameworkCore;

namespace jwt_security_token_handler_asymmetric.Contexts
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            :base(options)
        {            
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
    }
}