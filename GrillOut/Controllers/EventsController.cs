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
using RestSharp;
using RestSharp.Authenticators;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace GrillOut.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EventsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Events.Include(e => e.Customer).Include(e => e.Employee);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var events = await _context.Events
                .Include(e => e.Customer)
                .Include(e => e.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (events == null)
            {
                return NotFound();
            }

            return View(events);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CustomerId,EmployeeId,EventDate,StreetAddress,CityStateZip,IsDelivered,IsPickedUp")] Events events)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                var customer = await _context.Customers.Where(m => m.ApplicationUserId == userId).FirstOrDefaultAsync();
                var customerId = customer.CustomerId;
                events.CustomerId = customerId;

                _context.Add(events);
                await _context.SaveChangesAsync();
                await SendConfirmationEmail();
                //SendSimpleMessage();
                return RedirectToAction("Payment", "Customers");
            }
            //ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", events.CustomerId);
            //ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", events.EmployeeId);
            return View(events);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerId,EmployeeId,EventDate,StreetAddress,CityStateZip,IsDelivered,IsPickedUp")] Events events)
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", events.CustomerId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", events.EmployeeId);
            return View(events);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var events = await _context.Events
                .Include(e => e.Customer)
                .Include(e => e.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (events == null)
            {
                return NotFound();
            }

            return View(events);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var events = await _context.Events.FindAsync(id);
            _context.Events.Remove(events);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventsExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }

        static async Task SendConfirmationEmail()
        {
            var apiKey = Keys.sendGridKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("test@example.com", "GrillOut");
            var subject = "GrillOut Confirmation";
            var to = new EmailAddress("wiscoditka@gmail.com", "Example User");
            var plainTextContent = "Your Grillout has been confirmed!";
            var htmlContent = "Your Grillout has been confirmed!";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }

        //public static IRestResponse SendSimpleMessage()
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
        //    request.AddParameter("text", "Testing some Mailgun awesomness!");
        //    request.Method = Method.POST;
        //    return client.Execute(request);
        //}

        public async Task<IActionResult> EventMap(int? id)
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

        public async Task<IActionResult> DistanceMatrix(int? id)
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
                return View();
            }
        }
    }
}
