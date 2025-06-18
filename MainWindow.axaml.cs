using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CarWorkshop.DB;

namespace CarWorkshop;

public partial class MainWindow : Window
{
    private readonly DatabaseOperations _dbOps;
    private int? _lastCarId;
    private int? _lastServiceId;
    private int? _lastPartId;

    // Data collections for dropdowns
    private List<DatabaseOperations.CarOwnerResult> _carOwners = new();
    private List<CarWorkshop.Classes.Parts> _parts = new();
    private List<DatabaseOperations.ServiceResult> _services = new();
    private List<string> _carBrands = new();
    private List<string> _carModels = new(); // For current brand models

    // Track if dropdown selection is in progress to avoid recursion
    private bool _isUpdatingFromDropdown = false;

    // Track edit mode states
    private bool _isOwnerNameEditMode = false;
    private bool _isPartEditMode = false;
    private bool _isServiceEditMode = false;
    private bool _isBrandEditMode = false;
    private bool _isModelEditMode = false;

    // Track original values for comparison when saving edits
    private string _originalOwnerName = "";
    private string _originalOwnerPhone = "";
    private string _originalPartName = "";
    private decimal _originalPartPrice = 0;
    private string _originalServiceType = "";
    private decimal _originalServicePrice = 0;
    private string _originalBrand = "";
    private string _originalModel = "";

    public MainWindow()
    {
        InitializeComponent();
        _dbOps = new DatabaseOperations();
    }

    private async void WindowLoaded(object? sender, RoutedEventArgs e)
    {
        try
        {
            await LoadAllDropdownData();
            PopulateDropdowns();
        }
        catch (Exception ex)
        {
            UpdateStatus($"❌ Error loading data: {ex.Message}", "#FF6B6B");
        }
    }

    private async System.Threading.Tasks.Task LoadAllDropdownData()
    {
        // Load all data from database
        _carOwners = _dbOps.LoadCarOwners();
        _parts = _dbOps.LoadParts();
        _services = _dbOps.LoadServices();
        _carBrands = _dbOps.LoadCarBrands();
    }

    private void PopulateDropdowns()
    {
        // Populate Car Owner dropdown - use HashSet to prevent duplicates
        CarOwnerNameDropdown.Items.Clear();
        CarOwnerNameDropdown.Items.Add("-");
        var uniqueOwnerNames = new HashSet<string>();
        foreach (var owner in _carOwners)
        {
            if (!string.IsNullOrEmpty(owner.Name) && uniqueOwnerNames.Add(owner.Name))
            {
                CarOwnerNameDropdown.Items.Add(owner.Name);
            }
        }

        CarOwnerNameDropdown.SelectedIndex = 0;

        // Populate Parts dropdown - use HashSet to prevent duplicates
        PartNameDropdown.Items.Clear();
        PartNameDropdown.Items.Add("-");
        var uniquePartNames = new HashSet<string>();
        foreach (var part in _parts)
        {
            if (!string.IsNullOrEmpty(part.Part_name) && uniquePartNames.Add(part.Part_name))
            {
                PartNameDropdown.Items.Add(part.Part_name);
            }
        }

        PartNameDropdown.SelectedIndex = 0;

        // Populate Service dropdown - use HashSet to prevent duplicates
        ServiceTypeDropdown.Items.Clear();
        ServiceTypeDropdown.Items.Add("-");
        var uniqueServiceTypes = new HashSet<string>();
        foreach (var service in _services)
        {
            if (!string.IsNullOrEmpty(service.Service_type) && uniqueServiceTypes.Add(service.Service_type))
            {
                ServiceTypeDropdown.Items.Add(service.Service_type);
            }
        }

        ServiceTypeDropdown.SelectedIndex = 0;

        // Populate Car Brand dropdown - use HashSet to prevent duplicates
        CarBrandDropdown.Items.Clear();
        CarBrandDropdown.Items.Add("-");
        var uniqueBrands = new HashSet<string>();
        foreach (var brand in _carBrands)
        {
            if (!string.IsNullOrEmpty(brand) && uniqueBrands.Add(brand))
            {
                CarBrandDropdown.Items.Add(brand);
            }
        }

        CarBrandDropdown.SelectedIndex = 0;
    }

