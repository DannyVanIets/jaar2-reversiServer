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

        private readonly ReversiContext _context;

        public ReversiController(ReversiContext context)
        {
            _context = context;
        }

        // GET: api/Spel/5
        [HttpGet("{id}")]
        public ActionResult<Kleur[,]> Get(int id)
        {
            var result = _context.Spel.FirstOrDefault(item => item.ID == id);
            if (result != null)
            {
                return result.Bord;
            }
            return null;
        }

        // POST: api/Spel
        [HttpPost]
        public void Post([FromBody] Spel spel)
        {
            games.Add(spel);
        }

        // PUT: api/Spel/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Spel spel)
        {
            var result = games.FirstOrDefault(item => item.ID == id);
            if (result != null)
            {
                result.Omschrijving = spel.Omschrijving;
                result.AandeBeurt = spel.AandeBeurt;
                result.Token = spel.Token;
            }
        }

        // DELETE: api/Spel/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var itemToRemove = games.FirstOrDefault(item => item.ID == id);
            if (itemToRemove != null)
            {
                games.Remove(itemToRemove);
            }
        }
    }
}