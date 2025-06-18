using System.Collections.Generic;

namespace CarWorkshop.Classes;

public class Service
{
    public int Service_id { get; set; }
    public required string Service_type { get; set; }
    public decimal Service_price { get; set; }

    private List<Parts> Parts { get; set; } = new();
}