using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.WebUI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebUI.Controllers
{
    [Authorize(Roles = "SuperAdmin,ShopManager")]
    public class ServiceManagerController : Controller
    {
        IRepository<Service> context;
        IRepository<Shop> shopContext;
        private ApplicationDbContext userContext;

        public ServiceManagerController(IRepository<Service> Servcontext, IRepository<Shop> Shopcontext)
        {
            context = Servcontext;
            shopContext = Shopcontext;
            userContext = new ApplicationDbContext();
        }


        // GET: ProductManager
        public ActionResult Index(string shopId)
        {
            if (TempData["Msg"] != null)
            {
                ViewBag.Msg = TempData["Msg"].ToString();
            }
            List<Service> services = null;

            if (User.IsInRole("SuperAdmin"))
            {
                services = context.Collection().ToList();
                return View(services);
            }
            else
            {
                var user = userContext.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
                var shop = shopContext.Collection().Where(x => x.UserID == user.Id).FirstOrDefault();
                if (shop != null)
                {
                    services = context.Collection().Where(x => x.ShopID == shop.Id).ToList();
                    return View(services);
                }
                else
                {
                    TempData["Msg"] = "You don't have store account!";
                    return RedirectToAction("Index", "Home");
                }
            }

        }


        [HttpGet]
        public ActionResult Create()
        {

            //GET USER
            var user = userContext.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();

            var shop = shopContext.Collection().Where(x => x.UserID == user.Id).FirstOrDefault();
            if (shop != null)
            {
                Service service = new Service()
                {
                    ShopID = shop.Id,
                };
                return View(service);
            }
            else
            {
                TempData["Msg"] = "Only shop owners can add service!";
                return RedirectToAction("Index");
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Service service, HttpPostedFileBase file1, HttpPostedFileBase file2)
        {

            string msg = "";
            //LOAD ALL CATEGORIES
            service.Image1 = "";
            service.Image2 = "";

            try
            {

                // CHECK IF IMAGE1 UPLOAD FILE IS EMPTY
                if (file1 == null || file1.ContentLength < 0 ||
                    file2 == null || file2.ContentLength < 0)
                {
                    ViewBag.Msg = "Please select a file, Try again!";

                    return View(service);
                }
                else if (file1 != null && file1.ContentLength > 0 &&
                    file1.FileName.ToLower().EndsWith("jpg") ||
                    file1.FileName.ToLower().EndsWith("png") ||
                    file2.ContentLength > 0 &&
                    file2.FileName.ToLower().EndsWith("jpg") ||
                    file2.FileName.ToLower().EndsWith("png"))
                {

                    //UPLOAD NOT EMPTY


                    int postFix = 1;

                    service.Image1 = service.Id + postFix.ToString() + Path.GetExtension(file1.FileName);


                    string pathx = Server.MapPath("~/Content/ServiceImages/" + service.Image1);
                    if (System.IO.File.Exists(pathx))
                        System.IO.File.Delete(pathx);
                    file1.SaveAs(pathx);

                    //CHANGE IMAGE 2 FILE NAME
                    if (file2 != null)
                    {

                        postFix++;

                        service.Image2 = service.Id + postFix.ToString() + Path.GetExtension(file2.FileName);

                        string pathy = Server.MapPath("~/Content/ServiceImages/" + service.Image2);
                        if (System.IO.File.Exists(pathy))
                            System.IO.File.Delete(pathy);
                        file2.SaveAs(pathy);

                    }

                    if (ModelState.IsValid)
                    {
                        var addRec = context.Insert(service);
                        if (addRec != null)
                        {
                            TempData["Msg"] = service.Name + " added to the database!";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["Msg"] = service.Name + " was not added to the database!";
                        }
                    }
                    else
                    {
                        TempData["Msg"] = "Update failed: Error 100 --> Invalid entry!";
                    }
                }
                else
                {
                    TempData["Msg"] = "Image 1 was not valid!";
                }
            }
            catch (Exception ex)
            {

                if (ex.InnerException.Message != null)
                {
                    TempData["Msg"] = "Please copy response and send to admin: \n" +
                                            ex.Message.ToString() + "\n" +
                                            ex.InnerException.Message.ToString();
                    ViewBag.Msg = TempData["Msg"].ToString();
                    return View();
                }
                else
                {
                    TempData["Msg"] = "Please copy response and send to admin: \n" + ex.Message.ToString();
                    ViewBag.Msg = TempData["Msg"].ToString();
                    return View();
                }
            }


            ViewBag.Msg = TempData["Msg"].ToString();
            return View();
        }


        [HttpGet]
        public ActionResult Edit(string id)
        {
            var model = context.Find(id);

            if (model != null)
            {
                var service = new Service()
                {
                    Id = model.Id,
                    Name = model.Name,
                    ShopID = model.ShopID,
                    Price = model.Price,
                    _Image1 = model.Image1,
                    _Image2 = model.Image2
                };

                return View(service);
            }
            else
            {
                TempData["Msg"] = "RECORD NOT FOUND WITH SUPPLIED ID!";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Service service, HttpPostedFileBase file1, HttpPostedFileBase file2)
        {
            string msg = "";



            // CHECK IF IMAGE1 UPLOAD FILE IS EMPTY
            if (file1 == null || file1.ContentLength < 0)
            {
                msg = "Please select a file, Try again!";
                ViewBag.Msg = msg;

                return View(service);
            }
            else if (file1 != null && file1.ContentLength > 0 &&
                file1.FileName.ToLower().EndsWith("jpg") ||
                file1.FileName.ToLower().EndsWith("png") ||
                file2.ContentLength > 0 &&
                file2.FileName.ToLower().EndsWith("jpg") ||
                file2.FileName.ToLower().EndsWith("png"))
            {

                //UPLOAD NOT EMPTY


                int postFix = 1;

                service.Image1 = service.Id + postFix.ToString() + Path.GetExtension(file1.FileName);


                string pathx = Server.MapPath("~/Content/ServiceImages/" + service.Image1);
                if (System.IO.File.Exists(pathx))
                    System.IO.File.Delete(pathx);
                file1.SaveAs(pathx);

                //CHANGE IMAGE 2 FILE NAME
                if (file2 != null)
                {
                    postFix++;
                    service.Image2 = service.Id + postFix.ToString() + Path.GetExtension(file2.FileName);

                    string pathy = Server.MapPath("~/Content/ServiceImages/" + service.Image2);
                    if (System.IO.File.Exists(pathy))
                        System.IO.File.Delete(pathy);
                    file2.SaveAs(pathy);

                }

                #region UPDATE PRODUCT


                var objService = context.Find(service.Id);
                if (objService != null)
                {
                    //UPDATE THE MODEL
                    objService.Name = service.Name;
                    objService.ShopID = service.ShopID;
                    objService.Price = service.Price;
                    objService.Image1 = service.Image1;
                    objService.Image2 = service.Image2;



                    if (ModelState.IsValid)
                    {
                        var addRec = context.Update(objService);
                        if (addRec != null)
                        {
                            TempData["Msg"] = objService.Name + " has been updated successfully!";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["Msg"] = objService.Name + " record update failed!";
                        }
                    }
                    else
                    {
                        TempData["Msg"] = "Update failed: Error 100 --> Invalid entry!";
                    }
                }
                else
                {
                    msg = "Your request could not be processed!";
                }


                #endregion
            }
            else
            {
                TempData["Msg"] = "Image 1 was not valid!";
            }


            ViewBag.Msg = TempData["Msg"].ToString();


            return View(service);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Details(string id)
        {
            if (id != null)
            {
                try
                {
                    var service = context.Find(id);
                    if (service != null)
                    {

                        //GET USER
                        string shopName = "";
                        //var user = userContext.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
                        var shop = shopContext.Collection().Where(x => x.UserID == service.ShopID).FirstOrDefault();

                        if (shop != null)
                        {
                            shopName = shop.Name;
                        }

                        Service serviceVm = new Service()
                        {
                            Id = service.Id,
                            Name = service.Name,
                            ShopID = shopName,
                            _Image1 = service.Image1,
                            _Image2 = service.Image2,
                            Price = service.Price,
                        };


                        return View(serviceVm);
                    }
                    else
                    {
                        ViewBag.Msg = "NO RECORD FOUND!";
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message != null)
                    {
                        TempData["Msg"] = ex.InnerException.Message.ToString();
                    }
                    else
                    {
                        TempData["Msg"] = ex.Message.ToString();
                    }

                    ViewBag.Msg = TempData["Msg"].ToString();
                    return View();
                }
            }
            else
            {
                ViewBag.Msg = "Invalid request, please check the shop identification!";
                return View();
            }

        }

        public ActionResult Delete(string Id)
        {
            Service serviceToDelete = context.Find(Id);

            if (serviceToDelete == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(serviceToDelete);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public ActionResult ConfrimDelete(string Id)
        {

            Service serviceToDelete = context.Find(Id);

            if (serviceToDelete == null)
            {
                return HttpNotFound();
            }
            else
            {
                context.Delete(Id);
                return RedirectToAction("Index");
            }
        }
    }
}