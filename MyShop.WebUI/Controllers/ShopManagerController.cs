using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.Supports;
using MyShop.WebUI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebUI.Controllers
{
    public class ShopManagerController : Controller
    {
        IRepository<Shop> repository;
        private ApplicationDbContext userContext;
        public ShopManagerController(IRepository<Shop> Repos)
        {
            repository = Repos;
            userContext = new ApplicationDbContext();
        }

        ExtraProperties xy = new ExtraProperties();

        // GET: ProductManager
        public ActionResult Index()
        {
            if (TempData["Msg"] != null)
            {
                ViewBag.Msg = TempData["Msg"].ToString();
            }
            var user = userContext.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();

            List<Shop> shops = repository.Collection().ToList();
            foreach (var shop in shops)
            {
                shop.UserID = user.Email;
            }
            return View(shops);
        }


        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            //GET USER
            var user = userContext.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            Shop shop = new Shop()
            {
                UserID = user.Id,
                Image1 = "dd"
            };

            ViewBag.WorkingDays = xy.GetWorkingDays();
            return View(shop);
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Shop shop, HttpPostedFileBase file1, HttpPostedFileBase file2)
        {

            string msg = "";
            string pathy = "";

            shop.Image1 = "";
            shop.Image2 = "";

            try
            {

                // CHECK IF IMAGE1 UPLOAD FILE IS EMPTY
                if (file1 == null || file1.ContentLength < 0 ||
                    file2 == null || file2.ContentLength < 0)
                {
                    msg = "Please select a file, Try again!";
                    ViewBag.Msg = msg;
                    ViewBag.WorkingDays = xy.GetWorkingDays();

                    return View(shop);
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

                    shop.Image1 = shop.Id + postFix.ToString() + Path.GetExtension(file1.FileName);


                    string pathx = Server.MapPath("~/Content/ShopImages/" + shop.Image1);
                    if (System.IO.File.Exists(pathx))
                        System.IO.File.Delete(pathx);

                    //CHANGE IMAGE 2 FILE NAME
                    if (file2 != null)
                    {

                        postFix++;

                        shop.Image2 = shop.Id + postFix.ToString() + Path.GetExtension(file2.FileName);

                        pathy = Server.MapPath("~/Content/ShopImages/" + shop.Image2);
                        if (System.IO.File.Exists(pathy))
                            System.IO.File.Delete(pathy);
                    

                    }

                    if (ModelState.IsValid)
                    {
                        var addRec = repository.Insert(shop);
                        if (addRec != null)
                        {
                            file1.SaveAs(pathx);
                            file2.SaveAs(pathy);

                            TempData["Msg"] = shop.Name + " added to the database!";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["Msg"] = shop.Name + " was not added to the database!";
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
                    ViewBag.WorkingDays = xy.GetWorkingDays();

                    return View();
                }
                else
                {
                    TempData["Msg"] = "Please copy response and send to admin: \n" + ex.Message.ToString();
                    ViewBag.Msg = TempData["Msg"].ToString();
                    ViewBag.WorkingDays = xy.GetWorkingDays();

                    return View();
                }
            }

            ViewBag.Msg = TempData["Msg"].ToString();
            ViewBag.WorkingDays = xy.GetWorkingDays();

            return View();
        }


        [HttpGet]
        public ActionResult Edit(string id)
        {
            var model = repository.Find(id);

            if (model != null)
            {
                var shop = new Shop()
                {
                    Id = model.Id,
                    Name = model.Name,
                    Contact1 = model.Contact1,
                    Contact2 = model.Contact2,
                    Days = model.Days,
                    Location = model.Location,
                    OpeningHour = model.OpeningHour,
                    ClosingHour = model.ClosingHour,
                    PostCode = model.PostCode,
                    State = model.State,
                    LGA = model.LGA,
                    _Image1 = model.Image1,
                    _Image2 = model.Image2,
                    UserID = model.UserID
                };
                ViewBag.WorkingDays = xy.GetWorkingDays();

                return View(shop);
            }
            else
            {
                TempData["Msg"] = "RECORD NOT FOUND WITH SUPPLIED ID!";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Shop model, HttpPostedFileBase file1, HttpPostedFileBase file2)
        {
            string msg = "";
            string pathy = "";

            // CHECK IF IMAGE1 UPLOAD FILE IS EMPTY
            if (file1 == null || file1.ContentLength < 0)
            {
                msg = "Please select a file, Try again!";
                ViewBag.Msg = msg;
                ViewBag.WorkingDays = xy.GetWorkingDays();

                return View(model);
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

                model.Image1 = model.Id + postFix.ToString() + Path.GetExtension(file1.FileName);


                string pathx = Server.MapPath("~/Content/ShopImages/" + model.Image1);
                if (System.IO.File.Exists(pathx))
                    System.IO.File.Delete(pathx);


                //CHANGE IMAGE 2 FILE NAME
                if (file2 != null)
                {
                    postFix++;
                    model.Image2 = model.Id + postFix.ToString() + Path.GetExtension(file2.FileName);

                    pathy = Server.MapPath("~/Content/ShopImages/" + model.Image2);
                    if (System.IO.File.Exists(pathy))
                        System.IO.File.Delete(pathy);


                }

                #region UPDATE PRODUCT


                var objShop = repository.Find(model.Id);
                if (objShop != null)
                {
                    //UPDATE THE MODEL
                    objShop.Name = model.Name;
                    objShop.Contact1 = model.Contact1;
                    objShop.Contact2 = model.Contact2;
                    objShop.Days = model.Days;
                    objShop.Location = model.Location;
                    objShop.OpeningHour = model.OpeningHour;
                    objShop.ClosingHour = model.ClosingHour;
                    objShop.PostCode = model.PostCode;
                    objShop.State = model.State;
                    objShop.LGA = model.LGA;
                    objShop.Image1 = model.Image1;
                    objShop.Image2 = model.Image2;
                    objShop.UserID = model.UserID;



                    if (ModelState.IsValid)
                    {
                        var addRec = repository.Update(objShop);
                        if (addRec != null)
                        {
                            //SAVE IMAGES
                            file1.SaveAs(pathx);
                            file2.SaveAs(pathy);

                            TempData["Msg"] = objShop.Name + " has been updated successfully!";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["Msg"] = objShop.Name + " record update failed!";
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
            ViewBag.WorkingDays = xy.GetWorkingDays();

            return View(model);
        }

        [HttpGet]
        public ActionResult Details(string id)
        {
            if (id != null)
            {
                try
                {
                    var shop = repository.Find(id);
                    if (shop != null)
                    {

                        //GET USER
                        string userName = "";
                        var user = userContext.Users.Where(x => x.Id == shop.UserID).FirstOrDefault();
                        if (user != null)
                        {
                            userName = user.Email;
                        }

                        Shop shopVm = new Shop()
                        {
                            Id = shop.Id,
                            Name = shop.Name,
                            Contact1 = shop.Contact1,
                            Contact2 = shop.Contact2,
                            Days = shop.Days,
                            Location = shop.Location,
                            OpeningHour = shop.OpeningHour,
                            ClosingHour = shop.ClosingHour,
                            PostCode = shop.PostCode,
                            State = shop.State,
                            LGA = shop.LGA,
                            _Image1 = shop.Image1,
                            _Image2 = shop.Image2,
                            UserID = userName
                        };


                        return View(shopVm);
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
            Shop shopToDelete = repository.Find(Id);

            if (shopToDelete == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(shopToDelete);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public ActionResult ConfrimDelete(string Id)
        {

            Shop shopToDelete = repository.Find(Id);

            if (shopToDelete == null)
            {
                return HttpNotFound();
            }
            else
            {
                repository.Delete(Id);
                return RedirectToAction("Index");
            }
        }

    }
}