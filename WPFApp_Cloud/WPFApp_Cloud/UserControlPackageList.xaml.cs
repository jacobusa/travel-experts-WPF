using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPFApp_Cloud
{
    /// <summary>
    /// Interaction logic for UserControlPackagesList.xaml
    /// </summary>
    public partial class UserControlPackageList : UserControl
    {
        public UserControlPackageList()
        {
            InitializeComponent();
        }
        public async void Window_Loaded(object sender, RoutedEventArgs e)
        {

            // On page load, make API call to get List of Packages from database
            var packages = await GetPackages("https://travelexperts.azurewebsites.net/api/PackagesAPI");
            foreach (var package in packages)
            {
                // Convert image name in PkgImage column to string path to find corresponding image
                var images = new List<string>{ "asia", "caribbean", "europe", "polynesia", "goldengate", "camels", "hawaii", "Alex_arch", "Alex_lavender",
                "Alex_sunflowers", "Alex_tree", "Alex-green", "beachvan", "blue-4145659_1920", "boatmountains", "bridge", "building", "compass", "mediterranean",
                "mountainbeach", "mountains"};

                // if package image tag is in the images list, replace that name with the path to the image
                if (images.Contains($"{package.PkgImage}"))
                {
                    package.PkgImage = $"/Images/{package.PkgImage}.jpg";
                }
                else
                {
                    // else replace with default earth image
                    package.PkgImage = $"/Images/default.jpg";
                }
                DateTime EndDate = (DateTime)package.PkgEndDate;
                String EndDateString = EndDate.ToShortDateString();
                package.PkgEndDate = Convert.ToDateTime(EndDateString);

            }

            // Bind ListView to the Packages List from API call for display
            ListViewPackages.ItemsSource = packages;
            
        }
        private async Task<List<Packages>> GetPackages(string path)
        {
            // Instantiate HTTP Client
            HttpClient client = new System.Net.Http.HttpClient();

            // Placeholder for Packages List
            List<Packages> pkgs = null;

            // Make API Call and collect response
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                // Deserialize response and place inside Packages List placeholder
                pkgs = JsonConvert.DeserializeObject<List<Packages>>(await response.Content.ReadAsStringAsync());
            }

            // Return list of packages
            return pkgs;
        }
    }
}
