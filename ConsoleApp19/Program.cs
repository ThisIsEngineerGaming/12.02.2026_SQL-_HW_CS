using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        using var db = new AppDbContext();

        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        var user = new User
        {
            Name = "John",
            Email = "john@mail.com"
        };

        var product = new Product
        {
            Name = "Laptop",
            Price = 4500.50m
        };

        var category = new Category
        {
            Name = "Electronics"
        };

        product.Categories.Add(category);

        var order = new Order
        {
            User = user
        };

        var review = new Review
        {
            User = user,
            Product = product,
            Content = "Great product!"
        };

        db.Users.Add(user);
        db.Products.Add(product);
        db.Categories.Add(category);
        db.Orders.Add(order);
        db.Reviews.Add(review);
        db.SaveChanges();

        Console.WriteLine("Everything saved successfully.");
    }
}


class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseMySql(
            "server=127.0.0.1;port=3306;database=test2;user=root;password=;charset=utf8mb4",
            new MySqlServerVersion(new Version(8, 0, 36))
        );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Product>()
            .HasMany(p => p.Categories)
            .WithMany(c => c.Products)
            .UsingEntity(j => j.ToTable("ProductCategories"));
    }
}



class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }

    public List<Order> Orders { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
}

class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }

    public List<Category> Categories { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
}

class Category
{
    public int Id { get; set; }
    public string Name { get; set; }

    public List<Product> Products { get; set; } = new();
}

class Order
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public int UserId { get; set; }
    public User User { get; set; }
}

class Review
{
    public int Id { get; set; }
    public string Content { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; }
}
