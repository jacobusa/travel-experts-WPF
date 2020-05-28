using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
    /// Interaction logic for UserControlSupplierAdd.xaml
    /// </summary>
    public partial class UserControlSupplierAdd : UserControl
    {
        public UserControlSupplierAdd()
        {
            InitializeComponent();
        }
        public async void Window_Loaded(object sender, RoutedEventArgs e)
        {

            var products = await GetProducts("https://travelexperts.azurewebsites.net/api/ProductsAPI");
            var suppliers = await GetSuppliers("https://travelexperts.azurewebsites.net/api/SuppliersAPI");
            productsList = products;
            lastEntered = suppliers.Last();
            allListView.ItemsSource = productsList;

        }
        private List<Products> productsList { get; set; }
        private Suppliers lastEntered { get; set; }
        public async void addSubmit_ClickAsync(object sender, EventArgs e)
        {
            // Clear Status text when submit button is clicked
            statusTextBlock.Text = "";

            string supplierName = nameTextbox.Text;

            // Validation: Name is required
            if (nameTextbox.Text == "")
            {
                statusTextBlock.Foreground = Brushes.DarkOrange;
                statusTextBlock.Text = "Missing Fields!!";
                submitButton.Background = Brushes.DarkOrange;
                return;
            }

            // Create new Supplier object from input field to post into database
            var supplier = new Suppliers
            {
                SupName = nameTextbox.Text
            };

            // Get all selected products from ListView
            List<Products> productsSelectedList = new List<Products>();
            var selectedProducts = allListView.SelectedItems;
            foreach (var item in selectedProducts)
            {
                productsSelectedList.Add((Products)item);
            }


            // Make Post API call to database to insert Supplier
            var task = await PostSupplierAsync("https://travelexperts.azurewebsites.net/api/SuppliersAPI", supplier);
            var items = task;
            if (items != null)
            {
                // Add Products into ProductsSuppliers Table with newly added Supplier
                int newSupplierID = items.SupplierId;
                var productTask = await PostProductsForSupplierAsync(productsSelectedList, newSupplierID);
                if (!productTask)
                {
                    statusTextBlock.Foreground = Brushes.Red;
                    statusTextBlock.Text = "Something Went Wrong!";
                    submitButton.Background = Brushes.Red;
                    return;
                }

                // Successful insert of Supplier object into database
                statusTextBlock.Foreground = Brushes.Green;
                statusTextBlock.Text = "Supplier Created!";
                submitButton.Background = Brushes.Green;
                nameTextbox.Text = "";
            }
            else
            {
                // Post Request failed
                statusTextBlock.Foreground = Brushes.Red;
                statusTextBlock.Text = "Unable to Add Supplier!";
                submitButton.Background = Brushes.Red;
            }

        }

        private async Task<bool> PostProductsForSupplierAsync(List<Products> productsSelectedList, int newSupplierID)
        {
            // Post API call to insert passed in Suppliers object into database. Return status code for verification
            HttpClient client = new System.Net.Http.HttpClient();

            // For every Product, post into ProductsSuppliers using Product and Supplier
            List<HttpStatusCode> codes = new List<HttpStatusCode>();
            foreach (var product in productsSelectedList)
            {
                var productSupplier = new ProductsSuppliers { ProductId = product.ProductId, SupplierId = newSupplierID };
                var content = JsonConvert.SerializeObject(productSupplier);
                var httpContent = new StringContent(content, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("https://travelexperts.azurewebsites.net/api/ProductsSuppliersAPI", httpContent);
                codes.Add(response.StatusCode);
            }

            bool success = true;
            // Loop through responses, return false if one failed
            foreach (var code in codes)
            {
                if (code != HttpStatusCode.Created)
                {
                    success = false;
                }
            }
            return success;
        }

        private async Task<Suppliers> PostSupplierAsync(string path, Suppliers supplier)
        {
            // Post API call to insert passed in Suppliers object into database. Return status code for verification
            HttpClient client = new System.Net.Http.HttpClient();
            supplier.SupplierId = lastEntered.SupplierId + 1;
            var content = JsonConvert.SerializeObject(supplier);
            var httpContent = new StringContent(content, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(path, httpContent);
            var returnContent = JsonConvert.DeserializeObject<Suppliers>(await response.Content.ReadAsStringAsync());
            return returnContent;
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
        private async Task<List<Suppliers>> GetSuppliers(string path)
        {
            // Get List of Suppliers Objects from Get Request
            HttpClient client = new System.Net.Http.HttpClient();
            List<Suppliers> supps = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                supps = JsonConvert.DeserializeObject<List<Suppliers>>(await response.Content.ReadAsStringAsync());
            }
            return supps;
        }
    }
}
