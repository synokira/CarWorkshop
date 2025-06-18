namespace CarWorkshop.Classes;

public class CarSearchResult
{
  public int CarId { get; set; }
  public string Brand { get; set; } = string.Empty;
  public string Model { get; set; } = string.Empty;
  public string OwnerName { get; set; } = string.Empty;
  public string OwnerPhone { get; set; } = string.Empty;
}

public class CarOwnerResult
{
  public int OwnerId { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Phone { get; set; } = string.Empty;
}

public class ServiceResult
{
  public int ServiceId { get; set; }
  public string ServiceType { get; set; } = string.Empty;
  public decimal ServicePrice { get; set; }
} 