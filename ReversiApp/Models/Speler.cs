using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReversiApp.Models
{
    public class Speler : IdentityUser
    {
        public string Token { get; set; }
        public Kleur Kleur { get; set; }
        public int Highscore { get; set; }
        public bool Archived { get; set; }
    }
}
