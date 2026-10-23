using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelDemo.Models; // Allowing us to use our models within a controller

namespace ModelDemo.Controllers
{
    public class ProductsController : Controller
    {
        // GET: Products
        public ActionResult Index()
        {
            // Product p = new Product();
            // After putting the using statement, we can access to classes like any other classes

            List<Product> products = ProductData.GetProducts();


            return View(products);
        }

        public ActionResult Details(int id)
        {
            List<Product> products = ProductData.GetProducts();

            Product matchingProduct = null;

            foreach (Product product in products)
            {
                if (product.ProductId == id)
                    matchingProduct = product;
            }


            return View(matchingProduct);
        }

        public ActionResult Create()
        {
            return View();
            // Goes through the URL Create, will use this method
        }

        [HttpPost]
        public ActionResult Create(Product p)
        {

            return View();
            // When data gets posted, it will go throught this other Create() method instead
        }
    }
}