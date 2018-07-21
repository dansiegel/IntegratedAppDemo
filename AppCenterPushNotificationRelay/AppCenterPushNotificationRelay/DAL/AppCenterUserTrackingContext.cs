using System.Configuration;
using Microsoft.EntityFrameworkCore;

namespace AppCenterPushNotificationRelay.DAL
{
    internal class AppCenterUserTrackingContext : DbContext
    {
        public AppCenterUserTrackingContext() 
            : base(GetOptions())
        {
        }

        public DbSet<AppCenterPushRelay.Models.UserRegistration> UserRegistrations { get; set; }

        static string GetConnectionString() => 
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        static DbContextOptions GetOptions()
        {
            var builder = new DbContextOptionsBuilder();
            builder.UseSqlServer(GetConnectionString());
            return builder.Options;
        }
    }
}
