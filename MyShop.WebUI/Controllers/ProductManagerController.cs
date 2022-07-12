using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using MyShop.DataAccess.SQL;
using System.IO;
using MyShop.WebUI.Models;

namespace MyShop.WebUI.Controllers
{
    [Authorize(Roles = "SuperAdmin,StoreManager")]
    public class ProductManagerController : Controller
    {

        IRepository<Product> context;
        IRepository<ProductCategory> Catecontext;
        IRepository<Shop> shopcontext;
        private ApplicationDbContext userContext;

        public ProductManagerController(IRepository<Product> Prodcontext, IRepository<Shop> Shopcontext, IRepository<ProductCategory> ProdCatecontext)
        {
            context = Prodcontext;
            shopcontext = Shopcontext;
            Catecontext = ProdCatecontext;
            userContext = new ApplicationDbContext();
        }


        // GET: ProductManager
        public ActionResult Index()
        {
            if (TempData["Msg"] != null)
            {
                ViewBag.Msg = TempData["Msg"].ToString();
            }


            IEnumerable<Product> prods = null;
            ApplicationUser user = new ApplicationUser();


            if (TempData["Msg"] != null)
            {
                ViewBag.Msg = TempData["Msg"].ToString();
            }
            if (User.IsInRole("SuperAdmin"))
            {
                prods = context.Collection();

            }
            else
            {
                user = userContext.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
                var shopOwner = shopcontext.Collection().Where(x => x.UserID == user.Id).FirstOrDefault();
                if (shopOwner != null)
                {
                    if (shopOwner.StoreType.Name.Contains("Store"))
                    {
                        prods = context.Collection().Where(x => x.ShopID == shopOwner.Id).ToList();

                        foreach (var prod in prods)
                        {
                            prod.UserID = user.Email;
                        }
                    }
                    else
                    {
                        ViewBag.Msg = "Access Denied, Only Store Manager can access!";
                    }
                }

            }
            return View(prods);

        }


