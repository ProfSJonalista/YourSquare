using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YourSquare1.Data;
using YourSquare1.Models;

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
        public async Task<IActionResult> Index(string sortOrder, string searchString, decimal? minimumPrice, decimal? maximumPrice)
        {
            ViewData["LocalizationSortParam"] = String.IsNullOrEmpty(sortOrder) ? "location_desc" : "";
            ViewData["CurrentFilter"] = searchString;
            ViewData["MinimumPrice"] = minimumPrice;
            ViewData["MaximumPrice"] = maximumPrice;

            var advertisments = from a in _context.Advertisments.Where(a => a.Accepted == true && a.DecisionMade == true) select a;

            if (!String.IsNullOrEmpty(searchString))
            {
                advertisments = advertisments.Where(a => a.Address.Contains(searchString));
            }

            if (minimumPrice.HasValue && !maximumPrice.HasValue)
            {
                advertisments = advertisments.Where(a => minimumPrice.Value <= a.Price);
            }

            if (maximumPrice.HasValue && !minimumPrice.HasValue)
            {
                advertisments = advertisments.Where(a => a.Price <= maximumPrice.Value);
            }

            if (maximumPrice.HasValue && minimumPrice.HasValue)
            {
                advertisments = advertisments.Where(a => minimumPrice.Value <= a.Price && a.Price <= maximumPrice.Value);
            }

            switch (sortOrder)
            {
                case "location_desc":
                    advertisments = advertisments.OrderByDescending(s => s.Address);
                    break;
                default:
                    advertisments = advertisments.OrderBy(a => a.DateOfPublication);
                    break;
            }

            return View(await advertisments
                .Include(a => a.AdvertismentCreator)
                .AsNoTracking()
                .Include(a => a.AdvertismentImages)
                .AsNoTracking()
                .ToListAsync());
        }

        public async Task<IActionResult> UserAdvertisments()
        {
            var userId = _userManager.GetUserId(User);
            var advertismentList = await _context.Advertisments.Where(a => a.UserID.Equals(userId)).ToListAsync();

            return View(advertismentList);
        }

        public async Task<IActionResult> AcceptAdvertisments()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.Administrator)
                return View(await _context.Advertisments.Where(a => a.Accepted == false && a.DecisionMade == false).ToListAsync());
            else
                return RedirectToAction("Index");
        }

        [HttpPost, ActionName("AcceptAdvertisments")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptAdvertismentsPost(int? advertismentId, string result)
        {
            var advertismentToUpdate = await GetDetailAdvertismentAsync(advertismentId);

            if (advertismentToUpdate == null)
            {
                return NotFound();
            }

            switch (result)
            {
                case "accept":
                    advertismentToUpdate.DecisionMade = true;
                    advertismentToUpdate.Accepted = true;
                    break;

                case "decline":
                    advertismentToUpdate.DecisionMade = true;
                    advertismentToUpdate.Accepted = false;
                    break;

                default:
                    advertismentToUpdate.DecisionMade = false;
                    advertismentToUpdate.Accepted = false;
                    break;
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
                .Include(a => a.AdvertismentImages)
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
        public async Task<IActionResult> Create([Bind("Description,AdditionalEquipmentDescription,Price,Address")] Advertisment advertisment, List<IFormFile> files)
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

                var filePath = Path.GetTempFileName();

                foreach (var formFile in files)
                {
                    if(formFile.Length > 0)

                    {
                        using (var stream = new MemoryStream())
                        {
                            await formFile.CopyToAsync(stream);
                            var image = new Image();
                            image.Advertisment = newAdvertisment;
                            image.ImageFile = stream.ToArray();

                            _context.Images.Add(image);
                        }
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(UserAdvertisments));
            }
            return View(advertisment);
        }

        // GET: Advertisments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            return await GetEdit(id);
        }

        public async Task<IActionResult> AcceptAdvertismentEdit(int? id)
        {
            return await GetEdit(id);
        }

        public async Task<IActionResult> GetEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var advertisment = await _context.Advertisments
                .Include(a => a.AdvertismentCreator)
                .AsNoTracking()
                .Include(a => a.AdvertismentImages)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ID == id);

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
        public async Task<IActionResult> Edit(int id, [Bind("ID,UserID,DateOfPublication,Accepted,DecisionMade,Description,AdditionalEquipmentDescription,Price,Address,AdvertismentCreator,AdvertismentImages")] Advertisment advertisment)
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
                return RedirectToAction(nameof(UserAdvertisments));
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
                .Include(a => a.AdvertismentCreator)
                .AsNoTracking()
                .Include(a => a.AdvertismentImages)
                .AsNoTracking()
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
            var advertisment = await _context.Advertisments
                .Include(a => a.AdvertismentCreator)
                .AsNoTracking()
                .Include(a => a.AdvertismentImages)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ID == id);
            _context.Advertisments.Remove(advertisment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(UserAdvertisments));
        }

        private bool AdvertismentExists(int id)
        {
            return _context.Advertisments.Any(e => e.ID == id);
        }
    }
}