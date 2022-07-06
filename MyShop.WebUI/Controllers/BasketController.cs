using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyShop.Core.Contracts;
using System.Web.Mvc;
using MyShop.Core.Models;

namespace MyShop.WebUI.Controllers
{
    public class BasketController : Controller
    {
        IRepository<Customer> customers;
        IBasketService basketService;
        IOrderService orderService;

        public BasketController(IBasketService Basketservice, IOrderService OrderService, IRepository<Customer> Customers)
        {
            this.basketService = Basketservice;
            this.orderService = OrderService;
            this.customers = Customers;
        }

        // GET: Basket
        public ActionResult Index()
        {
            var model = basketService.GetBasketItem(this.HttpContext);
            return View(model);
        }

        public ActionResult AddToBasket(string id)
        {
            basketService.AddToBasket(this.HttpContext, id);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult RemoveFromBasket(string id)
        {
            basketService.RemoveBasket(this.HttpContext, id);
            return RedirectToAction("Index");
        }


        // Account Summary amount and quantity on Menu-bar
        public PartialViewResult BasketSummary()
        {
            var basketsum = basketService.GetBasketSummary(this.HttpContext);

            return PartialView(basketsum);
        }

        [Authorize]
        public ActionResult Checkout()
        {
            Customer customer = customers.Collection().FirstOrDefault(i => i.Email == User.Identity.Name);

            if (customer != null)
            {
                Order ord = new Order()
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    City = customer.City,
                    State = customer.State,
                    Zipcode = customer.Zipcode,
                    Street = customer.Street,

                };
                return View(ord);
            }

            else
            {

                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult Checkout(Order order)
        {
            var basketItem = basketService.GetBasketItem(this.HttpContext);
            order.OrderStatus = "Order Created";
            order.Email = User.Identity.Name;
            //Payment Process


            order.OrderStatus = "Payment processed";
            orderService.CreateOrder(order, basketItem);


            foreach (var item in basketItem)
            {
                basketService.RemoveBasket(this.HttpContext, item.Id);
            }

            basketService.ClearBasket(this.HttpContext);


            return RedirectToAction("Thankyou", new { orderId = order.Id });
        }

        public ActionResult Thankyou(string orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }
    }
}