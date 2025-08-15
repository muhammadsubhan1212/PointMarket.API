using Microsoft.EntityFrameworkCore;
using PointMarket.API.Models;

namespace PointMarket.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.ID);
                
                entity.Property(e => e.UserID)
                    .HasDefaultValueSql("NEWID()");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.UserType)
                    .HasDefaultValue("Customer");

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.Property(e => e.IsEmailVerified)
                    .HasDefaultValue(false);

                entity.Property(e => e.IsPhoneNumberVerified)
                    .HasDefaultValue(false);

                entity.Property(e => e.IsDeleted)
                    .HasDefaultValue(false);

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedDate)
                    .HasDefaultValueSql("GETDATE()");

                // Indexes
                entity.HasIndex(e => e.UserID).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.PhoneNumber).IsUnique();
                entity.HasIndex(e => e.UserType);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.IsDeleted);
                entity.HasIndex(e => new { e.UserType, e.IsActive, e.IsDeleted });
                entity.HasIndex(e => new { e.PhoneNumber, e.IsPhoneNumberVerified });


                // Self-referencing relationships
                entity.HasOne(e => e.CreatedByUser)
                    .WithMany(e => e.CreatedUsers)
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.UpdatedByUser)
                    .WithMany(e => e.UpdatedUsers)
                    .HasForeignKey(e => e.UpdatedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(tb => tb.HasCheckConstraint("CK_User_UserType", 
                    "UserType IN ('Guest', 'Customer', 'SuperAdmin', 'Manager', 'StaffMember', 'InventoryManager')"));

            });
        }
    }
}
