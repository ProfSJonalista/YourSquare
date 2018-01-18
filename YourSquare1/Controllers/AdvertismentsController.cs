using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YourSquare1.Models;
using YourSquare1.Models.AccountViewModels;
using YourSquare1.Services;
using YourSquare1.Data;
using Microsoft.EntityFrameworkCore;

namespace YourSquare1.Controllers
{
    public class AdvertismentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdvertismentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Advertisments
        public async Task<IActionResult> Index()
        {
            return View(await _context.Advertisments.Where(a => a.Accepted == true && a.DecisionMade == true).ToListAsync());
        }

        public async Task<IActionResult> UserAdvertisments()
        {
            var userId = _userManager.GetUserId(User);
            var advertismentList = await _context.Advertisments.Where(a => a.UserID.Equals(userId)).ToListAsync();

            return View(advertismentList);
        }

        public async Task<IActionResult> AcceptAdvertisments()
        {
            return View(await _context.Advertisments.Where(a => a.Accepted == false && a.DecisionMade == false).ToListAsync());
        }

        [HttpPost, ActionName("AcceptAdvertisments")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptAdvertismentsPost(string result, int? advertismentId)
        {
            var advertismentToUpdate = await GetDetailAdvertismentAsync(advertismentId);

            if(advertismentToUpdate == null)
            {
                return NotFound();
            }

            if (result.Equals("accept"))
            {
                advertismentToUpdate.DecisionMade = true;
                advertismentToUpdate.Accepted = true;
            } else if (result.Equals("decline"))
            {
                advertismentToUpdate.DecisionMade = true;
                advertismentToUpdate.Accepted = false;
            }

            try
            {
                _context.Update(advertismentToUpdate);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdvertismentExists(advertismentToUpdate.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(AcceptAdvertisments));
        }
        // GET: Advertisments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var advertisment = await GetDetailAdvertismentAsync(id);

            if (advertisment == null)
            {
                return NotFound();
            }

            return View(advertisment);
        }

        public async Task<IActionResult> UserAdvertismentsDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var advertisment = await GetDetailAdvertismentAsync(id);

            if (advertisment == null)
            {
                return NotFound();
            }

            return View(advertisment);
        }

        public async Task<Advertisment> GetDetailAdvertismentAsync(int? id)
        {
            return await _context.Advertisments
                .Include(a => a.AdvertismentCreator)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ID == id);
        }

        // GET: Advertisments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Advertisments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Description,AdditionalEquipmentDescription,Price,Address")] Advertisment advertisment)
        {
            var advertismentCreator = await _userManager.GetUserAsync(User);

            var newAdvertisment = new Advertisment()
            {
                UserID = _userManager.GetUserId(User),
                DateOfPublication = DateTime.Now,
                Accepted = false,
                DecisionMade = false,
                AdvertismentCreator = advertismentCreator
            };

            if (ModelState.IsValid)
            {
                newAdvertisment.Description = advertisment.Description;
                newAdvertisment.AdditionalEquipmentDescription = advertisment.AdditionalEquipmentDescription;
                newAdvertisment.Price = advertisment.Price;
                newAdvertisment.Address = advertisment.Address;

                _context.Add(newAdvertisment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(advertisment);
        }

        // GET: Advertisments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var advertisment = await _context.Advertisments.SingleOrDefaultAsync(m => m.ID == id);
            if (advertisment == null)
            {
                return NotFound();
            }
            return View(advertisment);
        }

        // POST: Advertisments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Description,AdditionalEquipmentDescription,Price,Address")] Advertisment advertisment)
        {
            if (id != advertisment.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(advertisment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdvertismentExists(advertisment.ID))
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
            return View(advertisment);
        }

        // GET: Advertisments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var advertisment = await _context.Advertisments
                .SingleOrDefaultAsync(m => m.ID == id);
            if (advertisment == null)
            {
                return NotFound();
            }

            return View(advertisment);
        }

        // POST: Advertisments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var advertisment = await _context.Advertisments.SingleOrDefaultAsync(m => m.ID == id);
            _context.Advertisments.Remove(advertisment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdvertismentExists(int id)
        {
            return _context.Advertisments.Any(e => e.ID == id);
        }
    }
}
