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
using Microsoft.AspNetCore.Identity;
using RestSharp;
using RestSharp.Authenticators;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace GrillOut.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        UserManager<IdentityUser> _userManager;
        UserManager<ApplicationUser> _aspUserManager;

        public EmployeesController(ApplicationDbContext context, UserManager<IdentityUser> userManager, UserManager<ApplicationUser> aspUserManager)
        {
            _context = context;
            _userManager = userManager;
            _aspUserManager = aspUserManager;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Employees.Include(e => e.ApplicationUser);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> EmployeesEvents()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.ApplicationUserId == userId);
            if (employee == null)
            {
                return NotFound();
            }
            var applicationDbContext = _context.Events.Where(c => c.EmployeeId == employee.EmployeeId).Include(c => c.Customer);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.ApplicationUser)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,EmployeeName,ApplicationUserId")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                employee.ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), "Events");
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", employee.ApplicationUserId);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", employee.ApplicationUserId);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeId,EmployeeName,IsAssigned,ApplicationUserId")] Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.EmployeeId))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", employee.ApplicationUserId);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.ApplicationUser)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }

        public async Task<IActionResult> MarkDelivered(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var events = await _context.Events.FindAsync(id);
            if (events == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", events.CustomerId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", events.EmployeeId);
            return View(events);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkDelivered(int id, [Bind("Id,CustomerId,EmployeeId,EventDate,StreetAddress,CityStateZip,IsDelivered,IsPickedUp")] Events events)
        {
            if (id != events.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(events);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventsExists(events.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(EmployeesEvents));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", events.CustomerId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", events.EmployeeId);
            return View(events);
        }

        public async Task<IActionResult> MarkPickedUp(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var events = await _context.Events.FindAsync(id);
            if (events == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", events.CustomerId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", events.EmployeeId);
            return View(events);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkPickedUp(int id, [Bind("Id,CustomerId,EmployeeId,EventDate,StreetAddress,CityStateZip,IsDelivered,IsPickedUp")] Events events)
        {
            if (id != events.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(events);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventsExists(events.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(EmployeesEvents));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", events.CustomerId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", events.EmployeeId);
            return View(events);
        }
        private bool EventsExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }

        //public static IRestResponse SendSimpleMessage(string text)
        //{
        //    RestClient client = new RestClient();
        //    client.BaseUrl = new Uri("https://api.mailgun.net/v3");
        //    client.Authenticator =
        //        new HttpBasicAuthenticator("api",
        //                                    Keys.mailgunKey);
        //    RestRequest request = new RestRequest();
        //    request.AddParameter("domain", "sandbox704c2ec99b85406fa343c888c7f3507f.mailgun.org", ParameterType.UrlSegment);
        //    request.Resource = "{domain}/messages";
        //    request.AddParameter("from", "Excited User <mailgun@sandbox704c2ec99b85406fa343c888c7f3507f.mailgun.org>");
        //    request.AddParameter("to", "svolbrecht@yahoo.com");
        //    request.AddParameter("to", "YOU@sandbox704c2ec99b85406fa343c888c7f3507f.mailgun.org");
        //    request.AddParameter("subject", "Hello");
        //    request.AddParameter("text", text);
        //    request.Method = Method.POST;
        //    return client.Execute(request);
        //}

        public async Task<IActionResult> EventDirections(int? id)
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
                var eventStreetAddress = events.StreetAddress;
                var eventCityStateZip = events.CityStateZip;
                ViewBag.completeAddress = (eventStreetAddress + " " + eventCityStateZip);
                return View(events);
            }
        }

        //public IActionResult SendDepartureEmail(int? id)
        //{
        //    var currentEvent = _context.Events.Where(c => c.Id == id);
        //    var customerId = currentEvent.Select(c => c.CustomerId).FirstOrDefault();
        //    var eventCustomer = _context.Customers.Where(c => c.CustomerId == customerId).FirstOrDefault();
        //    var customerUserId = eventCustomer.ApplicationUserId;
        //    ApplicationUser aspUser = _userManager.GetEmailAsync(eventCustomer);
        //    var customerEmail = aspUser.Email;
        //    var claims = _userManager.GetEmailAsync(eventCustomer);
        //    DepartureEmail(customerEmail);
        //    return RedirectToAction(nameof(EmployeesEvents));
        //}
        static async Task DepartureEmail(string customerEmail)
        {
            var apiKey = Keys.sendGridKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("test@example.com", "Example User");
            var subject = "GrillOut Inbound!";
            var to = new EmailAddress(customerEmail, "Example User");
            var plainTextContent = "Your Grillout is on the way!";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }

    }
}
