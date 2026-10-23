using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RoutingDemo.Controllers
{
    public class ProductsController : Controller // Creates a folder in Views called whatever the controler is
    {
        // GET: Products
        public ActionResult Index()
        {
            return View();
        }


        // This is where we are going to set attribute route - annotation
        [Route("Products/GetProductID/{productName}")]
        public ActionResult GetProductID(string productName)
        {
            var products = new[]
            {
                new {ProductID = 1, ProductName = "Banana", Cost = 5.99},
                new {ProductID = 2, ProductName = "Apple", Cost = 8.99},
                new {ProductID = 3, ProductName = "Mango", Cost = 3.99 }
            };

            string prodName = "";
            int prodId = 0;

            if (productName == null)
            {
                return Content("Please Pass any product Name");
            }
            else
            {
                foreach (var pro in products)
                {
                    if (pro.ProductName == productName)
                    {
                        prodId = pro.ProductID;

                    }
                }
            }

            return Content(prodId.ToString());
        }

        [Route("Products/Details/{id:range(1,3)?}")] // Can put regex and other things
        public ActionResult Details(int? id)
        {

            var products = new[]
            {
                new {ProductID = 1, ProductName = "Banana", Cost = 5.99},
                new {ProductID = 2, ProductName = "Apple", Cost = 8.99},
                new {ProductID = 3, ProductName = "Mango", Cost = 3.99 }
            };

            int prodId = 0;
            string prodName = "";

            foreach (var pro in products)
            {
                if (pro.ProductID == id)
                {
                    prodId = pro.ProductID;
                    prodName = pro.ProductName;
                }
            }

            return Content(prodName.ToString());
        }
    }
}