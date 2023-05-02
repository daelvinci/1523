﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.DAL
{
    public class PustokDbContext : IdentityDbContext
    {
        public PustokDbContext(DbContextOptions<PustokDbContext> options) : base(options)
        {

        }

        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<Book>Books { get; set; }
        public DbSet<Author>Authors { get; set; }
        public DbSet<Genre>Genres { get; set; }
        public DbSet<BookTag> BookTags { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<BookImage> BookImages{ get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<BookReview> BookReviews { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BookTag>(x => x.HasKey(bt => new { bt.BookId, bt.TagId }));
            modelBuilder.Entity<Setting>(x => x.HasKey(s =>s.Key));
        }

    }


}
