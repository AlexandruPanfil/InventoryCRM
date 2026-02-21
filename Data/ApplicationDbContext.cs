using InventoryCRM.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;



namespace InventoryCRM.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<TodoItem> Todos { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Deposit> Deposits { get; set; }
        public DbSet<User> AppUsers { get; set; }
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
                entity.Ignore(e => e.IsExpanded);
            });

            var defaultDepositId = Guid.Parse("00000000-0000-0000-0000-000000000001");

            modelBuilder.Entity<Deposit>().HasData(new Deposit
            {
                Id = defaultDepositId,
                Name = "Main Deposit",
                IsExpanded = false,
                User = null
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
