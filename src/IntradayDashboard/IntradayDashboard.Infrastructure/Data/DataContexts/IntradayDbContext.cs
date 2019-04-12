using System;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using IntradayDashboard.Core.Model.Entities;
using IntradayDashboard.Core.Model.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;

namespace IntradayDashboard.Infrastructure.Data.DataContexts
{
    public class IntradayDbContext : IdentityDbContext<User, Role, int,
     IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, 
     IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public IntradayDbContext(DbContextOptions<IntradayDbContext> options ) : base(options){

        }

        public DbSet<Offer> Offer { get; set; }
        public DbSet<Consumption> Consumption {get;set;}
        public DbSet<Tenant> Tenant { get; set;}
        public DbSet<User> User {get;set;}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Offer>(ConfigureOffer);
            builder.Entity<Consumption>(ConfigureConsumption);
            builder.Entity<User>();
        }

        private void ConfigureOffer(EntityTypeBuilder<Offer> builder)
        {
            builder.Property(cio => cio.Data)
                .HasMaxLength(100)
                .IsRequired();
        }
        public void ConfigureConsumption(EntityTypeBuilder<Consumption> builder) {
            builder.Property(cio => cio.Data)
                .HasMaxLength(100)
                .IsRequired();
        }

        public virtual void Save()  
        {  
            base.SaveChanges();  
        }  

        public string UserProvider  
        {  
            get  
            {  
                if (!string.IsNullOrEmpty(WindowsIdentity.GetCurrent().Name))  
                return WindowsIdentity.GetCurrent().Name.Split('\\')[1];  
                return string.Empty;  
            }  
        }  

        /// Shows local date not utc date.
        public Func<DateTime> TimestampProvider { get; set; } = ()  => DateTime.Now;  
        public override int SaveChanges()  
        {  
            TrackChanges();  
            return base.SaveChanges();  
        }  
        
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())  
        {  
            TrackChanges();  
            return await base.SaveChangesAsync(cancellationToken);  
        }  
    
        ///Tracking Configuration
        private void TrackChanges()  
        {  
            foreach (var entry in this.ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted))  
            {  
                if (entry.Entity is IAuditEntity)  
                {  
                    var auditable = entry.Entity as IAuditEntity;  
                    if (entry.State == EntityState.Added)  
                    {
                        auditable.CreatedBy = UserProvider;
                        auditable.CreatedOn = TimestampProvider();
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        auditable.UpdatedBy = UserProvider;
                        auditable.UpdatedOn = TimestampProvider();
                    }
                    else {
                        auditable.DeletedBy = UserProvider;
                        auditable.DeletedOn = TimestampProvider();
                    }
                }  
            }  
        }


    }
}