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

            // 1. Claim belongs to a User
            modelBuilder.Entity<ClaimModel>()
                .HasOne(c => c.User)
                .WithMany(u => u.Claims)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 2. Document → Claim
            modelBuilder.Entity<DocumentModel>()
                .HasOne(d => d.Claim)
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.ClaimId)
                .OnDelete(DeleteBehavior.Cascade);

            // 3. Approval → Claim (One-to-One)
            modelBuilder.Entity<ApprovalModel>()
                .HasOne(a => a.Claim)
                .WithOne(c => c.Approval)
                .HasForeignKey<ApprovalModel>(a => a.ClaimId)
                .OnDelete(DeleteBehavior.Cascade);

            // 4. Approval → VerifiedBy / ApprovedBy (can be Coordinator or Manager)
            modelBuilder.Entity<ApprovalModel>()
                .HasOne(a => a.VerifiedBy)
                .WithMany()
                .HasForeignKey(a => a.VerifiedById)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ApprovalModel>()
                .HasOne(a => a.ApprovedBy)
                .WithMany()
                .HasForeignKey(a => a.ApprovedById)
                .OnDelete(DeleteBehavior.SetNull);

            // 5. One-to-One Relationships (User → Role-Specific Entity)
            modelBuilder.Entity<HRModel>()
                .HasOne(h => h.User)
                .WithOne(u => u.HR)
                .HasForeignKey<HRModel>(h => h.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CoordinatorModel>()
                .HasOne(c => c.User)
                .WithOne(u => u.Coordinator)
                .HasForeignKey<CoordinatorModel>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ManagerModel>()
                .HasOne(m => m.User)
                .WithOne(u => u.Manager)
                .HasForeignKey<ManagerModel>(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LecturerModel>()
                .HasOne(l => l.User)
                .WithOne(u => u.Lecturer)
                .HasForeignKey<LecturerModel>(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 6. CRITICAL: Only Lecturers can submit claims → Enforce via DB constraint
            modelBuilder.Entity<ClaimModel>()
                .HasCheckConstraint("CK_Claim_Submitter_Role",
                    "EXISTS (SELECT 1 FROM Users u WHERE u.UserId = UserId AND u.Role = 'Lecturer')");

            // 7. Seed Users (HR, Coordinator, Manager — no claims allowed)
            var hasher = new PasswordHasher<User>();

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    FirstName = "HR",
                    LastName = "Admin",
                    Email = "hr@cmcs.com",
                    PasswordHash = hasher.HashPassword(null!, "hr123"),
                    Role = UserRole.HR
                },
                new User
                {
                    UserId = 2,
                    FirstName = "Luis",
                    LastName = "De Sousa",
                    Email = "coordinator@cmcs.com",
                    PasswordHash = hasher.HashPassword(null!, "coord123"),
                    Role = UserRole.Coordinator
                },
                new User
                {
                    UserId = 3,
                    FirstName = "Ligia",
                    LastName = "Lorenzini",
                    Email = "manager@cmcs.com",
                    PasswordHash = hasher.HashPassword(null!, "mgr123"),
                    Role = UserRole.Manager
                }
            );

            // 8. Seed Role-Specific Records
            modelBuilder.Entity<HRModel>().HasData(
                new HRModel { HRId = 1, UserId = 1 }
            );

            modelBuilder.Entity<CoordinatorModel>().HasData(
                new CoordinatorModel { CoordinatorId = 1, UserId = 2 }
            );

            modelBuilder.Entity<ManagerModel>().HasData(
                new ManagerModel { ManagerId = 1, UserId = 3 }
            );

            // No Lecturer seeded yet — HR will create them later
        }
    }
}