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
        public ActionResult<Kleur[,]> Speelbord(int id)
        {
            var result = _context.Spel.FirstOrDefault(item => item.ID == id);
            if (result != null)
            {
                return result.Bord;
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

        // PUT: api/Spel/ZetMogelijk/object
        [HttpPut("ZetMogelijk")]
        public ActionResult ZetMogelijk([FromBody] ZetModel zet)
        {
            var result = _context.Spel.FirstOrDefault(item => item.ID == zet.Id);
            if (result != null)
            {
                if (result.DoeZet(zet.Rij, zet.Kolom))
                {
                    _context.SaveChanges();
                    return StatusCode(204);
                }
                else
                {
                    if (result.Afgelopen() && result.OverwegendeKleur() == Kleur.Wit)
                    {
                        result.Status = Status.WitGewonnen;
                    }
                    else if (result.Afgelopen() && result.OverwegendeKleur() == Kleur.Zwart)
                    {
                        result.Status = Status.ZwartGewonnen;
                    }
                    _context.SaveChanges();
                    return StatusCode(406);
                }
            }
            return NotFound();
        }
    }
}