using Homy.Domin.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Homy.Infurastructure.Data
{
    // بنستخدم IdentityDbContext<User, IdentityRole<Guid>, Guid> 
    // عشان الـ User بتاعنا يورث من IdentityUser<Guid>
    public class HomyContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public HomyContext(DbContextOptions<HomyContext> options)
            : base(options)
        {
        }

        // ====================== DbSets ======================
        public DbSet<City> Cities { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<PropertyType> PropertyTypes { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyImage> PropertyImages { get; set; }
        public DbSet<PropertyAmenity> PropertyAmenities { get; set; }
        public DbSet<SavedProperty> SavedProperties { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<PropertyReview> PropertyReviews { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        // ====================== Model Configuration ======================
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // مهم جداً للـ Identity

            // ====================== PropertyAmenity (Many-to-Many) ======================
            builder.Entity<PropertyAmenity>()
                .HasKey(pa => pa.Id); // استخدم الـ Id من BaseEntity

            builder.Entity<PropertyAmenity>()
                .HasIndex(pa => new { pa.PropertyId, pa.AmenityId })
                .IsUnique(); // عشان منكررش نفس العلاقة

            builder.Entity<PropertyAmenity>()
                .HasOne(pa => pa.Property)
                .WithMany(p => p.PropertyAmenities)
                .HasForeignKey(pa => pa.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PropertyAmenity>()
                .HasOne(pa => pa.Amenity)
                .WithMany(a => a.PropertyAmenities)
                .HasForeignKey(pa => pa.AmenityId)
                .OnDelete(DeleteBehavior.Cascade);

            // ====================== SavedProperty (Many-to-Many) ======================
            builder.Entity<SavedProperty>()
                .HasKey(sp => sp.Id); // استخدم الـ Id من BaseEntity

            builder.Entity<SavedProperty>()
                .HasIndex(sp => new { sp.UserId, sp.PropertyId })
                .IsUnique(); // عشان اليوزر ميحفظش نفس الإعلان مرتين

            builder.Entity<SavedProperty>()
                .HasOne(sp => sp.User)
                .WithMany(u => u.SavedProperties)
                .HasForeignKey(sp => sp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<SavedProperty>()
                .HasOne(sp => sp.Property)
                .WithMany(p => p.SavedByUsers)
                .HasForeignKey(sp => sp.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // ====================== Property Relations ======================
            builder.Entity<Property>()
                .HasOne(p => p.User)
                .WithMany(u => u.Properties)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict); // عشان منحذفش اليوزر لو حذفنا Property

            builder.Entity<Property>()
                .HasOne(p => p.City)
                .WithMany(c => c.Properties)
                .HasForeignKey(p => p.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Property>()
                .HasOne(p => p.District)
                .WithMany(d => d.Properties)
                .HasForeignKey(p => p.DistrictId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Property>()
                .HasOne(p => p.PropertyType)
                .WithMany(pt => pt.Properties)
                .HasForeignKey(p => p.PropertyTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Property>()
                .HasOne(p => p.Project)
                .WithMany(pr => pr.Properties)
                .HasForeignKey(p => p.ProjectId)
                .OnDelete(DeleteBehavior.SetNull); // لو حذفنا المشروع، الـ ProjectId يبقى null

            // ====================== PropertyImage ======================
            builder.Entity<PropertyImage>()
                .HasOne(pi => pi.Property)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // ====================== District - City ======================
            builder.Entity<District>()
                .HasOne(d => d.City)
                .WithMany(c => c.Districts)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.Cascade);

            // ====================== Project Relations ======================
            builder.Entity<Project>()
                .HasOne(pr => pr.City)
                .WithMany(c => c.Projects)
                .HasForeignKey(pr => pr.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Project>()
                .HasOne(pr => pr.District)
                .WithMany(d => d.Projects)
                .HasForeignKey(pr => pr.DistrictId)
                .OnDelete(DeleteBehavior.Restrict);

            // ====================== UserSubscription ======================
            builder.Entity<UserSubscription>()
                .HasOne(us => us.User)
                .WithMany(u => u.Subscriptions)
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserSubscription>()
                .HasOne(us => us.Package)
                .WithMany(p => p.Subscriptions)
                .HasForeignKey(us => us.PackageId)
                .OnDelete(DeleteBehavior.Restrict);

            // ====================== BaseEntity Relations (CreatedBy/UpdatedBy) ======================
            // SavedProperty Relations
            builder.Entity<SavedProperty>()
                .HasOne<User>(e => e.CreatedBy)
                .WithMany()
                .HasForeignKey(e => e.CreatedById)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SavedProperty>()
                .HasOne<User>(e => e.UpdatedBy)
                .WithMany()
                .HasForeignKey(e => e.UpdatedById)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // PropertyAmenity Relations
            builder.Entity<PropertyAmenity>()
                .HasOne<User>(e => e.CreatedBy)
                .WithMany()
                .HasForeignKey(e => e.CreatedById)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PropertyAmenity>()
                .HasOne<User>(e => e.UpdatedBy)
                .WithMany()
                .HasForeignKey(e => e.UpdatedById)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // باقي الـ Entities
            ConfigureBaseEntityRelations<City>(builder);
            ConfigureBaseEntityRelations<District>(builder);
            ConfigureBaseEntityRelations<PropertyType>(builder);
            ConfigureBaseEntityRelations<Amenity>(builder);
            ConfigureBaseEntityRelations<Property>(builder);
            ConfigureBaseEntityRelations<PropertyImage>(builder);
            ConfigureBaseEntityRelations<Package>(builder);
            ConfigureBaseEntityRelations<UserSubscription>(builder);
            ConfigureBaseEntityRelations<Project>(builder);
            ConfigureBaseEntityRelations<PropertyReview>(builder);
            ConfigureBaseEntityRelations<Notification>(builder);

            // ====================== PropertyReview Relations ======================
            builder.Entity<PropertyReview>()
                .HasOne(pr => pr.Property)
                .WithMany(p => p.Reviews)
                .HasForeignKey(pr => pr.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PropertyReview>()
                .HasOne(pr => pr.Admin)
                .WithMany()
                .HasForeignKey(pr => pr.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            // ====================== Notification Relations ======================
            builder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Notification>()
                .HasOne(n => n.Property)
                .WithMany()
                .HasForeignKey(n => n.PropertyId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // ====================== Indexes for Performance ======================
            builder.Entity<Property>()
                .HasIndex(p => p.CityId);

            builder.Entity<Property>()
                .HasIndex(p => p.DistrictId);

            builder.Entity<Property>()
                .HasIndex(p => p.PropertyTypeId);

            builder.Entity<Property>()
                .HasIndex(p => p.Status);

            builder.Entity<Property>()
                .HasIndex(p => p.IsFeatured);

            builder.Entity<Property>()
                .HasIndex(p => p.Price);

            builder.Entity<User>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique();

            builder.Entity<User>()
                .HasIndex(u => u.IsActive);

            // ====================== Decimal Precision Configuration ======================
            builder.Entity<Property>()
                .Property(p => p.Latitude)
                .HasPrecision(10, 7); // 10 digits total, 7 after decimal point (suitable for GPS)

            builder.Entity<Property>()
                .Property(p => p.Longitude)
                .HasPrecision(10, 7); // 10 digits total, 7 after decimal point (suitable for GPS)

            // ====================== Performance Indexes for Soft Delete ======================
            builder.Entity<Property>()
                .HasIndex(p => p.IsDeleted);

            builder.Entity<User>()
                .HasIndex(u => u.IsDeleted);

            builder.Entity<City>()
                .HasIndex(c => c.IsDeleted);

            builder.Entity<District>()
                .HasIndex(d => d.IsDeleted);

            builder.Entity<PropertyType>()
                .HasIndex(pt => pt.IsDeleted);

            // ====================== Query Filters (Soft Delete) ======================
            builder.Entity<City>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<District>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<PropertyType>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Amenity>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Property>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<PropertyImage>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Package>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<UserSubscription>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Project>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<SavedProperty>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<PropertyAmenity>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<PropertyReview>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Notification>().HasQueryFilter(e => !e.IsDeleted);
        }

        // ====================== SaveChanges Override للـ Audit ======================
        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAuditFields()
        {
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.Now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.Now;
                }
            }
        }

        // ====================== Helper Method للـ BaseEntity Relations ======================
        private void ConfigureBaseEntityRelations<TEntity>(ModelBuilder builder)
            where TEntity : BaseEntity
        {
            builder.Entity<TEntity>()
                .HasOne(e => e.CreatedBy)
                .WithMany()
                .HasForeignKey(e => e.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TEntity>()
                .HasOne(e => e.UpdatedBy)
                .WithMany()
                .HasForeignKey(e => e.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}