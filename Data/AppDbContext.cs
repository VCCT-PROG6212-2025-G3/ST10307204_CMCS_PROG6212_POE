// Data/AppDbContext.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CMCS_PROG6212_POE.Models;

namespace CMCS_PROG6212_POE.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<LecturerModel> Lecturers { get; set; }
        public DbSet<CoordinatorModel> Coordinators { get; set; }
        public DbSet<ManagerModel> Managers { get; set; }
        public DbSet<HRModel> HRs { get; set; }
        public DbSet<ClaimModel> Claims { get; set; }
        public DbSet<DocumentModel> Documents { get; set; }
        public DbSet<ApprovalModel> Approvals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========================= USERS =========================
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Role).HasConversion<string>();
            });

            // ========================= ONE-TO-ONE RELATIONSHIPS =========================
            modelBuilder.Entity<User>()
                .HasOne(u => u.Lecturer)
                .WithOne(l => l.User)
                .HasForeignKey<LecturerModel>(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Coordinator)
                .WithOne(c => c.User)
                .HasForeignKey<CoordinatorModel>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Manager)
                .WithOne(m => m.User)
                .HasForeignKey<ManagerModel>(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(u => u.HR)
                .WithOne(h => h.User)
                .HasForeignKey<HRModel>(h => h.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ========================= CLAIMS =========================
            modelBuilder.Entity<ClaimModel>(entity =>
            {
                entity.HasKey(c => c.ClaimId);
               
                entity.HasOne(c => c.User)
                      .WithMany(u => u.Claims)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.Approval)
                      .WithOne(a => a.Claim)
                      .HasForeignKey<ApprovalModel>(a => a.ClaimId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<ClaimModel>(entity =>
            {
                entity.Property(c => c.HoursWorked)
                      .HasColumnType("decimal(18,2)");

                entity.Property(c => c.HourlyRate)
                      .HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<LecturerModel>(entity =>
            {
                entity.Property(l => l.HourlyRate)
                      .HasColumnType("decimal(18,2)");
            });

            // ========================= DOCUMENTS =========================
            modelBuilder.Entity<DocumentModel>(entity =>
            {
                entity.HasKey(d => d.DocumentId);

                entity.HasOne(d => d.Claim)
                      .WithMany(c => c.Documents)
                      .HasForeignKey(d => d.ClaimId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ========================= APPROVALS — FIXED CASCADE CONFLICT =========================
            modelBuilder.Entity<ApprovalModel>(entity =>
            {
                entity.HasKey(a => a.ApprovalId);

                // These two MUST be RESTRICT to avoid cascade cycle
                entity.HasOne(a => a.VerifiedBy)
                      .WithMany()
                      .HasForeignKey(a => a.VerifiedById)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.ApprovedBy)
                      .WithMany()
                      .HasForeignKey(a => a.ApprovedById)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ========================= SEED DATA =========================
            var hasher = new PasswordHasher<User>();

            var hr = new User { UserId = 1, FirstName = "HR", LastName = "Admin", Email = "hr@cmcs.com", Role = UserRole.HR };
            hr.PasswordHash = hasher.HashPassword(hr, "hr123");

            var coordinator = new User { UserId = 2, FirstName = "Luis", LastName = "Sousa", Email = "coordinator@cmcs.com", Role = UserRole.Coordinator };
            coordinator.PasswordHash = hasher.HashPassword(coordinator, "coord123");

            var manager = new User { UserId = 3, FirstName = "Ligia", LastName = "Lorenzini", Email = "manager@cmcs.com", Role = UserRole.Manager };
            manager.PasswordHash = hasher.HashPassword(manager, "mgr123");

            modelBuilder.Entity<User>().HasData(hr, coordinator, manager);
            modelBuilder.Entity<HRModel>().HasData(
             new HRModel { HRId = -1, UserId = 1 }  // Negative ID = seed only
            );

            modelBuilder.Entity<CoordinatorModel>().HasData(
                new CoordinatorModel { CoordinatorId = -2, UserId = 2 }
            );

            modelBuilder.Entity<ManagerModel>().HasData(
                new ManagerModel { ManagerId = -3, UserId = 3 }
            );
        }
    }
}