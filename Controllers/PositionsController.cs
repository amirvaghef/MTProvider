using MTProvider.Models;
using MTProvider.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Core.Objects;

namespace MTProvider.Controllers
{
    public class PositionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Positions
        public ActionResult Index(Positions positions)
        {

            ViewBag.Symbols = new SelectList(db.MTSymbols.ToList(), "Name", "Name");

            string symbol = ((SelectList)ViewBag.Symbols).First().Value;
            if (positions.SymbolName != null)
            {
                symbol = positions.SymbolName;
                foreach (var modelValue in ModelState.Values)
                    modelValue.Errors.Clear();
            }

            ViewBag.LastYearD1 = (from ms in db.MTHistoryMS
                                  join dt in db.MTHistoryDT on ms.ID equals dt.MSID
                                  where ms.Symbol == symbol && ms.Period == 16408 /*D1*/ && dt.Time >= EntityFunctions.AddDays(EntityFunctions.AddYears(DateTime.Now, -1), -15) && dt.Time <= EntityFunctions.AddDays(EntityFunctions.AddYears(DateTime.Now, -1), 15)
                                  select dt).ToArray();
            ViewBag.CurYearD1 = (from ms in db.MTHistoryMS
                                 join dt in db.MTHistoryDT on ms.ID equals dt.MSID
                                 where ms.Symbol == symbol && ms.Period == 16408 /*D1*/ && dt.Time >= EntityFunctions.AddDays(DateTime.Now, -15) && dt.Time <= DateTime.Now
                                 select dt).ToArray();

            ViewBag.Last3YearMN1 = (from ms in db.MTHistoryMS
                                    join dt in db.MTHistoryDT on ms.ID equals dt.MSID
                                    where ms.Symbol == symbol && ms.Period == 49153 /*MN1*/ && dt.Time >= EntityFunctions.AddYears(DateTime.Now, -3) && dt.Time <= DateTime.Now
                                    select dt).ToArray();

            ViewBag.Last3MonthsD1 = (from ms in db.MTHistoryMS
                                     join dt in db.MTHistoryDT on ms.ID equals dt.MSID
                                     where ms.Symbol == symbol && ms.Period == 16408 /*D1*/ && dt.Time >= EntityFunctions.AddMonths(DateTime.Now, -3) && dt.Time <= DateTime.Now
                                     select dt).ToArray();


            return View();
        }

        //[HttpPost]
        //[MultipleButton(Name = "action", Argument = "Change")]
        //public ActionResult Change(Positions positions)
        //{
        //    //ViewBag.Symbols = new SelectList(db.MTSymbols.ToList(), "Name", "Name");
        //    string symbol = positions.SymbolName;
        //    ViewBag.Symbol = symbol;
        //    ViewBag.LastYearD1 = (from ms in db.MTHistoryMS
        //                          join dt in db.MTHistoryDT on ms.ID equals dt.MSID
        //                          where ms.Symbol == symbol && ms.Period == 16408 /*D1*/ && dt.Time >= EntityFunctions.AddDays(EntityFunctions.AddYears(DateTime.Now, -1), -15) && dt.Time <= EntityFunctions.AddDays(EntityFunctions.AddYears(DateTime.Now, -1), 15)
        //                          select dt).ToArray();
        //    ViewBag.CurYearD1 = (from ms in db.MTHistoryMS
        //                         join dt in db.MTHistoryDT on ms.ID equals dt.MSID
        //                         where ms.Symbol == symbol && ms.Period == 16408 /*D1*/ && dt.Time >= EntityFunctions.AddDays(DateTime.Now, -15) && dt.Time <= DateTime.Now
        //                         select dt).ToArray();

        //    ViewBag.Last3YearMN1 = (from ms in db.MTHistoryMS
        //                            join dt in db.MTHistoryDT on ms.ID equals dt.MSID
        //                            where ms.Symbol == symbol && ms.Period == 49153 /*MN1*/ && dt.Time >= EntityFunctions.AddYears(DateTime.Now, -3) && dt.Time <= DateTime.Now
        //                            select dt).ToArray();

        //    ViewBag.Last3MonthsD1 = (from ms in db.MTHistoryMS
        //                             join dt in db.MTHistoryDT on ms.ID equals dt.MSID
        //                             where ms.Symbol == symbol && ms.Period == 16408 /*D1*/ && dt.Time >= EntityFunctions.AddMonths(DateTime.Now, -3) && dt.Time <= DateTime.Now
        //                             select dt).ToArray();

        //    return View("Index");
        //}

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