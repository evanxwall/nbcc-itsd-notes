using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyFirstMVCApp.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.CompanyName = "Acme Corporation";
            ViewBag.EndTag = "All Rights Reserved, 2026";
            // Simillar to storing data in tags with JSON.

            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Products()
        {
            return View("OurCompanyProduct");
            // Can Override the default view if names don't match.
        }

        public ActionResult GetPaySlip(int empID)
        {
            // Same as GetEmpName, Didn't work for me but probably using the wrong url
            // https://localhost:44377/home/GetPaySlip/empID=1
            string fileNmae = "~/PaySlip" + empID + ".pdf";
            return File(fileNmae, "application/pdf");
        }

        public ActionResult Redirect()
        {
            // Redirects to a Different website, this one worked no issue
            // https://localhost:44377/home/redirect

            string fbUrl = "https://www.sportsnet.ca";
            return Redirect(fbUrl);
        }

        public ActionResult GetEmpName(int empID)
        {
            // You can actually access this through the URL without buttons or anything
            // Didn't work for me but https://localhost:44377/home/GetEmoName/EmpID=2 atleast something along theses lines

            var employees = new[]
            {
                new { EmpID = 1, EmpName = "Chad", Salary = 8000},
                new { EmpID = 2, EmpName = "Thad", Salary = 6500},
                new { EmpID = 3, EmpName = "Brad", Salary = 9000}
            };

            string matchEmpName = null;

            foreach (var item in employees)
            {
                if (item.EmpID == empID)
                {
                    matchEmpName = item.EmpName;
                }
            }

            return Content(matchEmpName, "text/plain");
        }







    }
}