using MTProvider.Models;
using MTProvider.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MTProvider.Controllers
{
    public class PositionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Positions
        public ActionResult Index()
        {
            ViewBag.Symbols = new SelectList(db.MTSymbols.ToList(), "Name", "Name");
            return View();
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Buy")]
        public ActionResult Buy(Positions positions)
        {
            if (ModelState.IsValid)
            {
                IEnumerable<MTAccounts> mTAccounts = db.MTAccounts.ToArray();
                int rowNO = db.Positions.Select(m => m.RowNO).DefaultIfEmpty(0).Max();
                foreach (MTAccounts mTAccount in mTAccounts)
                {
                    if (db.SymbolsEnable.Where(a => a.SymbolName == positions.SymbolName && a.UserName == mTAccount.UserName).SingleOrDefault<SymbolsEnable>().Status == true)
                    {
                        Positions position = new Positions();
                        position.RowNO = rowNO + 1;
                        position.ExpireDate = DateTime.Now.AddMinutes(60);
                        position.IsClose = false;
                        position.PositionState = false;
                        position.PositionStatus = false;
                        position.SymbolName = positions.SymbolName;
                        position.PriceInUSDSymbol = db.MTSymbols.Where(a => a.Name == positions.SymbolName).FirstOrDefault().ChngFromUSDSymbol;
                        position.UserName = mTAccount.UserName;
                        position.Volume = Double.Parse(((positions.Volume / 10) * (mTAccount.Volume.Equals(DBNull.Value) ? 0 : mTAccount.Volume)).ToString());
                        db.Positions.Add(position);
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Sell")]
        public ActionResult Sell(Positions positions)
        {
            if (ModelState.IsValid)
            {
                IEnumerable<MTAccounts> mTAccounts = db.MTAccounts.ToArray();
                foreach (MTAccounts mTAccount in mTAccounts)
                {
                    if (db.SymbolsEnable.Where(a => a.SymbolName == positions.SymbolName && a.UserName == mTAccount.UserName).SingleOrDefault<SymbolsEnable>().Status == true)
                    {
                        Positions position = new Positions();
                        position.ExpireDate = DateTime.Now.AddMinutes(60);
                        position.IsClose = false;
                        position.PositionState = true;
                        position.PositionStatus = false;
                        position.SymbolName = positions.SymbolName;
                        position.PriceInUSDSymbol = db.MTSymbols.Where(a => a.Name == positions.SymbolName).FirstOrDefault().ChngFromUSDSymbol;
                        position.UserName = mTAccount.UserName;
                        position.Volume = Double.Parse(((positions.Volume / 10) * (mTAccount.Volume.Equals(DBNull.Value) ? 0 : mTAccount.Volume)).ToString());
                        db.Positions.Add(position);
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}