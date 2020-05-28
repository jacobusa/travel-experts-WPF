using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Runtime.CompilerServices;
using System.Net;
using System.Windows.Controls;

namespace WPFApp_Cloud
{
    /// <summary>
    /// Interaction logic for UserControlPackageEdit.xaml
    /// </summary>
    public partial class UserControlPackageEdit : UserControl
    {
        public UserControlPackageEdit()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Used for storing List of Packages from Get API Call
        private List<Packages> packageList { get; set; }

        public async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Clear fields when Control is Loaded
            PackageStart = null;
            PackageEnd = null;
            costTextbox.Text = "";

            // Make API call to collect List of Packages
            var packages = await GetPackages("https://travelexperts.azurewebsites.net/api/PackagesAPI");
            var products = await GetProducts("https://travelexperts.azurewebsites.net/api/ProductsAPI");
            productsList = products;
            allListView.ItemsSource = productsList;

            // Store List of Packages in Class Property
            packageList = packages;

            // Pass in Packages List to ComboBox for selection
            packagesComboBox.ItemsSource = packages;

        }
        private List<Products> productsList { get; set; }

        public async void ComboBox_Changed(object sender, EventArgs e)
        {
            // If API Get call fails, return. Otherwise populate fields with returned data
            try
            {
                statusTextBlock.Text = "";
                Packages selectedPackage = (Packages)packagesComboBox.SelectedItem;
                int packageID = packageList.Find(p => p.PackageId == selectedPackage.PackageId).PackageId;
                var package = await GetPackage("https://travelexperts.azurewebsites.net/api/PackagesAPI/" + packageID.ToString());
                var productsForPackage = await GetProductsForPackage(packageID);
                filteredListView.ItemsSource = productsForPackage;
                nameTextbox.Text = package.PkgName;
                desc.Text = package.PkgDesc;
                startDate.SelectedDate = package.PkgStartDate;
                endDate.SelectedDate = package.PkgEndDate;
                costTextbox.Text = package.PkgBasePrice.ToString();
                image.Text = package.PkgImage;
                commissionTextbox.Text = package.PkgAgencyCommission.ToString();
            }
            catch (Exception)
            {
                return;
            }

        }
        private void costTextbox_TextChanged(object sender, EventArgs e)
        {
            // This textbox should only include numbers

            // Return if nothing is entered
            if (costTextbox.Text == "")
            {
                return;
            }

            // Check if text entered is number, and format as number if able to Parse
            if (double.TryParse(costTextbox.Text, out double number))
            {
                // number is entered
                costTextbox.Text = string.Format("{0:#.00}", number);
                costTextbox.CaretIndex = costTextbox.Text.Length - 3;
            }
            else
            {
                // letter is entered
                costTextbox.Text = "Only Numbers Allowed";
                costTextbox.Focus();
                costTextbox.SelectAll();
            }

        }

        private void commissionTextbox_TextChanged(object sender, EventArgs e)
        {

            // This textbox should only include numbers

            // Return if nothing is entered
            if (commissionTextbox.Text == "")
            {
                return;
            }

            // Check if text entered is number, and format as number if able to Parse
            if (double.TryParse(commissionTextbox.Text, out double number))
            {
                // number is entered
                commissionTextbox.Text = string.Format("{0:#.00}", number);
                commissionTextbox.CaretIndex = commissionTextbox.Text.Length - 3;
            }
            else
            {
                // letter is entered
                commissionTextbox.Text = "Only Numbers Allowed";
                commissionTextbox.Focus();
                commissionTextbox.SelectAll();
            }

        }
        public async void deleteSubmit_ClickAsync(object sender, EventArgs e) 
        {
            // User Experience Are You Sure dialog
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);


