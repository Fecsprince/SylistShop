using MyShop.Core.Contracts;
using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebUI.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class StoreTypeController : Controller
    {
        IRepository<StoreType> context;


        public StoreTypeController(IRepository<StoreType> context)
        {
            this.context = context;
        }

        // GET: ProductManager
        public ActionResult Index()
        {
            List<StoreType> storeTypes = context.Collection().ToList();

            return View(storeTypes);
        }

        public ActionResult Create()
        {
            StoreType storeType = new StoreType();
            return View(storeType);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StoreType storeType)
        {
            if (!ModelState.IsValid)
            {
                return View(storeType);
            }
            else
            {
                context.Insert(storeType);

                return RedirectToAction("Index");
            }
        }

        public ActionResult Edit(string Id)
        {
            var storeType = context.Find(Id);

            if (storeType == null)
            {
                return HttpNotFound();
            }
            else
            {

                return View(storeType);
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StoreType store, string Id)
        {
            var storeTypeToEdit = context.Find(Id);

            if (storeTypeToEdit != null)
            {

                if (!ModelState.IsValid)
                {
                    return View(store);

                }

                else
                {
                    storeTypeToEdit.Name = store.Name;

                    context.Update(storeTypeToEdit);

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
            StoreType storetypeToDelete = context.Find(Id);

            if (storetypeToDelete == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(storetypeToDelete);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public ActionResult ConfirmDelete(string Id)
        {
            StoreType storeTypeToDelete = context.Find(Id);

            if (storeTypeToDelete == null)
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