using Microsoft.EntityFrameworkCore;
using TechMoveApp.Models;

namespace TechMoveApp.Data
{
    public class TechMoveDbContext : DbContext
    {
        public TechMoveDbContext(DbContextOptions<TechMoveDbContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
    }
}
