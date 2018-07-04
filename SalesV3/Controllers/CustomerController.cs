using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using SalesV3.Models;

namespace SalesV3.Controllers
{
    interface ICustomer
    {
        ActionResult Index();
        ActionResult GetCustomers();
        ActionResult Edit(int Id);
        ActionResult Edit(CustomerViewModel model);
        ActionResult Delete(int Id);
    }

    public class CustomerController : Controller, ICustomer
    {
        ShoppingEntities db = new ShoppingEntities();
        // GET: Customer
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetCustomers()
        {
            var customerslist = db.Customers.Select(x => new Models.CustomerViewModel()
            {
                Id = x.Id,
                CustomerName = x.CustomerName,
                CustomerAddress = x.CustomerAddress
            }).ToList();
            string json = JsonConvert.SerializeObject(customerslist, Formatting.Indented);
            var result = new { Customers = json };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int Id)
        {
            string name, address;
            if(Id == 0)
            { // new record

                name = "";
                address = "";
            } else
            {
                var customer = db.Customers.Find(Id);
                if (customer == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    name = customer.CustomerName;
                    address = customer.CustomerAddress;
                }
            }  
            var result = new {Id, Name  = name, Address = address };
            return Json(result, JsonRequestBehavior.AllowGet);
        } 

        [HttpPost]
        public ActionResult Edit(CustomerViewModel model)
        {        
            string msg;
            if (model.Id == 0)
            { // new record
                var customer = new Customer();
                customer.CustomerName = model.CustomerName;
                customer.CustomerAddress = model.CustomerAddress;
                try {
                    db.Customers.Add(customer);
                    db.SaveChanges();
                    msg = "New record created.";

                } catch (Exception)
                {
                    msg = "Unable to save new record. Contact the administrator.";
                }
            } else
            {
                var customer = db.Customers.Find(model.Id);
                if (customer == null)
                {
                    return HttpNotFound();
                }
                try
                {
                    customer.CustomerName = model.CustomerName;
                    customer.CustomerAddress = model.CustomerAddress;
                    db.SaveChanges();
                    msg = string.Format("Changes to {0} have been saved.", model.Id);
                }
                catch (Exception)
                {
                    msg = string.Format("Unable to save changes to record {0}. Contact the administrator.", model.Id);
                }
            }
 
            var result = new { Message = msg };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int Id)
        {
            string msg;
            var customer = db.Customers.Find(Id);
            if (customer == null)
            {
                return HttpNotFound();
            }

            if (customer.Sales.Any())
            {
                msg = string.Format("Unable to delete record {0}. Check this Customer is not used by a Sales record.", Id);
            }
            else
            {
                try
                {
                    db.Customers.Remove(customer);
                    db.SaveChanges();
                    msg = string.Format("Record {0} has been deleted.", Id);
                }
                catch (Exception)
                {
                    msg = string.Format("Unable to delete record {0}. Contact the administrator.", Id);
                }
            }

            var result = new {Message = msg };
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
