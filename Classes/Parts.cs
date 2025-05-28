namespace CarWorkshop.Classes;

public abstract class Parts
{
    public int PartId { get; set; }
    public required string PartName { get; set; }
    public decimal PartPrice { get; set; }
}