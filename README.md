# CarWorkshop Application

A desktop application built with Avalonia UI for managing car workshop operations, including car owner registration, service tracking, parts management, and invoice generation.

## Features

### ✅ Implemented Functionality

1. **Data Entry & Validation**
   - Car owner registration (name, phone)
   - Car details (brand, model)
   - Parts information (name, price)
   - Service details (type, price)
   - Real-time input validation with user feedback

2. **Database Operations**
   - Save all data to MySQL database
   - Proper error handling with user-friendly messages
   - Transaction-like operations ensuring data integrity

3. **User Feedback System**
   - Color-coded status messages (Green for success, Red for errors, Yellow for processing)
   - Clear validation messages with focus on problematic fields
   - Success confirmation with automatic form clearing

4. **Search Functionality**
   - Search cars by owner name (partial matching)
   - Display detailed results including car and owner information
   - Clear result formatting

5. **Invoice Generation**
   - Professional invoice layout
   - Automatic calculation of total amounts
   - Date and invoice ID generation
   - Customer and vehicle information display

## How to Use

### 1. Data Entry
- Fill in all required fields:
  - **Car Owner**: Name and phone number
  - **Car Details**: Brand and model
  - **Parts**: Name and price (numeric values only)
  - **Service**: Type and price (numeric values only)

### 2. Save Data
- Click the **Save** button to store all information
- The system will validate inputs and show status messages
- Upon successful save, an invoice will be automatically generated
- The form will clear after 2 seconds for the next entry

### 3. Generate Invoice
- Click **Print Invoice** to display the invoice for the last saved data
- The invoice appears in the search results area
- Contains all relevant customer, car, service, and billing information

### 4. Search Cars
- Enter an owner's name (or partial name) in the search box
- Click **Search** to find all cars belonging to that owner
- Results show car details, owner information, and contact details

## Technical Details

### Database Schema
The application expects a MySQL database with the following tables:
- `Car_owner` (Owner_id, Name, Phone)
- `Car` (Car_id, Brand, Model, Owner_id)
- `Parts` (Part_id, Part_name, Part_price)
- `Service` (Service_id, Service_type, Service_price)
- `Car_Workshop_Invoice` (Invoice_id, Car_id, Service_id, Part_id, Invoice_date, total_amount)

### Technologies Used
- **UI Framework**: Avalonia UI 11.3.0
- **Database**: MySQL with MySqlConnector
- **ORM**: Dapper
- **Language**: C# (.NET 9.0)

### Error Handling
- All database operations are wrapped in try-catch blocks
- User-friendly error messages are displayed
- Form validation prevents invalid data submission
- Connection management with proper disposal

## Status Messages

- ✅ **Green**: Success operations (data saved, search completed)
- ❌ **Red**: Errors (validation failures, database errors)
- ⚠️ **Yellow**: Processing status (saving, searching, generating)

## Requirements

- .NET 9.0 Runtime
- MySQL Server running locally
- Database configured with proper connection string in `DatabaseOperations.cs`

## Getting Started

1. Ensure MySQL is running and the database schema is created
2. Update the connection string in `DB/DatabaseOperations.cs` if needed
3. Build and run the application: `dotnet run`
4. Start entering car workshop data!

---

*The application provides a complete workflow for car workshop management with proper validation, error handling, and user feedback.* 