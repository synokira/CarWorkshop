using System.Collections.Generic;

namespace CarWorkshop.Classes;

public abstract class Car
{
    public int CarId { get; set; }
    public required string Brand { get; set; }
    public required string Model { get; set; }
    public int OwnerId { get; set; }
    public required CarOwner Owner { get; set; }
    
    private List<Service> Services { get; set; }
}