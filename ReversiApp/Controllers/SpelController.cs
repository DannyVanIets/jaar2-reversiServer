using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReversiApp.DAL;
using ReversiApp.Models;

namespace ReversiApp.Controllers
{
    public class SpelController : Controller
    {
        private readonly ReversiContext _context;
        private readonly UserManager<Speler> UserManager;
        private readonly RoleManager<IdentityRole> RoleManager;

        public SpelController(ReversiContext context, UserManager<Speler> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            UserManager = userManager;
            RoleManager = roleManager;
        }

        // GET: Spel
        public async Task<IActionResult> Index()
        {
            Speler speler = await UserManager.FindByNameAsync(User.Identity.Name);
            if (speler.SpelId != null)
            {
                return RedirectToAction(nameof(Game), new { ID = speler.SpelId });
            }
            var spellen = await _context.Spel.Include(s => s.Spelers).Where(s => s.Spelers.Count < 2 && s.Status == Status.NietGestart).ToListAsync();
            ViewBag.AantalSpellen = spellen.Count;
            return View(spellen);
        }

        // GET: Spel/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spel = await _context.Spel
                .FirstOrDefaultAsync(m => m.ID == id);
            if (spel == null)
            {
                return NotFound();
            }

            return View(spel);
        }

        // GET: Spel/Game/5
        public async Task<IActionResult> Game(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spel = await _context.Spel.Include(s => s.Spelers).FirstOrDefaultAsync(m => m.ID == id);
            if (spel == null)
            {
                return NotFound();
            }
            foreach (var speler in spel.Spelers)
            {
                if (speler.Email == User.Identity.Name)
                {
                    if (spel.Spelers.Count == 1)
                    {
                        Response.Headers.Add("Refresh", "5");
                    }
                    return View(spel);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Spel/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Spel/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Omschrijving")] Spel spel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(spel);
                await _context.SaveChangesAsync();
                var lastAddedGame = _context.Spel.ToList().LastOrDefault();

                Speler speler = await UserManager.FindByNameAsync(User.Identity.Name);
                speler.SpelId = lastAddedGame.ID;
                speler.Kleur = Kleur.Wit;
                var result = await UserManager.UpdateAsync(speler);

                return RedirectToAction(nameof(Game), new { ID = lastAddedGame.ID });
            }
            return View(spel);
        }

        // GET: Spel/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spel = await _context.Spel.FindAsync(id);
            if (spel == null)
            {
                return NotFound();
            }
            return View(spel);
        }

        // POST: Spel/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Omschrijving")] Spel spel)
        {
            if (id != spel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(spel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpelExists(spel.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(spel);
        }

        // GET: Spel/Join/5
        [HttpGet]
        public async Task<IActionResult> Join(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spel = await _context.Spel.FindAsync(id);
            var spelersInSpel = _context.Speler.Where(s => s.SpelId == id).Count();

            if (spel == null)
            {
                return NotFound();
            }
            else if (spelersInSpel >= 2)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(spel);
        }

        // POST: Spel/Join/5
        [HttpPost]
        public async Task<IActionResult> Join(int id, [Bind("ID")] Spel spel)
        {
            if (id != spel.ID)
            {
                return NotFound();
            }

            Speler speler = await UserManager.FindByNameAsync(User.Identity.Name);
            if (speler != null)
            {
                speler.SpelId = id;
                speler.Kleur = Kleur.Zwart;
                var result = await UserManager.UpdateAsync(speler);

                spel.Status = Status.Bezig;
                _context.Update(spel);
                await _context.SaveChangesAsync();

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(spel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Won(int id, Kleur kleur)
        {
            var spel = await _context.Spel.Include(s => s.Spelers).Where(s => s.ID == id).FirstOrDefaultAsync();
            if (spel != null)
            {
                if (kleur == Kleur.Wit)
                {
                    spel.Status = Status.WitGewonnen;
                }
                else
                {
                    spel.Status = Status.ZwartGewonnen;
                }
                _context.Update(spel);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Game), new { ID = id });
        }

        // GET: Spel/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spel = await _context.Spel
                .FirstOrDefaultAsync(m => m.ID == id);
            if (spel == null)
            {
                return NotFound();
            }

            return View(spel);
        }

        // POST: Spel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Speler speler = await UserManager.FindByNameAsync(User.Identity.Name);
            var spel = await _context.Spel.FindAsync(id);
            if (speler != null && spel != null)
            {
                speler.SpelId = null;
                speler.Kleur = Kleur.Geen;
                var result = await UserManager.UpdateAsync(speler);
                if (result.Succeeded)
                {
                    _context.Spel.Remove(spel);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Leave(int id)
        {
            Speler speler = await UserManager.FindByNameAsync(User.Identity.Name);
            var spel = await _context.Spel.Include(s => s.Spelers).Where(s => s.ID == id).FirstOrDefaultAsync();
            if (speler != null && spel != null)
            {
                speler.SpelId = null;
                speler.Kleur = Kleur.Geen;
                var result = await UserManager.UpdateAsync(speler);
                if (result.Succeeded && spel.Spelers.Count < 1)
                {
                    _context.Spel.Remove(spel);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SpelExists(int id)
        {
            return _context.Spel.Any(e => e.ID == id);
        }
    }
}
