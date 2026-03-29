using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QrCode.Domain.AggregateModels.TagModels;
using QrCode.Domain.AggregateModels.Tags;
using QrCode.Domain.AggregateModels.Users;
using QrCode.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Infrastructure.Context
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>,IUnitOfWork
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer("Data Source=94.73.148.5;Initial Catalog=u1717048_tag; User Id=u1717048_tag;Password=Cr6l30Orp6_hrYpso");
        }
        //public DbSet<ApplicationUser> MyUsers { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<View_TagUser> View_TagUsers { get; set; }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    //modelBuilder.Entity<MyUser>().ToTable("AspNetUsers");
        //    base.OnModelCreating(modelBuilder);
        //    //modelBuilder.Entity<EntityB>().ToTable("EntityB");

        //    //modelBuilder.Entity<User>()
        //    //    .Property(p => p.NameSurname)
        //    //    .HasConversion(nameSurname => nameSurname.Value, value => new(value));
        //    //modelBuilder.Entity<User>()
        //    //    .Property(p => p.Email)
        //    //    .HasConversion(email => email.Value, value => new(value));
        //    //modelBuilder.Entity<User>()
        //    //    .Property(p => p.Password)
        //    //    .HasConversion(password => password.Value, value => new(value));
        //}
    }
}
