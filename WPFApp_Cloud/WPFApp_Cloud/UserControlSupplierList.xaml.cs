using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPFApp_Cloud
{
    /// <summary>
    /// Interaction logic for UserControlSupplierList.xaml
    /// </summary>
    public partial class UserControlSupplierList : UserControl
    {
        public UserControlSupplierList()
        {
            InitializeComponent();
        }
        public async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // When page loads, make Async Get request getting List of Suppliers objects and bind to ListView for display
            var suppliers = await GetSuppliers("https://travelexperts.azurewebsites.net/api/SuppliersAPI");
            ListViewSuppliers.ItemsSource = suppliers;

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
