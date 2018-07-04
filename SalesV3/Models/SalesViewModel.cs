using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalesV3.Models
{
    public class SalesViewModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string CustomerName { get; set; }
        public string StoreName { get; set; }
        public System.DateTime SaleDate { get; set; }
    }
}