    // Edit button event handlers
    private void EditOwnerNameButtonClick(object? sender, RoutedEventArgs e)
    {
        _isOwnerNameEditMode = !_isOwnerNameEditMode;

        if (_isOwnerNameEditMode)
        {
            // Store original value for comparison
            _originalOwnerName = CarOwnerNameDropdown.SelectedItem?.ToString() == "-"
                ? ""
                : CarOwnerNameDropdown.SelectedItem?.ToString() ?? "";

            // Switch to edit mode
            CarOwnerNameDropdown.IsVisible = false;
            CarOwnerName.IsVisible = true;
            CarOwnerName.Text = _originalOwnerName;
            CarOwnerName.Focus();
            EditOwnerNameButton.Content = "Done";
        }
        else
        {
            var newName = CarOwnerName.Text?.Trim() ?? "";

            if (!string.IsNullOrEmpty(newName))
            {
                // Just update the UI state - database will be updated on Save
                var existingOwner = _carOwners.FirstOrDefault(o =>
                    o.Name.Equals(newName, StringComparison.OrdinalIgnoreCase));

                if (existingOwner != null)
                {
                    // Select existing owner in dropdown
                    UpdateStatus($"✓ Selected existing owner: {newName}", "#90EE90");
                    CarOwnerNameDropdown.SelectedItem = existingOwner.Name;
                    CarOwnerPhone.Text = existingOwner.Phone;
                }
                else
                {
                    // New owner - will be created on save
                    UpdateStatus($"✓ New owner name set: {newName}. Will be saved when you click Save.", "#FFD700");

                    // Add to dropdown temporarily for UI consistency
                    CarOwnerNameDropdown.Items.Add(newName);
                    CarOwnerNameDropdown.SelectedItem = newName;

                    // Auto-enable phone editing for new owner
                    if (string.IsNullOrEmpty(CarOwnerPhone.Text))
                    {
                        CarOwnerPhone.Text = "";
                        CarOwnerPhone.IsReadOnly = false;
                        EditPhoneButton.Content = "Done";
                        UpdateStatus($"💡 Please enter phone number for new owner: {newName}", "#FFD700");
                    }
                }
            }

            // Switch back to dropdown mode
            CarOwnerName.IsVisible = false;
            CarOwnerNameDropdown.IsVisible = true;
            EditOwnerNameButton.Content = "Edit";
        }
    }

    private void EditPhoneButtonClick(object? sender, RoutedEventArgs e)
    {
        if (!CarOwnerPhone.IsReadOnly)
        {
            // Done clicked - just update UI state, don't save to database
            var newPhone = CarOwnerPhone.Text?.Trim() ?? "";
            if (!string.IsNullOrEmpty(newPhone))
            {
                UpdateStatus($"✓ Phone number set: {newPhone}. Will be saved when you click Save.", "#FFD700");
            }

            CarOwnerPhone.IsReadOnly = true;
            EditPhoneButton.Content = "Edit";
        }
        else
        {
            // Edit clicked - store original value and enable editing
            _originalOwnerPhone = CarOwnerPhone.Text?.Trim() ?? "";
            CarOwnerPhone.IsReadOnly = false;
            EditPhoneButton.Content = "Done";
            CarOwnerPhone.Focus();
        }
    }

    private void EditBrandButtonClick(object? sender, RoutedEventArgs e)
    {
        _isBrandEditMode = !_isBrandEditMode;

        if (_isBrandEditMode)
        {
            // Store original value
            _originalBrand = CarBrandDropdown.SelectedItem?.ToString() == "-"
                ? ""
                : CarBrandDropdown.SelectedItem?.ToString() ?? "";

            CarBrandDropdown.IsVisible = false;
            CarBrand.IsVisible = true;
            CarBrand.Text = _originalBrand;
            CarBrand.Focus();
            EditBrandButton.Content = "Done";
        }
        else
        {
            var newBrand = CarBrand.Text?.Trim() ?? "";

            if (!string.IsNullOrEmpty(newBrand))
            {
                // Just update UI state - database will be updated on Save
                var existingBrand = _carBrands.FirstOrDefault(b =>
                    b.Equals(newBrand, StringComparison.OrdinalIgnoreCase));

                if (existingBrand != null)
                {
                    // Select existing brand
                    UpdateStatus($"✓ Selected existing brand: {newBrand}", "#90EE90");
                    CarBrandDropdown.SelectedItem = existingBrand;

                    // Load models for this brand
                    CarBrandDropdownSelectionChanged(CarBrandDropdown, null);
                }
                else
                {
                    // New brand - will be created on save
                    UpdateStatus($"✓ New brand set: {newBrand}. Will be saved when you click Save.", "#FFD700");

                    // Add to dropdown temporarily for UI consistency
                    CarBrandDropdown.Items.Add(newBrand);
                    CarBrandDropdown.SelectedItem = newBrand;

                    // Clear model dropdown since it's a new brand
                    CarModelDropdown.Items.Clear();
                    CarModelDropdown.Items.Add("-");
                    CarModelDropdown.SelectedIndex = 0;
                    _carModels.Clear();
                }
            }

            CarBrand.IsVisible = false;
            CarBrandDropdown.IsVisible = true;
            EditBrandButton.Content = "Edit";
        }
    }

