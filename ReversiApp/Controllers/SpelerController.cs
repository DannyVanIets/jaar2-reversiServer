using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReversiApp.DAL;
using ReversiApp.Models;

namespace ReversiApp.Controllers
{
    public class SpelerController : Controller
    {
        private readonly SpelerContext _context;

        public SpelerController(SpelerContext context) => _context = context;

        [HttpGet]
        public IActionResult Index() => View(_context.Speler.ToList());

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SpelerModel spelerModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(spelerModel);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(spelerModel);
        }

        //Get variant, HttpGet is optioneel
        [HttpGet]
        public IActionResult Details(int id)
        {
            var spelerModel = _context.Speler.Find(id);
            if (spelerModel == null)
            {
                return NotFound();
            }
            return View(spelerModel);
        }

        //Get variant, HttpGet is optioneel
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var spelerModel = _context.Speler.Find(id);
            if (spelerModel == null)
            {
                return NotFound();
            }
            return View(spelerModel);
        }

        //Post variant
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, SpelerModel spelerModel)
        {
            if (ModelState.IsValid)
            {
                _context.Update(spelerModel);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(spelerModel);
        }

        public IActionResult Delete(int id)
        {
            var spelerModel = _context.Speler.FirstOrDefault(m => m.SpelerId == id);
            if (spelerModel == null)
            {
                return NotFound();
            }
            return View(spelerModel);
        }

        [HttpPost, ActionName("Delete")] //Kan maar een methode met de naam Delete hebben en dezelfde signatuur
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var spelerModel = _context.Speler.Find(id);
            _context.Speler.Remove(spelerModel);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}