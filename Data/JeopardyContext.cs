using Data.Models;
using EntityFramework.DbContextScope.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class JeopardyContext : DbContext, IDbContext
    {
        public JeopardyContext(DbContextOptions<JeopardyContext> options) : base(options)
        {
        }

        public JeopardyContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string? connectionString = "server=127.0.0.1;uid=root;database=jeopardy"; // TODO
                optionsBuilder.UseMySql(connectionString, MySqlServerVersion.AutoDetect(connectionString));
            }
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Session> Sessions { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Professor> Professors { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Theme> Themes { get; set; }
        public DbSet<QuestionOfPackage> QuestionOfPackages { get; set; }

        public void OnSave() => SaveChanges();

        public void OnSaveAsync() => SaveChangesAsync();

        public void OnDispose() => Dispose();
    }
}
