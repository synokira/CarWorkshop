using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using MySqlConnector;

namespace CarWorkshop.DB;

public class DatabaseOperations
{
    private const string ConnString =
        "Server=localhost;Database=Car_Workshop;User ID=datagrip_user;Password=your_password;";

    // Public property to access connection string
    public string ConnectionString => ConnString;

    public int AddCarOwner(string name, string phone)
    {
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();
            const string sql = "INSERT INTO Car_owner (Name, Phone) VALUES (@Name, @Phone); SELECT LAST_INSERT_ID();";
            return connection.QuerySingle<int>(sql, new { Name = name, Phone = phone });
        }
        catch (Exception ex)
        {
            throw new Exception($"Error adding car owner: {ex.Message}");
        }
    }

    public int AddCar(string brand, string model, int ownerId)
    {
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();
            const string sql =
                "INSERT INTO Car (Brand, Model, Owner_id) VALUES (@Brand, @Model, @Owner_id); SELECT LAST_INSERT_ID();";
            return connection.QuerySingle<int>(sql, new { Brand = brand, Model = model, Owner_id = ownerId });
        }
        catch (Exception ex)
        {
            throw new Exception($"Error adding car: {ex.Message}");
        }
    }

    public int AddPart(string partName, decimal partPrice)
    {
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();
            const string sql =
                "INSERT INTO Parts (Part_name, Part_price) VALUES (@Part_name, @Part_price); SELECT LAST_INSERT_ID();";
            return connection.QuerySingle<int>(sql, new { Part_name = partName, Part_price = partPrice });
        }
        catch (Exception ex)
        {
            throw new Exception($"Error adding part: {ex.Message}");
        }
    }

    public int AddService(string serviceType, decimal servicePrice)
    {
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();
            const string sql =
                "INSERT INTO Service (Service_type, Service_price) VALUES (@Service_type, @Service_price); SELECT LAST_INSERT_ID();";
            return connection.QuerySingle<int>(sql, new { Service_type = serviceType, Service_price = servicePrice });
        }
        catch (Exception ex)
        {
            throw new Exception($"Error adding service: {ex.Message}");
        }
    }

    public void AddInvoice(int carId, int serviceId, int partId, DateTime invoiceDate, decimal totalAmount)
    {
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();
            const string sql =
                "INSERT INTO Car_Workshop_Invoice (Car_id, Service_id, Part_id, Invoice_date, total_amount) " +
                "VALUES (@Car_id, @Service_id, @Part_id, @Invoice_date, @total_amount);";
            connection.Execute(sql, new
            {
                Car_id = carId,
                Service_id = serviceId,
                Part_id = partId,
                Invoice_date = invoiceDate,
                total_amount = totalAmount
            });
        }
        catch (Exception ex)
        {
            throw new Exception($"Error adding invoice: {ex.Message}");
        }
    }

    public List<CarSearchResult> SearchCarsByOwnerName(string ownerName)
    {
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();
            const string sql = @"
                SELECT c.Car_id, c.Brand, c.Model, co.Name as OwnerName, co.Phone as OwnerPhone
                FROM Car c
                INNER JOIN Car_owner co ON c.Owner_id = co.Owner_id
                WHERE co.Name LIKE @OwnerName";

            return connection.Query<CarSearchResult>(sql, new { OwnerName = $"%{ownerName}%" }).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error searching cars: {ex.Message}");
        }
    }

    public List<CarWorkshop.Classes.Parts> LoadParts()
    {
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();
            const string sql = @"
                SELECT DISTINCT Part_id, Part_name, Part_price 
                FROM Parts 
                ORDER BY Part_name";

            return connection.Query<CarWorkshop.Classes.Parts>(sql).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error loading parts: {ex.Message}");
        }
    }

    public List<CarOwnerResult> LoadCarOwners()
    {
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();
            const string sql = @"
                SELECT DISTINCT Owner_id, Name, Phone 
                FROM Car_owner 
                ORDER BY Name";

            return connection.Query<CarOwnerResult>(sql).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error loading car owners: {ex.Message}");
        }
    }

    public List<ServiceResult> LoadServices()
    {
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();
            const string sql = @"
                SELECT DISTINCT Service_id, Service_type, Service_price 
                FROM Service 
                ORDER BY Service_type";

            return connection.Query<ServiceResult>(sql).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error loading services: {ex.Message}");
        }
    }

    public List<string> LoadCarBrands()
    {
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();
            const string sql = @"SELECT DISTINCT Brand FROM Car ORDER BY Brand";

            return connection.Query<string>(sql).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error loading car brands: {ex.Message}");
        }
    }

    public List<string> LoadCarModelsByBrand(string brand)
    {
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();
            const string sql = @"SELECT DISTINCT Model FROM Car WHERE Brand = @Brand ORDER BY Model";

            return connection.Query<string>(sql, new { Brand = brand }).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error loading car models: {ex.Message}");
        }
    }

    public int? GetExistingCarOwnerId(string name, string phone)
    {
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();
            const string sql = @"SELECT Owner_id FROM Car_owner WHERE Name = @Name AND Phone = @Phone LIMIT 1";

            return connection.QuerySingleOrDefault<int?>(sql, new { Name = name, Phone = phone });
        }
        catch (Exception ex)
        {
            throw new Exception($"Error checking existing car owner: {ex.Message}");
        }
    }

    public int? GetExistingPartId(string partName)
    {
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();
            const string sql = @"SELECT Part_id FROM Parts WHERE Part_name = @Part_name LIMIT 1";

            return connection.QuerySingleOrDefault<int?>(sql, new { Part_name = partName });
        }
        catch (Exception ex)
        {
            throw new Exception($"Error checking existing part: {ex.Message}");
        }
    }

    public int? GetExistingServiceId(string serviceType)
    {
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();
            const string sql = @"SELECT Service_id FROM Service WHERE Service_type = @Service_type LIMIT 1";

            return connection.QuerySingleOrDefault<int?>(sql, new { Service_type = serviceType });
        }
        catch (Exception ex)
        {
            throw new Exception($"Error checking existing service: {ex.Message}");
        }
    }

    public int? GetExistingCarId(string brand, string model, int ownerId)
    {
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();
            const string sql =
                @"SELECT Car_id FROM Car WHERE Brand = @Brand AND Model = @Model AND Owner_id = @Owner_id LIMIT 1";

            return connection.QuerySingleOrDefault<int?>(sql, new { Brand = brand, Model = model, Owner_id = ownerId });
        }
        catch (Exception ex)
        {
            throw new Exception($"Error checking existing car: {ex.Message}");
        }
    }

    public void UpdatePartPrice(int partId, decimal newPrice)
    {
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();
            const string sql = @"UPDATE Parts SET Part_price = @Part_price WHERE Part_id = @Part_id";
            connection.Execute(sql, new { Part_price = newPrice, Part_id = partId });
        }
        catch (Exception ex)
        {
            throw new Exception($"Error updating part price: {ex.Message}");
        }
    }

    public void UpdateServicePrice(int serviceId, decimal newPrice)
    {
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();
            const string sql = @"UPDATE Service SET Service_price = @Service_price WHERE Service_id = @Service_id";
            connection.Execute(sql, new { Service_price = newPrice, Service_id = serviceId });
        }
        catch (Exception ex)
        {
            throw new Exception($"Error updating service price: {ex.Message}");
        }
    }

    public decimal? GetPartPrice(int partId)
    {
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();
            const string sql = @"SELECT Part_price FROM Parts WHERE Part_id = @Part_id";
            return connection.QuerySingleOrDefault<decimal?>(sql, new { Part_id = partId });
        }
        catch (Exception ex)
        {
            throw new Exception($"Error getting part price: {ex.Message}");
        }
    }

    public decimal? GetServicePrice(int serviceId)
    {
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();
            const string sql = @"SELECT Service_price FROM Service WHERE Service_id = @Service_id";
            return connection.QuerySingleOrDefault<decimal?>(sql, new { Service_id = serviceId });
        }
        catch (Exception ex)
        {
            throw new Exception($"Error getting service price: {ex.Message}");
        }
    }

    public void UpdateCarOwnerPhone(int ownerId, string newPhone)
    {
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();
            const string sql = @"UPDATE Car_owner SET Phone = @Phone WHERE Owner_id = @Owner_id";
            connection.Execute(sql, new { Phone = newPhone, Owner_id = ownerId });
        }
        catch (Exception ex)
        {
            throw new Exception($"Error updating car owner phone: {ex.Message}");
        }
    }

    public void UpdateCarOwnerName(int ownerId, string newName)
    {
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();
            const string sql = @"UPDATE Car_owner SET Name = @Name WHERE Owner_id = @Owner_id";
            connection.Execute(sql, new { Name = newName, Owner_id = ownerId });
        }
        catch (Exception ex)
        {
            throw new Exception($"Error updating car owner name: {ex.Message}");
        }
    }

    public void UpdatePartName(int partId, string newName)
    {
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();
            const string sql = @"UPDATE Parts SET Part_name = @Part_name WHERE Part_id = @Part_id";
            connection.Execute(sql, new { Part_name = newName, Part_id = partId });
        }
        catch (Exception ex)
        {
            throw new Exception($"Error updating part name: {ex.Message}");
        }
    }

    public void UpdateServiceType(int serviceId, string newType)
    {
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();
            const string sql = @"UPDATE Service SET Service_type = @Service_type WHERE Service_id = @Service_id";
            connection.Execute(sql, new { Service_type = newType, Service_id = serviceId });
        }
        catch (Exception ex)
        {
            throw new Exception($"Error updating service type: {ex.Message}");
        }
    }

    public class CarSearchResult
    {
        public int Car_id { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;
        public string OwnerPhone { get; set; } = string.Empty;
    }

    public class CarOwnerResult
    {
        public int Owner_id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    public class ServiceResult
    {
        public int Service_id { get; set; }
        public string Service_type { get; set; } = string.Empty;
        public decimal Service_price { get; set; }
    }
}