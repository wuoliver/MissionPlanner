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

using interoperability;



namespace Interoperability_GUI_Forms
{
    public partial class Settings_GUI : Form
    {
        private string IP_ADDRESS_TEXT = "http://192.168.56.101";
        private string USERNAME = "testuser";
        private string PASSWORD = "testpass";

        private string dist_units = "Metres";
        private string airspd_units = "Metres per Second";
        private string geo_cords = "DD.DDDDDD";

        private string gui_format = "AUVSI";

        Action<Interop_Callback_Struct> InteroperabilityCallback;
        Action<int> InteroperabilityGUICallback;
        Interoperability_Settings Settings;

        public bool isOpened = false;

        public Settings_GUI(Action<Interop_Callback_Struct> _InteroperabilityCallback, Action<int> _InteroperabilityGUICallback, Interoperability_Settings _Settings)
        {
            InitializeComponent();
            InteroperabilityCallback = _InteroperabilityCallback;
            InteroperabilityGUICallback = _InteroperabilityGUICallback;
            Settings = _Settings;

            IP_ADDRESS_TEXT = Settings["address"];
            USERNAME = Settings["username"];
            PASSWORD = Settings["password"];

            dist_units = Settings["dist_units"];
            airspd_units = Settings["airspd_units"];
            geo_cords = Settings["geo_cords"];

            gui_format = Settings["gui_format"];

            IP_ADDR_BOX.Text = IP_ADDRESS_TEXT;
            USERNAME_BOX.Text = USERNAME;
            PASSWORD_BOX.Text = PASSWORD;

            Distance_Units_Combo.Text = dist_units;
            Airspeed_Units_Combo.Text = airspd_units;
            Coordinate_System_Combo.Text = geo_cords;

            GUI_FORMAT_BOX.Text = gui_format;
            ShowGUI_Checkbox.Checked = Convert.ToBoolean(Settings["showInteroperability_GUI"]);

        }



        private void Settings_Load(object sender, EventArgs e)
        {
            validation_label.Text = "";
            isOpened = true;
        }

        private void Settings_FormClosed(object sender, FormClosedEventArgs e)
        {
            isOpened = false;
        }



        private void Cancel_Click(object sender, EventArgs e)
        {
            isOpened = false;
            this.Close();
        }

        private void Save_Click(object sender, EventArgs e)
        {

            DialogResult result;
            result = MessageBox.Show("Warning, this will reset all open TCP connections.\nAre you sure you want to save?", "Interoperability Control Panel", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if(result == DialogResult.Yes)
            {
                string old_gui_format = Settings["gui_format"];

                Settings["address"] = IP_ADDR_BOX.Text;
                Settings["username"] = USERNAME_BOX.Text;
                Settings["password"] = PASSWORD_BOX.Text;
                Settings["dist_units"] = Distance_Units_Combo.Text;
                Settings["airspd_units"] = Airspeed_Units_Combo.Text;
                Settings["geo_cords"] = Coordinate_System_Combo.Text;
                Settings["gui_format"] = GUI_FORMAT_BOX.Text;
                Settings["showInteroperability_GUI"] = ShowGUI_Checkbox.Checked.ToString();
                Settings.Save();

                //Restarts all the threads relying on HTTP to update credentials
                InteroperabilityCallback(new Interop_Callback_Struct(Interoperability.Interop_Action.Restart_Threads_Settings));

                //Change tab layout 
                if (GUI_FORMAT_BOX.Text != old_gui_format)
                {
                    if (GUI_FORMAT_BOX.Text == "AUVSI")
                    {
                        InteroperabilityGUICallback(0);
                    }
                    else if(GUI_FORMAT_BOX.Text == "USC")
                    {
                        InteroperabilityGUICallback(1);
                    }
                    else
                    {
                        InteroperabilityGUICallback(2);
                    }
                }
                isOpened = false;
                this.Close();
            }
            else
            {
                //Do nothing
            }
            
        }

        async void testLogin()
        {
            try
            {
                using (var client = new HttpClient())
                {

                    client.BaseAddress = new Uri(IP_ADDR_BOX.Text); // This seems to change every time
                    Console.WriteLine("Client Timeout" + client.Timeout.ToString());

                    //Set HTTP timeout to short timeout, if we continiously time out, then increase value
                    TimeSpan timeout = new TimeSpan(0, 0, 0, 1);
                    client.Timeout = timeout;
                    Console.WriteLine("Client Timeout" + client.Timeout.ToString());

                    // Log in.
                    Console.WriteLine("---INITIAL LOGIN---");
                    var v = new Dictionary<string, string>();
                    v.Add("username", USERNAME_BOX.Text);
                    v.Add("password", PASSWORD_BOX.Text);
                    var auth = new FormUrlEncodedContent(v);
                    HttpResponseMessage resp = await client.PostAsync("/api/login", auth);
                    Console.WriteLine("Login POST result: " + resp.Content.ReadAsStringAsync().Result);
                    Console.WriteLine("---LOGIN FINISHED---");
                    //resp.IsSuccessStatusCode;
                    if (!resp.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Invalid Credentials");
                        validation_label.ForeColor = Color.Red;
                        validation_label.Text = "Error, Credentials Invalid";
                    }
                    else
                    {
                        Console.WriteLine("Credentials Valid");
                        validation_label.ForeColor = Color.DarkGreen;
                        validation_label.Text = "Success, Credentials Valid";

                    }
                }
            }
            catch
            {
                //Most likely the IP adress was invalid. So we set as invalid login
                Console.WriteLine("Error, exception thrown while testing login");
                validation_label.ForeColor = Color.Red;
                validation_label.Text = "Error, Invalid Server Address";
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
