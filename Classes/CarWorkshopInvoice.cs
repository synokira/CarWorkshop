using System;
using System.Collections.Generic;

namespace CarWorkshop.Classes
{
    public class CarWorkshopInvoice
    {
        public int Invoice_id { get; set; }
        public int Car_id { get; set; }
        public int Service_id { get; set; }
        public int Part_id { get; set; }
        public DateTime Invoice_date { get; set; }
        public decimal total_amount { get; set; }

        public Car? Car { get; set; }
        public Service? Service { get; set; }
        public Parts? Part { get; set; }

        private List<Car> cars = new();
    }
}