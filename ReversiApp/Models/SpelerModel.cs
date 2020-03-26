using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReversiApp.Models
{
    public enum Kleur { Wit, Zwart, Geen }

    public class SpelerModel
    {
        [Key]
        public int SpelerId { get; set; }
        public string Naam { get; set; }
        public string Email { get; set; }
        public string Wachtwoord { get; set; }
        public string Rol { get; set; }
        public string Token { get; set; }
        public Kleur Kleur { get; set; }
    }
}
