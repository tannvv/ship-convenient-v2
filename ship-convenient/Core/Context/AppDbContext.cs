using Microsoft.EntityFrameworkCore;
using ship_convenient.Entities;
using System.Reflection;
using Route = ship_convenient.Entities.Route;

namespace ship_convenient.Core.Context
{
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public AppDbContext(DbContextOptions options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        #region Dbset
        public virtual DbSet<Account> Accounts => Set<Account>();
        public virtual DbSet<InfoUser> InfoUsers => Set<InfoUser>();
        public virtual DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public virtual DbSet<Route> Routes => Set<Route>();
        public virtual DbSet<ConfigApp> Configs => Set<ConfigApp>();
        public virtual DbSet<Discount> Discounts => Set<Discount>();
        public virtual DbSet<Notification> Notifications => Set<Notification>();
        public virtual DbSet<Package> Packages => Set<Package>();
        public virtual DbSet<Product> Products => Set<Product>();
        public virtual DbSet<Deposit> Deposits => Set<Deposit>();
        // public virtual DbSet<Role> Roles => Set<Role>();
        public virtual DbSet<Transaction> Transactions => Set<Transaction>();
        public virtual DbSet<TransactionPackage> TransactionPackages => Set<TransactionPackage>();

        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            string connectionString = _configuration.GetConnectionString("DevConnectionPartner");
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                connectionString = _configuration.GetConnectionString("DevConnectionPartner");
            }
            else
            {
                connectionString = _configuration.GetConnectionString("AzureConnection");
            }
            if (!string.IsNullOrEmpty(connectionString)) optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DevConnectionPartner"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
