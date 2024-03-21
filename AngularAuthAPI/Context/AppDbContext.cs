using AngularAuthAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AngularAuthAPI.Context //2. add NUget packages as required and then Add DbContext and make dbset and override methods
{
    public class AppDbContext : DbContext //Code First Approch
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) // create instance and passed base
        {
                
        }

        public DbSet<User> Users { get; set; } //DBSet to communicate with DB

        protected override void OnModelCreating(ModelBuilder modelBuilder) //To send records to table,
                                                                           
        {
            modelBuilder.Entity<User>().ToTable("users");   //Modeltbuilder help to take entity form .net Core to DB
        }
    }
}
