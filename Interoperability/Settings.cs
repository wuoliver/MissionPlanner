using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

//For HTTP requests to server 
using System.Net;
using System.Net.Http;

using Interoperability;



namespace Interoperability_GUI
{
    public partial class Settings_GUI : Form
    {
        string IP_ADDRESS_TEXT = "http://192.168.56.101";
        string USERNAME = "testuser";
        string PASSWORD = "testpass";

        Action<int> restartInteroperabilityCallback;
        Interoperability_Settings Settings;

        public static bool isOpened = false;

        public Settings_GUI(Action<int> _restartInteroperabilityCallback, Interoperability_Settings _Settings)
        {
            isOpened = true;
            InitializeComponent();
            restartInteroperabilityCallback = _restartInteroperabilityCallback;
            Settings = _Settings;

            IP_ADDRESS_TEXT = Settings["address"];
            USERNAME = Settings["username"];
            PASSWORD = Settings["password"];

            IP_ADDR_BOX.Text = IP_ADDRESS_TEXT;
            USERNAME_BOX.Text = USERNAME;
            PASSWORD_BOX.Text = PASSWORD;
        }

        private void IP_ADDR_BOX_TextChanged(object sender, EventArgs e)
        {
            IP_ADDRESS_TEXT = IP_ADDR_BOX.Text;
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            validation_label.Text = "";
            error_label.Text = "";
        }

        private void Settings_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings_GUI.isOpened = false;
        }

        private void USERNAME_BOX_TextChanged(object sender, EventArgs e)
        {
            USERNAME = USERNAME_BOX.Text;
        }

        private void PASSWORD_BOX_TextChanged(object sender, EventArgs e)
        {
            PASSWORD = PASSWORD_BOX.Text;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            isOpened = false;
            this.Close();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            Settings["address"] = IP_ADDRESS_TEXT;
            Settings["username"] = USERNAME;
            Settings["password"] =  PASSWORD;
            Settings.Save();

            //restarts the interoperability thread, allowing for changes to be made
            restartInteroperabilityCallback(0);
            isOpened = false;
            this.Close();
        }

        async void testLogin()
        {
            try
            {
                using (var client = new HttpClient())
                {

                    client.BaseAddress = new Uri(IP_ADDRESS_TEXT); // This seems to change every time
                    Console.WriteLine("Client Timeout" + client.Timeout.ToString());

                    //Set HTTP timeout to short timeout, if we continiously time out, then increase value
                    TimeSpan timeout = new TimeSpan(0, 0, 0, 1);
                    client.Timeout = timeout;
                    Console.WriteLine("Client Timeout" + client.Timeout.ToString());

                    // Log in.
                    Console.WriteLine("---INITIAL LOGIN---");
                    var v = new Dictionary<string, string>();
                    v.Add("username", USERNAME);
                    v.Add("password", PASSWORD);
                    var auth = new FormUrlEncodedContent(v);
                    HttpResponseMessage resp = await client.PostAsync("/api/login", auth);
                    Console.WriteLine("Login POST result: " + resp.Content.ReadAsStringAsync().Result);
                    Console.WriteLine("---LOGIN FINISHED---");
                    //resp.IsSuccessStatusCode;
                    if (!resp.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Invalid Credentials");
                        validation_label.Text = "";
                        error_label.Text = "Error, Invalid Credentials";
                    }
                    else
                    {
                        Console.WriteLine("Credentials Valid");
                        error_label.Text = "";
                        validation_label.Text = "Success, Credentials Valid";
                        
                    }
                }
            }
            catch
            {
                //Most likely the IP adress was invalid. So we set as invalid login
                Console.WriteLine("Error, exception thrown while testing login");
                //Console.WriteLine(e.Message);
                validation_label.Text = "";
                error_label.Text = "Error, Invalid Server Address";
            }
            Console.WriteLine("Finished Testing Login");
        }

        private void Verify_Click(object sender, EventArgs e)
        {
            Thread bg = new Thread(new ThreadStart(this.testLogin));
            bg.Start();
            bg.Join();
        }
    }
}
