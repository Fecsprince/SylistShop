using MyShop.Core.Contracts;
using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebUI.Controllers
{
    public class HomeController : Controller
    {

        IRepository<Product> context;
        IRepository<ProductCategory> Catecontext;

        public HomeController(IRepository<Product> Prodcontext, IRepository<ProductCategory> ProdCatecontext)
        {
            context = Prodcontext;
            Catecontext = ProdCatecontext;
        }

        public ActionResult Index()
        {
            List<Product> Products = context.Collection().ToList();
            return View(Products);
        }

        public ActionResult Details(string id)
        {
            Product prod = context.Find(id);
            if (prod == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(prod);
            }
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}