using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModelDemo.Models
{
    public static class ProductData
    {

        public static List<Product> GetProducts()
        {

            List<Product> products = new List<Product>()
            {
                new Product() {ProductId = 1, ProductName = "Banana", Rate = 7.99},
                new Product() {ProductId = 2, ProductName = "Mango", Rate = 5.99},
                new Product() {ProductId = 3, ProductName = "Grapes", Rate = 12.99}
            };

            return products;

        }
        
    }
}