    private void EditModelButtonClick(object? sender, RoutedEventArgs e)
    {
        _isModelEditMode = !_isModelEditMode;

        if (_isModelEditMode)
        {
            // Store original value
            _originalModel = CarModelDropdown.SelectedItem?.ToString() == "-"
                ? ""
                : CarModelDropdown.SelectedItem?.ToString() ?? "";

            CarModelDropdown.IsVisible = false;
            CarModel.IsVisible = true;
            CarModel.Text = _originalModel;
            CarModel.Focus();
            EditModelButton.Content = "Done";
        }
        else
        {
            var newModel = CarModel.Text?.Trim() ?? "";

            if (!string.IsNullOrEmpty(newModel))
            {
                // Just update UI state - database will be updated on Save
                var existingModel = _carModels.FirstOrDefault(m =>
                    m.Equals(newModel, StringComparison.OrdinalIgnoreCase));

                if (existingModel != null)
                {
                    // Select existing model
                    UpdateStatus($"✓ Selected existing model: {newModel}", "#90EE90");
                    CarModelDropdown.SelectedItem = existingModel;
                }
                else
                {
                    // New model - will be created on save
                    UpdateStatus($"✓ New model set: {newModel}. Will be saved when you click Save.", "#FFD700");

                    // Add to dropdown temporarily for UI consistency
                    CarModelDropdown.Items.Add(newModel);
                    CarModelDropdown.SelectedItem = newModel;
                }
            }

            CarModel.IsVisible = false;
            CarModelDropdown.IsVisible = true;
            EditModelButton.Content = "Edit";
        }
    }

