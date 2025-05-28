using System;
using System.Data;
using System.Data.SqlClient;
using Dapper;


namespace CarWorkshop.DB;

public class DatabaseOperations
{
    private const string ConnString = "Server=localhost;Database=Car_Workshop;User=datagrip_user;Password=your_password;";
    private readonly IDbConnection _db = new SqlConnection(ConnString);
    

    public void AddCarOwner(string name, string phone)
    {
        const string sql = "INSERT INTO Car_owner (Name, Phone) VALUES (@Name, @Phone);";
        object[] parameters = [new { Name = name, Phone = phone }];
        _db.Execute(sql, parameters);
    }
    public void AddCar(string brand, string model, int ownerId)
    {
        const string sql = "INSERT INTO Car (Brand, Model, Owner_id) VALUES (@Brand, @Model, @Owner_id);";
        object[] parameters = [new { Brand = brand, Model = model, Owner_id = ownerId }];
        _db.Execute(sql, parameters);
    }
    public void AddPart(string partName, decimal partPrice)
    {
        const string sql = "INSERT INTO Parts (Part_name, Part_price) VALUES (@Part_name, @Part_price);";
        object[] parameters = [new { Part_name = partName, Part_price = partPrice }];
        _db.Execute(sql, parameters);
    }
    public void AddService(string serviceType, decimal servicePrice)
    {
        const string sql = "INSERT INTO Service (Service_type, Service_price) VALUES (@Service_type, @Service_price);";
        object[] parameters = [new { Service_type = serviceType, Service_price = servicePrice }];
        _db.Execute(sql, parameters);
    }
    public void AddInvoice(int carId, int serviceId, int partId, DateTime invoiceDate, decimal totalAmount)
    {
        const string sql = "INSERT INTO Car_Workshop_Invoice (Car_id, Service_id, Part_id, Invoice_date, total_amount) " +
                           "VALUES (@Car_id, @Service_id, @Part_id, @Invoice_date, @total_amount);";
        object[] parameters = [new { Car_id = carId, Service_id = serviceId, Part_id = partId, Invoice_date = invoiceDate, total_amount = totalAmount }
        ];
        _db.Execute(sql, parameters);
    }
}