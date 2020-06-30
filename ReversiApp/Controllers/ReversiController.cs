using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReversiApp.DAL;
using ReversiApp.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ReversiRestApi.Controllers
{
    [Route("api/Spel")]
    [ApiController]
    public class ReversiController : Controller
    {
        /*private static List<Spel> games = new List<Spel>()
        {
            new Spel{ ID = 1, Omschrijving = "Naam1", AandeBeurt = Kleur.Wit, Token = "5EPNSFGFGEE55" },
            new Spel{ ID = 2, Omschrijving = "Naam2", AandeBeurt = Kleur.Zwart, Token = "MFD2948REWRT" },
            new Spel{ ID = 3, Omschrijving = "Naam3", AandeBeurt = Kleur.Wit, Token = "ERWELRFN8545" },
        };*/

        public readonly ReversiContext _context;

        public ReversiController(ReversiContext context)
        {
            _context = context;
        }

        // GET: api/Spel/Speelbord/5
        [HttpGet("Speelbord/{id}")]
        public ActionResult<string> Speelbord(int id)
        {
            var result = _context.Spel.FirstOrDefault(item => item.ID == id);
            if (result != null)
            {
                return result.SerializedBord;
            }
            return null;
        }

        // GET: api/Spel/IsBezig/5
        [HttpGet("IsBezig/{id}")]
        public ActionResult<bool> IsBezig(int id)
        {
            var result = _context.Spel.FirstOrDefault(item => item.ID == id);
            if (result != null)
            {
                return result.Status == Status.Bezig;
            }
            return false;
        }

        // GET: api/Spel/AanDeBeurt/5
        [HttpGet("AanDeBeurt/{id}")]
        public ActionResult<Kleur> AanDeBeurt(int id)
        {
            var result = _context.Spel.FirstOrDefault(item => item.ID == id);
            if (result != null)
            {
                return result.AandeBeurt;
            }
            return null;
        }

        // GET: api/Spel/ZetMogelijk/5
        [HttpGet("ZetMogelijk/{id}/{rij}/kolom")]
        public ActionResult<bool> ZetMogelijk(int id, int rij, int kolom)
        {
            var result = _context.Spel.FirstOrDefault(item => item.ID == id);
            if (result != null)
            {
                if (result.DoeZet(rij, kolom))
                {
                    return true;
                }
            }
            return false;
        }
    }
}