// Data/AppDbContext.cs
using System.Collections.Generic;
using System.Reflection.Emit;
using CMCS_PROG6212_POE.Models;
using Microsoft.EntityFrameworkCore;

namespace CMCS_PROG6212_POE.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<ClaimModel> Claims { get; set; }
        public DbSet<DocumentModel> Documents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "HR",
                    LastName = "Admin",
                    Email = "hr@cmcs.com",
                    PasswordHash = hasher.HashPassword(null, "hr123"),
                    Role = UserRole.HR
                },
                new User
                {
                    Id = 2,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john@cmcs.com",
                    PasswordHash = hasher.HashPassword(null, "lec123"),
                    Role = UserRole.Lecturer,
                    HourlyRate = 350.00m
                },
                new User
                {
                    Id = 3,
                    FirstName = "Luis",
                    LastName = "De Sousa",
                    Email = "coordinator@cmcs.com",
                    PasswordHash = hasher.HashPassword(null, "coord123"),
                    Role = UserRole.Coordinator
                },
                new User
                {
                    Id = 4,
                    FirstName = "Ligia",
                    LastName = "Lorenzini",
                    Email = "manager@cmcs.com",
                    PasswordHash = hasher.HashPassword(null, "mgr123"),
                    Role = UserRole.Manager
                }
            );

            // Relationships
            modelBuilder.Entity<ClaimModel>()
                .HasOne(c => c.User)
                .WithMany(u => u.Claims)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DocumentModel>()
                .HasOne(d => d.Claim)
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.ClaimId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApprovalModel>()
                .HasOne(a => a.Claim)
                .WithOne(c => c.Approval)
                .HasForeignKey<ApprovalModel>(a => a.ClaimId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}