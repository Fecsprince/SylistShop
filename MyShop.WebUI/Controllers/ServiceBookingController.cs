using Microsoft.AspNet.Identity;
using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.Supports;
using MyShop.Core.ViewModels;
using MyShop.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebUI.Controllers
{
    [Authorize(Roles = "Customer, SuperAdmin, StoreManager, ShopManager")]
    public class ServiceBookingController : Controller
    {
        IRepository<Service> serviceContext;
        IRepository<Booking> bookingContext;
        IRepository<Shop> shopcontext;
        private ApplicationDbContext context = new ApplicationDbContext();

        public ServiceBookingController(IRepository<Service> Servicecontext,
                                        IRepository<Booking> Bookingcontext,
                                        IRepository<Shop> Shopcontext)
        {
            this.serviceContext = Servicecontext;
            this.shopcontext = Shopcontext;
            this.bookingContext = Bookingcontext;
        }


        [HttpGet]
        public ActionResult BookNow(string Id)
        {
            //GET SERVICE AND USE IT TO INSTANTIATE NEW INSTANCE OF BOOKING AND RETURN TO BOOKING VIEW
            var service = serviceContext.Find(Id);
            var user = context.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();

            var shop = shopcontext.Find(service.ShopID);
            if (shop != null)
            {
                string id = Guid.NewGuid().ToString();

                Booking model = new Booking()
                {
                    Id = id,
                    ShopName = shop.Name,
                    ServiceID = service.Id,
                    Address = shop.Location + ", " + shop.LGA + ", " + shop.State,
                    UserID = user.Id

                };
                TempData["id"] = id;

                ViewBag.OpeningTime = shop.OpeningHour;
                ViewBag.ClosingTime = shop.ClosingHour;

                ExtraProperties extraPro = new ExtraProperties();
                ViewBag.TimeList = extraPro.TimeList();

                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BookNow(Booking model)
        {
            var shop = shopcontext.Collection().Where(x => x.Name == model.ShopName).FirstOrDefault();
            var service = serviceContext.Find(model.ServiceID);
            var user = context.Users.Where(x => x.Email == User.Identity.Name).SingleOrDefault();

            ExtraProperties extraPro = new ExtraProperties();


            if (TempData["id"] != null)
            {
                model.Id = TempData["id"].ToString();
            }
            else
            {
                model.Id = Guid.NewGuid().ToString();
            }


            //CHECK IF THE SHOP HAS 2 SCHEDULE OF SAME TIME
            var chkBooking = bookingContext.Collection().Where(x => x.AppointmentDate ==
            model.AppointmentDate && x.AppointmentTime == model.AppointmentTime).ToList();
            if (chkBooking.Count < 2)
            {
                if (ModelState.IsValid)
                {

                    var add = bookingContext.Insert(model);
                    if (add != null)
                    {

                        //PROCESS EMAIL
                        var callbackUrl = Url.Action("ViewBooking", "ServiceBooking", new { @BookingId = add.Id }, protocol: Request.Url.Scheme);

                        //SEND EMAIL TO CUSTOMER

                        IdentityMessage custidentyMessage = new IdentityMessage()
                        {
                            Destination = user.Email,
                            Subject = "Service Booking"
                        };

                        string html = "You've successfully booked for " + service.Name + " service and your booking ID is " + model.Id +
                            ". Your Booking ID can be use to re-schedule your booking date." +
                             "To view your schedule, kindly click on here <a href=\"" + callbackUrl + "\">here</a>";

                        MailHelper sendMail = new MailHelper();
                        ConfirmEmailSend sendMsg = sendMail.SendMail(custidentyMessage, html);

                        //SEND EMAIL TO SHOP OWNER

                        var shopOwnerEmail = context.Users.Where(x => x.Id == shop.UserID).FirstOrDefault();
                        if (shopOwnerEmail != null)
                        {
                            IdentityMessage shopOwnerIdentityMessage = new IdentityMessage()
                            {
                                Destination = shopOwnerEmail.Email,
                                Subject = "Service Booking"
                            };

                            string html2 = user.Email + " successfully booked for " + service.Name + " service " + ", to view details, kindly click on here <a href=\"" + callbackUrl + "\">here</a>";

                            MailHelper sendMail2 = new MailHelper();
                            ConfirmEmailSend sendMsg2 = sendMail.SendMail(shopOwnerIdentityMessage, html2);

                            if (sendMsg2.Result == true || sendMsg2.Result == true)
                            {
                                TempData["Msg"] = "Appointment booked successfully, " +
                                                    "Please kindly check your email for confirmation!\nCustomer Mail Response:" +
                                                    sendMsg2.Message;
                            }
                            else
                            {
                                TempData["Msg"] = "Appointment booked successfully\nCustomer Mail Response:" + sendMsg2.Message;

                            }
                        }


                        return RedirectToAction("Index", "Home");
                    }



                    TempData["Msg"] = "Service booking failed!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.OpeningTime = shop.OpeningHour;
                    ViewBag.ClosingTime = shop.ClosingHour;


                    ViewBag.TimeList = extraPro.TimeList();

                    ViewBag.Msg = "Please check your entries and try again!";
                    return View(model);
                }
            }
            else
            {
                ViewBag.OpeningTime = shop.OpeningHour;
                ViewBag.ClosingTime = shop.ClosingHour;


                ViewBag.TimeList = extraPro.TimeList();

                ViewBag.Msg = "The date and time selected has been filled, " +
                    "please choose another date or different time and try again!";
                return View(model);
            }


        }


        [HttpGet]
        public ActionResult ViewBooking(string BookingId)
        {
            if (BookingId != null)
            {
                var booking = bookingContext.Find(BookingId);
                if (booking != null)
                {
                    var service = serviceContext.Find(booking.ServiceID);
                    var shop = shopcontext.Collection().Where(x => x.Id == service.ShopID).FirstOrDefault();
                    var user = context.Users.Where(x => x.Id == booking.UserID).SingleOrDefault();

                    string svc = "";
                    string ShopName = "";
                    decimal amount = 0.0M;
                    string customerPhone = "";

                    //CONVERT DATE TO STRING
                    string year = booking.AppointmentDate.Year.ToString();
                    string month = booking.AppointmentDate.Month.ToString();
                    string day = booking.AppointmentDate.Day.ToString();

                    string date = day + "/" + month + "/" + year;
                    svc = service.Name;
                    ShopName = shop.Name;
                    amount = service.Price;
                    customerPhone = user.PhoneNumber;
                    //GET


                    //POPULATE BOOKING VIEW MODEL
                    BookingViewModel model = new BookingViewModel()
                    {
                        Address = booking.Address,
                        Amount = amount,
                        CustomerPhone = customerPhone,
                        Date = date,
                        Service = svc,
                        Shop = ShopName,
                        Time = booking.AppointmentTime
                    };

                    return View(model);
                }
                else
                {
                    TempData["Msg"] = "BOOKING NOT FOUND!";
                    return RedirectToAction("Index", "Home");
                }
            }
            TempData["Msg"] = "SOMETHING WENT WRONG!";
            return RedirectToAction("Index", "Home"); ;
        }

        [HttpGet]
        public ActionResult CancelBooking(string bookingId)
        {
            if (bookingId != null && bookingId != "")
            {
                var deleteDbBooking = bookingContext.Find(bookingId);
                if (deleteDbBooking != null)
                {
                    bookingContext.Delete(deleteDbBooking.Id);
                    TempData["Msg"] = "Service Booking canceled successfully!";
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        //USE TO VIEW BOOKING
        [HttpGet]
        public ActionResult Booking()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Booking(string bookingId)
        {
            if (bookingId != null && bookingId != "")
            {
                var booking = bookingContext.Find(bookingId);
                if (booking != null)
                {
                    var service = serviceContext.Find(booking.ServiceID);
                    var shop = shopcontext.Collection().Where(x => x.Id == service.ShopID).FirstOrDefault();
                    var user = context.Users.Where(x => x.Id == booking.UserID).SingleOrDefault();

                    string svc = "";
                    string ShopName = "";
                    decimal amount = 0.0M;
                    string customerPhone = "";

                    //CONVERT DATE TO STRING
                    string year = booking.AppointmentDate.Year.ToString();
                    string month = booking.AppointmentDate.Month.ToString();
                    string day = booking.AppointmentDate.Day.ToString();

                    string date = day + "/" + month + "/" + year;
                    svc = service.Name;
                    ShopName = shop.Name;
                    amount = service.Price;
                    customerPhone = user.PhoneNumber;
                    //GET


                    //POPULATE BOOKING VIEW MODEL
                    BookingViewModel model = new BookingViewModel()
                    {
                        Id = booking.Id,
                        Address = booking.Address,
                        Amount = amount,
                        CustomerPhone = customerPhone,
                        Date = date,
                        Service = svc,
                        Shop = ShopName,
                        Time = booking.AppointmentTime
                    };
                    //TempData["BkVM"] = model;
                    return View("ViewBooking", model);
                    // return View(model);
                }
                else
                {
                    ViewBag.Msg = "BOOKING NOT FOUND!";
                    return View();
                }
            }
            TempData["Msg"] = "BOOKING ID IS INVALID!";
            return RedirectToAction("Index", "Home");
        }

        //[HttpGet]
        //public ActionResult ViewBooking()
        //{
        //    return View();
        //}

        [HttpGet]
        public ActionResult RescheduleBooking(string bookingId)
        {
            if (bookingId != null && bookingId != "")
            {
                //GET BOOKING RECORD FROM DB
                var dbBooking = bookingContext.Find(bookingId);
                if (dbBooking != null)
                {
                    var service = serviceContext.Find(dbBooking.ServiceID);
                    var user = context.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
                    var shop = shopcontext.Find(service.ShopID);
                    ExtraProperties extraPro = new ExtraProperties();
                    ViewBag.OpeningTime = shop.OpeningHour;
                    ViewBag.ClosingTime = shop.ClosingHour;

                    Booking model = new Booking()
                    {
                        Id = dbBooking.Id,
                        ShopName = shop.Name,
                        ServiceID = dbBooking.ServiceID,
                        Address = dbBooking.Address,
                        UserID = dbBooking.UserID,
                        AppointmentDate = dbBooking.AppointmentDate,
                        AppointmentTime = dbBooking.AppointmentTime,
                        CreatedAt = dbBooking.CreatedAt

                    };

                    ViewBag.TimeList = extraPro.TimeList();
                    return View(model);
                }
                else
                {
                    TempData["Msg"] = "BOOKING NOT FOUND, MAY HAVE BEEN DELETED/CANCELED!";
                    return RedirectToAction("Index", "Home");
                }
            }
            ViewBag.Msg = "INVALID BOOKING ID!";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RescheduleBooking(Booking booking)
        {
            ExtraProperties extraPro = new ExtraProperties();
            var service = serviceContext.Find(booking.ServiceID);

            var shop = shopcontext.Collection().Where(x => x.Id == service.ShopID).FirstOrDefault();
            if (ModelState.IsValid)
            {
                var update = bookingContext.Update(booking);
                if (update != null)
                {
                    //SEND MAIL TO CUSTOMER AND SHOP OWNER
                    var user = context.Users.Where(x => x.Email == User.Identity.Name).SingleOrDefault();

                    //PROCESS EMAIL
                    var callbackUrl = Url.Action("ViewBooking", "ServiceBooking", new { @BookingId = update.Id }, protocol: Request.Url.Scheme);

                    //SEND EMAIL TO CUSTOMER

                    IdentityMessage custidentyMessage = new IdentityMessage()
                    {
                        Destination = user.Email,
                        Subject = "Service Booking Update"
                    };

                    string html = "You've successfully rescheduled for " + service.Name + " and your booking ID is " + booking.Id +
                        ". Your Booking ID can be use to re-schedule your booking date." +
                         "To view your schedule, kindly click on here <a href=\"" + callbackUrl + "\">here</a>";

                    MailHelper sendMail = new MailHelper();
                    ConfirmEmailSend sendMsg = sendMail.SendMail(custidentyMessage, html);

                    //SEND EMAIL TO SHOP OWNER

                    var shopOwnerEmail = context.Users.Where(x => x.Id == shop.UserID).FirstOrDefault();
                    if (shopOwnerEmail != null)
                    {
                        IdentityMessage shopOwnerIdentityMessage = new IdentityMessage()
                        {
                            Destination = shopOwnerEmail.Email,
                            Subject = "Service Booking"
                        };

                        string html2 = user.Email + " has successfully rescheduled for " + service.Name +
                            ", to view details, kindly click on here <a href=\"" + callbackUrl + "\">here</a>";

                        MailHelper sendMail2 = new MailHelper();
                        ConfirmEmailSend sendMsg2 = sendMail.SendMail(shopOwnerIdentityMessage, html2);

                        if (sendMsg.Result == true || sendMsg2.Result == true)
                        {
                            TempData["Msg"] = "Appointment rescheduled successfully, " +
                                                "Please kindly check your email for confirmation!\nCustomer Mail Response:" + sendMsg.Message;
                        }
                        else
                        {
                            TempData["Msg"] = "Appointment booked successfully\nCustomer Mail Response:" + sendMsg.Message + "\n";

                        }
                    }
                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    ViewBag.TimeList = extraPro.TimeList();
                    return View(booking);
                }
            }
            else
            {

                ViewBag.TimeList = extraPro.TimeList();
                return View(booking);

            }
        }
    }
}