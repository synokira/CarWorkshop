using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using MySqlConnector;
using CarWorkshop.Classes;

namespace CarWorkshop.DB;

public class DatabaseOperations
{
    private const string ConnString =
        "Server=localhost;Database=Car_Workshop;User ID=datagrip_user;Password=your_password;";

    public InvoiceSaveResult SaveCompleteInvoice(InvoiceData invoiceData)
    {
        using var connection = new MySqlConnection(ConnString);
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            var result = new InvoiceSaveResult();

            // 1. Handle Car Owner
            result.OwnerId =
                GetExistingCarOwnerId(connection, transaction, invoiceData.OwnerName, invoiceData.OwnerPhone) ??
                AddCarOwner(connection, transaction, invoiceData.OwnerName, invoiceData.OwnerPhone);

            // 2. Handle Car
            result.CarId = GetExistingCarId(connection, transaction, invoiceData.CarBrand, invoiceData.CarModel,
                               result.OwnerId) ??
                           AddCar(connection, transaction, invoiceData.CarBrand, invoiceData.CarModel, result.OwnerId);

            // 3. Handle Part
            var existingPartId = GetExistingPartId(connection, transaction, invoiceData.PartName);
            if (existingPartId.HasValue)
            {
                result.PartId = existingPartId.Value;
                var existingPartPrice = GetPartPrice(connection, transaction, existingPartId.Value);
                if (existingPartPrice.HasValue && existingPartPrice.Value != invoiceData.PartPrice)
                {
                    UpdatePartPrice(connection, transaction, existingPartId.Value, invoiceData.PartPrice);
                }
            }
            else
            {
                result.PartId = AddPart(connection, transaction, invoiceData.PartName, invoiceData.PartPrice);
            }

            // 4. Handle Service
            var existingServiceId = GetExistingServiceId(connection, transaction, invoiceData.ServiceType);
            if (existingServiceId.HasValue)
            {
                result.ServiceId = existingServiceId.Value;
                var existingServicePrice = GetServicePrice(connection, transaction, existingServiceId.Value);
                if (existingServicePrice.HasValue && existingServicePrice.Value != invoiceData.ServicePrice)
                {
                    UpdateServicePrice(connection, transaction, existingServiceId.Value, invoiceData.ServicePrice);
                }
            }
            else
            {
                result.ServiceId = AddService(connection, transaction, invoiceData.ServiceType,
                    invoiceData.ServicePrice);
            }

            // 5. Create Invoice
            var totalAmount = invoiceData.PartPrice + invoiceData.ServicePrice;
            AddInvoice(connection, transaction, result.CarId, result.ServiceId, result.PartId, DateTime.Now,
                totalAmount);

            transaction.Commit();
            return result;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw new Exception($"Error saving complete invoice: {ex.Message}", ex);
        }
    }

    // Search and Load operations (read-only, simple connections)
    public List<CarSearchResult> SearchCarsByOwnerName(string ownerName)
    {
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();
            const string sql = @"
                SELECT c.CarId, c.Brand, c.Model, co.Name as OwnerName, co.Phone as OwnerPhone
                FROM Car c
                INNER JOIN CarOwner co ON c.OwnerId = co.OwnerId
                WHERE co.Name LIKE @OwnerName";

            return connection.Query<CarSearchResult>(sql, new { OwnerName = $"%{ownerName}%" }).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error searching cars: {ex.Message}");
        }
    }

    public List<Parts> LoadParts()
    {
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();
            const string sql = @"
                SELECT DISTINCT PartId, PartName, PartPrice 
                FROM Parts 
                ORDER BY PartName";

            return connection.Query<Parts>(sql).ToList();
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
                SELECT DISTINCT OwnerId, Name, Phone 
                FROM CarOwner 
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
                SELECT DISTINCT ServiceId, ServiceType, ServicePrice 
                FROM Service 
                ORDER BY ServiceType";

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

    // Individual update operations (for editing existing data)
    public void UpdateCarOwnerPhone(int ownerId, string newPhone)
    {
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();
            const string sql = @"UPDATE CarOwner SET Phone = @Phone WHERE OwnerId = @OwnerId";
            connection.Execute(sql, new { Phone = newPhone, OwnerId = ownerId });
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
            const string sql = @"UPDATE CarOwner SET Name = @Name WHERE OwnerId = @OwnerId";
            connection.Execute(sql, new { Name = newName, OwnerId = ownerId });
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
            const string sql = @"UPDATE Parts SET PartName = @PartName WHERE PartId = @PartId";
            connection.Execute(sql, new { PartName = newName, PartId = partId });
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
            const string sql = @"UPDATE Service SET ServiceType = @ServiceType WHERE ServiceId = @ServiceId";
            connection.Execute(sql, new { ServiceType = newType, ServiceId = serviceId });
        }
        catch (Exception ex)
        {
            throw new Exception($"Error updating service type: {ex.Message}");
        }
    }

    // Private transaction-based methods for batch operations
    private int AddCarOwner(MySqlConnection connection, MySqlTransaction transaction, string name, string phone)
    {
        const string sql = "INSERT INTO CarOwner (Name, Phone) VALUES (@Name, @Phone); SELECT LAST_INSERT_ID();";
        return connection.QuerySingle<int>(sql, new { Name = name, Phone = phone }, transaction);
    }

    private int AddCar(MySqlConnection connection, MySqlTransaction transaction, string brand, string model,
        int ownerId)
    {
        const string sql =
            "INSERT INTO Car (Brand, Model, OwnerId) VALUES (@Brand, @Model, @OwnerId); SELECT LAST_INSERT_ID();";
        return connection.QuerySingle<int>(sql, new { Brand = brand, Model = model, OwnerId = ownerId }, transaction);
    }

    private int AddPart(MySqlConnection connection, MySqlTransaction transaction, string partName, decimal partPrice)
    {
        const string sql =
            "INSERT INTO Parts (PartName, PartPrice) VALUES (@PartName, @PartPrice); SELECT LAST_INSERT_ID();";
        return connection.QuerySingle<int>(sql, new { PartName = partName, PartPrice = partPrice }, transaction);
    }

    private int AddService(MySqlConnection connection, MySqlTransaction transaction, string serviceType,
        decimal servicePrice)
    {
        const string sql =
            "INSERT INTO Service (ServiceType, ServicePrice) VALUES (@ServiceType, @ServicePrice); SELECT LAST_INSERT_ID();";
        return connection.QuerySingle<int>(sql, new { ServiceType = serviceType, ServicePrice = servicePrice },
            transaction);
    }

    private void AddInvoice(MySqlConnection connection, MySqlTransaction transaction, int carId, int serviceId,
        int partId, DateTime invoiceDate, decimal totalAmount)
    {
        const string sql =
            "INSERT INTO CarWorkshopInvoice (CarId, ServiceId, PartId, InvoiceDate, TotalAmount) " +
            "VALUES (@CarId, @ServiceId, @PartId, @InvoiceDate, @TotalAmount);";
        connection.Execute(sql, new
        {
            CarId = carId,
            ServiceId = serviceId,
            PartId = partId,
            InvoiceDate = invoiceDate,
            TotalAmount = totalAmount
        }, transaction);
    }

    private int? GetExistingCarOwnerId(MySqlConnection connection, MySqlTransaction transaction, string name,
        string phone)
    {
        const string sql = "SELECT OwnerId FROM CarOwner WHERE Name = @Name AND Phone = @Phone LIMIT 1";
        return connection.QuerySingleOrDefault<int?>(sql, new { Name = name, Phone = phone }, transaction);
    }

    private int? GetExistingCarId(MySqlConnection connection, MySqlTransaction transaction, string brand, string model,
        int ownerId)
    {
        const string sql =
            "SELECT CarId FROM Car WHERE Brand = @Brand AND Model = @Model AND OwnerId = @OwnerId LIMIT 1";
        return connection.QuerySingleOrDefault<int?>(sql, new { Brand = brand, Model = model, OwnerId = ownerId },
            transaction);
    }

    private int? GetExistingPartId(MySqlConnection connection, MySqlTransaction transaction, string partName)
    {
        const string sql = "SELECT PartId FROM Parts WHERE PartName = @PartName LIMIT 1";
        return connection.QuerySingleOrDefault<int?>(sql, new { PartName = partName }, transaction);
    }

    private int? GetExistingServiceId(MySqlConnection connection, MySqlTransaction transaction, string serviceType)
    {
        const string sql = "SELECT ServiceId FROM Service WHERE ServiceType = @ServiceType LIMIT 1";
        return connection.QuerySingleOrDefault<int?>(sql, new { ServiceType = serviceType }, transaction);
    }

    private decimal? GetPartPrice(MySqlConnection connection, MySqlTransaction transaction, int partId)
    {
        const string sql = "SELECT PartPrice FROM Parts WHERE PartId = @PartId";
        return connection.QuerySingleOrDefault<decimal?>(sql, new { PartId = partId }, transaction);
    }

    private decimal? GetServicePrice(MySqlConnection connection, MySqlTransaction transaction, int serviceId)
    {
        const string sql = "SELECT ServicePrice FROM Service WHERE ServiceId = @ServiceId";
        return connection.QuerySingleOrDefault<decimal?>(sql, new { ServiceId = serviceId }, transaction);
    }

    private void UpdatePartPrice(MySqlConnection connection, MySqlTransaction transaction, int partId, decimal newPrice)
    {
        const string sql = "UPDATE Parts SET PartPrice = @PartPrice WHERE PartId = @PartId";
        connection.Execute(sql, new { PartPrice = newPrice, PartId = partId }, transaction);
    }

    private void UpdateServicePrice(MySqlConnection connection, MySqlTransaction transaction, int serviceId,
        decimal newPrice)
    {
        const string sql = "UPDATE Service SET ServicePrice = @ServicePrice WHERE ServiceId = @ServiceId";
        connection.Execute(sql, new { ServicePrice = newPrice, ServiceId = serviceId }, transaction);
    }
}