            if (messageBoxResult == MessageBoxResult.Yes)
            {
                // User wants to delete

                // get selected package item from ComboBox and make Delete API call
                Packages selectedPackage = (Packages)packagesComboBox.SelectedItem;
                int packageID = packageList.Find(p => p.PackageId == selectedPackage.PackageId).PackageId;
                try
                {
                    bool deleteConstraint = await DeletePackagesProductsSuppliers(packageID);
                    if (!deleteConstraint)
                    {
                        statusTextBlock.Foreground = Brushes.Red;
                        statusTextBlock.Text = "Something went wrong. Please try Again!";
                        return;
                    }
                    var returnPackage = await DeletePackageAsync("https://travelexperts.azurewebsites.net/api/PackagesAPI", packageID);
                    if (returnPackage != null)
                    {
                        // Delete package from packageList
                        packageList.Remove(returnPackage);
                        packagesComboBox.ItemsSource = packageList;
                        packagesComboBox.SelectedItem = packageList[0];

                        // show success and clear
                        statusTextBlock.Foreground = Brushes.Green;
                        statusTextBlock.Text = $"Package '{returnPackage.PkgName}' Deleted!";
                        nameTextbox.Text = "";
                        startDate.SelectedDate = null;
                        endDate.SelectedDate = null;
                        image.Text = "";
                        desc.Text = "";
                        costTextbox.Text = "";
                        commissionTextbox.Text = "";
                        packagesComboBox.ItemsSource = packageList;
                    }
                    else
                    {
                        // API Delete call failed
                        statusTextBlock.Foreground = Brushes.Red;
                        statusTextBlock.Text = "Unable to Delete Package!";
                    }
            }
                catch (Exception)
            {

                statusTextBlock.Foreground = Brushes.Red;
                statusTextBlock.Text = "Error Occured. Please Try Again!";
            }


        }
            else
            {
                // User changes their mind on deletion
                return;
            }
        }
        public async void editSubmit_ClickAsync(object sender, EventArgs e)
        {
            Packages selectedPackage = (Packages)packagesComboBox.SelectedItem;

            // If user did not change anything, don't edit, just return
            if (selectedPackage.PkgName == nameTextbox.Text && selectedPackage.PkgStartDate == startDate.SelectedDate && selectedPackage.PkgEndDate == endDate.SelectedDate && selectedPackage.PkgDesc == desc.Text && selectedPackage.PkgBasePrice.ToString() == costTextbox.Text && selectedPackage.PkgAgencyCommission.ToString() == commissionTextbox.Text)
            {
                return;
            }

            // Clear Status Text
            statusTextBlock.Text = "";

            // Image Text is optional, but is required for database, so place dummy text
            if (image.Text == "")
            {
                image.Text = "None Provided";
            }

            string packageName = nameTextbox.Text;
            DateTime? packageStart = startDate.SelectedDate;
            DateTime? packageEnd = endDate.SelectedDate;
            string packageDesc = desc.Text;
            string packagePrice = costTextbox.Text;
            string packageImage = image.Text;
            string packageCommission = commissionTextbox.Text;

            // If any required fields are empty, show error and return
            if (nameTextbox.Text == "" || startDate.SelectedDate == null || endDate.SelectedDate == null || desc.Text == "" || costTextbox.Text == "" || commissionTextbox.Text == "")
            {
                statusTextBlock.Foreground = Brushes.DarkOrange;
                statusTextBlock.Text = "Missing Fields!!";
                editButton.Background = Brushes.DarkOrange;
                return;
            }

            // Start Date cannot be after End Date
            if (startDate.SelectedDate > endDate.SelectedDate)
            {
                statusTextBlock.Foreground = Brushes.DarkOrange;
                statusTextBlock.Text = "Start Date cannot be later than End Date!!";
                editButton.Background = Brushes.DarkOrange;
                return;
            }

            // Cost cannot be less than Commission
            if (decimal.Parse(costTextbox.Text) < decimal.Parse(commissionTextbox.Text))
            {
                statusTextBlock.Foreground = Brushes.DarkOrange;
                statusTextBlock.Text = "Base Price cannot be less than Commission!!";
                editButton.Background = Brushes.DarkOrange;
                return;
            }

            // Create new Packages object from input fields for API call
            var package = new Packages
            {
                PackageId = selectedPackage.PackageId,
                PkgName = nameTextbox.Text,
                PkgStartDate = startDate.SelectedDate,
                PkgEndDate = endDate.SelectedDate,
                PkgDesc = desc.Text,
                PkgBasePrice = (decimal)double.Parse(costTextbox.Text),
                PkgImage = image.Text,
                PkgAgencyCommission = (decimal?)double.Parse(commissionTextbox.Text),
            };
            // Get all selected products from ListView
            List<Products> productsSelectedList = new List<Products>();
            var selectedProducts = allListView.SelectedItems;
            foreach (var item in selectedProducts)
            {
                productsSelectedList.Add((Products)item);
            }

            // Make PUT API calls and return status code
            var statusCode = await PutProductsForPackage(productsSelectedList, package.PackageId);
            var task = await PutPackageAsync("https://travelexperts.azurewebsites.net/api/PackagesAPI", package);
            if (task == HttpStatusCode.NoContent)
            {
                // Update Package in packageList
                int oldPackageIndex = packageList.FindIndex(p => p.PackageId==package.PackageId);
                packageList[oldPackageIndex] = package;
                packagesComboBox.SelectedItem = package;
                packagesComboBox.ItemsSource = packageList;

                // Set Edited Message and Clear
                statusTextBlock.Foreground = Brushes.Green;
                statusTextBlock.Text = "Package Edited!";
                editButton.Background = Brushes.Green;
                nameTextbox.Text = "";
                startDate.SelectedDate = null;
                endDate.SelectedDate = null;
                image.Text = "";
                desc.Text = "";
                costTextbox.Text = "";
                commissionTextbox.Text = "";
            }
            else
            {
                // Put API call failed
                statusTextBlock.Foreground = Brushes.Red;
                statusTextBlock.Text = "Unable to Edit Package!";
            }

        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string PackageName { get { return _packageName; } set { _packageName = value; OnPropertyChanged(); } }
        private string _packageName;
        public string PackageDesc { get { return _packageDesc; } set { _packageDesc = value; OnPropertyChanged(); } }
        private string _packageDesc;
        public DateTime? PackageStart { get { return _packageStart; } set { _packageStart = value; OnPropertyChanged(); } }
        private DateTime? _packageStart;
        public DateTime? PackageEnd { get { return _packageEnd; } set { _packageEnd = value; OnPropertyChanged(); } }
        private DateTime? _packageEnd;
        public decimal PackagePrice { get { return _packagePrice; } set { _packagePrice = value; OnPropertyChanged(); } }
        private decimal _packagePrice;
        public string PackageImage { get { return _packageImage; } set { _packageImage = value; OnPropertyChanged(); } }
        private string _packageImage;
        public decimal? PackageCommission { get { return _packageCommission; } set { _packageCommission = value; OnPropertyChanged(); } }
        private decimal? _packageCommission;

        private async Task<Packages> GetPackage(string path)
        {
            // Instantiate HTTP Client
            HttpClient client = new System.Net.Http.HttpClient();

            // placeholder for returned Packages object
            Packages pkg = null;

            // Make call
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                pkg = JsonConvert.DeserializeObject<Packages>(await response.Content.ReadAsStringAsync());
            }
            // return packages object
            return pkg;
        }
        private async Task<List<Packages>> GetPackages(string path)
        {
            // Instantiate HTTP Client
            HttpClient client = new System.Net.Http.HttpClient();

            // placeholder for returned Packages object List
            List<Packages> pkgs = null;

            // Make call
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                pkgs = JsonConvert.DeserializeObject<List<Packages>>(await response.Content.ReadAsStringAsync());
            }
            // return packages object List
            return pkgs;
        }
        private async Task<List<Products>> GetProducts(string path)
        {
            // Get List of Products Objects from Get Request, path does not include ProductsID
            HttpClient client = new System.Net.Http.HttpClient();
            List<Products> pdts = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                pdts = JsonConvert.DeserializeObject<List<Products>>(await response.Content.ReadAsStringAsync());
            }
            return pdts;
        }
        private async Task<List<Products>> GetProductsForPackage(int packageID)
        {
            HttpClient client = new System.Net.Http.HttpClient();

            // Get List of Packages_Products_Suppliers Objects
            List<PackagesProductsSuppliers> PPSFull = null;
            HttpResponseMessage response = await client.GetAsync("https://travelexperts.azurewebsites.net/api/PackagesProductsSuppliersAPI");
            if (response.IsSuccessStatusCode)
            PPSFull = JsonConvert.DeserializeObject<List<PackagesProductsSuppliers>>(await response.Content.ReadAsStringAsync());

            // Filter Packages_Products_Suppliers list by packageID
            var PPSFiltered = PPSFull.FindAll(pps => pps.PackageId == packageID);

            // Get List of Products_Suppliers Objects
            List<ProductsSuppliers> productsSuppliersFull = null;
            HttpResponseMessage responseFromProductsSuppliers = await client.GetAsync("https://travelexperts.azurewebsites.net/api/ProductsSuppliersAPI");
            productsSuppliersFull = JsonConvert.DeserializeObject<List<ProductsSuppliers>>(await responseFromProductsSuppliers.Content.ReadAsStringAsync());

            // Filter ProductsSuppliers List to include only the ones which are associated with the package
            List<ProductsSuppliers> productsSuppliersFiltered = new List<ProductsSuppliers>();
            foreach (var PackProdSup in PPSFiltered)
            {
                productsSuppliersFiltered.Add(productsSuppliersFull.Find(ps => ps.ProductSupplierId == PackProdSup.ProductSupplierId));
            }

            // Get List of Products
            List<Products> productsFull = null;
            HttpResponseMessage responseFromProducts = await client.GetAsync("https://travelexperts.azurewebsites.net/api/ProductsAPI");
            productsFull = JsonConvert.DeserializeObject<List<Products>>(await responseFromProducts.Content.ReadAsStringAsync());

            // Filter Products List to include only the ones which are associated with the ProductSuppliers for the package
            List<Products> productsFiltered = new List<Products>();
            foreach (var ProdSup in productsSuppliersFiltered)
            {
                productsFiltered.Add(productsFull.Find(ps => ps.ProductId == ProdSup.ProductId));
            }
            return productsFiltered;
        }

        private async Task<HttpStatusCode> PutPackageAsync(string path, Packages package)
        {
            // Instantiate HTTP Client
            HttpClient client = new System.Net.Http.HttpClient();

            // Convert Packages object to JSON for API Call
            var content = JsonConvert.SerializeObject(package);
            var httpContent = new StringContent(content, Encoding.UTF8, "application/json");

            // make Put Call to specific ID corresponding to Packages object passed in 
            HttpResponseMessage response = await client.PutAsync(path + "/" + package.PackageId, httpContent);

            // return Status Code
            return response.StatusCode;
        }
        private async Task<Packages> DeletePackageAsync(string path, int packageID)
        {
            // Instantiate HTTP Client
            HttpClient client = new System.Net.Http.HttpClient();

            // make Delete Call to specific ID corresponding to Packages object passed in 
            HttpResponseMessage response = await client.DeleteAsync(path + "/" + packageID);

            // collect return package and return
            var returnPackage = JsonConvert.DeserializeObject<Packages>(await response.Content.ReadAsStringAsync());
            return returnPackage;
        }
        private async Task<bool> DeletePackagesProductsSuppliers(int packageID)
        {
            // Instantiate HTTP Client
            HttpClient client = new System.Net.Http.HttpClient();

            // Get All PackagesProductsSuppliers
            List<PackagesProductsSuppliers> PPS = null;
            HttpResponseMessage response = await client.GetAsync("https://travelexperts.azurewebsites.net/api/PackagesProductsSuppliersAPI");
            PPS = JsonConvert.DeserializeObject<List<PackagesProductsSuppliers>>(await response.Content.ReadAsStringAsync());

            // Filter PackagesProductsSuppliers list to get only those object whose packageID == input packageID
            List<PackagesProductsSuppliers> filteredPPS = null;
            filteredPPS = PPS.FindAll(pps => pps.PackageId == packageID);

            // for each PPS in filtered List, make delete request for that package ID
            var codes = new List<HttpStatusCode>();
            foreach (var packProdSupp in filteredPPS)
            {
                HttpResponseMessage responsePackProdSupp = await client.DeleteAsync($"https://travelexperts.azurewebsites.net/api/PackagesProductsSuppliersAPI/{packProdSupp.PackageId},{packProdSupp.ProductSupplierId}");
                codes.Add(responsePackProdSupp.StatusCode);
            }

            // return true if all succeeded, false if any failed
            bool success = true;
            foreach (var code in codes)
            {
                if (code != HttpStatusCode.Accepted && code == HttpStatusCode.OK && code == HttpStatusCode.NoContent)
                {
                    success = false;
                }
            }
            return success;
        }
        private async Task<bool> PostProductsForPackageAsync(List<Products> products, int packageID)
        {
            HttpClient client = new System.Net.Http.HttpClient();

            // Get All ProductsSuppliers into List
            List<ProductsSuppliers> prodsSuppsListFull;
            HttpResponseMessage response = await client.GetAsync("https://travelexperts.azurewebsites.net/api/ProductsSuppliersAPI");
            prodsSuppsListFull = JsonConvert.DeserializeObject<List<ProductsSuppliers>>(await response.Content.ReadAsStringAsync());

            // Get all ProductsSuppliers Id's for products in the list
            List<int> inputProductIds = new List<int>();
            foreach (var item in products)
            {
                inputProductIds.Add(item.ProductId);
            }

            // Filter ProductsSuppliers List for only products that are in input products
            var PerProductSupplier = new List<ProductsSuppliers>();
            List<ProductsSuppliers> prodsSuppsListFiltered = new List<ProductsSuppliers>();

            // Assign the first ProductSupplierId to the Product which package uses
            foreach (int productid in inputProductIds)
            {
                prodsSuppsListFiltered.Add(prodsSuppsListFull.Find(ps => ps.ProductId == productid));
            }


            // Got filtered list of productSupplierIds, now post this list 1 by 1 to Packages_Products_Suppliers
            List<HttpStatusCode> codes = new List<HttpStatusCode>();
            foreach (var productSupplier in prodsSuppsListFiltered)
            {
                var newPackageProductSupplier = new PackagesProductsSuppliers { PackageId = packageID, ProductSupplierId = productSupplier.ProductSupplierId };
                var content = JsonConvert.SerializeObject(newPackageProductSupplier);
                var httpContent = new StringContent(content, Encoding.UTF8, "application/json");
                HttpResponseMessage responseFromPost = await client.PostAsync("https://travelexperts.azurewebsites.net/api/PackagesProductsSuppliersAPI", httpContent);
                codes.Add(responseFromPost.StatusCode);
            }

            // return true if all status codes are OK otherwise return false
            bool IsGoodRequest = true;
            foreach (HttpStatusCode code in codes)
            {
                if (code != HttpStatusCode.Created)
                {
                    IsGoodRequest = false;
                }
            }
            return IsGoodRequest;


        }
        private async Task<bool> PutProductsForPackage(List<Products> products, int packageID)
        {
            var deleteReturn = await DeletePackagesProductsSuppliers(packageID);
            var postReturn = await PostProductsForPackageAsync(products, packageID);
            if (postReturn && deleteReturn)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
