using MTProvider.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MTProvider.Controllers
{
    [Authorize]
    public class ValuesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        [AllowAnonymous]
        public IEnumerable<Positions> GetOpenPositions(string Username)
        {
            return db.Positions.Where(a => a.ExpireDate >= DateTime.Now && a.UserName == Username && a.IsClose == false && a.PositionStatus == false && a.TicketNO == 0).ToArray();
        }

        [AllowAnonymous]
        public IList<Positions> GetClosePositions(string Username)
        {
            return db.Positions.Where(a => a.ExpireDate >= DateTime.Now && a.UserName == Username && a.IsClose == false && a.PositionStatus == true).ToList();
        }

        [AllowAnonymous]
        [HttpPost]
        public void SetOpenPositions(long ID, int TicketNO, double Price, double USDPrice)
        {
            Positions newpositions = db.Positions.Find(ID);
            /*Positions samePositions = db.Positions.Where(a => a.SymbolName == newpositions.SymbolName && a.PositionState == newpositions.PositionState && a.TicketNO != 0 && a.IsClose == false).SingleOrDefault();
            Positions opsPositions = db.Positions.Where(a => a.SymbolName == newpositions.SymbolName && a.PositionState == !newpositions.PositionState && a.TicketNO != 0 && a.IsClose == false).SingleOrDefault();
            if(samePositions != null)
            {
                samePositions.Price = (samePositions.Price * samePositions.Volume + Price * newpositions.Volume) / (samePositions.Volume + newpositions.Volume);
                samePositions.Volume += newpositions.Volume;
                newpositions.IsClose = true;
                db.Entry(newpositions).State = System.Data.Entity.EntityState.Modified;
                db.Entry(samePositions).State = System.Data.Entity.EntityState.Modified;
            }
            else if (opsPositions != null)
            {
                if(newpositions.Volume > opsPositions.Volume)
                {
                    newpositions.TicketNO = TicketNO;
                    newpositions.Price = Price;
                    newpositions.Volume -= opsPositions.Volume;
                    opsPositions.IsClose = true;
                    db.Entry(newpositions).State = System.Data.Entity.EntityState.Modified;
                    db.Entry(opsPositions).State = System.Data.Entity.EntityState.Modified;
                }
                else if (newpositions.Volume < opsPositions.Volume)
                {
                    opsPositions.Volume -= newpositions.Volume;
                    newpositions.IsClose = true;
                    db.Entry(newpositions).State = System.Data.Entity.EntityState.Modified;
                    db.Entry(opsPositions).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    opsPositions.IsClose = true;
                    opsPositions.ClosePrice = Price;
                    newpositions.IsClose = true;
                    db.Entry(opsPositions).State = System.Data.Entity.EntityState.Modified;
                }
                
            }
            else
            {*/
            newpositions.TicketNO = TicketNO;
            newpositions.Price = Price;
            newpositions.PriceInUSD = USDPrice;
            db.Entry(newpositions).State = System.Data.Entity.EntityState.Modified;
            //}
            db.SaveChanges();
        }

        [AllowAnonymous]
        [HttpPost]
        public void SetClosePositions(long ID, double ClosePrice)
        {
            Positions positions = db.Positions.Find(ID);
            positions.IsClose = true;
            positions.ClosePrice = ClosePrice;
            db.Entry(positions).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        [AllowAnonymous]
        [HttpPost]
        public void SetSymbolPrice(string Symbol, double AskPrice, double BidPrice, double PerUSDAsk, double PerUSDBid)
        {
            MTSymbols mTSymbols = db.MTSymbols.Find(Symbol);
            mTSymbols.AskPrice = AskPrice;
            mTSymbols.BidPrice = BidPrice;
            mTSymbols.PerUSDAsk = PerUSDAsk;
            mTSymbols.PerUSDBid = PerUSDBid;
            db.Entry(mTSymbols).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        [AllowAnonymous]
        public IList<MTHistoryMSDTO> GetHistoryMS()
        {
            return (from ms in db.MTHistoryMS
                   select new MTHistoryMSDTO
                   {
                       ID = ms.ID,
                       Symbol = ms.Symbol,
                       Period = ms.Period,
                       StartTime = db.MTHistoryDT.Where(a => a.MSID == ms.ID).Max(a => a.Time) ?? ms.StartTime,
                       EndTime = DateTime.Now
                   }).Union(
                    from ms in db.MTHistoryMS
                    select new MTHistoryMSDTO
                    {
                        ID = ms.ID,
                        Symbol = ms.Symbol,
                        Period = ms.Period,
                        StartTime = ms.StartTime,
                        EndTime = db.MTHistoryDT.Where(a => a.MSID == ms.ID).Min(a => a.Time) ?? ms.StartTime
                    }).ToList();

            //return db.MTHistoryMS.Select(a => new { ID = a.ID, Symbol = a.Symbol, Period = a.Period, StartTime = Max(db.MTHistoryDT.Where(b => b.MSID == a.ID).Max(b => b.Time)/*Select(a=>a.Time).DefaultIfEmpty(DateTime.MinValue).Max()*/, a.StartTime) }).ToArray();
        }

        [AllowAnonymous]
        [HttpPost]
        public void SetHistoryMSTicks(short ID, int allTicks)
        {
            MTHistoryMS mtHistoryMS = db.MTHistoryMS.Where(a => a.ID == ID).FirstOrDefault();
            mtHistoryMS.AllTicks += allTicks;
            
            db.SaveChanges();
        }

        [AllowAnonymous]
        [HttpPost]
        public void SetHistoryDT(short MSID, double Close, double Open, double High, double Low, DateTime Time, double Volume)
        {
            MTHistoryDT mTHistorys = new MTHistoryDT();
            //mTHistorys.ID = BitConverter.GetBytes(DateTime.UtcNow.Ticks).Reverse().ToArray();
            mTHistorys.MSID = MSID;
            mTHistorys.Close = Close;
            mTHistorys.Open = Open;
            mTHistorys.High = High;
            mTHistorys.Low = Low;
            mTHistorys.Time = Time;
            mTHistorys.Volume = Volume;
            db.MTHistoryDT.Add(mTHistorys);

            MTHistoryMS mtHistoryMS = db.MTHistoryMS.Where(a => a.ID == MSID).FirstOrDefault();
            mtHistoryMS.MyTicks++;

            //db.Entry(mTHistorys).State = System.Data.Entity.EntityState.Added;
            db.SaveChanges();
        }


        static DateTime? Max(DateTime? a, DateTime? b)
        {
            if (!a.HasValue && !b.HasValue) return a;  // doesn't matter

            if (!a.HasValue) return b;
            if (!b.HasValue) return a;

            return a.Value > b.Value ? a : b;
        }



        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [AllowAnonymous]
        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
