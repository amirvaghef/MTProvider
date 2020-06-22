using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MTProvider.Controllers
{
    public class MTSymbolsController : Controller
    {
        // GET: MTSymbols
        public ActionResult Index()
        {
            return View();
        }

        // GET: MTSymbols/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MTSymbols/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MTSymbols/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: MTSymbols/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MTSymbols/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: MTSymbols/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MTSymbols/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
