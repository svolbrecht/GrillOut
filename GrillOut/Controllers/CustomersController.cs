using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GrillOut.Data;
using GrillOut.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace GrillOut.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CustomersController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> CustomersEvents()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.ApplicationUserId == userId);
            if (customer == null)
            {
                return NotFound();
            }
            var applicationDbContext = _context.Events.Where(c => c.CustomerId == customer.CustomerId).Include(e => e.Employee);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Customers.Include(c => c.ApplicationUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var user = await _context.Customers
                .FirstOrDefaultAsync(m => m.ApplicationUserId == userId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);

            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var customer = await _context.Customers
            //    .Include(c => c.ApplicationUser)
            //    .FirstOrDefaultAsync(m => m.CustomerID == id);
            //if (customer == null)
            //{
            //    return NotFound();
            //}

            //return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerID,CustomerName,StreetAddress,CityStateZip,IsDelivered,IsPickedUp,ApplicationUserId")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                customer.ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details");
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", customer.ApplicationUserId);
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", customer.ApplicationUserId);
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerID,CustomerName,StreetAddress,CityStateZip,IsDelivered,IsPickedUp,ApplicationUserId")] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", customer.ApplicationUserId);
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.ApplicationUser)
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }

        public IActionResult Payment(int? id)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var ad = _context.Customers
            //    .FirstOrDefault(m => m.ApplicationUserId == userid);
            return View();
        }

        public async Task<IActionResult> CustomerMap(int? id)
        {
            {
                if (id == null)
                {
                    //not sure how to revise this for Core.  This code should alert user in thr case there is no user logged in.
                    //return HttpStatusCode.BadRequest;
                }
                Events events = _context.Events.Find(id);
                if (events == null)
                {
                    return NotFound();
                }
                //ViewBag.ApplicationUserId = new SelectList(_context.Users, "Id", "UserRole", businessProfile.ApplicationUser);
                ViewBag.EventsAddress = events.StreetAddress;
                ViewBag.CityStateZip = events.CityStateZip;
                return View(events);
            }
        }
    }
}
