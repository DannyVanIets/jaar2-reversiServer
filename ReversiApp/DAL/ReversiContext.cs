using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReversiApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReversiApp.DAL
{
    public class ReversiContext : IdentityDbContext
    {
        public ReversiContext(DbContextOptions<ReversiContext> options) : base(options) { }

        public DbSet<Speler> Speler { get; set; }
        public DbSet<Spel> Spel { get; set; }
    }
}
