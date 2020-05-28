using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFApp_Cloud
{
    /// <summary>
    /// Interaction logic for UserControlPackages.xaml
    /// </summary>
    public partial class UserControlLogin : UserControl
    {
        public UserControlLogin()
        {
            InitializeComponent();
        }
        public async void loginButton_Click(object sender, EventArgs e)
        {
            // Clear Status Text when button is clicked
            statusTextBox.Text = "";


            // Validation: Ensure no empty fields
            if (usernameTextBox.Text=="" || passwordTextBox.Password=="")
            {
                statusTextBox.Text = "Missing Fields";
                statusTextBox.Foreground = Brushes.DarkOrange;
                loginButton.Background = Brushes.DarkOrange;
                return;
            }

            // Create new Agents object from input fields
            Agents inputAgent = new Agents { Username = usernameTextBox.Text, Password = passwordTextBox.Password };

            //Make API call to get List of Agents
            var agents = await GetAgents("https://travelexperts.azurewebsites.net/api/AgentsAPI");

            // Check if new input Agent Object is in List of Agents from API Call
            if (agents.Find(a => a.Username == inputAgent.Username && a.Password == inputAgent.Password) != null)
            {
                // Agents object found
                statusTextBox.Text = "Success!";
                statusTextBox.Foreground = Brushes.Green;
                loginButton.Background = Brushes.Green;
                usernameTextBox.Text = "";
                passwordTextBox.Password = "";
                MainWindow.IsAuthenticated = true;
            }
            else
            {
                // no Agents object found
                statusTextBox.Text = "Unable to Authenticate";
                statusTextBox.Foreground = Brushes.Red;
                loginButton.Background = Brushes.Red;
                return;
            }
        }
        private async Task<List<Agents>> GetAgents(string path)
        {
            // API method used to get List of Agents from AgentsAPI Controller
            var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(path);
            List<Agents> agents = JsonConvert.DeserializeObject<List<Agents>>(await response.Content.ReadAsStringAsync());
            return agents;
        }
    }
}
