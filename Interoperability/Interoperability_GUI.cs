using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Interoperability;

namespace Interoperability_Settings_GUI
{
    public partial class Interoperability : Form
    {
        Action<int> restartInteroperabilityCallback;
        protected int pollRate = 10;
        Interoperability_Settings_GUI.Settings_GUI settings_gui;
        Interoperability_Settings Settings;

        public Interoperability(Action<int> _restartInteroperabilityCallback, Interoperability_Settings _Settings)
        {
            InitializeComponent();
            restartInteroperabilityCallback = _restartInteroperabilityCallback;
            Settings = _Settings;

            pollRateInput.Text = pollRate.ToString();
        }

        public int getPollRate()
        {
            return pollRate;
        }

        public Settings_GUI getSettings_GUI()
        {
            return settings_gui;
        }
        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        public void setUniqueTelUploadText(string text)
        {
            this.uniqueTelUploadText.BeginInvoke((MethodInvoker)delegate ()
            {
                this.uniqueTelUploadText.Text = text;
            });
        }

        public void setAvgTelUploadText(string text)
        {
            this.avgTelUploadText.BeginInvoke((MethodInvoker)delegate ()
            {
                this.avgTelUploadText.Text = text;
            });
            return;
        }

        public void setTotalTelemUpload(int num)
        {
            this.Total_Telem_Rate.BeginInvoke((MethodInvoker)delegate ()
            {
                this.Total_Telem_Rate.Text = num.ToString();
            });
            return;
        }

        public void TelemResp(string resp)
        {
            if (resp != this.Telem_Tab.Text)
            {
                this.TelemServerResp.BeginInvoke((MethodInvoker)delegate ()
                {
                    this.TelemServerResp.Text = resp;
                });
            }
        }

        public void setObstacles(Obstacles _Obstacles)
        {
            this.SDA_Obstacles.BeginInvoke((MethodInvoker)delegate ()
            {
                Console.WriteLine("In setObstacles");
                SDA_Obstacles.Text = "";
                SDA_Obstacles.AppendText("MOVING OBJECTS\n");
                for (int i = 0; i < _Obstacles.moving_obstacles.Count(); i++)
                {
                    SDA_Obstacles.AppendText("\tAltitude MSL: " + _Obstacles.moving_obstacles[i].altitude_msl.ToString("N") + "\n");
                    SDA_Obstacles.AppendText("\tSphere Radius: " + _Obstacles.moving_obstacles[i].sphere_radius.ToString("N") + "\n");
                    SDA_Obstacles.AppendText("\tLatitude: " + _Obstacles.moving_obstacles[i].latitude.ToString("N6") + "\n");
                    SDA_Obstacles.AppendText("\tLongitude: " + _Obstacles.moving_obstacles[i].longitude.ToString("N6") + "\n");
                    SDA_Obstacles.AppendText("\n");
                }
                SDA_Obstacles.AppendText("STATIONARY OBJECTS\n");
                for (int i = 0; i < _Obstacles.stationary_obstacles.Count(); i++)
                {
                    SDA_Obstacles.AppendText("\tCylinder Height: " + _Obstacles.stationary_obstacles[i].cylinder_height.ToString("N") + "\n");
                    SDA_Obstacles.AppendText("\tCylinder Radius: " + _Obstacles.stationary_obstacles[i].cylinder_radius.ToString("N") + "\n");
                    SDA_Obstacles.AppendText("\tLatitude: " + _Obstacles.stationary_obstacles[i].latitude.ToString("N6") + "\n");
                    SDA_Obstacles.AppendText("\tLongitude: " + _Obstacles.stationary_obstacles[i].longitude.ToString("N6") + "\n");
                    SDA_Obstacles.AppendText("\n");
                }
            });
        }


        private void applyPollRateButton_Click(object sender, EventArgs e)
        {
            try
            {
                pollRate = Int32.Parse(this.pollRateInput.Text);
            }
            catch (FormatException error)
            {
                this.pollRateInput.Text = pollRate.ToString();
            }
        }

        private void Server_Settings_Click(object sender, EventArgs e)
        {
            if (!Settings_GUI.isOpened)
            {
                settings_gui = new Settings_GUI(restartInteroperabilityCallback, Settings);
                settings_gui.Show();
            }

        }

        private void Reset_Stats_Click(object sender, EventArgs e)
        {
            //Restarts the Telemetry Uplaod Stats
            restartInteroperabilityCallback(4);
        }

        private void SDA_Test_Button_Click(object sender, EventArgs e)
        {
            //Starts the SDA thread
            restartInteroperabilityCallback(1);
        }

        private void Mission_Enable_Click(object sender, EventArgs e)
        {
            restartInteroperabilityCallback(3);
        }
    }
}
