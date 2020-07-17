﻿using System;
using System.Collections.Generic;
using System.Text;
using Library.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Library.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Patron> Patrons { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BranchHours> BranchHours { get; set; }
        public DbSet<Checkout> checkouts { get; set; }
        public DbSet<CheckoutHistory> checkoutHistories { get; set; }
        public DbSet<Hold> Holds { get; set; }
        public DbSet<LibraryAsset> LibraryAssets { get; set; }
        public DbSet<LibraryBranch> LibraryBranches { get; set; }
        public DbSet<LibraryCard> LibraryCards { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Video> Videos { get; set; }
    }
}
