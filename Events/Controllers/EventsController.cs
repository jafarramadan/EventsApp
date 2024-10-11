using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventsApp.Models;
using EventsApp.Data;

namespace EventsApp.Controllers
{
    public class EventsController : Controller
    {
        private readonly EventsAppDbContext _context;

        public EventsController(EventsAppDbContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            return _context.Events != null ?
                        View(await _context.Events.ToListAsync()) :
                        Problem("Entity set 'ERSDbContext.Events'  is null.");
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Events == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create

        [HttpPost]
        //  [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Title, string Date, string Description, string Capacity)
        {
            var x = new Event()
            {

                Title = Title,
                Date = Date,
                Description = Description,
                Capacity = Capacity
            };
            if (ModelState.IsValid)
            {
                _context.Events.Add(x);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(x);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Events == null)
            {
                return NotFound();
            }

            var x = await _context.Events.FindAsync(id);
            if (x == null)
            {
                return NotFound();
            }
            return View(x);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int EventId, string Title, string Date, string Description, string Capacity)
        {
            // Find the existing event by EventId
            var existingEvent = await _context.Events.FindAsync(EventId);

            if (existingEvent == null)
            {
                return NotFound();
            }

            // Update the properties of the existing event
            existingEvent.Title = Title;

            // Parsing the string Date to DateTime


            existingEvent.Date = Date;
            existingEvent.Description = Description;
            existingEvent.Capacity = Capacity;

            if (ModelState.IsValid)
            {
                try
                {
                    // Update the event in the database
                    _context.Update(existingEvent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(EventId))
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

            return View(existingEvent);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Events == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Events == null)
            {
                return Problem("Entity set 'ERSDbContext.Events'  is null.");
            }
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                _context.Events.Remove(@event);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return (_context.Events?.Any(e => e.EventId == id)).GetValueOrDefault();
        }
    }
}
