using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventsApp.Data;
using EventsApp.Repository.Services;
using EventsApp.Models;
using System.Dynamic;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace EventsApp.Controllers
{
    public class SubscribesController : Controller
    {
        private readonly EventsAppDbContext _context;
        private readonly MailjetService _mailjetService;
        private readonly UserManager<ApplicationUser> _userManager;

        public SubscribesController(EventsAppDbContext context , MailjetService mailjetService , UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mailjetService = mailjetService;
            _userManager = userManager;
        }

        // GET: subscriptions
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User); // Get the currently logged-in user

            // Retrieve subscriptions related to the logged-in user
            var subscriptions = await _context.Subscribes
                .Where(s => s.Email == user.Email) // Filter by email or user ID as needed
                .Include(s => s.Event) // Include related event data
                .ToListAsync();

            return View(subscriptions);
        }

        // GET: subscriptions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Subscribes == null)
            {
                return NotFound();
            }

            var registration = await _context.Subscribes
                .Include(r => r.Event)
                .FirstOrDefaultAsync(m => m.SubscribeId == id);
            if (registration == null)
            {
                return NotFound();
            }

            return View(registration);
        }

        // GET: Registrations/Create
        public IActionResult Create()
        {
            var events = _context.Events.ToList(); 
            ViewBag.EventId = new SelectList(events, "EventId", "Title");

            // Pass the list of events to the view
            ViewBag.EventsList = events;

            var userName = User.Identity.Name; 
            var userEmail = User.FindFirstValue(ClaimTypes.Email); 

            // Store user information in ViewData
            ViewData["UserName"] = userName;
            ViewData["UserEmail"] = userEmail;

            return View();
        }

        // POST: Registrations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubscribeId,ParticipantName,Email,EventId")] Subscribe subscribe)
        {
            _context.Add(subscribe);
            await _context.SaveChangesAsync();

            // Send confirmation email
            var subject = "Registration Confirmation";
            var textPart = "Welcome to our event! Your registration has been confirmed.";
            var htmlPart = "<h3>Welcome to our event!</h3><p>Your registration has been confirmed.</p>";
            await _mailjetService.SendEmail(subscribe.ParticipantName, subscribe.Email, $"<h2>Dear {subscribe.ParticipantName},</h2><p>You have successfully registered to the event .</p>"
                );

            return RedirectToAction(nameof(Index));

        }

        // GET: subscriptions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Subscribes == null)
            {
                return NotFound();
            }

            var registration = await _context.Subscribes.FindAsync(id);
            if (registration == null)
            {
                return NotFound();
            }
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventId", registration.EventId);
            return View(registration);
        }

        // POST: subscriptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RegistrationId,ParticipantName,Email,EventId")] Subscribe registration)
        {
            if (id != registration.SubscribeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(registration);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegistrationExists(registration.SubscribeId))
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
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventId", registration.EventId);
            return View(registration);
        }

        // GET: subscriptions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Subscribes == null)
            {
                return NotFound();
            }

            var registration = await _context.Subscribes
                .Include(r => r.Event)
                .FirstOrDefaultAsync(m => m.SubscribeId == id);
            if (registration == null)
            {
                return NotFound();
            }

            return View(registration);
        }

        // POST: subscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Subscribes == null)
            {
                return Problem("Entity set 'ERSDbContext.Registrations'  is null.");
            }
            var registration = await _context.Subscribes.FindAsync(id);
            if (registration != null)
            {
                _context.Subscribes.Remove(registration);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RegistrationExists(int id)
        {
            return (_context.Subscribes?.Any(e => e.SubscribeId == id)).GetValueOrDefault();
        }


    }
}
