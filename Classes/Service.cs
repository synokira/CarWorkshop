using System.Collections.Generic;

namespace CarWorkshop.Classes;
    
    public abstract class Service
    {
        public int ServiceId { get; set; }
        public required string ServiceType { get; set; }
        public decimal ServicePrice { get; set; }
        
        private List<Parts> Parts { get; set; }
    }