using InventoryCRM.Models;
using Microsoft.EntityFrameworkCore;



namespace InventoryCRM.Data
{
    //public class ApplicationDbContext
    //{
    // public DbSet<TodoItem> Tasks { get; set; }
    //}

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<TodoItem> Todos { get; set; }
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
        }
    }
}
