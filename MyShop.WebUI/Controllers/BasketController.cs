using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyShop.Core.Contracts;
using System.Web.Mvc;
using MyShop.Core.Models;
using Microsoft.AspNet.Identity;
using MyShop.Core.Supports;
using MyShop.WebUI.Models;
using System.Text;
using MyShop.DataAccess.SQL;

namespace MyShop.WebUI.Controllers
{
    public class BasketController : Controller
    {
        IRepository<Customer> customers;
        IRepository<Order> orders;
        IRepository<Product> products;
        IBasketService basketService;
        IOrderService orderService;
        private ApplicationDbContext con = new ApplicationDbContext();


        public BasketController(IBasketService Basketservice, IOrderService OrderService,
                                IRepository<Customer> Customers,
                                IRepository<Order> Orders, IRepository<Product> Products)
        {
            this.basketService = Basketservice;
            this.orderService = OrderService;
            this.customers = Customers;
            orders = Orders;
            products = Products;
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

        [Authorize(Roles = "Customer")]
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
        [Authorize(Roles = "Customer")]
        public ActionResult Checkout(Order order)
        {


            StringBuilder mailResponse = new StringBuilder();
            //var prod = products.Collection();


            var basketItem = basketService.GetBasketItem(this.HttpContext);
            order.OrderStatus = "Order Created";
            order.Email = User.Identity.Name;
            //Payment Process


            order.OrderStatus = "Payment processed";
            orderService.CreateOrder(order, basketItem);
            //PROCESS EMAIL AND SEND TO THE CUSTOMER AND STORE MANAGER


            //EMAIL TO CUSTOMER
            IdentityMessage custidentyMessage = new IdentityMessage()
            {
                Destination = order.Email,
                Subject = "Order Processed"
            };
            var callbackUrl = Url.Action("ViewOrder", "Basket", new { @OrderId = order.Id }, protocol: Request.Url.Scheme);

            string html = String.Format("<h3>Customer: " + order.FirstName + " " + order.LastName + "</h3>" +
                "<h3>Email: " + order.Email + "</h3>" +
                "<h3>Address: " + order.Street + ", " + order.City + ", " + order.State + "</h3>" +
                "<h3>Product Id: " + order.ProductID + "</h3>" +
                "<h3>Order: click on the link to view customer's order " + callbackUrl + "</h3>");


            MailHelper sendMail = new MailHelper();
            ConfirmEmailSend sendMsg = sendMail.SendMail(custidentyMessage, html);

            mailResponse.Append(sendMsg.Message.ToString());
            TempData["Msg"] = "Order Processed\n" + sendMsg.Message.ToString();
            //EMPTY CART
            foreach (var item in basketItem)
            {
                basketService.RemoveBasket(this.HttpContext, item.Id);
            }

            basketService.ClearBasket(this.HttpContext);

            //SEND EMAIL TO SHOP OWNER
            //GET PRODUCT TO GET USER WHO CREATED IT

            //var orderDetails = order.OrderItems;
            //foreach (var prodId in orderDetails)
            //{
            //    //var prod = prodRepo.Find(prodId.ProductId);//.Where(x => x.Id == prodId.ProductId).FirstOrDefault();

            //    if (prod != null)
            //    {
            //        //PRODUCT STILL EXIST
            //        //GET USER
            //        var storeManager = con.Users.Where(x => x.Id == "ProductID Here").FirstOrDefault();
            //        if (storeManager != null)
            //        {
            //            //STORE MANAGER EXIST
            //            IdentityMessage shopOwnerIdentityMessage = new IdentityMessage()
            //            {
            //                Destination = storeManager.Email,
            //                Subject = "Order Processed"
            //            };

            //            string html2 = String.Format("<h3>Customer: " + order.FirstName + " " + order.LastName + "</h3>" +
            //                        "<h3>Email: " + order.Email + "</h3>" +
            //                        "<h3>Address: " + order.Street + ", " + order.City + ", " + order.State + ", ZipCode" + order.Zipcode + "</h3>" +
            //                        "<h3>Product Id: " + order.ProductID + "</h3>" +
            //                        "<h3>Order: click on the link to view customer's order " + callbackUrl + "</h3>");

            //            MailHelper sendMail2 = new MailHelper();
            //            ConfirmEmailSend sendMsg2 = sendMail.SendMail(shopOwnerIdentityMessage, html2);


            //            mailResponse.Append("\n " + sendMsg2.Message.ToString());

            //            TempData["Msg"] = mailResponse.ToString();


            //            //EMPTY CART
            //            foreach (var item in basketItem)
            //            {
            //                basketService.RemoveBasket(this.HttpContext, item.Id);
            //            }

            //            basketService.ClearBasket(this.HttpContext);


            //            return RedirectToAction("Index", "Home");

            //        }
            //        else
            //        {
            //            //STORE MANAGER DOES NOT EXIST
            //            TempData["Msg"] = "STORE MANAGER DOES NOT EXIST OR HAS BEEN SUSPENDED, " +
            //                "HENCE YOUR ORDER WILL NOT BE PROCESSED!";
            //            return RedirectToAction("Index", "Home");
            //        }
            //    }
            //    else
            //    {
            //        //PRODUCT DOES NOT EXIST AGAIN
            //        TempData["Msg"] = "PRODUCT DOES NOT EXIST OR HAS BEEN DELETED, " +
            //               "HENCE YOUR ORDER WILL NOT BE PROCESSED!";
            //        return RedirectToAction("Index", "Home");
            //    }
            //}
            //TempData["Msg"] = "ORDER FAILED";
            return RedirectToAction("Index", "Home");

        }

        [HttpGet]
        public ActionResult ViewOrder(string OrderId)
        {
            var dbOrder = orders.Find(OrderId);
            if (dbOrder != null)
            {
                return View(dbOrder);
            }
            TempData["Msg"] = "Order not found!";
            return RedirectToAction("Index","Home");
        }


        public ActionResult Thankyou(string orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }
    }
}