﻿// <auto-generated />
using System;
using Junjuria.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Junjuria.Infrastructure.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Junjuria.Infrastructure.Models.AppUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(256);

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName")
                        .HasMaxLength(64);

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("LastName")
                        .HasMaxLength(64);

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<string>("Town")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Junjuria.Infrastructure.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CategoryId");

                    b.Property<DateTime>("DateOfCreation");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("Junjuria.Infrastructure.Models.CommentSympathy", b =>
                {
                    b.Property<string>("SympathiserId");

                    b.Property<int>("CommentId");

                    b.Property<int>("Attitude");

                    b.HasKey("SympathiserId", "CommentId");

                    b.HasIndex("CommentId");

                    b.ToTable("CommentSympathies");
                });

            modelBuilder.Entity("Junjuria.Infrastructure.Models.Log", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateOfCreation");

                    b.Property<string>("EventDescription")
                        .IsRequired()
                        .HasMaxLength(256);

                    b.Property<string>("EventName")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.Property<int>("Importance");

                    b.Property<bool>("IsDeleted");

                    b.HasKey("Id");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("Junjuria.Infrastructure.Models.Manufacturer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateOfCreation");

                    b.Property<string>("Email");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<string>("PhoneNumber");

                    b.Property<string>("WebAddress");

                    b.HasKey("Id");

                    b.ToTable("Manufacturers");
                });

            modelBuilder.Entity("Junjuria.Infrastructure.Models.Order", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CustomerId");

                    b.Property<DateTime>("DateOfCreation");

                    b.Property<decimal>("DeliveryFee");

                    b.Property<bool>("IsDeleted");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Junjuria.Infrastructure.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CategoryId");

                    b.Property<DateTime>("DateOfCreation");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(10240);

                    b.Property<decimal>("Discount");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("MainPicURL");

                    b.Property<int>("ManufacturerId");

                    b.Property<long?>("MonthsWarranty");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<decimal>("Price");

                    b.Property<long>("Quantity");

                    b.Property<string>("ReviewURL");

                    b.Property<double>("Weight");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ManufacturerId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Junjuria.Infrastructure.Models.ProductCharacteristic", b =>
                {
                    b.Property<int>("ProductId");

                    b.Property<string>("Title");

                    b.Property<DateTime>("DateOfCreation");

                    b.Property<bool>("IsDeleted");

                    b.Property<double?>("NumericValue");

                    b.Property<string>("TextValue")
                        .HasMaxLength(64);

                    b.HasKey("ProductId", "Title");

                    b.ToTable("ProductCharacteristics");
                });

            modelBuilder.Entity("Junjuria.Infrastructure.Models.ProductComment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AuthorId");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasMaxLength(10240);

                    b.Property<DateTime>("DateOfCreation");

                    b.Property<bool>("IsDeleted");

                    b.Property<int>("ProductId");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductComments");
                });

            modelBuilder.Entity("Junjuria.Infrastructure.Models.ProductOrder", b =>
                {
                    b.Property<int>("ProductId");

                    b.Property<string>("OrderId");

                    b.Property<decimal>("CurrentPrice");

                    b.Property<int>("Quantity");

                    b.HasKey("ProductId", "OrderId");

                    b.HasIndex("OrderId");

                    b.ToTable("ProductOrders");
                });

            modelBuilder.Entity("Junjuria.Infrastructure.Models.ProductPicture", b =>
                {
                    b.Property<int>("ProductId");

                    b.Property<string>("PictureURL");

                    b.HasKey("ProductId", "PictureURL");

                    b.ToTable("ProductPictures");
                });

            modelBuilder.Entity("Junjuria.Infrastructure.Models.ProductVote", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<int>("ProductId");

                    b.Property<int>("Grade");

                    b.HasKey("UserId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductVotes");
                });

            modelBuilder.Entity("Junjuria.Infrastructure.Models.Recomendation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateOfCreation");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(512);

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(16);

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Recomendations");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Junjuria.Infrastructure.Models.Category", b =>
                {
                    b.HasOne("Junjuria.Infrastructure.Models.Category", "OuterCategory")
                        .WithMany("SubCategories")
                        .HasForeignKey("CategoryId");
                });

            modelBuilder.Entity("Junjuria.Infrastructure.Models.CommentSympathy", b =>
                {
                    b.HasOne("Junjuria.Infrastructure.Models.ProductComment", "Comment")
                        .WithMany("UsersAttitude")
                        .HasForeignKey("CommentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Junjuria.Infrastructure.Models.AppUser", "Sympathiser")
                        .WithMany("CommentSympaties")
                        .HasForeignKey("SympathiserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Junjuria.Infrastructure.Models.Order", b =>
                {
                    b.HasOne("Junjuria.Infrastructure.Models.AppUser", "Customer")
                        .WithMany("Orders")
                        .HasForeignKey("CustomerId");
                });

            modelBuilder.Entity("Junjuria.Infrastructure.Models.Product", b =>
                {
                    b.HasOne("Junjuria.Infrastructure.Models.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Junjuria.Infrastructure.Models.Manufacturer", "Manufacturer")
                        .WithMany("Products")
                        .HasForeignKey("ManufacturerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Junjuria.Infrastructure.Models.ProductCharacteristic", b =>
                {
                    b.HasOne("Junjuria.Infrastructure.Models.Product", "Product")
                        .WithMany("Characteristics")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Junjuria.Infrastructure.Models.ProductComment", b =>
                {
                    b.HasOne("Junjuria.Infrastructure.Models.AppUser", "Author")
                        .WithMany("ProductComments")
                        .HasForeignKey("AuthorId");

                    b.HasOne("Junjuria.Infrastructure.Models.Product", "Product")
                        .WithMany("ProductComments")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Junjuria.Infrastructure.Models.ProductOrder", b =>
                {
                    b.HasOne("Junjuria.Infrastructure.Models.Order", "Order")
                        .WithMany("OrderProducts")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Junjuria.Infrastructure.Models.Product", "Product")
                        .WithMany("ProductOrders")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Junjuria.Infrastructure.Models.ProductPicture", b =>
                {
                    b.HasOne("Junjuria.Infrastructure.Models.Product", "Product")
                        .WithMany("ProductPictures")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Junjuria.Infrastructure.Models.ProductVote", b =>
                {
                    b.HasOne("Junjuria.Infrastructure.Models.Product", "Product")
                        .WithMany("Votes")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Junjuria.Infrastructure.Models.AppUser", "Voter")
                        .WithMany("ProductVotes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Junjuria.Infrastructure.Models.Recomendation", b =>
                {
                    b.HasOne("Junjuria.Infrastructure.Models.AppUser", "Author")
                        .WithMany("Recomendations")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Junjuria.Infrastructure.Models.AppUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Junjuria.Infrastructure.Models.AppUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Junjuria.Infrastructure.Models.AppUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Junjuria.Infrastructure.Models.AppUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
