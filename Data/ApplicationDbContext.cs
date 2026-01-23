using InventoryCRM.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;



namespace InventoryCRM.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<TodoItem> Todos { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Deposit> Deposits { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TodoItem>(entity =>
            {
                entity.ToTable("Todos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Todo)
                    .HasMaxLength(200)
                    .HasColumnType("varchar(200)")
                    .IsRequired();
                entity.Property(e => e.IsCompleted).IsRequired();
                entity.Property(e => e.Date).IsRequired();
            });

            modelBuilder.Entity<Unit>(entity =>
            {
                entity.ToTable("Units");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnType("varchar(100)")
                    .IsRequired();
                entity.Property(e=> e.DepositId).IsRequired();
            });

            modelBuilder.Entity<Deposit>(entity =>
            {
                entity.ToTable("Deposits");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnType("varchar(100)")
                    .IsRequired();
                entity.Ignore(e => e.IsExpanded);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Username)
                    .HasMaxLength(100)
                    .HasColumnType("varchar(100)")
                    .IsRequired();
            });
        }
    }
}
