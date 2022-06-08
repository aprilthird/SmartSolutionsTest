using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSolutionsTest.Entities.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        
        public int OrderId { get; set; }

        public Order Order { get; set; }

        public string ProductDetail { get; set; }

        public int Quantity { get; set; }

        public string ProductPresentation { get; set; }

        public float ProductUnitPrice { get; set; }

        public float SubTotal { get; set; }
    }
}
