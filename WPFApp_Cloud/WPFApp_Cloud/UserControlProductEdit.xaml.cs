using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for UserControlProductEdit.xaml
    /// </summary>
    public partial class UserControlProductEdit : UserControl
    {
        public UserControlProductEdit()
        {
            InitializeComponent();
        }
        public async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Make API call to get List of Products
            var products = await GetProducts("https://travelexperts.azurewebsites.net/api/ProductsAPI");

            // Collect Products list in productList Class property
            productList = products;

            // Bind ComboBox to Products List
            productsComboBox.ItemsSource = products;

        }

        // Used for storing Products List from API call
        private List<Products> productList { get; set; }
        public async void ComboBox_Changed(object sender, EventArgs e)
        {
            // Try to display selected Products Object into fields. Return if failed
            try
            {
                statusTextBlock.Text = "";
                Products selectedProduct = (Products)productsComboBox.SelectedItem;
                int productID = productList.Find(p => p.ProductId == selectedProduct.ProductId).ProductId;
                var product = await GetProduct("https://travelexperts.azurewebsites.net/api/ProductsAPI/" + productID.ToString());
                nameTextbox.Text = product.ProdName;
            }
            catch (Exception)
            {
                return;
            }

        }
        public async void deleteSubmit_ClickAsync(object sender, EventArgs e)
        {

            // User Experience Are You Sure dialog
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                try
                {
                    // Try to delete Product. ComboBox selection might throw exception

                    // User is sure they want to delete object

                    // Get selected products object from ComboBox
                    Products selectedProduct = (Products)productsComboBox.SelectedItem;

                    // get ProductID from object
                    int productID = productList.Find(p => p.ProdName == selectedProduct.ProdName).ProductId;

                    // Make Delete API call to database
                    var returnProduct = await DeleteProductAsync("https://travelexperts.azurewebsites.net/api/ProductsAPI", productID);
                    if (returnProduct != null)
                    {
                        // Delete call succeeded
                        statusTextBlock.Foreground = Brushes.Green;
                        statusTextBlock.Text = $"Product '{returnProduct.ProdName}' Deleted!";
                        nameTextbox.Text = "";
                    }
                    else
                    {
                        // Delete call failed
                        statusTextBlock.Foreground = Brushes.Red;
                        statusTextBlock.Text = "Unable to Delete Product!";
                    }
                }
                catch (Exception)
                {
                    // Delete call failed
                    statusTextBlock.Foreground = Brushes.Red;
                    statusTextBlock.Text = "Unable to Delete Product!";
                }

            }
            else
            {
                // User changed their mind
                return;
            }
        }
        public async void editSubmit_ClickAsync(object sender, EventArgs e)
        {
            // Get current Products object from ComboBox Selection
            Products selectedProduct = (Products)productsComboBox.SelectedItem;

            // User did not change anything so return
            if (selectedProduct.ProdName == nameTextbox.Text)
            {
                return;
            }

            // Clear Status Text
            statusTextBlock.Text = "";

            string productName = nameTextbox.Text;

            // Cannot have empty name field
            if (nameTextbox.Text == "")
            {
                statusTextBlock.Foreground = Brushes.DarkOrange;
                statusTextBlock.Text = "Missing Fields!!";
                editButton.Background = Brushes.DarkOrange;
                return;
            }

            // Create new Products Object from input field and ComboBox to update in database
            var product = new Products
            {
                ProductId = selectedProduct.ProductId,
                ProdName = nameTextbox.Text,
            };


            // Make Put API call with created Products object
            var task = await PutProductAsync("https://travelexperts.azurewebsites.net/api/ProductsAPI", product);
            if (task == HttpStatusCode.NoContent)
            {
                // Update Succeeded
                // Update Product in productList
                int oldProductIndex = productList.FindIndex(p => p.ProductId == product.ProductId);
                productList[oldProductIndex] = product;
                productsComboBox.SelectedItem = product;

                // Set Edited Message and Clear
                statusTextBlock.Foreground = Brushes.Green;
                statusTextBlock.Text = "Product Edited!";
                nameTextbox.Text = "";
            }
            else
            {
                // Update Failed
                statusTextBlock.Foreground = Brushes.Red;
                statusTextBlock.Text = "Unable to Edit Product!";
            }

        }
        private async Task<Products> GetProduct(string path)
        {
            // Get Products object from Get Request, path includes ProductsId
            HttpClient client = new System.Net.Http.HttpClient();
            Products pdts = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                pdts = JsonConvert.DeserializeObject<Products>(await response.Content.ReadAsStringAsync());
            }
            return pdts;
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
        private async Task<HttpStatusCode> PutProductAsync(string path, Products product)
        {
            // Update Product Objects in PUT Request, path includes ProductsID
            HttpClient client = new System.Net.Http.HttpClient();
            var content = JsonConvert.SerializeObject(product);
            var httpContent = new StringContent(content, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(path + "/" + product.ProductId, httpContent);
            return response.StatusCode;
        }
        private async Task<Products> DeleteProductAsync(string path, int productID)
        {
            // Delete Product Object in Delete Request, path includes ProductsID
            HttpClient client = new System.Net.Http.HttpClient();
            HttpResponseMessage response = await client.DeleteAsync(path + "/" + productID);
            var returnProduct = JsonConvert.DeserializeObject<Products>(await response.Content.ReadAsStringAsync());
            return returnProduct;
        }
    }
}
