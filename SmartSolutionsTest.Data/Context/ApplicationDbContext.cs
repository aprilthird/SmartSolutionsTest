using Microsoft.EntityFrameworkCore;
using SmartSolutionsTest.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSolutionsTest.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<Operation> Operations { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
    }
}
