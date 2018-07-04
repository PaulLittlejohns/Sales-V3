using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalesV3.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public Nullable<decimal> ProductPrice { get; set; }
        public string ProductCost { get; set; }
    }
}