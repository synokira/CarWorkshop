namespace CarWorkshop.Classes;

public abstract class CarOwner
{
    public int OwnerId { get; set; }
    public required string Name { get; set; }
    public required string Phone { get; set; }
}