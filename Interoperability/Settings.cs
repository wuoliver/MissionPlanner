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
        private string IP_ADDRESS_TEXT = "http://192.168.56.101";
        private string USERNAME = "testuser";
        private string PASSWORD = "testpass";

        private string dist_units = "Metres";
        private string airspd_units = "Metres per Second";
        private string geo_cords = "DD.DDDDDD";


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

            dist_units = Settings["dist_units"];
            airspd_units = Settings["airspd_units"];
            geo_cords = Settings["geo_cords"];

            IP_ADDR_BOX.Text = IP_ADDRESS_TEXT;
            USERNAME_BOX.Text = USERNAME;
            PASSWORD_BOX.Text = PASSWORD;

            Distance_Units_Combo.Text = dist_units;
            Airspeed_Units_Combo.Text = airspd_units;
            Coordinate_System_Combo.Text = geo_cords;
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

        

        private void Cancel_Click(object sender, EventArgs e)
        {
            isOpened = false;
            this.Close();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            Settings["address"] = IP_ADDR_BOX.Text;
            Settings["username"] = USERNAME_BOX.Text;
            Settings["password"] = PASSWORD_BOX.Text;
            Settings["dist_units"] = Distance_Units_Combo.Text;
            Settings["airspd_units"] = Airspeed_Units_Combo.Text;
            Settings["geo_cords"] = Coordinate_System_Combo.Text;         
            Settings.Save();

            //Restarts all the threads relying on HTTP to update credentials
            restartInteroperabilityCallback(6);
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
