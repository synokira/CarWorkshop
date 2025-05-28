using System;
using System.Collections.Generic;

namespace CarWorkshop.Classes;
    
    public class CarWorkshopInvoice
    {
        public int InvoiceId { get; set; }
        public int CarId { get; set; }
        public int ServiceId { get; set; }
        public int PartId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
    
        public Car Car { get; set; }
        public Service Service { get; set; }
        public Parts Part { get; set; }
        
        private List<Car> cars = [];
    }