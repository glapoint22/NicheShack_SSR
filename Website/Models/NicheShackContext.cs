using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Models
{
    public class NicheShackContext : IdentityDbContext<Customer, IdentityRole<string>, string>
    {
        public NicheShackContext(DbContextOptions<NicheShackContext> options)
            : base(options)
        {
        }

        // Tables
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Filter> Filters { get; set; }
        public virtual DbSet<FilterOption> FilterOptions { get; set; }
        public virtual DbSet<List> Lists { get; set; }
        public virtual DbSet<ListCollaborator> ListCollaborators { get; set; }
        public virtual DbSet<ListProduct> ListProducts { get; set; }
        public virtual DbSet<Niche> Niches { get; set; }
        public virtual DbSet<OrderProduct> OrderProducts { get; set; }
        public virtual DbSet<PriceRange> PriceRanges { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductContent> ProductContent { get; set; }
        public virtual DbSet<ProductFilter> ProductFilters { get; set; }
        public virtual DbSet<ProductMedia> ProductMedia { get; set; }
        public virtual DbSet<ProductOrder> ProductOrders { get; set; }
        public virtual DbSet<ProductPricePoint> ProductPricePoints { get; set; }
        public virtual DbSet<ProductReview> ProductReviews { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");


            // Categories
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever();

                entity.Property(e => e.Icon)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });


            // Customers
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable(name: "Customers");
                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.ReviewName)
                    .IsRequired()
                    .IsUnicode(false);
            });


            // Filters
            modelBuilder.Entity<Filter>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });


            // FilterOptions
            modelBuilder.Entity<FilterOption>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.Filter)
                    .WithMany(p => p.FilterOptions)
                    .HasForeignKey(d => d.FilterId)
                    .HasConstraintName("FK_FilterOptions_Filters");
            });


            // Lists
            modelBuilder.Entity<List>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                   .IsRequired()
                   .HasMaxLength(255)
                   .IsUnicode(false);

                entity.Property(e => e.Description)
                    .IsRequired(false)
                    .HasColumnType("varchar(max)")
                    .IsUnicode(false);

                entity.Property(e => e.CollaborateId)
                   .IsRequired()
                   .HasMaxLength(32)
                   .IsUnicode(false);
            });


            // ListCollaborators
            modelBuilder.Entity<ListCollaborator>(entity =>
            {
                entity.Property(e => e.CustomerId)
                   .HasMaxLength(10)
                   .IsUnicode(false);

                entity.Property(e => e.ListId)
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.IsOwner)
                   .IsRequired();

                entity.Property(e => e.ListId)
                   .IsRequired();

                entity.Property(e => e.CustomerId)
                   .IsRequired();

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.ListCollaborators)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_ListCollaborators_Customers");

                entity.HasOne(d => d.List)
                    .WithMany(p => p.Collaborators)
                    .HasForeignKey(d => d.ListId)
                    .HasConstraintName("FK_ListCollaborators_Lists");
            });


            // ListProducts
            modelBuilder.Entity<ListProduct>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.CollaboratorId })
                    .HasName("PK_ListProducts");

                entity.Property(e => e.ProductId)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CollaboratorId)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.DateAdded)
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ListProducts)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_ListProducts_Products");

                entity.HasOne(d => d.Collaborator)
                    .WithMany(p => p.ListProducts)
                    .HasForeignKey(d => d.CollaboratorId)
                    .HasConstraintName("FK_ListProducts_ListCollaborators");
            });


            // Niches
            modelBuilder.Entity<Niche>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever();

                entity.Property(e => e.Icon)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Niches)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_Niches_Categories");
            });


            // OrderProducts
            modelBuilder.Entity<OrderProduct>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.OrderId })
                    .HasName("PK_OrderProducts");

                entity.Property(e => e.Id)
                   .HasMaxLength(25)
                   .IsUnicode(false)
                   .ValueGeneratedNever();

                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.Type)
                    .IsRequired();

                entity.Property(e => e.OrderId)
                    .IsRequired();

                entity.Property(e => e.IsMain)
                   .IsRequired();


                entity.HasOne(d => d.ProductOrder)
                    .WithMany(p => p.OrderProducts)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_OrderProducts_ProductOrders");
            });


            // PriceRanges
            modelBuilder.Entity<PriceRange>(entity =>
            {
                entity.Property(e => e.Label)
                   .IsRequired();

                entity.Property(e => e.Min)
                   .IsRequired();

                entity.Property(e => e.Max)
                   .IsRequired();
            });


            // Products
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("varchar(max)");

                entity.Property(e => e.Hoplink)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Image)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ShareImage)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.UrlTitle)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.Niche)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.NicheId)
                    .HasConstraintName("FK_Products_Niches");
            });


            // ProductContent
            modelBuilder.Entity<ProductContent>(entity =>
            {
                entity.Property(e => e.ProductId)
                    .HasMaxLength(10)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.Type)
                    .IsRequired();

                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.PriceIndices)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .IsRequired();

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductContent)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_ProductContent_Products");
            });


            // ProductFilters
            modelBuilder.Entity<ProductFilter>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever();

                entity.Property(e => e.ProductId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.FilterOption)
                    .WithMany(p => p.ProductFilters)
                    .HasForeignKey(d => d.FilterOptionId)
                    .HasConstraintName("FK_ProductFilters_FilterOptions");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductFilters)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_ProductFilters_Products");
            });


            // ProductMedia
            modelBuilder.Entity<ProductMedia>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.Url })
                    .HasName("PK_ProductMedia");

                entity.Property(e => e.ProductId)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Url)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Thumbnail)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.Type)
                    .IsRequired();

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductMedia)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_ProductMedia_Products");
            });


            // ProductOrders
            modelBuilder.Entity<ProductOrder>(entity =>
            {
                entity.Property(e => e.Id)
                   .HasMaxLength(21)
                   .IsUnicode(false)
                   .ValueGeneratedNever();

                entity.Property(e => e.CustomerId)
                   .HasMaxLength(10)
                   .IsRequired()
                   .IsUnicode(false);

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .IsRequired();

                entity.Property(e => e.PaymentMethod)
                    .IsRequired();

                entity.Property(e => e.ProductId)
                    .IsRequired(true);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.ProductOrders)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_ProductOrders_Customers");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductOrders)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_ProductOrders_Products");
            });


            // ProductPricePoints
            modelBuilder.Entity<ProductPricePoint>(entity =>
            {
                entity.Property(e => e.ProductId)
                    .HasMaxLength(10)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.Frequency)
                    .IsRequired();

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductPricePoints)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_ProductPricePoints_Products");
            });


            // ProductReviews
            modelBuilder.Entity<ProductReview>(entity =>
            {
                entity.Property(e => e.ProductId)
                    .HasMaxLength(10)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.CustomerId)
                    .HasMaxLength(10)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()")
                    .IsRequired();

                entity.Property(e => e.Text)
                    .HasColumnType("varchar(max)")
                    .IsUnicode(false)
                    .IsRequired();

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductReviews)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_ProductReviews_Products");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.ProductReviews)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_ProductReviews_Customers");
            });


            // RefreshTokens
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.CustomerId })
                    .HasName("PK_RefreshTokens");

                entity.Property(e => e.Id)
                   .HasMaxLength(255)
                   .IsUnicode(false)
                   .ValueGeneratedNever();

                entity.Property(e => e.CustomerId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .ValueGeneratedNever()
                    .IsUnicode(false);

                entity.Property(e => e.Expiration)
                    .HasColumnType("datetime")
                    .IsRequired();

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.RefreshTokens)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_RefreshTokens_Customers");
            });
        }
    }
}
