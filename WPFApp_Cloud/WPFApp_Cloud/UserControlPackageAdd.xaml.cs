using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFApp_Cloud
{
    /// <summary>
    /// Interaction logic for UserControlAddPackage.xaml
    /// </summary>
    public partial class UserControlPackageAdd : UserControl
    {
        public UserControlPackageAdd()
        {
            InitializeComponent();
        }
        public async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Make API call to get List of Products
            var products = await GetProducts("https://travelexperts.azurewebsites.net/api/ProductsAPI");

            // Collect Products list in productList Class property
            productsList = products;

            // Bind ComboBox to Products List
            myListView.ItemsSource = products;

        }
        private List<Products> productsList { get; set; }
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
        public async void addSubmit_ClickAsync(object sender, EventArgs e)
        {
            // clera Status Text every time add button is clicked
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
            if (nameTextbox.Text=="" || startDate.SelectedDate == null|| endDate.SelectedDate==null || desc.Text=="" || costTextbox.Text=="" || commissionTextbox.Text=="")
            {
                statusTextBlock.Foreground = Brushes.DarkOrange;
                statusTextBlock.Text = "Missing Fields!!";
                submitButton.Background = Brushes.DarkOrange;
                return;
            }

            // Start Date cannot be after End Date
            if (startDate.SelectedDate > endDate.SelectedDate)
            {
                statusTextBlock.Foreground = Brushes.DarkOrange;
                statusTextBlock.Text = "Start Date cannot be later than End Date!!";
                submitButton.Background = Brushes.DarkOrange;
                return;
            }

            // Cost cannot be less than Commission
            if (decimal.Parse(costTextbox.Text) < decimal.Parse(commissionTextbox.Text))
            {
                statusTextBlock.Foreground = Brushes.DarkOrange;
                statusTextBlock.Text = "Base Price cannot be less than Commmission!!";
                submitButton.Background = Brushes.DarkOrange;
                return;
            }

            // Create new Packages object from input fields for API call
            var package = new Packages
            {
                PkgName = nameTextbox.Text,
                PkgStartDate = startDate.SelectedDate,
                PkgEndDate = endDate.SelectedDate,
                PkgDesc = desc.Text,
                PkgBasePrice = (decimal)double.Parse(costTextbox.Text),
                PkgImage = image.Text,
                PkgAgencyCommission = (decimal?)double.Parse(commissionTextbox.Text),
            };

            // Get all selected products from ListView
            List <Products > productsSelectedList = new List<Products>();
            var selectedProducts = myListView.SelectedItems;
            foreach (var item in selectedProducts)
            {
                productsSelectedList.Add((Products)item);
            }

            // Make POST API call and return status code
            var responseMessage = await PostPackageAsync("https://travelexperts.azurewebsites.net/api/PackagesAPI", package);
            if (responseMessage.StatusCode == HttpStatusCode.Created)
            {
                // Make POST API call to Packages_Products_Suppliers table and return bool
                int newPackageID = JsonConvert.DeserializeObject<Packages>(await responseMessage.Content.ReadAsStringAsync()).PackageId;
                var productTask = await PostProductsForPackageAsync(productsSelectedList, newPackageID);
                if (!productTask)
                {
                    statusTextBlock.Foreground = Brushes.Red;
                    statusTextBlock.Text = "Something Went Wrong!";
                    submitButton.Background = Brushes.Red;
                    return;
                }
                // Status code shows object created
                statusTextBlock.Foreground = Brushes.Green;
                statusTextBlock.Text = "Package Created!";
                submitButton.Background = Brushes.Green;
                nameTextbox.Text = "";
                startDate.Text = "";
                endDate.Text = "";
                desc.Text = "";
                image.Text = "";
                costTextbox.Text = "";
                commissionTextbox.Text = "";
            }
            else
            {
                // Status code shows anything other than object created
                statusTextBlock.Foreground = Brushes.Red;
                statusTextBlock.Text = "Unable to Add Package!";
                submitButton.Background = Brushes.Red;
            }

        }
        private async Task<HttpResponseMessage> PostPackageAsync(string path, Packages package)
        {
            // Logic needed for Post API Call
            HttpClient client = new System.Net.Http.HttpClient();

            // convert Packages object passed in to JSON for API call
            var content = JsonConvert.SerializeObject(package);
            var httpContent = new StringContent(content, Encoding.UTF8, "application/json");

            // Make async call and send back content
            HttpResponseMessage response = await client.PostAsync(path,httpContent);
            return response;
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

    }
}
