using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GrillOut.Data;
using GrillOut.Models;
using System.Security.Claims;

namespace GrillOut.Controllers
{
    public class PackagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public string basicGrillOutDetails = "Grill, Charcol, Lighter Fluid, Spatula, Tongs, Small Astroturf, 28qt Cooler, Small Table";
        public string averageGrillOutDetails = "Grill, Charcol, Lighter Fluid, Spatula, Tongs, Small Astroturf, 5 Chairs, Small Popup Canopy, 60qt Cooler, Medium Table";
        public string eliteGrillOutDetails = "Grill, Charcol, Lighter Fluid, Spatula, Tongs, Large Astroturf, 10 Chairs, Large Popup Canopy, 120qt Cooler, Large Table";

        public PackagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Packages
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Packages.Include(p => p.Customer);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Packages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var currentEvent = await _context.Events.Where(e => e.Id == id).FirstOrDefaultAsync();
            var customerId = currentEvent.CustomerId;
            var package = await _context.Packages.Where(c => c.CustomerId == customerId).FirstOrDefaultAsync();
            //var package = await _context.Packages
            //    .Include(p => p.Customer)
            //    .FirstOrDefaultAsync(m => m.PackageId == id);
            if (package == null)
            {
                return NotFound();
            }

            return View(package);
        }

        // GET: Packages/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");
            return View();
        }

        // POST: Packages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PackageId,GrillOutPackage,choseBasicGrillOut,choseAverageGrillOut,choseEliteGrillOut,CustomerId")] Package package)
        {
            //if (ModelState.IsValid)
            //{
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(m => m.ApplicationUserId == userId);
                package.CustomerId = customer.CustomerId;
                _context.Add(package);
                await _context.SaveChangesAsync();
                await InsertPackageDetails(package);
                return RedirectToAction(nameof(Create), "Events");
        //}
        ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", package.CustomerId);
            return View(package);
        }

        public async Task InsertPackageDetails(Package package)
        {
            if (package.choseBasicGrillOut == true)
            {
                package.GrillOutPackage = basicGrillOutDetails;
                await _context.SaveChangesAsync();
            }
            else if(package.choseAverageGrillOut == true)
            {
                package.GrillOutPackage = averageGrillOutDetails;
                await _context.SaveChangesAsync();
            }
            else if(package.choseEliteGrillOut == true)
            {
                package.GrillOutPackage = eliteGrillOutDetails;
                await _context.SaveChangesAsync();
            }
        }

        // GET: Packages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var package = await _context.Packages.FindAsync(id);
            if (package == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", package.CustomerId);
            return View(package);
        }

        // POST: Packages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PackageId,GrillOutPackage,choseBasicGrillOut,choseAverageGrillOut,choseEliteGrillOut,CustomerId")] Package package)
        {
            if (id != package.PackageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(package);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PackageExists(package.PackageId))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", package.CustomerId);
            return View(package);
        }

        // GET: Packages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var package = await _context.Packages
                .Include(p => p.Customer)
                .FirstOrDefaultAsync(m => m.PackageId == id);
            if (package == null)
            {
                return NotFound();
            }

            return View(package);
        }

        // POST: Packages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var package = await _context.Packages.FindAsync(id);
            _context.Packages.Remove(package);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PackageExists(int id)
        {
            return _context.Packages.Any(e => e.PackageId == id);
        }
    }
}
