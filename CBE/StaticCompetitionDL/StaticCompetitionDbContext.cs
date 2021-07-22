using StaticCompetitionModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaticCompetitionDL
{
    public class StaticCompetitionDbContext : DbContext
    {
        public StaticCompetitionDbContext(DbContextOptions options) : base(options) { }
        protected StaticCompetitionDbContext() { }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Competition> Competitions { get; set; }
        public DbSet<CompetitionStat> CompetitionStats { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(user => user.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<User>()
                .HasIndex(user => user.Auth0Id)
                .IsUnique();
            modelBuilder.Entity<Category>()
                .Property(cat => cat.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Category>()
                .HasIndex(cat => cat.Name)
                .IsUnique();
            modelBuilder.Entity<Competition>()
                .Property(comp => comp.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<CompetitionStat>()
                .HasKey(cS => new { cS.UserId, cS.CompetitionId });
        }
    }
}
