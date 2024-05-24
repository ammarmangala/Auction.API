using Auction_API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auction_API.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Jets" },
                new Category { Id = 2, Name = "Real Estate" },
                new Category { Id = 3, Name = "Watches" }
            );
            
            modelBuilder.Entity<AuctionItem>()
                .HasOne(ai => ai.Category)
                .WithMany(c => c.AuctionItems)
                .HasForeignKey(ai => ai.CategoryId);

            modelBuilder.Entity<Bid>()
                .HasOne(b => b.AuctionItem)
                .WithMany(ai => ai.Bids)
                .HasForeignKey(b => b.AuctionItemId);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<AuctionItem> AuctionItems { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
    }
}
