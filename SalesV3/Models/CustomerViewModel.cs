using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SalesV3.Models
{
    public class CustomerViewModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
    }
}