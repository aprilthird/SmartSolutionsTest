using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSolutionsTest.Entities.Models
{
    public class Operation
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }

        public string ProductDetail { get; set; }

        public string ProductPresentation { get; set; }

        public double Income { get; set; }

        public double Outcome { get; set; }

        public double Balance { get; set; }
    }
}
