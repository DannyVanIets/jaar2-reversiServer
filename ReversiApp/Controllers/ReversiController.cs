using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReversiApp.DAL;
using ReversiApp.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ReversiRestApi.Controllers
{
    [Route("api/Spel")]
    [ApiController]
    public class ReversiController : Controller
    {
        public readonly ReversiContext _context;
        private readonly UserManager<Speler> UserManager;

        public ReversiController(ReversiContext context, UserManager<Speler> userManager)
        {
            _context = context;
            UserManager = userManager;
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
            var result = _context.Spel.Include(s => s.Spelers).FirstOrDefault(item => item.ID == zet.Id);
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
                        foreach(var speler in result.Spelers)
                        {
                            if(speler.Kleur == Kleur.Wit)
                            {
                                speler.Highscore++;
                                UserManager.UpdateAsync(speler).Wait();
                            }
                        }
                    }
                    else if (result.Afgelopen() && result.OverwegendeKleur() == Kleur.Zwart)
                    {
                        result.Status = Status.ZwartGewonnen;
                        foreach (var speler in result.Spelers)
                        {
                            if (speler.Kleur == Kleur.Zwart)
                            {
                                speler.Highscore++;
                                UserManager.UpdateAsync(speler).Wait();
                            }
                        }
                    }
                    _context.SaveChanges();
                    return StatusCode(406);
                }
            }
            return NotFound();
        }
    }
}