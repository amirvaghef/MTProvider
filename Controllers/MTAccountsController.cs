using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MTProvider.Models;

namespace MTProvider.Controllers
{
    public class MTAccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MTAccounts
        public ActionResult Index()
        {
            return View(db.MTAccounts.ToList());
        }

        //public ActionResult Percents()
        //{
        //    return View(db.MTAccounts.ToList());
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(IEnumerable<MTAccounts> mTAccounts)
        {
            if (ModelState.IsValid)
            {
                foreach(MTAccounts mTAccount in mTAccounts)
                    db.Entry(mTAccount).State = EntityState.Modified;
                db.SaveChanges();
                return View(mTAccounts);
            }
            return View(mTAccounts);
        }

        // GET: MTAccounts/Details/5
        //public ActionResult Details(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    MTAccounts mTAccounts = db.MTAccounts.Find(id);
        //    if (mTAccounts == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(mTAccounts);
        //}

        // GET: MTAccounts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MTAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserName,Password,Volume")] MTAccounts mTAccounts)
        {
            if (ModelState.IsValid)
            {
                db.MTAccounts.Add(mTAccounts);
                foreach(MTSymbols symbol in db.MTSymbols.ToArray())
                {
                    SymbolsEnable symbolsEnable = new SymbolsEnable();
                    symbolsEnable.UserName = mTAccounts.UserName;
                    symbolsEnable.SymbolName = symbol.Name;
                    db.SymbolsEnable.Add(symbolsEnable);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mTAccounts);
        }

        // GET: MTAccounts/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MTAccounts mTAccounts = db.MTAccounts.Find(id);
            if (mTAccounts == null)
            {
                return HttpNotFound();
            }
            return View(mTAccounts);
        }

        // POST: MTAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserName,Password")] MTAccounts mTAccounts)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mTAccounts).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mTAccounts);
        }

        // GET: MTAccounts/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MTAccounts mTAccounts = db.MTAccounts.Find(id);
            if (mTAccounts == null)
            {
                return HttpNotFound();
            }
            return View(mTAccounts);
        }

        // POST: MTAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            MTAccounts mTAccounts = db.MTAccounts.Find(id);
            IEnumerable<SymbolsEnable> symbolsEnables = db.SymbolsEnable.Where(a => a.UserName == mTAccounts.UserName).ToArray();
            db.SymbolsEnable.RemoveRange(symbolsEnables);
            db.MTAccounts.Remove(mTAccounts);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
