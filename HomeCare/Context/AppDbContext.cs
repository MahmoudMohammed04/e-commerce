using HomeCare.Models.AuthSchema;
using HomeCare.Models.LocationSchema;
using HomeCare.Models.ProductSchema;
using HomeCare.Models.UserSchema;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Reflection.Emit;

namespace HomeCare.Context
{
    public class AppDbContext:IdentityDbContext<User,Role,string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().ToTable("users");
            builder.Entity<User>().HasIndex(x => x.Email).IsUnique();
            builder.Entity<User>().HasIndex(x => x.GoogleId).IsUnique();

            builder.Entity<Role>().ToTable("roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("user_roles");

            builder.Ignore<IdentityUserToken<string>>();
            builder.Ignore<IdentityUserClaim<string>>();
            builder.Ignore<IdentityUserLogin<string>>();
            builder.Ignore<IdentityRoleClaim<string>>();

            builder.Entity<RefreshToken>().ToTable("refresh_tokens");
            builder.Entity<RefreshToken>().HasIndex(x => x.CreatedByIp);
            builder.Entity<RefreshToken>().HasIndex(x => x.Token);


            builder.Entity<Models.ProductSchema.CategoryAttribute>()
        .HasKey(x => (new { x.CategoryId, x.AttributeId }));

            builder.Entity<Models.ProductSchema.CategoryAttribute>()
                .HasOne(x => x.Category)
                .WithMany(x => x.CategoryAttributes)
                .HasForeignKey((x => x.CategoryId));

            builder.Entity<Models.ProductSchema.CategoryAttribute>()
                .HasOne(x => x.Attribute)
                .WithMany( x => x.categoryAttributes)
                .HasForeignKey(x => x.AttributeId);


            // ProductAttribute composite key
            builder.Entity<ProductAttribute>()
                .HasKey(x => new { x.ProductId, x.AttributeValueId });

            builder.Entity<ProductAttribute>()
                .HasOne(x => x.Product)
                .WithMany(x => x.ProductAttributes)
                .HasForeignKey(x => x.ProductId);

            builder.Entity<ProductAttribute>()
                .HasOne(x => x.AttributeValue)
                .WithMany(x => x.ProductAttributes)
                .HasForeignKey(x => x.AttributeValueId);

            builder.Entity<FacetIndexTable>()
                .HasIndex(x => new { x.AttributeId, x.AttributeValueString });

            builder.Entity<FacetIndexTable>()
                .HasIndex(x => new { x.AttributeId, x.AttributeValueNumeric });

            builder.Entity<Cart>()
                .HasKey(x => new { x.UserId, x.ProductId });
            builder.Entity<Cart>()
                .HasOne(x => x.User)
                .WithMany(x => x.CartItems)
                .HasForeignKey(x => x.UserId);
            builder.Entity<Cart>()
                .HasOne(x => x.Product)
                .WithMany(x => x.Carts)
                .HasForeignKey(x => x.ProductId);

            builder.Entity<Order>()
                .HasIndex(x => x.UserId );

            builder.Entity<OrderProduct>()
                .HasKey(x => new { x.OrderId, x.ProductId });
            builder.Entity<OrderProduct>()
                .HasOne(x => x.Product)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.ProductId);
            builder.Entity<OrderProduct>()
                .HasOne(x => x.Order)
                .WithMany(x => x.OrderProducts)
                .HasForeignKey(x => x.OrderId);

            builder.Entity<Brand>()
                .HasIndex(x => x.CategoryID);


        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<AttributeValue> AttributeValues { get; set; }
        public DbSet<AttributeType> AttributeTypes { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<Models.ProductSchema.CategoryAttribute> CategoryAttributes { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<FacetIndexTable> FacetIndexTables  { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders  { get; set; }

        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<Review> Reviews { get; set; }       

        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
    }
}
