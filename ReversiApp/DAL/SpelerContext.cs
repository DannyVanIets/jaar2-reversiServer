using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReversiApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReversiApp.DAL
{
    public class SpelerContext : IdentityDbContext
    {
        public SpelerContext(DbContextOptions<SpelerContext> options) : base(options) { }

        public DbSet<SpelerModel> Speler { get; set; }
    }
}
