using MTProvider.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MTProvider.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(int? refreshTime)
        {
            ViewBag.Title = "Current Positions";
            ViewBag.sum = 0;

            IEnumerable<PositionsDTO> positions = (from p in db.Positions
                                                   join s in db.MTSymbols on p.SymbolName equals s.Name
                                                   where p.IsClose == false && p.TicketNO != 0
                                                   orderby p.PositionState ascending
                                                   select new PositionsDTO
                                                   {
                                                       ID = p.ID,
                                                       RowNO = p.RowNO,
                                                       PositionState = p.PositionState,
                                                       Volume = p.Volume,
                                                       SymbolName = p.SymbolName,
                                                       MarketPrice = !p.PositionState ? s.BidPrice : s.AskPrice,
                                                       Profit = !p.PositionState ? (s.BidPrice - p.Price) * p.Volume * 100000 * s.PerUSDAsk : (p.Price - s.AskPrice) * p.Volume * 100000 * s.PerUSDBid,
                                                       ProfitPercent = (!p.PositionState ? (s.BidPrice - p.Price) * p.Volume * 100000 * s.PerUSDAsk : (p.Price - s.AskPrice) * p.Volume * 100000 * s.PerUSDBid) / (p.Volume * 100000 * p.PriceInUSD / p.Leverage) * 100,
                                                       Price = p.Price,
                                                       UserName = p.UserName,
                                                       Leverage = p.Leverage
                                                   }).ToArray();
            if (refreshTime > 0)
            {
                ViewBag.refreshTime = refreshTime;
                Response.AppendHeader("Refresh", refreshTime.ToString());
            }
            else
            {
                ViewBag.refreshTime = 30;
                Response.AppendHeader("Refresh", "30");
            }
            

            return View(positions);
        }

        public ActionResult Close(long id)
        {
            ViewBag.Title = "close";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Positions positions = db.Positions.Find(id);
            if (positions == null)
            {
                return HttpNotFound();
            }
            positions.PositionStatus = true;
            positions.ExpireDate = DateTime.Now.AddMinutes(50);
            db.Entry(positions).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult CloseGroup(string symbolName, bool positionState)
        {
            IEnumerable<Positions> positions = db.Positions.Where(a => a.IsClose == false && a.TicketNO != 0 && a.SymbolName.Equals(symbolName) && a.PositionState == positionState);
            foreach (Positions position in positions)
            {
                position.PositionStatus = true;
                position.ExpireDate = DateTime.Now.AddMinutes(50);
                db.Entry(position).State = System.Data.Entity.EntityState.Modified;
            }
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult CloseAll(bool profitPoses = false)
        {
            ViewBag.Title = "Close all";
            IEnumerable<Positions> positions = db.Positions.Where(a => a.IsClose == false && a.TicketNO != 0);
            if(profitPoses)
                positions = (from p in db.Positions
                             join s in db.MTSymbols on p.SymbolName equals s.Name
                             where p.IsClose == false && p.TicketNO != 0 && (!p.PositionState ? (s.BidPrice - p.Price) > 0 : (p.Price - s.AskPrice) > 0)
                             select p).ToArray();
            foreach (Positions position in positions)
            {
                position.PositionStatus = true;
                position.ExpireDate = DateTime.Now.AddMinutes(50);
                db.Entry(position).State = System.Data.Entity.EntityState.Modified;
            }
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
