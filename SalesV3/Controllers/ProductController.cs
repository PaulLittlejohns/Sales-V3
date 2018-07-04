using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using SalesV3.Models;

namespace SalesV3.Controllers
{
    interface IProduct
    {
        ActionResult Index();
        ActionResult GetProducts();
        ActionResult Edit(int Id);
        ActionResult Edit(ProductViewModel model);
        ActionResult Delete(int Id);
    }

    public class ProductController : Controller, IProduct
    {
        ShoppingEntities db = new ShoppingEntities();
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetProducts()
        {
            var productlist = db.Products.AsEnumerable().Select(x => new Models.ProductViewModel
            {
                Id = x.Id,
                ProductName = x.ProductName,
                ProductCost = String.Format("{0:0.00}", x.ProductPrice)
            }).ToList();
            string json = JsonConvert.SerializeObject(productlist, Formatting.Indented);
            var result = new { Success = "True", Products = json };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int Id)
        {
            string name;
            decimal price = 0; 

            if (Id == 0)
            {
                name = "";
                price = 0;
            } else {
                var product = db.Products.Find(Id);
                if (product == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    name = product.ProductName;
                    price = product.ProductPrice??0;
                }
            }            
            var result = new ProductViewModel { Id = Id, ProductName = name, ProductPrice = price };               
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit(ProductViewModel model)
        {
            string msg;
            if (model.Id == 0) // new record
            {
                try
                {
                    var newproduct = new Product()
                    {
                        ProductName = model.ProductName,
                        ProductPrice = model.ProductPrice
                    };
                    db.Products.Add(newproduct);
                    db.SaveChanges();
                    msg = "New record created";
                }
                catch (Exception)
                {
                    msg = "Unable to created new record. Contact the administrator.";
                }
            } else
            {
                var product = db.Products.Find(model.Id);
                if (product == null)
                {
                    return HttpNotFound();
                }
                try
                {
                    product.ProductName = model.ProductName;
                    product.ProductPrice = model.ProductPrice;
                    db.SaveChanges();
                    msg = String.Format("Changes to {0} have been saved.", model.Id);
                }
                catch (Exception)
                {
                    msg = String.Format("Unable save changes for {0}. Contact the administrator", model.Id);
                }
            } 
            var result = new {Message = msg };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete (int Id)
        {
            string msg;
            var product = db.Products.Find(Id);
            if (product == null)
            {
                return HttpNotFound();
            }

            if (product.Sales.Any())
            {
                msg = string.Format("Record {0} cannot be deleted. Check product is not used by a Sales record.", Id);
            } else
            {
                try
                {
                    db.Products.Remove(product);
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