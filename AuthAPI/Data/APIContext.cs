using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.Configuration;

namespace AuthAPI.Data
{
    public class APIContext : DbContext
    {
        private static APIContext context = null;
        private static readonly object padlock = new object();
        public static IConfiguration Configuration { get; set; }
        private APIContext() { }
        public static APIContext SqlServer 
        {
            get
            {
                if (context == null)
                {
                    lock(padlock)
                    {
                        if(context == null)
                        {
                            context = new APIContext();
                        }
                    }
                }
                return context;
            }
        }
        public APIContext(DbContextOptions<APIContext> options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = Configuration.GetConnectionString("APIContextConnection");
                optionsBuilder.UseSqlServer(connectionString);                
            }
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);            
        }
        public DbSet<Models.User> AspNetUsers { get; set; }
    }
}