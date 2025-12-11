using DataBase;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Project.Data
{
    public class ApplicationContext : IdentityDbContext <ApplicationUser>
    {

        public ApplicationContext (DbContextOptions options) : base(options)
        {

        }

        public DbSet<Article> Articles { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Client> Clients { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Payment> Payment { get; set; }

        public DbSet<User> users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Article - Category relationship (many-to-one)
            modelBuilder.Entity<Article>()
                .HasOne(a => a.Category)
                .WithMany()
                .HasForeignKey(a => a.CategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            // Cart - Client relationship (many-to-one)
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Client)
                .WithMany()
                .HasForeignKey(c => c.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            // Cart - Article many-to-many via join table CartArticle
            modelBuilder.Entity<Cart>()
                .HasMany(c => c.listOfArticles)
                .WithMany()
                .UsingEntity(j => j.ToTable("CartArticles"));

            // Client - Order one-to-many relationship
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Client)
                .WithMany(c => c.OrderHistory)
                .HasForeignKey(o => o.ClientID)
                .OnDelete(DeleteBehavior.Restrict);

            // Order - Article many-to-many via join table OrderReceipt
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Receipt)
                .WithMany()
                .UsingEntity(j => j.ToTable("OrderReceipt"));

            // Payment - Order one-to-one relationship (or many-to-one if multiple payments per order)
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithMany()
                .HasForeignKey(p => p.OrderID)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
