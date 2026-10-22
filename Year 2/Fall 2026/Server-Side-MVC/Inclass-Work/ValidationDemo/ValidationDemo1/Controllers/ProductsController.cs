using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelExample.Models;

namespace ValidationDemo1.Controllers
{
    public class ProductsController : Controller
    {
        // GET: Products
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Product p)
        {
            if (ModelState.IsValid)
            {
                // We are not connected to a DB but you can:
                // Write to a Database
                // Return a view or redirect somewheres else
                // etc...
                return View();
            }
            else
            {
                return View();
            }
        }
    }
}