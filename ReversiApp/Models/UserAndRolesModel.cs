using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReversiApp.Models
{
    public class UserAndRolesModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Rol { get; set; }
        public Kleur Kleur { get; set; }
        public int Highscore { get; set; }
        public bool Archived { get; set; }
    }
}