using CompanyManagerFinal.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyManagerFinal.Infastructure.Data
{
    public class DBContext : DbContext
    {
        public DbSet<CompanyModel> Companies { get; set; }
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<CompanyModel>().HasKey(c => c.Id);
        }
    }
}
