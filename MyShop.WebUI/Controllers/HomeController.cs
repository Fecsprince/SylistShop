using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using MyShop.WebUI.Models;
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
        IRepository<Service> serviceContext;
        IRepository<Shop> shopContext;
        IRepository<ProductCategory> Catecontext;

        public HomeController(IRepository<Product> Prodcontext,
                            IRepository<Service> Servicecontext,
                            IRepository<Shop> Shopcontext,
                            IRepository<ProductCategory> ProdCatecontext)
        {
            context = Prodcontext;
            serviceContext = Servicecontext;
            shopContext = Shopcontext;
            Catecontext = ProdCatecontext;
        }

        [HttpGet]
        public ActionResult Index(string serviceLocation)
        {
            if (TempData["Msg"] != null)
            {
                ViewBag.Msg = TempData["Msg"].ToString();
            }
            List<Service> Services = new List<Service>();


            List<Product> Products = context.Collection().ToList();
            if (serviceLocation != null)
            {

                List<Shop> Shops = shopContext.Collection().Where(x => x.Location.Contains(serviceLocation) ||
                                                       x.LGA.Contains(serviceLocation) ||
                                                       x.Id == serviceLocation ||
                                                       x.State.Contains(serviceLocation)).ToList();

                if (Shops.Count > 0)
                {
                    foreach (var shop in Shops)
                    {
                        var servs = serviceContext.Collection().Where(x => x.ShopID == shop.Id).ToList();

                        if (servs.Count > 0)
                        {
                            foreach (var s in servs)
                            {
                                Services.Add(s);
                            }
                        }
                    }
                }

            }
            else
            {
                Services = serviceContext.Collection().ToList();
            }

            IndexVM model = new IndexVM
            {
                Products = Products,
                services = Services
            };

            return View(model);
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