using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ElectronEf.Models;
using ElectronEf.NW;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace ElectronEf.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly NorthwindContext _context;
        public HomeController (ILogger<HomeController> logger, NorthwindContext context) {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index () {
            return View ();
        }

        public IActionResult Chart () {
            ViewBag.CategoryProduct = this.getProductsByCategory ();
            return View ();
        }

        public async Task<IActionResult> SaveAs (string path) {
            System.IO.StringWriter writer = new System.IO.StringWriter ();
            writer.WriteLine ("Name,Count");

            var query = this.getProductsByCategory ();
            query.ForEach (item => {
                writer.Write (item.GetType ().GetProperty ("Name").GetValue (item));
                writer.Write (",");
                writer.WriteLine (item.GetType ().GetProperty ("Count").GetValue (item));
            });

            await System.IO.File.WriteAllTextAsync (path, writer.ToString ());
            return RedirectToAction ("Index");
        }

        public IActionResult Privacy () {
            return View ();
        }

        [ResponseCache (Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error () {
            return View (new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private List<object> getProductsByCategory () {
            var query = _context.Products
                .Include (c => c.Category)
                .GroupBy (p => p.Category.CategoryName)
                .Select (g => new {
                    Name = g.Key,
                        Count = g.Count ()
                })
                .OrderByDescending (cp => cp.Count);

            return query.ToList<object> ();
        }

    }
}