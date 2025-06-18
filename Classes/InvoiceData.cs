namespace CarWorkshop.Classes;

public class InvoiceData
{
    public string OwnerName { get; set; } = string.Empty;
    public string OwnerPhone { get; set; } = string.Empty;
    public string CarBrand { get; set; } = string.Empty;
    public string CarModel { get; set; } = string.Empty;
    public string PartName { get; set; } = string.Empty;
    public decimal PartPrice { get; set; }
    public string ServiceType { get; set; } = string.Empty;
    public decimal ServicePrice { get; set; }
} 