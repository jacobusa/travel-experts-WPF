using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFApp_Cloud
{
    /// <summary>
    /// Interaction logic for UserControlProductAdd.xaml
    /// </summary>
    public partial class UserControlProductAdd : UserControl
    {
        public UserControlProductAdd()
        {
            InitializeComponent();
        }
        public async void addSubmit_ClickAsync(object sender, EventArgs e)
        {

            // Clear status text
            statusTextBlock.Text = "";

            string packageName = nameTextbox.Text;

            // Validatoin: Name must be entered
            if (nameTextbox.Text == "")
            {
                statusTextBlock.Foreground = Brushes.DarkOrange;
                statusTextBlock.Text = "Missing Fields!!";
                submitButton.Background = Brushes.DarkOrange;
                return;
            }

            // Create new Products Object from input field
            var product = new Products
            {
                ProdName = nameTextbox.Text
            };

            // Insert new Products object into database through API
            var task = await PostProductAsync("https://travelexperts.azurewebsites.net/api/ProductsAPI", product);
            var items = task;
            if (items == HttpStatusCode.Created)
            {
                // Creation succeeded
                statusTextBlock.Foreground = Brushes.Green;
                statusTextBlock.Text = "Product Created!";
                submitButton.Background = Brushes.Green;
                nameTextbox.Text = "";
            }
            else
            {
                // Creation Failed
                statusTextBlock.Foreground = Brushes.Red;
                statusTextBlock.Text = "Unable to Add Product!";
                submitButton.Background = Brushes.Red;
            }

        }
        private async Task<HttpStatusCode> PostProductAsync(string path, Products product)
        {
            // Instantiate new HTTP Client
            HttpClient client = new System.Net.Http.HttpClient();

            // Serialize Products object tp JSON for API Call
            var content = JsonConvert.SerializeObject(product);
            var httpContent = new StringContent(content, Encoding.UTF8, "application/json");

            // Make Post Request and collect response
            HttpResponseMessage response = await client.PostAsync(path, httpContent);

            // return status code from response
            return response.StatusCode;
        }
    }
}
