using System.ComponentModel.DataAnnotations;
using testproject;

namespace ScrabbleApp.Models
{
    public class Speler
    {
        [Key]
        public int SpelerId { get; set; }
        public string Naam { get; set; }
        public string Wachtwoord { get; set; }
        public string Token { get; set; }
        public Kleur Kleur { get; set; }
    }
}