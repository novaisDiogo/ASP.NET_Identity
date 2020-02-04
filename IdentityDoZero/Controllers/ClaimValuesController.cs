using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IdentityDoZero.Data;
using IdentityDoZero.Models;

namespace IdentityDoZero.Controllers
{
    public class ClaimValuesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClaimValuesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ClaimValues
        public async Task<IActionResult> Index()
        {
            return View(await _context.ClaimValue.ToListAsync());
        }

        // GET: ClaimValues/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claimValue = await _context.ClaimValue
                .FirstOrDefaultAsync(m => m.Id == id);
            if (claimValue == null)
            {
                return NotFound();
            }

            return View(claimValue);
        }

        // GET: ClaimValues/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ClaimValues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] ClaimValue claimValue)
        {
            if (ModelState.IsValid)
            {
                claimValue.Id = Guid.NewGuid();
                _context.Add(claimValue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(claimValue);
        }

        // GET: ClaimValues/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claimValue = await _context.ClaimValue.FindAsync(id);
            if (claimValue == null)
            {
                return NotFound();
            }
            return View(claimValue);
        }

        // POST: ClaimValues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name")] ClaimValue claimValue)
        {
            if (id != claimValue.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(claimValue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClaimValueExists(claimValue.Id))
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
            return View(claimValue);
        }

        // GET: ClaimValues/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claimValue = await _context.ClaimValue
                .FirstOrDefaultAsync(m => m.Id == id);
            if (claimValue == null)
            {
                return NotFound();
            }

            return View(claimValue);
        }

        // POST: ClaimValues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var claimValue = await _context.ClaimValue.FindAsync(id);
            _context.ClaimValue.Remove(claimValue);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClaimValueExists(Guid id)
        {
            return _context.ClaimValue.Any(e => e.Id == id);
        }
    }
}
