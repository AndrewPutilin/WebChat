using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using WebChat.DAL.Domain;

namespace WebChat.Models
{
    public class OperatorContext : DbContext
    {
        private string ConnectionString { get; set; }

        public DbSet<Chat> Chats { get; set; }
        public DbSet<Messаge> Messages { get; set; }
        public DbSet<CheckLiveBackUser> CheckLiveBackUsers { get; set; }

        public OperatorContext() : this(GetConnectionString())
        {
            Database.EnsureCreated();
        }

        public OperatorContext(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            dbContextOptionsBuilder.UseSqlServer(ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Messаge>()
                .HasOne(c => c.Chat)
                .WithMany();
        }

        protected static string GetConnectionString()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

            return configuration.GetConnectionString("SecondConnection");
        }
    }
}
