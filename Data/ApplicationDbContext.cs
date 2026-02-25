using InventoryCRM.Models;
using InventoryCRM.Models.UnitModels;
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
        public DbSet<UnitInstalled> UnitsInstalled { get; set; }
        public DbSet<UnitReserved> UnitsReserved { get; set; }

        public DbSet<Deposit> Deposits { get; set; }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
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

            modelBuilder.Entity<UnitInstalled>(entity =>
            {
                entity.ToTable("UnitsInstalled");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnType("varchar(100)")
                    .IsRequired();
                entity.Property(e => e.CustomerId).IsRequired();
                entity.Ignore(e => e.IsExpanded);
            });

            modelBuilder.Entity<UnitReserved>(entity =>
            {
                entity.ToTable("UnitsReserved");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnType("varchar(100)")
                    .IsRequired();
                entity.Property(e => e.CustomerId).IsRequired();
                entity.Ignore(e => e.IsExpanded);
            });

            var defaultDepositId = Guid.Parse("00000000-0000-0000-0000-000000000001");

            modelBuilder.Entity<Deposit>().HasData(new Deposit
            {
                Id = defaultDepositId,
                Name = "Main Deposit",
                IsExpanded = false,
                Worker = null
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

            modelBuilder.Entity<Worker>(entity =>
            {
                entity.ToTable("Workers");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Workername)
                    .HasMaxLength(100)
                    .HasColumnType("varchar(100)")
                    .IsRequired();
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customers");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(entity => entity.IDNO)
                    .HasMaxLength(20)
                    .HasColumnType("varchar(20)")
                    .IsRequired();

                entity.Property(e => e.Name)
                    .HasMaxLength(200)
                    .HasColumnType("varchar(200)")
                    .IsRequired();

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .HasColumnType("varchar(500)")
                    .IsRequired(false);

                entity.Property(entity => entity.PhoneNumber)
                    .HasMaxLength(20)
                    .HasColumnType("varchar(20)")
                    .IsRequired(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(200)
                    .HasColumnType("varchar(200)")
                    .IsRequired(false);

                entity.Property(e => e.Description)
                    .HasMaxLength(1000)
                    .HasColumnType("varchar(1000)")
                    .IsRequired(false); 
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Description)
                      .HasMaxLength(1000)
                      .HasColumnType("varchar(1000)");

                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();

                entity.Property(e => e.Status)
                      .HasMaxLength(50)
                      .HasColumnType("varchar(50)")
                      .IsRequired()
                      .HasDefaultValue(OrderStatus.New);

                entity.Property(e => e.CustomersId).IsRequired();
                entity.HasOne(o => o.Customers)
                      .WithMany(c => c.Orders)
                      .HasForeignKey(o => o.CustomersId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.WorkerId).IsRequired(false);
                entity.HasOne(o => o.Worker)
                      .WithMany()
                      .HasForeignKey(o => o.WorkerId)
                      .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}
