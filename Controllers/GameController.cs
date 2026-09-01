using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RelationsNaN.Data;
using RelationsNaN.Models;

namespace RelationsNaN.Controllers
{
    public class GameController : Controller
    {
        private readonly RelationsNaNContext _context;

        public GameController(RelationsNaNContext context)
        {
            _context = context;
        }

        // GET: Game
        public async Task<IActionResult> Index()
        {
            var relationsNaNContext = _context.Game.Include(g => g.Genre).Include(g => g.Platforms);
            return View(await relationsNaNContext.ToListAsync());
        }

        // GET: Game/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Game
                .Include(g => g.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // GET: Game/Create
        public IActionResult Create()
        {
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name");
            return View();
        }

        // POST: Game/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Image,ReleaseYear,GenreId")] Game game)
        {
            if (ModelState.IsValid)
            {
                _context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name", game.GenreId);
            return View(game);
        }

        // GET: Game/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Game.Include(p => p.Platforms).FirstOrDefaultAsync(g => g.Id == id);
            if (game == null)
            {
                return NotFound();
            }
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name", game.GenreId);

            var isDejaAjoutee = game.Platforms.Select(p => p.Id).ToList();
            ViewData["Platforms"] = new SelectList(_context.Platform.Where(p => !isDejaAjoutee.Contains(p.Id)), "Id", "Name");
            return View(game);
        }

        // POST: Game/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Image,ReleaseYear,GenreId")] Game game)
        {
            if (id != game.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.Id))
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
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name", game.GenreId);

            var isDejaAjoutee = game.Platforms.Select(p => p.Id).ToList();
            ViewData["Platforms"] = new SelectList(_context.Platform.Where(p => !isDejaAjoutee.Contains(p.Id)), "Id", "Name");

            return View(game);
        }

        // GET: Game/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Game
                .Include(g => g.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Game/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var game = await _context.Game.FindAsync(id);
            if (game != null)
            {
                _context.Game.Remove(game);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPlatform(int id, int platformId) 
        {
            var game = await _context.Game.Include(g => g.Platforms)
                .FirstOrDefaultAsync(p => p.Id == id);

            var platforms = await _context.Platform.FindAsync(platformId);

            if (game == null || platforms == null) 
            {
                return NotFound();
            }
            if (!game.Platforms.Contains(platforms)) 
            {
                game.Platforms.Add(platforms);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Edit), new {id});
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePlatform(int id, int platformId)
        {
            var game = await _context.Game.Include(g => g.Platforms)
                .FirstOrDefaultAsync(p => p.Id == id);

            var platforms = await _context.Platform.FindAsync(platformId);

            if (game == null || platforms == null)
            {
                return NotFound();
            }
            if (game.Platforms.Contains(platforms))
            {
                game.Platforms.Remove(platforms);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Edit), new { id });
        }

        private bool GameExists(int id)
        {
            return _context.Game.Any(e => e.Id == id);
        }
    }
}
