using MTProvider.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MTProvider.Controllers
{
    public class SymbolsEnableController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: SymbolsEnable
        public ActionResult Index()
        {
            ViewBag.MTAccounts = db.MTAccounts.ToList();
            ViewBag.MTSymbols = db.MTSymbols.ToList();

            return View(db.SymbolsEnable.ToList());
        }

        // POST: SymbolsEnable
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(IEnumerable<SymbolsEnable> symbolsEnable)
        {
            ViewBag.MTAccounts = db.MTAccounts.ToList();
            ViewBag.MTSymbols = db.MTSymbols.ToList();

            if (ModelState.IsValid)
            {
                foreach (SymbolsEnable symbolEnable in symbolsEnable)
                    db.Entry(symbolEnable).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(symbolsEnable);
        }
    }
}
