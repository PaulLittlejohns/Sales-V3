using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using SalesV3.Models;

namespace SalesV3.Controllers
{
    interface IStore
    {
        ActionResult Index();
        ActionResult GetStores();
        ActionResult Edit(int Id);
        ActionResult Edit(Store model);
        ActionResult Delete(int Id);
    }

    public class StoreController : Controller, IStore
    {
        ShoppingEntities db = new ShoppingEntities();
        // GET: Store
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetStores()
        {
            var storeslist = db.Stores.Select(x => new Models.StoreViewModel()
            {
                Id = x.Id,
                StoreName = x.StoreName,
                StoreAddress = x.StoreAddress
            }).ToList();
            string json = JsonConvert.SerializeObject(storeslist, Formatting.Indented);
            var result = new {Stores = json };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int Id)
        {
            string name;
            string address;
            if (Id == 0)
            { // new record
                name = "";
                address = "";
            } else
            {
                var store = db.Stores.Find(Id);               
                if (store == null)
                {
                    return HttpNotFound();
                }  else
                {
                    name = store.StoreName;
                    address = store.StoreAddress;
                }
            }
            var result = new {Id, Name = name, Address = address };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit(Store model)
        {
            string msg;
            if (model.Id == 0)
            { // new record
                 try
                {
                    var newstore = new Store()
                    {
                        StoreName = model.StoreName,
                        StoreAddress = model.StoreAddress
                    };
                    db.Stores.Add(newstore);
                    db.SaveChanges();
                    msg = "New record created.";
                } catch (Exception)
                {
                    msg = "Unable to create new record. Contact the administrator.";
                }
            }
            else
            {
                var store = db.Stores.Find(model.Id);
                if (store == null)
                {
                    return HttpNotFound();
                }
                try
                {
                    store.StoreName = model.StoreName;
                    store.StoreAddress = model.StoreAddress;
                    db.SaveChanges();
                    msg = string.Format("Changes to record {0} have been saved.", model.Id);
                }
                catch (Exception)
                {
                    msg = string.Format("Record {0} cannot be updated. Contact the administor,", model.Id);
                }
            }  
            var result = new { Message = msg };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int Id)
        {
            var store = db.Stores.Find(Id);
            string msg;
            if (store == null)
            {
                return HttpNotFound();
            }
            if (store.Sales.Any())
            {
                msg = string.Format("Record {0} cannot be deleted. Check it is not used by a Sales record.", Id);
            }
            else
            {
                try {
                    db.Stores.Remove(store);
                    db.SaveChanges();
                    msg = string.Format("Record {0} has been deleted.", Id);
                }
                catch (Exception)
                {
                    msg = string.Format("Record {0} cannot be deleted. Contact the administrator.", Id);
                }
            }
            var result = new { Message = msg };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}