        [HttpGet]
        public ActionResult Create()
        {
            //LOAD ALL CATEGORIES
            var categories = Catecontext.Collection();
            ViewBag.Categories = categories;

            //GET USER
            var user = userContext.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            Product prod = new Product()
            {
                UserID = user.Id,
                Image1 = "dd"
            };

            return View(prod);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product prod, HttpPostedFileBase file1, HttpPostedFileBase file2)
        {

            string msg = "";
            //LOAD ALL CATEGORIES
            var categories = Catecontext.Collection();
            prod.Image1 = "";
            prod.Image2 = "";

            //GET SHOPOWNER_USER
            var user = userContext.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();

            //GET SHOP
            var shopx = shopcontext.Collection().Where(x => x.UserID == user.Id).FirstOrDefault();
            if (shopx != null)
            {
                try
                {

                    // CHECK IF IMAGE1 UPLOAD FILE IS EMPTY
                    if (file1 == null || file1.ContentLength < 0 ||
                        file2 == null || file2.ContentLength < 0)
                    {
                        msg = "Please select a file, Try again!";
                        ViewBag.Msg = msg;
                        ViewBag.Category = categories;

                        return View(prod);
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

                        prod.Image1 = prod.Id + postFix.ToString() + Path.GetExtension(file1.FileName);
                        prod.ShopID = shopx.Id;

                        string pathx = Server.MapPath("~/Content/ProductImages/" + prod.Image1);
                        if (System.IO.File.Exists(pathx))
                            System.IO.File.Delete(pathx);
                        file1.SaveAs(pathx);

                        //CHANGE IMAGE 2 FILE NAME
                        if (file2 != null)
                        {

                            postFix++;

                            prod.Image2 = prod.Id + postFix.ToString() + Path.GetExtension(file2.FileName);

                            string pathy = Server.MapPath("~/Content/ProductImages/" + prod.Image2);
                            if (System.IO.File.Exists(pathy))
                                System.IO.File.Delete(pathy);
                            file2.SaveAs(pathy);

                        }

                        //GET USER
                        //var user = userContext.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
                        //if (user != null)
                        //{

                        //prod.UserID = user.Id;


                        if (ModelState.IsValid)
                        {
                            var addRec = context.Insert(prod);
                            if (addRec != null)
                            {
                                TempData["Msg"] = prod.Name + " added to the database!";
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                TempData["Msg"] = prod.Name + " was not added to the database!";
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
                        ViewBag.Categories = categories;
                        return View();
                    }
                    else
                    {
                        TempData["Msg"] = "Please copy response and send to admin: \n" + ex.Message.ToString();
                        ViewBag.Msg = TempData["Msg"].ToString();
                        ViewBag.Categories = categories;
                        return View();
                    }
                }
            }
            else
            {
                TempData["Msg"] = "YOU MUST BE A STORE OWNER BEFORE YOU ADD NEW PRODUCT ON SYLIST";
            }



            ViewBag.Categories = categories;

            ViewBag.Msg = TempData["Msg"].ToString();
            return View();
        }


        [HttpGet]
        public ActionResult Edit(string id)
        {
            var model = context.Find(id);

            if (model != null)
            {
                var product = new Product()
                {
                    Id = model.Id,
                    Name = model.Name,
                    Description = model.Description,
                    CategoryID = model.CategoryID,
                    Price = model.Price,
                    _Image1 = model.Image1,
                    _Image2 = model.Image2,
                    UserID = model.UserID
                };

                //LOAD ALL CATEGORIES
                var categories = Catecontext.Collection();

                ViewBag.Categories = categories;

                return View(product);
            }
            else
            {
                TempData["Msg"] = "RECORD NOT FOUND WITH SUPPLIED ID!";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product model, HttpPostedFileBase file1, HttpPostedFileBase file2)
        {
            string msg = "";


            //LOAD ALL CATEGORIES
            var categories = Catecontext.Collection();

            // CHECK IF IMAGE1 UPLOAD FILE IS EMPTY
            if (file1 == null || file1.ContentLength < 0)
            {
                msg = "Please select a file, Try again!";
                ViewBag.Msg = msg;
                ViewBag.Categories = categories;

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


                string pathx = Server.MapPath("~/Content/ProductImages/" + model.Image1);
                if (System.IO.File.Exists(pathx))
                    System.IO.File.Delete(pathx);
                file1.SaveAs(pathx);

                //CHANGE IMAGE 2 FILE NAME
                if (file2 != null)
                {
                    postFix++;
                    model.Image2 = model.Id + postFix.ToString() + Path.GetExtension(file2.FileName);

                    string pathy = Server.MapPath("~/Content/ProductImages/" + model.Image2);
                    if (System.IO.File.Exists(pathy))
                        System.IO.File.Delete(pathy);
                    file2.SaveAs(pathy);

                }

                #region UPDATE PRODUCT


                var objProd = context.Find(model.Id);
                if (objProd != null)
                {
                    //UPDATE THE MODEL
                    objProd.Name = model.Name;
                    objProd.Description = model.Description;
                    objProd.Price = model.Price;
                    objProd.CategoryID = model.CategoryID;
                    objProd.Image1 = model.Image1;
                    objProd.Image2 = model.Image2;



                    if (ModelState.IsValid)
                    {
                        var addRec = context.Update(objProd);
                        if (addRec != null)
                        {
                            TempData["Msg"] = objProd.Name + " has been updated successfully!";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["Msg"] = objProd.Name + " record update failed!";
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

            ViewBag.Categories = categories;

            return View(model);
        }

        [HttpGet]
        public ActionResult Details(string id)
        {
            if (id != null)
            {
                try
                {
                    var prod = context.Find(id);
                    if (prod != null)
                    {

                        //GET USER
                        string userName = "";
                        var user = userContext.Users.Where(x => x.Id == prod.UserID).FirstOrDefault();
                        if (user != null)
                        {
                            userName = user.Email;
                        }
                        string cateName = Catecontext.Find(prod.CategoryID).Name;

                        Product prodVm = new Product()
                        {
                            Id = prod.Id,
                            Name = prod.Name,
                            Description = prod.Description,
                            CategoryID = prod.CategoryID,
                            CategoryName = cateName,
                            _Image1 = prod.Image1,
                            _Image2 = prod.Image2,
                            Price = prod.Price,
                            UserID = userName
                        };


                        return View(prodVm);
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
            Product productToDelete = context.Find(Id);

            if (productToDelete == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(productToDelete);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public ActionResult ConfrimDelete(string Id)
        {

            Product productToDelete = context.Find(Id);

            if (productToDelete == null)
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