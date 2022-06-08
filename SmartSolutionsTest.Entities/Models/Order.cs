using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSolutionsTest.Entities.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string ClientName { get; set; }

        public DateTime Date { get; set; }

        public string Currency { get; set; }

        public float Total { get; set; }

        public List<OrderDetail> Details { get; set; }
    }
}
