using System.Collections.Generic;

namespace CarWorkshop.Classes;

public class Car
{
    public int Car_id { get; set; }
    public required string Brand { get; set; }
    public required string Model { get; set; }
    public int Owner_id { get; set; }
    public required CarOwner Owner { get; set; }

    private List<Service> Services { get; set; } = new();
}