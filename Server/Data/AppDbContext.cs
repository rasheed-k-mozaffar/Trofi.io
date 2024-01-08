using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Trofi.io.Server.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    #region DB SETS
    public DbSet<Cart> Carts { get; set; }
    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<DishImage> Images { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<CustomerReview> CustomerReviews { get; set; }
    #endregion

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }

    /// <summary>
    /// Db context configuration goes here
    /// </summary>
    /// <param name="optionsBuilder"></param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        // Telling EF Core that we want to use lazy loading
        optionsBuilder.UseLazyLoadingProxies();
    }

    /// <summary>
    /// Responsible for configuring the entity relations in the database
    /// </summary>
    /// <param name="builder"></param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // configure the relation between the cart and the app user
        // one - to - one
        builder.Entity<AppUser>()
            .HasOne(p => p.Cart)
            .WithOne(p => p.CartOwner)
            .HasForeignKey<Cart>(p => p.AppUserId);

        // one - to - many: A signle cart can contain multiple items
        builder.Entity<Cart>()
            .HasMany(p => p.Items)
            .WithOne();

        // one - to - many: A dish can have multiple images
        builder.Entity<MenuItem>()
            .HasMany(p => p.DishImages)
            .WithOne()
            .HasForeignKey(p => p.MenuItemId);

        // many - to - one: A user can have many reviews
        builder.Entity<AppUser>()
            .HasMany(p => p.Reviews)
            .WithOne()
            .HasForeignKey(p => p.AppUserId);

        // one - to - many: A review can belong to one item, and one item can have multiple reviews
        builder.Entity<CustomerReview>()
            .HasOne(p => p.ReviewedItem)
            .WithMany(p => p.CustomerReviews);

        // one - to - many: A category can have many products, while a product can belong to only one category
        builder.Entity<Category>()
            .HasMany(p => p.Items)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId);
    }
}
