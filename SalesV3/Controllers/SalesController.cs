using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using SalesV3.Models;

namespace SalesV3.Controllers
{
    interface ISales
    {
        ActionResult Index();
        ActionResult GetSales();
        ActionResult Edit(int Id);
        ActionResult Edit(Sale model);
        ActionResult Delete(int Id);
    }

    public class SalesController : Controller, ISales
    {
        ShoppingEntities db = new ShoppingEntities();
        // GET: Sales
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetSales()
        {
            var saleslist = db.TotalSales.Select(x => new Models.SalesViewModel()
            {
                Id = x.Id,
                ProductName = x.ProductName,
                CustomerName = x.CustomerName,
                StoreName = x.StoreName,
                SaleDate = x.SaleDate
            });
            return Json(saleslist, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int Id)
        {
            int selectedproduct = 0 , selectedcustomer = 0, selectedstore = 0;
            var products = db.Products.ToList();
            var customers = db.Customers.ToList();
            var stores = db.Stores.ToList();
            DateTime date = System.DateTime.Now;
            if (Id == 0)
            { // new record
                string firstselect = "---Please Select---";
                products.Add(new Product { Id = 0, ProductName = firstselect });
                customers.Add(new Customer { Id = 0, CustomerName = firstselect });
                stores.Add(new Store { Id = 0, StoreName = firstselect});
            } else
            {
                var sale = db.Sales.Find(Id);
                if (sale == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    selectedproduct = sale.ProductId;
                    selectedcustomer = sale.CustomerId;
                    selectedstore = sale.StoreId;
                    date = sale.SaleDate;
                }
            }
            SelectList productselection = new SelectList(products, "Id", "ProductName");
            SelectList customerselection = new SelectList(customers, "Id", "CustomerName");
            SelectList storeselection = new SelectList(stores, "Id", "StoreName");
            var result = new { Id,
                Products = productselection,
                SelectedProd = selectedproduct,
                Customers = customerselection,
                SelectedCust = selectedcustomer,
                Stores = storeselection,
                SelectedStore = selectedstore,                
                SaleDate = String.Format("{0:yyyy-MM-dd}", date)
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit(Sale model)
        {
            string msg;
            if (model.Id == 0)
            { // new record
                var sale = new Sale();
                sale.ProductId = model.ProductId;
                sale.CustomerId = model.CustomerId;
                sale.StoreId = model.StoreId;
                sale.SaleDate = model.SaleDate;
                try
                {
                    db.Sales.Add(sale);
                    db.SaveChanges();
                    msg = "New record created.";
                } catch (Exception)
                {
                    msg = "Unable to save changes. Contact the administrator.";
                }
            } else {
                var sale = db.Sales.Find(model.Id);
                if (sale == null)
                {
                    return HttpNotFound();
                } else
                { 
                    try {
                        sale.ProductId = model.ProductId;
                        sale.CustomerId = model.CustomerId;
                        sale.StoreId = model.StoreId;
                        sale.SaleDate = model.SaleDate;
                        db.SaveChanges();
                        msg = String.Format("Changes to record {0} have been saved.", model.Id);
                    }
                    catch (Exception)
                    {
                        msg = "Unable to save changes. Contact the administrator.";
                    }
                }
            }
            var result = new { Message = msg };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int Id)
        {
            string msg;
            var sale = db.Sales.Find(Id);
            if (sale == null)
            {
                return HttpNotFound();
            } else
            {
                try {
                    db.Sales.Remove(sale);
                    db.SaveChanges();                 
                    msg = string.Format("Record {0} has been deleted.", Id);
                } catch (Exception)
                {
                    msg = "Unable to delete record. Contact the administrator.";
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