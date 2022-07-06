using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyShop.Core.Models;
using MyShop.Core.Contracts;

namespace MyShop.WebUI.Controllers
{
    public class ProductCategoryManagerController : Controller
    {
        IRepository<ProductCategory> context;

        //public ProductCategoryManagerController()
        //{
        //    context = new InMemoryRepository<ProductCategory>();
        //}

        public ProductCategoryManagerController(IRepository<ProductCategory> context)
        {
            this.context = context;
        }

        // GET: ProductManager
        public ActionResult Index()
        {
            List<ProductCategory> productCates = context.Collection().ToList();

            return View(productCates);
        }

        public ActionResult Create()
        {
            ProductCategory prodCate = new ProductCategory();
            return View(prodCate);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductCategory prodCate)
        {
            if (!ModelState.IsValid)
            {
                return View(prodCate);
            }
            else
            {
                context.Insert(prodCate);
                context.Commit();

                return RedirectToAction("Index");
            }
        }

        public ActionResult Edit(string Id)
        {
            var prodCate = context.Find(Id);

            if (prodCate == null)
            {
                return HttpNotFound();
            }
            else
            {

                return View(prodCate);
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductCategory pc, string Id)
        {
            var prodCateToEdit = context.Find(Id);

            if (prodCateToEdit != null)
            {

                if (!ModelState.IsValid)
                {
                    return View(pc);

                }

                else
                {
                    prodCateToEdit.Name = pc.Name;

                    context.Commit();

                    return RedirectToAction("Index");
                }
            }

            else
            {
                return HttpNotFound();
            }
        }




        public ActionResult Delete(string Id)
        {
            ProductCategory productCategoryToDelete = context.Find(Id);

            if (productCategoryToDelete == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(productCategoryToDelete);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public ActionResult ConfirmDelete(string Id)
        {
            ProductCategory productCategoryToDelete = context.Find(Id);

            if (productCategoryToDelete == null)
            {
                return HttpNotFound();
            }
            else
            {
                context.Delete(Id);
                context.Commit();
                return RedirectToAction("Index");
            }
        }
        
    }
}