    private void CarModelDropdownSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_isUpdatingFromDropdown) return;

        var comboBox = sender as ComboBox;
        var selectedModel = comboBox?.SelectedItem?.ToString();

        if (!string.IsNullOrEmpty(selectedModel) && selectedModel != "-")
        {
            UpdateStatus($"✓ Model selected: {selectedModel}", "#90EE90");
        }
    }

    private void EditPartButtonClick(object? sender, RoutedEventArgs e)
    {
        _isPartEditMode = !_isPartEditMode;

        if (_isPartEditMode)
        {
            // Store original value
            _originalPartName = PartNameDropdown.SelectedItem?.ToString() == "-"
                ? ""
                : PartNameDropdown.SelectedItem?.ToString() ?? "";

            PartNameDropdown.IsVisible = false;
            PartName.IsVisible = true;
            PartName.Text = _originalPartName;
            PartName.Focus();
            EditPartButton.Content = "Done";
        }
        else
        {
            var newPartName = PartName.Text?.Trim() ?? "";

            if (!string.IsNullOrEmpty(newPartName))
            {
                // Just update UI state - database will be updated on Save
                var existingPart = _parts.FirstOrDefault(p =>
                    p.Part_name.Equals(newPartName, StringComparison.OrdinalIgnoreCase));

                if (existingPart != null)
                {
                    // Select existing part
                    UpdateStatus($"✓ Selected existing part: {newPartName}", "#90EE90");
                    PartNameDropdown.SelectedItem = existingPart.Part_name;
                    PartPrice.Text = existingPart.Part_price.ToString("F2");
                }
                else
                {
                    // New part - will be created on save
                    UpdateStatus($"✓ New part name set: {newPartName}. Will be saved when you click Save.", "#FFD700");

                    // Add to dropdown temporarily for UI consistency
                    PartNameDropdown.Items.Add(newPartName);
                    PartNameDropdown.SelectedItem = newPartName;

                    // Auto-enable price editing for new part
                    if (string.IsNullOrEmpty(PartPrice.Text) || PartPrice.Text == "0.00")
                    {
                        PartPrice.Text = "";
                        PartPrice.IsReadOnly = false;
                        EditPartPriceButton.Content = "Done";
                        UpdateStatus($"💡 Please enter price for new part: {newPartName}", "#FFD700");
                    }
                }
            }

            PartName.IsVisible = false;
            PartNameDropdown.IsVisible = true;
            EditPartButton.Content = "Edit";
        }
    }

    private void EditPartPriceButtonClick(object? sender, RoutedEventArgs e)
    {
        if (!PartPrice.IsReadOnly)
        {
            // Done clicked - just update UI state, don't save to database
            if (decimal.TryParse(PartPrice.Text?.Trim(), out var newPrice) && newPrice > 0)
            {
                UpdateStatus($"✓ Part price set: ${newPrice:F2}. Will be saved when you click Save.", "#FFD700");
            }

            PartPrice.IsReadOnly = true;
            EditPartPriceButton.Content = "Edit";
        }
        else
        {
            // Edit clicked
            decimal.TryParse(PartPrice.Text?.Trim(), out _originalPartPrice);
            PartPrice.IsReadOnly = false;
            EditPartPriceButton.Content = "Done";
            PartPrice.Focus();
        }
    }

    private void EditServiceButtonClick(object? sender, RoutedEventArgs e)
    {
        _isServiceEditMode = !_isServiceEditMode;

        if (_isServiceEditMode)
        {
            // Store original value
            _originalServiceType = ServiceTypeDropdown.SelectedItem?.ToString() == "-"
                ? ""
                : ServiceTypeDropdown.SelectedItem?.ToString() ?? "";

            ServiceTypeDropdown.IsVisible = false;
            ServiceType.IsVisible = true;
            ServiceType.Text = _originalServiceType;
            ServiceType.Focus();
            EditServiceButton.Content = "Done";
        }
        else
        {
            var newServiceType = ServiceType.Text?.Trim() ?? "";

            if (!string.IsNullOrEmpty(newServiceType))
            {
                // Just update UI state - database will be updated on Save
                var existingService = _services.FirstOrDefault(s =>
                    s.Service_type.Equals(newServiceType, StringComparison.OrdinalIgnoreCase));

                if (existingService != null)
                {
                    // Select existing service
                    UpdateStatus($"✓ Selected existing service: {newServiceType}", "#90EE90");
                    ServiceTypeDropdown.SelectedItem = existingService.Service_type;
                    ServicePrice.Text = existingService.Service_price.ToString("F2");
                }
                else
                {
                    // New service - will be created on save
                    UpdateStatus($"✓ New service type set: {newServiceType}. Will be saved when you click Save.",
                        "#FFD700");

                    // Add to dropdown temporarily for UI consistency
                    ServiceTypeDropdown.Items.Add(newServiceType);
                    ServiceTypeDropdown.SelectedItem = newServiceType;

                    // Auto-enable price editing for new service
                    if (string.IsNullOrEmpty(ServicePrice.Text) || ServicePrice.Text == "0.00")
                    {
                        ServicePrice.Text = "";
                        ServicePrice.IsReadOnly = false;
                        EditServicePriceButton.Content = "Done";
                        UpdateStatus($"💡 Please enter price for new service: {newServiceType}", "#FFD700");
                    }
                }
            }

            ServiceType.IsVisible = false;
            ServiceTypeDropdown.IsVisible = true;
            EditServiceButton.Content = "Edit";
        }
    }

    private void EditServicePriceButtonClick(object? sender, RoutedEventArgs e)
    {
        if (!ServicePrice.IsReadOnly)
        {
            // Done clicked - just update UI state, don't save to database
            if (decimal.TryParse(ServicePrice.Text?.Trim(), out var newPrice) && newPrice > 0)
            {
                UpdateStatus($"✓ Service price set: ${newPrice:F2}. Will be saved when you click Save.", "#FFD700");
            }

            ServicePrice.IsReadOnly = true;
            EditServicePriceButton.Content = "Edit";
        }
        else
        {
            // Edit clicked
            decimal.TryParse(ServicePrice.Text?.Trim(), out _originalServicePrice);
            ServicePrice.IsReadOnly = false;
            EditServicePriceButton.Content = "Done";
            ServicePrice.Focus();
        }
    }

    private void CarOwnerNameTextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        if (_isUpdatingFromDropdown) return;

        var textBox = sender as TextBox;
        var text = textBox?.Text?.Trim() ?? "";

        if (!string.IsNullOrEmpty(text))
        {
            // Check if text matches any existing owner
            var matchingOwner = _carOwners.FirstOrDefault(o => o.Name.Equals(text, StringComparison.OrdinalIgnoreCase));
            if (matchingOwner != null)
            {
                _isUpdatingFromDropdown = true;
                CarOwnerPhone.Text = matchingOwner.Phone;
                _isUpdatingFromDropdown = false;
            }
        }
    }

    private void CarOwnerDropdownSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_isUpdatingFromDropdown) return;

        var comboBox = sender as ComboBox;
        var selectedItem = comboBox?.SelectedItem?.ToString();

        if (selectedItem == "-")
        {
            // Clear fields for new entry
            _isUpdatingFromDropdown = true;
            CarOwnerPhone.Text = "";
            _isUpdatingFromDropdown = false;
        }
        else if (!string.IsNullOrEmpty(selectedItem))
        {
            // Fill fields with selected owner data
            var selectedOwner = _carOwners.FirstOrDefault(o => o.Name == selectedItem);
            if (selectedOwner != null)
            {
                _isUpdatingFromDropdown = true;
                CarOwnerPhone.Text = selectedOwner.Phone;
                _isUpdatingFromDropdown = false;
            }
        }
    }

    private void PartNameTextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        if (_isUpdatingFromDropdown) return;

        var textBox = sender as TextBox;
        var text = textBox?.Text?.Trim() ?? "";

        if (!string.IsNullOrEmpty(text))
        {
            var matchingPart = _parts.FirstOrDefault(p => p.Part_name.Equals(text, StringComparison.OrdinalIgnoreCase));
            if (matchingPart != null)
            {
                _isUpdatingFromDropdown = true;
                PartPrice.Text = matchingPart.Part_price.ToString("F2");
                _isUpdatingFromDropdown = false;
            }
        }
    }

    private void PartDropdownSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_isUpdatingFromDropdown) return;

        var comboBox = sender as ComboBox;
        var selectedItem = comboBox?.SelectedItem?.ToString();

        if (selectedItem == "-")
        {
            _isUpdatingFromDropdown = true;
            PartPrice.Text = "";
            _isUpdatingFromDropdown = false;
        }
        else if (!string.IsNullOrEmpty(selectedItem))
        {
            var selectedPart = _parts.FirstOrDefault(p => p.Part_name == selectedItem);
            if (selectedPart != null)
            {
                _isUpdatingFromDropdown = true;
                PartPrice.Text = selectedPart.Part_price.ToString("F2");
                _isUpdatingFromDropdown = false;
            }
        }
    }

    private void ServiceTypeTextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        if (_isUpdatingFromDropdown) return;

        var textBox = sender as TextBox;
        var text = textBox?.Text?.Trim() ?? "";

        if (!string.IsNullOrEmpty(text))
        {
            var matchingService =
                _services.FirstOrDefault(s => s.Service_type.Equals(text, StringComparison.OrdinalIgnoreCase));
            if (matchingService != null)
            {
                _isUpdatingFromDropdown = true;
                ServicePrice.Text = matchingService.Service_price.ToString("F2");
                _isUpdatingFromDropdown = false;
            }
        }
    }

    private void ServiceDropdownSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_isUpdatingFromDropdown) return;

        var comboBox = sender as ComboBox;
        var selectedItem = comboBox?.SelectedItem?.ToString();

        if (selectedItem == "-")
        {
            _isUpdatingFromDropdown = true;
            ServicePrice.Text = "";
            _isUpdatingFromDropdown = false;
        }
        else if (!string.IsNullOrEmpty(selectedItem))
        {
            var selectedService = _services.FirstOrDefault(s => s.Service_type == selectedItem);
            if (selectedService != null)
            {
                _isUpdatingFromDropdown = true;
                ServicePrice.Text = selectedService.Service_price.ToString("F2");
                _isUpdatingFromDropdown = false;
            }
        }
    }

    private void CarBrandDropdownSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_isUpdatingFromDropdown) return;

        var comboBox = sender as ComboBox;
        var selectedBrand = comboBox?.SelectedItem?.ToString();

        if (!string.IsNullOrEmpty(selectedBrand) && selectedBrand != "-")
        {
            try
            {
                // Load models for the selected brand
                _carModels = _dbOps.LoadCarModelsByBrand(selectedBrand);

                // Populate model dropdown
                _isUpdatingFromDropdown = true;
                CarModelDropdown.Items.Clear();
                CarModelDropdown.Items.Add("-");

                foreach (var model in _carModels)
                {
                    if (!string.IsNullOrEmpty(model))
                    {
                        CarModelDropdown.Items.Add(model);
                    }
                }

                CarModelDropdown.SelectedIndex = 0;
                _isUpdatingFromDropdown = false;

                // If there's only one model for this brand, auto-select it
                if (_carModels.Count == 1)
                {
                    CarModelDropdown.SelectedIndex = 1; // Select the first model (index 1, since "-" is at index 0)
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Error loading models: {ex.Message}", "#FF6B6B");
            }
        }
        else
        {
            // Clear model dropdown when brand is not selected
            _isUpdatingFromDropdown = true;
            CarModelDropdown.Items.Clear();
            CarModelDropdown.Items.Add("-");
            CarModelDropdown.SelectedIndex = 0;
            _carModels.Clear();
            _isUpdatingFromDropdown = false;
        }
    }

    private async void SaveDataOnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            // Clear previous status
            UpdateStatus("", "#90EE90");

            // Validate inputs
            if (!ValidateInputs())
                return;

            // Get input values - handle both edit mode and dropdown mode
            var ownerName = GetOwnerName();
            var ownerPhone = CarOwnerPhone.Text?.Trim() ?? "";
            var carBrand = GetCarBrand();
            var carModel = GetCarModel();
            var partName = GetPartName();
            var partPrice = decimal.Parse(PartPrice.Text?.Trim() ?? "0");
            var serviceType = GetServiceType();
            var servicePrice = decimal.Parse(ServicePrice.Text?.Trim() ?? "0");

            UpdateStatus("Saving data...", "#FFD700");

            // Check for existing data and get IDs, or create new entries
            var ownerId = _dbOps.GetExistingCarOwnerId(ownerName, ownerPhone) ??
                          _dbOps.AddCarOwner(ownerName, ownerPhone);

            _lastCarId = _dbOps.GetExistingCarId(carBrand, carModel, ownerId) ??
                         _dbOps.AddCar(carBrand, carModel, ownerId);

            // Handle parts - check if exists, create new or update price if different
            var existingPartId = _dbOps.GetExistingPartId(partName);
            if (existingPartId.HasValue)
            {
                _lastPartId = existingPartId.Value;
                var existingPartPrice = _dbOps.GetPartPrice(existingPartId.Value);
                if (existingPartPrice.HasValue && existingPartPrice.Value != partPrice)
                {
                    _dbOps.UpdatePartPrice(existingPartId.Value, partPrice);
                }
            }
            else
            {
                _lastPartId = _dbOps.AddPart(partName, partPrice);
            }

            // Handle services - check if exists, create new or update price if different  
            var existingServiceId = _dbOps.GetExistingServiceId(serviceType);
            if (existingServiceId.HasValue)
            {
                _lastServiceId = existingServiceId.Value;
                var existingServicePrice = _dbOps.GetServicePrice(existingServiceId.Value);
                if (existingServicePrice.HasValue && existingServicePrice.Value != servicePrice)
                {
                    _dbOps.UpdateServicePrice(existingServiceId.Value, servicePrice);
                }
            }
            else
            {
                _lastServiceId = _dbOps.AddService(serviceType, servicePrice);
            }

            // Calculate total amount and create invoice
            var totalAmount = partPrice + servicePrice;
            _dbOps.AddInvoice(_lastCarId.Value, _lastServiceId.Value, _lastPartId.Value, DateTime.Now, totalAmount);

            UpdateStatus("✓ Data saved successfully!", "#90EE90");

            // Refresh dropdown data to show any newly created entities (but preserve current selections)
            await LoadAllDropdownData();
        }
        catch (Exception ex)
        {
            UpdateStatus($"❌ Error: {ex.Message}", "#FF6B6B");
        }
    }

    // Helper methods to get values from either text input or dropdown
    private string GetOwnerName()
    {
        if (_isOwnerNameEditMode && CarOwnerName.IsVisible)
            return CarOwnerName.Text?.Trim() ?? "";

        var selected = CarOwnerNameDropdown.SelectedItem?.ToString();
        return selected == "-" ? "" : selected ?? "";
    }

    private string GetPartName()
    {
        if (_isPartEditMode && PartName.IsVisible)
            return PartName.Text?.Trim() ?? "";

        var selected = PartNameDropdown.SelectedItem?.ToString();
        return selected == "-" ? "" : selected ?? "";
    }

    private string GetServiceType()
    {
        if (_isServiceEditMode && ServiceType.IsVisible)
            return ServiceType.Text?.Trim() ?? "";

        var selected = ServiceTypeDropdown.SelectedItem?.ToString();
        return selected == "-" ? "" : selected ?? "";
    }

    private string GetCarBrand()
    {
        if (_isBrandEditMode && CarBrand.IsVisible)
            return CarBrand.Text?.Trim() ?? "";

        var selected = CarBrandDropdown.SelectedItem?.ToString();
        return selected == "-" ? "" : selected ?? "";
    }

    private string GetCarModel()
    {
        if (_isModelEditMode && CarModel.IsVisible)
            return CarModel.Text?.Trim() ?? "";

        var selected = CarModelDropdown.SelectedItem?.ToString();
        return selected == "-" ? "" : selected ?? "";
    }

    private async void PrintInvoiceOnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_lastCarId == null || _lastServiceId == null || _lastPartId == null)
            {
                UpdateStatus("❌ No recent data to print. Please save data first.", "#FF6B6B");
                return;
            }

            UpdateStatus("Generating invoice...", "#FFD700");

            // Create invoice content
            var invoice = GenerateInvoiceContent();

            // Display invoice in search result area for now
            SearchResult.Text = invoice;

            UpdateStatus("✓ Invoice generated successfully!", "#90EE90");
        }
        catch (Exception ex)
        {
            UpdateStatus($"❌ Error generating invoice: {ex.Message}", "#FF6B6B");
        }
    }

    private async void SearchCarOnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var searchTerm = SearchCar.Text?.Trim();
            if (string.IsNullOrEmpty(searchTerm))
            {
                UpdateStatus("❌ Please enter an owner name to search.", "#FF6B6B");
                return;
            }

            UpdateStatus("Searching...", "#FFD700");

            var results = _dbOps.SearchCarsByOwnerName(searchTerm);

            if (results.Any())
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Found {results.Count} car(s):");
                sb.AppendLine(new string('-', 50));

                foreach (var car in results)
                {
                    sb.AppendLine($"Car ID: {car.Car_id}");
                    sb.AppendLine($"Brand: {car.Brand}");
                    sb.AppendLine($"Model: {car.Model}");
                    sb.AppendLine($"Owner: {car.OwnerName}");
                    sb.AppendLine($"Phone: {car.OwnerPhone}");
                    sb.AppendLine(new string('-', 30));
                }

                SearchResult.Text = sb.ToString();
                UpdateStatus($"✓ Found {results.Count} car(s)!", "#90EE90");
            }
            else
            {
                SearchResult.Text = "No cars found for the specified owner.";
                UpdateStatus("No results found.", "#FFD700");
            }
        }
        catch (Exception ex)
        {
            UpdateStatus($"❌ Search error: {ex.Message}", "#FF6B6B");
            SearchResult.Text = "";
        }
    }

    private bool ValidateInputs()
    {
        // Check required fields using helper methods
        var ownerName = GetOwnerName();
        if (string.IsNullOrWhiteSpace(ownerName))
        {
            UpdateStatus("❌ Owner name is required.", "#FF6B6B");
            if (_isOwnerNameEditMode) CarOwnerName.Focus();
            else CarOwnerNameDropdown.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(CarOwnerPhone.Text))
        {
            UpdateStatus("❌ Owner phone is required.", "#FF6B6B");
            CarOwnerPhone.Focus();
            return false;
        }

        var carBrand = GetCarBrand();
        if (string.IsNullOrWhiteSpace(carBrand))
        {
            UpdateStatus("❌ Car brand is required.", "#FF6B6B");
            if (_isBrandEditMode) CarBrand.Focus();
            else CarBrandDropdown.Focus();
            return false;
        }

        var carModel = GetCarModel();
        if (string.IsNullOrWhiteSpace(carModel))
        {
            UpdateStatus("❌ Car model is required.", "#FF6B6B");
            if (_isModelEditMode) CarModel.Focus();
            else CarModelDropdown.Focus();
            return false;
        }

        var partName = GetPartName();
        if (string.IsNullOrWhiteSpace(partName))
        {
            UpdateStatus("❌ Part name is required.", "#FF6B6B");
            if (_isPartEditMode) PartName.Focus();
            else PartNameDropdown.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(PartPrice.Text) || !decimal.TryParse(PartPrice.Text, out var partPrice) ||
            partPrice <= 0)
        {
            UpdateStatus("❌ Valid part price is required.", "#FF6B6B");
            PartPrice.Focus();
            return false;
        }

        var serviceType = GetServiceType();
        if (string.IsNullOrWhiteSpace(serviceType))
        {
            UpdateStatus("❌ Service type is required.", "#FF6B6B");
            if (_isServiceEditMode) ServiceType.Focus();
            else ServiceTypeDropdown.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(ServicePrice.Text) ||
            !decimal.TryParse(ServicePrice.Text, out var servicePrice) || servicePrice <= 0)
        {
            UpdateStatus("❌ Valid service price is required.", "#FF6B6B");
            ServicePrice.Focus();
            return false;
        }

        return true;
    }

    private void UpdateStatus(string message, string color)
    {
        Dispatcher.UIThread.Post(() =>
        {
            StatusText.Text = message;
            StatusText.Foreground = Avalonia.Media.Brush.Parse(color);
        });
    }

    private async System.Threading.Tasks.Task ClearFormAsync()
    {
        // Refresh dropdown data first to remove any temporarily added items
        await LoadAllDropdownData();

        Dispatcher.UIThread.Post(() =>
        {
            _isUpdatingFromDropdown = true;

            // Reset edit modes
            if (_isOwnerNameEditMode)
            {
                _isOwnerNameEditMode = false;
                CarOwnerName.IsVisible = false;
                CarOwnerNameDropdown.IsVisible = true;
                EditOwnerNameButton.Content = "Edit";
            }

            if (_isBrandEditMode)
            {
                _isBrandEditMode = false;
                CarBrand.IsVisible = false;
                CarBrandDropdown.IsVisible = true;
                EditBrandButton.Content = "Edit";
            }

            if (_isModelEditMode)
            {
                _isModelEditMode = false;
                CarModel.IsVisible = false;
                CarModelDropdown.IsVisible = true;
                EditModelButton.Content = "Edit";
            }

            if (_isPartEditMode)
            {
                _isPartEditMode = false;
                PartName.IsVisible = false;
                PartNameDropdown.IsVisible = true;
                EditPartButton.Content = "Edit";
            }

            if (_isServiceEditMode)
            {
                _isServiceEditMode = false;
                ServiceType.IsVisible = false;
                ServiceTypeDropdown.IsVisible = true;
                EditServiceButton.Content = "Edit";
            }

            // Clear all fields
            CarOwnerName.Text = "";
            CarOwnerPhone.Text = "";
            CarOwnerPhone.IsReadOnly = true;
            EditPhoneButton.Content = "Edit";

            CarBrand.Text = "";
            CarModel.Text = "";

            PartName.Text = "";
            PartPrice.Text = "";
            PartPrice.IsReadOnly = true;
            EditPartPriceButton.Content = "Edit";

            ServiceType.Text = "";
            ServicePrice.Text = "";
            ServicePrice.IsReadOnly = true;
            EditServicePriceButton.Content = "Edit";

            // Refresh and reset all dropdowns to remove temporarily added items
            PopulateDropdowns();

            // Clear search results
            SearchResult.Text = "";

            _isUpdatingFromDropdown = false;

            // Focus on first field for next entry
            CarOwnerNameDropdown.Focus();
        });
    }

    private string GenerateInvoiceContent()
    {
        var partPrice = decimal.Parse(PartPrice.Text?.Trim() ?? "0");
        var servicePrice = decimal.Parse(ServicePrice.Text?.Trim() ?? "0");
        var totalAmount = partPrice + servicePrice;

        var sb = new StringBuilder();
        sb.AppendLine("═══════════════════════════════════════════");
        sb.AppendLine("              CAR WORKSHOP INVOICE");
        sb.AppendLine("═══════════════════════════════════════════");
        sb.AppendLine($"Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"Invoice ID: INV-{DateTime.Now:yyyyMMdd}-{_lastCarId}");
        sb.AppendLine();
        sb.AppendLine("CUSTOMER INFORMATION:");
        sb.AppendLine($"Name: {GetOwnerName()}");
        sb.AppendLine($"Phone: {CarOwnerPhone.Text}");
        sb.AppendLine();
        sb.AppendLine("VEHICLE INFORMATION:");
        sb.AppendLine($"Brand: {GetCarBrand()}");
        sb.AppendLine($"Model: {GetCarModel()}");
        sb.AppendLine($"Car ID: {_lastCarId}");
        sb.AppendLine();
        sb.AppendLine("SERVICE DETAILS:");
        sb.AppendLine($"Service: {GetServiceType()}");
        sb.AppendLine($"Service Price: ${servicePrice:F2}");
        sb.AppendLine();
        sb.AppendLine("PARTS USED:");
        sb.AppendLine($"Part: {GetPartName()}");
        sb.AppendLine($"Part Price: ${partPrice:F2}");
        sb.AppendLine();
        sb.AppendLine("═══════════════════════════════════════════");
        sb.AppendLine($"TOTAL AMOUNT: ${totalAmount:F2}");
        sb.AppendLine("═══════════════════════════════════════════");
        sb.AppendLine();
        sb.AppendLine("Thank you for choosing our workshop!");

        return sb.ToString();
    }

    private async void ClearButtonOnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            await ClearFormAsync();
            UpdateStatus("✓ Form cleared!", "#90EE90");
        }
        catch (Exception ex)
        {
            UpdateStatus($"❌ Error clearing form: {ex.Message}", "#FF6B6B");
        }
    }
}