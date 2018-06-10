using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using interoperability;
using MissionPlanner;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

//For javascript serializer
using System.Web.Script.Serialization;
using System.Net.Http;
using System.Net;

namespace Interoperability_GUI_Forms
{
    public partial class Interoperability_GUI_Main : Form
    {
        Action<Interop_Callback_Struct> InteroperabilityCallback;

        protected int telemPollRate = 10;
        protected int mapRefreshRate = 20;
        protected int sdaPollRate = 10;
        protected int UAS_Scale = 2;

        //Used for callout thread
        protected int Callout_minAirspeed = 10;
        protected int Callout_period = 1000;
        protected bool SpeechRecognition_Enabled = true;

        public bool isOpened = false;
        Settings_GUI Settings_GUI_Instance;
        Interoperability_Mission_Edit Interoperability_Mission_Edit_Instance;

        Interoperability_Settings Settings;


        //Put on top or something afterwards
        GMapOverlay g_Static_Overlay;
        GMapOverlay g_Image_Overlay;
        GMapOverlay g_Stationary_Obstacle_Overlay;
        GMapOverlay g_Moving_Obstacle_Overlay;
        GMapOverlay g_Plane_Overlay;
        GMapOverlay g_WP_Overlay;
        GMapOverlay g_OFAT_EM_DROP_Overlay;


        //Used for map control thread
        protected bool MAP_Bool_DrawWP = true;
        protected bool Map_Bool_DrawObstacles = true;
        protected bool Map_Bool_DrawPlane = true;
        protected bool Map_Bool_DrawGeofence = true;
        protected bool Map_Bool_DrawSearchArea = true;
        protected bool Map_Bool_DrawOFAT_EM_DROP = true;
        protected bool Map_Bool_UAS_FixedSize = false;
        protected bool MAP_Bool_Autopan_Enable = false;


        protected string gui_format = "AUVSI";
        private List<TabPage> TabList = new List<TabPage>();

        //GMAP Zoom
        private int zoom = 0;

        //Target Upload
        private List<Image_Uploads> Target_List = new List<Image_Uploads>();

        //Container for all possible targets
        List<PointLatLng> PossibleTargets;  //Targets that are found through the FPV camera
        List<PointLatLng> FoundTargets;     //Targets found through Davis's algorithm

        public Interoperability_GUI_Main(Action<Interop_Callback_Struct> _InteroperabilityCallback, Interoperability_Settings _Settings)
        {
            Console.WriteLine("Created GUI");
            InitializeComponent();
            InteroperabilityCallback = _InteroperabilityCallback;

            //Get Settings object, used for server settings
            Settings = _Settings;

            //Must be called after settings
            MAP_Settings_Init_Bool(ref Settings, ref MAP_Bool_DrawWP, "DrawWP");
            MAP_Settings_Init_Bool(ref Settings, ref Map_Bool_DrawObstacles, "DrawObstacles");
            MAP_Settings_Init_Bool(ref Settings, ref Map_Bool_DrawPlane, "DrawPlane");
            MAP_Settings_Init_Bool(ref Settings, ref Map_Bool_DrawGeofence, "DrawGeofence");
            MAP_Settings_Init_Bool(ref Settings, ref Map_Bool_DrawSearchArea, "DrawSearchArea");
            MAP_Settings_Init_Bool(ref Settings, ref Map_Bool_DrawOFAT_EM_DROP, "DrawOFAT_EN_DROP");
            MAP_Settings_Init_Bool(ref Settings, ref Map_Bool_UAS_FixedSize, "UAS_Fixedsize");
            MAP_Settings_Init_Bool(ref Settings, ref MAP_Bool_Autopan_Enable, "MAP_Autopan");


            MAP_Settings_Init_Int(ref Settings, ref UAS_Scale, "UAS_Scale");
            MAP_Settings_Init_String(ref Settings, ref gui_format, "gui_format");

            InitializeGUI_States();

            Settings.Save();

            Settings_GUI_Instance = new Settings_GUI(InteroperabilityCallback, InteroperabilityGUIAction, Settings);

            Interoperability_Mission_Edit_Instance = new Interoperability_Mission_Edit();

            //Set poll Rate text 
            Telemetry_pollRateInput.Text = telemPollRate.ToString();
            SDA_pollrateInput.Text = sdaPollRate.ToString();
            Map_RefreshRateInput.Text = mapRefreshRate.ToString();

            g_Static_Overlay = new GMapOverlay("Static_Overlays");
            g_Image_Overlay = new GMapOverlay("Image Overlay");
            g_Stationary_Obstacle_Overlay = new GMapOverlay("Stationary_Obstacles");
            g_Plane_Overlay = new GMapOverlay("Plane_Overlay");
            g_Moving_Obstacle_Overlay = new GMapOverlay("Moving_Obstacle");
            g_WP_Overlay = new GMapOverlay("Waypoints");
            g_OFAT_EM_DROP_Overlay = new GMapOverlay("OFAT_EN_DROP_Overlay");


            Target_List.Add(new Image_Uploads());
            Image_Upload_Target_Select.SelectedIndex = 0;
        }

        private void Interoperability_GUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            InteroperabilityCallback(new Interop_Callback_Struct(Interoperability.Interop_Action.Stop_All_Threads_Quit));
            isOpened = false;
        }

        private void showInteroperabilityControlPanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Opening Interoperability Control Panel");
            InteroperabilityCallback(new Interop_Callback_Struct(Interoperability.Interop_Action.Show_Interop_GUI));
        }

        private void Interoperability_GUI_Main_Shown(object sender, EventArgs e)
        {
            isOpened = true;
        }


        private void InteropGUIButton_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                InteroperabilityCallback(new Interop_Callback_Struct(Interoperability.Interop_Action.Easter_Egg_Action));
            }
            else if (e.Button == MouseButtons.Left)
            {
                Console.WriteLine("Opening Interoperability Control Panel");
                InteroperabilityCallback(new Interop_Callback_Struct(Interoperability.Interop_Action.Show_Interop_GUI));
            }

        }

        public enum Interop_Interface
        {
            AUVSI,
            USC,
            DRONE_PV
        }

        public void InteroperabilityGUIAction(int action)
        {
            int TabCount;
            switch (action)
            {
                //Disable USC Elements, Enable AUVSI
                case 0:
                    TabCount = Interoperability_GUI_Tab.TabPages.Count;
                    for (int i = 0; i < TabCount; i++)
                    {
                        Interoperability_GUI_Tab.TabPages.RemoveAt(0);
                    }
                    Interoperability_GUI_Tab.TabPages.Add(TabList[0]); //Telemtry Upload Tab
                    Interoperability_GUI_Tab.TabPages.Add(TabList[1]); //SDA Tab
                    Interoperability_GUI_Tab.TabPages.Add(TabList[2]); //Map control tab
                    Interoperability_GUI_Tab.TabPages.Add(TabList[3]); //Image Tab
                    Interoperability_GUI_Tab.TabPages.Add(TabList[4]); //Callout Tab
                    Interoperability_GUI_Tab.TabPages.Add(TabList[7]); //Callout Tab
                    this.Text = "UTAT UAV Interoperability Control Panel (AUVSI) - " + Interoperability.getinstance().Current_Mission.name;
                    break;
                //Disable AUVSI Elements, Enable USC 
                case 1:
                    TabCount = Interoperability_GUI_Tab.TabPages.Count;
                    for (int i = 0; i < TabCount; i++)
                    {
                        Interoperability_GUI_Tab.TabPages.RemoveAt(0);
                    }
                    Interoperability_GUI_Tab.TabPages.Add(TabList[2]); //Map control tab
                    Interoperability_GUI_Tab.TabPages.Add(TabList[4]); //Callout Tab
                    Interoperability_GUI_Tab.TabPages.Add(TabList[5]); //Geese Tab
                    this.Text = "UTAT UAV Interoperability Control Panel (USC) - " + Interoperability.getinstance().Current_Mission.name;
                    break;
                //Disable all elements, add Drone PV Cleaning
                case 2:
                    TabCount = Interoperability_GUI_Tab.TabPages.Count;
                    for (int i = 0; i < TabCount; i++)
                    {
                        Interoperability_GUI_Tab.TabPages.RemoveAt(0);
                    }
                    Interoperability_GUI_Tab.TabPages.Add(TabList[6]); //Drone PV Cleaning Tab
                    Interoperability_GUI_Tab.TabPages.Add(TabList[2]); //Map control tab
                    this.Text = "Drone PV Cleaning - " + Interoperability.getinstance().Current_Mission.name;
                    break;
                default:
                    break;
            }
        }


        public void MAP_Settings_Init_String(ref Interoperability_Settings Settings, ref string value, string key)
        {
            if (!Settings.ContainsKey(key))
            {
                Settings[key] = value;
            }
            else
            {
                value = Settings[key];
            }
        }

        public void MAP_Settings_Init_Int(ref Interoperability_Settings Settings, ref int value, string key)
        {
            if (!Settings.ContainsKey(key))
            {
                Settings[key] = value.ToString();
            }
            else
            {
                value = Convert.ToInt32(Settings[key]);
            }
        }

        public void MAP_Settings_Init_Bool(ref Interoperability_Settings Settings, ref bool value, string key)
        {
            if (!Settings.ContainsKey(key))
            {
                Settings[key] = value.ToString();
            }
            else
            {
                value = Convert.ToBoolean(Settings[key]);
            }
        }

        public void InitializeGUI_States()
        {
            showGeofenceToolStripMenuItem.Checked = Map_Bool_DrawGeofence;
            Geofence_Checkbox.Checked = Map_Bool_DrawGeofence;

            showSearchAreaToolStripMenuItem.Checked = Map_Bool_DrawSearchArea;
            SearchArea_Checkbox.Checked = Map_Bool_DrawSearchArea;

            showObstaclesToolStripMenuItem.Checked = Map_Bool_DrawObstacles;
            Obstacles_Checkbox.Checked = Map_Bool_DrawObstacles;

            showPlaneToolStripMenuItem.Checked = Map_Bool_DrawPlane;
            UASLoc_Checkbox.Checked = Map_Bool_DrawPlane;

            showWaypointsToolStripMenuItem.Checked = MAP_Bool_DrawWP;
            Waypoints_Checkbox.Checked = MAP_Bool_DrawWP;

            showOFATEmergentDropToolStripMenuItem.Checked = Map_Bool_DrawOFAT_EM_DROP;
            OFAT_EM_DROP_CheckBox.Checked = Map_Bool_DrawOFAT_EM_DROP;

            UAS_Trackbar.Value = UAS_Scale;
            Fixed_UAS_Size_Checkbox.Checked = Map_Bool_UAS_FixedSize;

            AutoPan_Checkbox.Checked = MAP_Bool_Autopan_Enable;

            foreach (TabPage TAB in Interoperability_GUI_Tab.TabPages)
            {
                TabList.Add(TAB);
                Interoperability_GUI_Tab.TabPages.Remove(TAB);
            }

            if (gui_format == "AUVSI")
            {
                InteroperabilityGUIAction(0);
            }
            else if (gui_format == "USC")
            {
                InteroperabilityGUIAction(1);
            }
            else
            {
                InteroperabilityGUIAction(2);
            }

        }

        //--------------------------------------//
        //               Accessors              //
        //--------------------------------------//
        public ToolStripItem getContextMenu()
        {
            return MissionPlanner_ContextMenuStrip.Items[0];
        }
        public ToolStripItem getMenuStrip()
        {
            return MissionPlannerMenuAddon.Items[0];
        }






        public Settings_GUI getSettings_GUI()
        {
            return Settings_GUI_Instance;
        }


        //Function is called when the user clicks a button thing
        public void AddTarget()
        {

        }

        //---------------------------------------------------------------//
        //                        Windows Form Actions                   //
        //---------------------------------------------------------------//

        //-------------------------------------------//
        //               Telemetry Tab               //
        //-------------------------------------------//

        private void applyPollRateButton_Click(object sender, EventArgs e)
        {
            try
            {
                telemPollRate = Int32.Parse(this.Telemetry_pollRateInput.Text);
            }
            catch
            {
                this.Telemetry_pollRateInput.Text = telemPollRate.ToString();
            }
        }

        private void Reset_Stats_Click(object sender, EventArgs e)
        {
            //Restarts the Telemetry Upload Stats
            InteroperabilityCallback(new Interop_Callback_Struct(Interoperability.Interop_Action.Telemetry_Thread_Reset_Stats));
        }

        private void Telem_Start_Stop_Button_Click(object sender, EventArgs e)
        {
            if (Telem_Start_Stop_Button.Text == "Start")
            {
                //Start
                InteroperabilityCallback(new Interop_Callback_Struct(Interoperability.Interop_Action.Telemtry_Thread_Start));
                Telem_Start_Stop_Button.Text = "Stop";
            }
            else
            {
                //Stop
                InteroperabilityCallback(new Interop_Callback_Struct(Interoperability.Interop_Action.Telemtry_Thread_Stop));
                Telem_Start_Stop_Button.Text = "Start";
            }
        }

        public void Telem_Start_Stop_Button_Off()
        {
            this.Telem_Start_Stop_Button.BeginInvoke((MethodInvoker)delegate ()
            {
                Telem_Start_Stop_Button.Text = "Start";
            });
        }

        public void set_telemetry_data_textbox(string text)
        {
            this.telemetry_data_textbox.BeginInvoke((MethodInvoker)delegate ()
            {
                telemetry_data_textbox.Clear();
                telemetry_data_textbox.Text = text;
            });
            return;
        }

        public int getTelemPollRate()
        {
            return telemPollRate;
        }


        /// <summary>
        /// Sets the text in the Unique Telemtry Upload Rate text box in the telemtry upload tab
        /// </summary>
        public void setUniqueTelUploadText(string text)
        {
            this.uniqueTelUploadText.BeginInvoke((MethodInvoker)delegate ()
            {
                this.uniqueTelUploadText.Text = text;
            });
        }

        /// <summary>
        /// Sets the text in the Average Telemtry Upload Rate text box in the telemtry upload tab
        /// </summary>
        public void setAvgTelUploadText(string text)
        {
            this.avgTelUploadText.BeginInvoke((MethodInvoker)delegate ()
            {
                this.avgTelUploadText.Text = text;
            });
            return;
        }

        /// <summary>
        /// Sets the number of total unique telemtry data uploaded
        /// </summary>
        public void setTotalTelemUpload(int num)
        {
            this.Total_Telem_Rate.BeginInvoke((MethodInvoker)delegate ()
            {
                this.Total_Telem_Rate.Text = num.ToString();
            });
            return;
        }

        /// <summary>
        /// Sets the server response in the telemtry upload tab
        /// </summary>
        public void setTelemResp(string resp)
        {
            if (resp != this.TelemServerResp.Text)
            {
                this.TelemServerResp.BeginInvoke((MethodInvoker)delegate ()
                {
                    this.TelemServerResp.Text = resp;
                });
            }
        }


        //-------------------------------------------//
        //                  SDA Tab                  //
        //-------------------------------------------//

        private void SDA_PollRateApply_Click(object sender, EventArgs e)
        {
            try
            {
                sdaPollRate = Int32.Parse(this.SDA_pollrateInput.Text);
            }
            catch
            {
                this.SDA_pollrateInput.Text = sdaPollRate.ToString();
            }
        }

        private void SDA_Start_Stop_Button_Click(object sender, EventArgs e)
        {
            if (SDA_Start_Stop_Button.Text == "Start SDA Polling")
            {
                //Start
                InteroperabilityCallback(new Interop_Callback_Struct(Interoperability.Interop_Action.Obstacle_SDA_Thread_Start));
                SDA_Start_Stop_Button.Text = "Stop SDA Polling";
            }
            else
            {
                //Stop
                InteroperabilityCallback(new Interop_Callback_Struct(Interoperability.Interop_Action.Obstacle_SDA_Thread_Stop));
                SDA_Start_Stop_Button.Text = "Start SDA Polling";
            }
        }

        public void SetSDAStart_StopButton_Off()
        {
            this.SDA_Start_Stop_Button.BeginInvoke((MethodInvoker)delegate ()
            {
                SDA_Start_Stop_Button.Text = "Start SDA Polling";
            });
        }

        /// <summary>
        /// Sets the server response text box in the Obstacle SDA tab
        /// </summary>
        public void setSDAResp(string resp)
        {
            if (resp != this.SDA_ServerResponseTextBox.Text)
            {
                this.SDA_ServerResponseTextBox.BeginInvoke((MethodInvoker)delegate ()
                {
                    this.SDA_ServerResponseTextBox.Text = resp;
                });
            }
        }

        /// <summary>
        /// Prints the obstacles to the text box in the Obstacle_SDA tab
        /// </summary>
        /// <param name="_Obstacles">The obstacles to be printed</param>
        public void WriteObstacles(Obstacles _Obstacles)
        {
            this.SDA_Obstacles.BeginInvoke((MethodInvoker)delegate ()
            {
                Console.WriteLine("In setObstacles");
                SDA_Obstacles.Text = "";
                String Buffer = "";
                Buffer += "MOVING OBJECTS\r\n";
                for (int i = 0; i < _Obstacles.moving_obstacles.Count(); i++)
                {
                    Buffer += "\tAltitude MSL: " + _Obstacles.moving_obstacles[i].altitude_msl.ToString("N") + "\r\n";
                    Buffer += "\tSphere Radius: " + _Obstacles.moving_obstacles[i].sphere_radius.ToString("N") + "\r\n";
                    Buffer += "\tLatitude: " + _Obstacles.moving_obstacles[i].latitude.ToString("N6") + "\r\n";
                    Buffer += "\tLongitude: " + _Obstacles.moving_obstacles[i].longitude.ToString("N6") + "\r\n";
                    Buffer += "\r\n";
                }
                Buffer += "STATIONARY OBJECTS\r\n";
                for (int i = 0; i < _Obstacles.stationary_obstacles.Count(); i++)
                {
                    Buffer += "\tCylinder Height: " + _Obstacles.stationary_obstacles[i].cylinder_height.ToString("N") + "\r\n";
                    Buffer += "\tCylinder Radius: " + _Obstacles.stationary_obstacles[i].cylinder_radius.ToString("N") + "\r\n";
                    Buffer += "\tLatitude: " + _Obstacles.stationary_obstacles[i].latitude.ToString("N6") + "\r\n";
                    Buffer += "\tLongitude: " + _Obstacles.stationary_obstacles[i].longitude.ToString("N6") + "\r\n";
                    Buffer += "\r\n";
                }

                SDA_Obstacles.Text = Buffer;
            });
        }

        private void SDA_Plane_Simulation_Start_Button_Click(object sender, EventArgs e)
        {
            if (SDA_Plane_Simulation_Start_Button.Text == "Start Simulation")
            {
                SDA_Plane_Simulation_Start_Button.Text = "Stop Simulation";
                InteroperabilityCallback(new Interop_Callback_Struct(Interoperability.Interop_Action.SDA_Plane_Simulator_Thread_Start));
            }
            else
            {
                SDA_Plane_Simulation_Start_Button.Text = "Start Simulation";
                InteroperabilityCallback(new Interop_Callback_Struct(Interoperability.Interop_Action.SDA_Plane_Simulator_Thread_Stop));
            }

        }

        private void SDA_Start_Algorithm_Button_Click(object sender, EventArgs e)
        {
            if (SDA_Start_Algorithm_Button.Text == "Start Algorithm")
            {
                SDA_Start_Algorithm_Button.Text = "Stop Algorithm";
                InteroperabilityCallback(new Interop_Callback_Struct(Interoperability.Interop_Action.SDA_Avoidance_Algorithm_Thread_Start));
            }
            else
            {
                SDA_Start_Algorithm_Button.Text = "Start Algorithm";
                InteroperabilityCallback(new Interop_Callback_Struct(Interoperability.Interop_Action.SDA_Avoidance_Algorithm_Thread_Stop));
            }

        }

        public int getsdaPollRate()
        {
            return sdaPollRate;
        }

        public int getPlaneSimulationAirspeed()
        {
            return Convert.ToInt32(Plane_Simulated_Airspeed_Select.Value);
        }


        //-------------------------------------------//
        //               Map Control Tab             //
        //-------------------------------------------//

        private void Geofence_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!Geofence_Checkbox.Checked)
            {
                //Geofence_Checkbox.Checked = false;
                showGeofenceToolStripMenuItem.Checked = false;
                Map_Bool_DrawGeofence = false;
            }
            else
            {
                //Geofence_Checkbox.Checked = true;
                showGeofenceToolStripMenuItem.Checked = true;
                Map_Bool_DrawGeofence = true;
            }
            Settings["DrawGeofence"] = Map_Bool_DrawGeofence.ToString();
            Settings.Save();
            Interoperability.getinstance().Invalidate_Map();
        }

        private void SearchArea_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!SearchArea_Checkbox.Checked)
            {
                showSearchAreaToolStripMenuItem.Checked = false;
                Map_Bool_DrawSearchArea = false;
            }
            else
            {
                showSearchAreaToolStripMenuItem.Checked = true;
                Map_Bool_DrawSearchArea = true;
            }
            Settings["DrawSearchArea"] = Map_Bool_DrawSearchArea.ToString();
            Settings.Save();
            Interoperability.getinstance().Invalidate_Map();
        }

        private void Obstacles_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!Obstacles_Checkbox.Checked)
            {
                showObstaclesToolStripMenuItem.Checked = false;
                Map_Bool_DrawObstacles = false;
            }
            else
            {
                showObstaclesToolStripMenuItem.Checked = true;
                Map_Bool_DrawObstacles = true;
            }
            Settings["DrawObstacles"] = Map_Bool_DrawObstacles.ToString();
            Settings.Save();
            Interoperability.getinstance().Invalidate_Map();
        }

        private void UASLoc_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!UASLoc_Checkbox.Checked)
            {
                showPlaneToolStripMenuItem.Checked = false;
                Map_Bool_DrawPlane = false;
            }
            else
            {
                showPlaneToolStripMenuItem.Checked = true;
                Map_Bool_DrawPlane = true;
            }
            Settings["DrawPlane"] = Map_Bool_DrawPlane.ToString();
            Settings.Save();
            Interoperability.getinstance().Invalidate_Map();
        }

        private void Waypoints_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!Waypoints_Checkbox.Checked)
            {
                showWaypointsToolStripMenuItem.Checked = false;
                MAP_Bool_DrawWP = false;
            }
            else
            {
                showWaypointsToolStripMenuItem.Checked = true;
                MAP_Bool_DrawWP = true;
            }
            Settings["DrawWP"] = MAP_Bool_DrawWP.ToString();
            Settings.Save();
            Interoperability.getinstance().Invalidate_Map();
        }

        private void OFAT_EM_DROP_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!OFAT_EM_DROP_CheckBox.Checked)
            {
                showOFATEmergentDropToolStripMenuItem.Checked = false;
                Map_Bool_DrawOFAT_EM_DROP = false;
            }
            else
            {
                showOFATEmergentDropToolStripMenuItem.Checked = true;
                Map_Bool_DrawOFAT_EM_DROP = true;
            }
            Settings["DrawOFAT_EN_DROP"] = Map_Bool_DrawOFAT_EM_DROP.ToString();
            Settings.Save();
            Interoperability.getinstance().Invalidate_Map();
        }

        private void UAS_Trackbar_Scroll(object sender, EventArgs e)
        {
            UAS_Scale = UAS_Trackbar.Value;
            Settings["UAS_Scale"] = UAS_Scale.ToString();
            Settings.Save();
            Interoperability.getinstance().Invalidate_Map();
        }

        private void Fixed_UAS_Size_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            Map_Bool_UAS_FixedSize = Fixed_UAS_Size_Checkbox.Checked;
            Settings["UAS_Fixedsize"] = Map_Bool_UAS_FixedSize.ToString();
            Settings.Save();
            Interoperability.getinstance().Invalidate_Map();
        }

        private void AutoPan_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            MAP_Bool_Autopan_Enable = AutoPan_Checkbox.Checked;
            Settings["MAP_Autopan"] = MAP_Bool_Autopan_Enable.ToString();
            Settings.Save();
            Interoperability.getinstance().Invalidate_Map();
        }

        private void gMapControl1_OnMapDrag()
        {
            MAP_Bool_Autopan_Enable = false;
            AutoPan_Checkbox.Checked = false;
            Settings["MAP_Autopan"] = MAP_Bool_Autopan_Enable.ToString();
            Settings.Save();
            Interoperability.getinstance().Invalidate_Map();
        }

        private void Mission_Download_Click(object sender, EventArgs e)
        {
            InteroperabilityCallback(new Interop_Callback_Struct(Interoperability.Interop_Action.Mission_Download_Run));
        }

        public bool getDrawWP()
        {
            return MAP_Bool_DrawWP;
        }

        public bool getDrawObstacles()
        {
            return Map_Bool_DrawObstacles;
        }

        public bool getDrawPlane()
        {
            return Map_Bool_DrawPlane;
        }

        public bool getDrawGeofence()
        {
            return Map_Bool_DrawGeofence;
        }

        public bool getDrawSearchArea()
        {
            return Map_Bool_DrawSearchArea;
        }

        public bool getDrawOFAT_EM_DROP()
        {
            return Map_Bool_DrawOFAT_EM_DROP;
        }

        public bool getAutopan()
        {
            return MAP_Bool_Autopan_Enable;
        }
        private void Map_ApplyRefreshRate_Click(object sender, EventArgs e)
        {
            try
            {
                mapRefreshRate = Int32.Parse(this.Map_RefreshRateInput.Text);
            }
            catch
            {
                this.Map_RefreshRateInput.Text = mapRefreshRate.ToString();
            }
        }
        public int getMapRefreshRate()
        {
            return mapRefreshRate;
        }

        //-------------------------------------------//
        //                Callouts Tab               //
        //-------------------------------------------//

        private void Callout_StartStop_Button_Click(object sender, EventArgs e)
        {
            if (Callout_StartStop_Button.Text == "Start")
            {
                Callout_StartStop_Button.Text = "Stop";
                InteroperabilityCallback(new Interop_Callback_Struct(Interoperability.Interop_Action.Callout_Thread_Start));
            }
            else
            {
                Callout_StartStop_Button.Text = "Start";
                InteroperabilityCallback(new Interop_Callback_Struct(Interoperability.Interop_Action.Callout_Thread_Stop));
            }

        }

        public int getCalloutPeriod()
        {
            return Callout_period;
        }

        public int getCalloutminAirspeed()
        {
            return Callout_period;
        }

        public bool getSpeechRecognition_Enabled()
        {
            return SpeechRecognition_Enabled;
        }

        public string getCalloutMode()
        {
            return Callout_Mode_ComboBox.Text;
        }

        //-------------------------------------------//
        //                     MAP                   //
        //-------------------------------------------//

        /// <summary>
        /// Called when the map is first loaded
        /// </summary>
        private void gMapControl1_Load(object sender, EventArgs e)
        {
            gMapControl1.DragButton = MouseButtons.Left;
            gMapControl1.Position = new PointLatLng(38.145228, -76.427938); //AUVSI 
            gMapControl1.Zoom = 15;
            gMapControl1.MapProvider = GMap.NET.MapProviders.GoogleSatelliteMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache;
        }

        /// <summary>
        /// Adds a static polygon to the map
        /// </summary>
        /// <param name="points">Points that define the polygon</param>
        /// <param name="width">Width of border colour in pixels</param>
        /// <param name="alpha">The transparancy of the fill colour</param>
        public void MAP_addStaticPoly(List<Waypoint> points, string name, Color Border_Color, Color Fill_Color, int width, int alpha)
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                List<PointLatLng> _points = new List<PointLatLng>();
                for (int i = 0; i < points.Count(); i++)
                {
                    _points.Add(new PointLatLng(points[i].latitude, points[i].longitude));
                }

                GMapPolygon Static_Polygon = new GMapPolygon(_points, name);
                Static_Polygon.Stroke = new Pen(Border_Color, width);
                Static_Polygon.Fill = new SolidBrush(Color.FromArgb(alpha, Fill_Color));
                g_Static_Overlay.Polygons.Add(Static_Polygon);
            });
        }


        /// <summary>
        /// Adds a moving obstacle to the map. The obstacle is defined as a sphere. 
        /// </summary>
        /// <param name="radius">Radius of the sphere in meters</param>
        /// <param name="altitude">Altitude of the centre of the sphere in meters</param>
        public void MAP_addMObstaclePoly(double radius, double altitude, double Lat, double Lon, string name)
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                GMapPolygon polygon = new GMapPolygon(getCirclePoly(radius, Lat, Lon), "name");
                polygon.Stroke = new Pen(Color.Red, 2);
                polygon.Fill = new SolidBrush(Color.FromArgb(100, Color.RoyalBlue));
                g_Moving_Obstacle_Overlay.Polygons.Add(polygon);

                //Show the altitude of the obstacle
                GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(Lat, Lon), new Bitmap(1, 1));
                marker.ToolTipMode = MarkerTooltipMode.Always;
                string altitudetext = "";
                switch (Settings["dist_units"])
                {
                    case "Meters":
                        altitudetext = altitude.ToString("0");
                        break;
                    case "Feet":
                        altitudetext = (altitude * 3.28084).ToString("0");
                        break;
                    default:
                        altitudetext = altitude.ToString("0");
                        break;
                }

                marker.ToolTipText = altitudetext;
                g_Moving_Obstacle_Overlay.Markers.Add(marker);
            });

        }

        /// <summary>
        /// Adds a static obstacle to the map. The obstacle is defined as a cylinder extending from the ground to a given altitude. 
        /// </summary>
        /// <param name="radius">Radius of the cylinder in metres </param>
        /// <param name="altitude">Altitude of the top part of the cylinder in metres</param>
        public void MAP_addSObstaclePoly(double radius, double altitude, double Lat, double Lon, string name)
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                //Add obstacle
                GMapPolygon polygon = new GMapPolygon(getCirclePoly(radius, Lat, Lon), name);
                polygon.Stroke = new Pen(Color.Red, 2);
                polygon.Fill = new SolidBrush(Color.FromArgb(100, Color.RoyalBlue));
                g_Stationary_Obstacle_Overlay.Polygons.Add(polygon);

                //Add altitude
                GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(Lat, Lon), new Bitmap(1, 1));
                marker.ToolTipMode = MarkerTooltipMode.Always;
                string altitudetext = "";
                switch (Settings["dist_units"])
                {
                    case "Meters":
                        altitudetext = altitude.ToString("0");
                        break;
                    case "Feet":
                        altitudetext = (altitude * 3.28084).ToString("0");
                        break;
                    default:
                        altitudetext = altitude.ToString("0");
                        break;
                }
                marker.ToolTipText = altitudetext;

                g_Moving_Obstacle_Overlay.Markers.Add(marker);
            });

        }


        /// <summary>
        /// Returns a list of coordinates that define a circle
        /// </summary>
        /// <param name="radius">Radius of the circle in metres</param>
        /// <param name="Lat">Latitude of the centre of the circle</param>
        /// <param name="Lon">Longitude of the centre of the circle</param>
        /// <returns></returns>
        public List<PointLatLng> getCirclePoly(double radius, double Lat, double Lon)
        {
            int numPoints = 200;
            double tempY, tempX;
            double X, Y;
            List<PointLatLng> Points = new List<PointLatLng>();
            Y = MercatorProjection.latToY(Lat);
            X = MercatorProjection.lonToX(Lon);
            for (int i = 0; i < numPoints; i++)
            {
                double point = 2 * Math.PI / numPoints;
                tempY = Y + radius * Math.Sin(i * 2 * Math.PI / numPoints);
                tempX = X + radius * Math.Cos(i * 2 * Math.PI / numPoints);
                Points.Add(new PointLatLng(MercatorProjection.yToLat(tempY), MercatorProjection.xToLon(tempX)));
            }
            return Points;
        }


        /// <summary>
        /// Updates the plane location in the map
        /// </summary>
        /// <param name="location">Location of the plane</param>
        /// <param name="altitude">Altitude of the plane in metres</param>
        /// <param name="heading">Heading of the plane in degrees</param>
        /// <param name="cog">Center of Gravity. Not necessary for plane simulations</param>
        /// <param name="nav_bearing">The compass bearing of where the plane wants to go</param>
        /// <param name="target">The compass bearing of the target waypoint</param>
        /// <param name="radius">Plane current turn radius</param>
        public void MAP_updatePlaneLoc(PointLatLng location, float altitude, float heading, float cog, float nav_bearing, float target, float radius)
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                if (location.Lat != 0 || location.Lng != 0)
                {
                    interoperability.GMapMarkerPlane marker = new interoperability.GMapMarkerPlane(location, zoom, UAS_Scale, Map_Bool_UAS_FixedSize, heading, cog, nav_bearing, target, radius);
                    //Show the altitude always
                    marker.ToolTipMode = MarkerTooltipMode.Always;
                    string altitudetext = "";
                    switch (Settings["dist_units"])
                    {
                        case "Meters":
                            altitudetext = altitude.ToString("0");
                            break;
                        case "Feet":
                            altitudetext = (altitude * 3.28084).ToString("0");
                            break;
                        default:
                            altitudetext = altitude.ToString("0");
                            break;
                    }
                    marker.ToolTipText = altitudetext;
                    g_Plane_Overlay.Markers.Add(marker);
                }

            });
        }


        /// <summary>
        /// Sets the plane GPS location text below the map
        /// </summary>
        public void MAP_updateGPSLabel(string label)
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                UAS_GPS_Label.Text = label;
            });
        }


        /// <summary>
        /// Sets the plane altitude label below the map
        /// </summary>
        /// <param name="altitude_asl">The altitude of the plane below sea level</param>
        /// <param name="altutide_agl">The distance between the plane and the ground</param>
        public void MAP_updateAltLabel(string altitude_asl, string altitude_agl)
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                UAS_Altitude_ASL_Label.Text = altitude_asl;
                UAS_D_Altitude_Label.Text = altitude_agl;
            });
        }

        /// <summary>
        /// Sets the plane AUVSI required altitude label
        /// </summary>
        /// <param name="altitude_asl">The altitude of the plane below sea level</param>
        public void MAP_updateAUVSIAltLabel(string altitude_asl)
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                AUVSI_Alt_Label.Text = "UAS Altitude: " + altitude_asl + " ft ASL";
            });
        }

        /// <summary>
        /// Sets the plane AUVSI required altitude label
        /// </summary>
        /// <param name="airspeed">The altitude of the plane below sea level</param>
        public void MAP_updateAUVSIArspdLabel(string airspeed)
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                AUVSI_ARSPD_Label.Text = "UAS Airspeed: " + airspeed + " Knots";
            });
        }


        /// <summary>
        /// Sets the flight timer below the map
        /// </summary>
        /// <param name="elapsedmiliseconds"></param>
        public void setFlightTimerLabel(long elapsedmiliseconds)
        {
            this.FlightTimeLabel.BeginInvoke((MethodInvoker)delegate ()
            {
                TimeSpan t = TimeSpan.FromMilliseconds(elapsedmiliseconds);
                FlightTimeLabel.Text = string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
            });
        }


        /// <summary>
        /// Adds waypoints to the map, given 
        /// </summary>
        public void MAP_addWP(List<Waypoint> waypoints)
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                interoperability.GMapMarkerWP marker;
                for (int i = 0; i < waypoints.Count(); i++)
                {
                    marker = new interoperability.GMapMarkerWP(new PointLatLng(waypoints[i].latitude, waypoints[i].longitude), i.ToString("0"));
                    //marker.ToolTipMode = MarkerTooltipMode.Always;
                    //marker.ToolTipText = i.ToString();
                    g_WP_Overlay.Markers.Add(marker);
                }
            });
        }

        public void MAP_addImage(PointLatLng p, float heading, float altitude, string path)
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                GMapMarkerImage marker;
                marker = new GMapMarkerImage(p, heading, altitude, path);
                g_WP_Overlay.Markers.Add(marker);

            });
        }

        /// <summary>
        /// Adds a route on the map, given a list of waypoints
        /// </summary>
        public void MAP_addWPRoute(List<Waypoint> _waypoints)
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                List<Waypoint> waypoints = new List<Waypoint>(_waypoints);
                for (int i = 0; i < waypoints.Count() - 1; i++)
                {
                    List<PointLatLng> list = new List<PointLatLng>();
                    list.Add(new PointLatLng(waypoints[i].latitude, waypoints[i].longitude));
                    list.Add(new PointLatLng(waypoints[i + 1].latitude, waypoints[i + 1].longitude));

                    g_WP_Overlay.Routes.Add(new GMapRoute(list, "route") { Stroke = new System.Drawing.Pen(System.Drawing.Color.Yellow, 4) });
                }
            });
        }

        /// <summary>
        /// Updates the Off Axis target, Air Drop, and Emergent Target
        /// </summary>
        public void MAP_updateOFAT_EM_DROP(Mission Current_Mission)
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                //Do not draw if targets are at 0,0

                GMarkerGoogle marker;

                //Off axis target
                if (Current_Mission.off_axis_target_pos.latitude != 0 || Current_Mission.off_axis_target_pos.longitude != 0)
                {
                    marker = new GMarkerGoogle(new PointLatLng(Current_Mission.off_axis_target_pos.latitude, Current_Mission.off_axis_target_pos.longitude), GMarkerGoogleType.yellow_pushpin);
                    marker.ToolTipMode = MarkerTooltipMode.Always;
                    marker.ToolTipText = "OFAT";
                    g_OFAT_EM_DROP_Overlay.Markers.Add(marker);
                }
                else
                {
                    //Console.WriteLine("Did not display off axis because coordinate at 0,0");
                }

                //Air Drop Location
                if (Current_Mission.air_drop_pos.latitude != 0 || Current_Mission.air_drop_pos.longitude != 0)
                {
                    marker = new GMarkerGoogle(new PointLatLng(Current_Mission.air_drop_pos.latitude, Current_Mission.air_drop_pos.longitude), GMarkerGoogleType.yellow_pushpin);
                    marker.ToolTipMode = MarkerTooltipMode.Always;
                    marker.ToolTipText = "Air Drop";
                    g_OFAT_EM_DROP_Overlay.Markers.Add(marker);
                }
                else
                {
                    //Console.WriteLine("Did not display air drop because coordinate at 0,0");
                }

                if (Current_Mission.air_drop_pos.latitude != 0 || Current_Mission.air_drop_pos.longitude != 0)
                {
                    marker = new GMarkerGoogle(new PointLatLng(Current_Mission.emergent_last_known_pos.latitude, Current_Mission.emergent_last_known_pos.longitude), GMarkerGoogleType.yellow_pushpin);
                    marker.ToolTipMode = MarkerTooltipMode.Always;
                    marker.ToolTipText = "Emergent Target";
                    g_OFAT_EM_DROP_Overlay.Markers.Add(marker);
                }
                else
                {

                }
            });
        }

        public void MAP_Update_Overlay()
        {
            IAsyncResult result = this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                Interoperability instance = Interoperability.getinstance();
                if (instance.mapinvalidateObstacle)
                {
                    gMapControl1.Overlays.Remove(g_Moving_Obstacle_Overlay);
                    gMapControl1.Overlays.Remove(g_Stationary_Obstacle_Overlay);
                    gMapControl1.Overlays.Add(g_Moving_Obstacle_Overlay);
                    gMapControl1.Overlays.Add(g_Stationary_Obstacle_Overlay);
                    instance.mapinvalidateObstacle = false;
                }

                if (instance.mapinvalidateWaypoints)
                {
                    gMapControl1.Overlays.Remove(g_WP_Overlay);
                    gMapControl1.Overlays.Add(g_WP_Overlay);
                    instance.mapinvalidateWaypoints = false;
                }

                if (instance.mapinvalidateSearchArea || instance.mapinvalidateGeofence)
                {
                    gMapControl1.Overlays.Remove(g_Static_Overlay);
                    gMapControl1.Overlays.Add(g_Static_Overlay);
                    instance.mapinvalidateGeofence = false;
                    instance.mapinvalidateSearchArea = false;
                }

                if (instance.mapinvalidateOFAT_EM_DROP)
                {
                    gMapControl1.Overlays.Remove(g_OFAT_EM_DROP_Overlay);
                    gMapControl1.Overlays.Add(g_OFAT_EM_DROP_Overlay);
                    instance.mapinvalidateOFAT_EM_DROP = false;
                }

                if (instance.mapinvalidateImage)
                {
                    gMapControl1.Overlays.Remove(g_Image_Overlay);
                    gMapControl1.Overlays.Add(g_Image_Overlay);
                    instance.mapinvalidateImage = false;
                }

                //We always update the plane location, since it is always moving. Unlike everything else
                gMapControl1.Overlays.Remove(g_Plane_Overlay);
                gMapControl1.Overlays.Add(g_Plane_Overlay);

                //gMapControl1.Overlays.Clear();   We don't clear all anymore to save CPU power
                gMapControl1.Invalidate();

            });

            //We call this to block returning to the function until the changes are applied. 
            //This prevents race conditions resulting from calling this too fast 
            this.gMapControl1.EndInvoke(result);

        }

        public void MAP_Clear_Overlays()
        {
            IAsyncResult result = this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {

                Interoperability instance = Interoperability.getinstance();
                if (instance.mapinvalidateObstacle)
                {
                    g_Moving_Obstacle_Overlay.Clear();
                    g_Stationary_Obstacle_Overlay.Clear();

                }

                if (instance.mapinvalidateWaypoints)
                {
                    g_WP_Overlay.Clear();
                }

                if (instance.mapinvalidateSearchArea || instance.mapinvalidateGeofence)
                {
                    g_Static_Overlay.Clear();
                }

                if (instance.mapinvalidateOFAT_EM_DROP)
                {
                    g_OFAT_EM_DROP_Overlay.Clear();
                }

                if (instance.mapinvalidateImage)
                {
                    g_Image_Overlay.Clear();
                }

                g_Plane_Overlay.Clear();
            });

            this.gMapControl1.EndInvoke(result);
        }

        public void MAP_Update_Loc(PointLatLng point)
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                gMapControl1.Position = point;
            });
        }

        private void Flight_Time_Label_DoubleClick(object sender, EventArgs e)
        {
            InteroperabilityCallback(new Interop_Callback_Struct(Interoperability.Interop_Action.Map_Thread_Restart_Flight_Timer));
        }

        private void gMapControl1_OnMapZoomChanged()
        {
            zoom = Convert.ToInt32(gMapControl1.Zoom);
        }

        private void gMapControl1_KeyPress(object sender, KeyPressEventArgs e)
        {
            /*if(e == "t")
            {
                InteroperabilityCallback(7);
            }*/
            Console.WriteLine(e.KeyChar);
            if (PV_Start_Drone_Control.Checked == true)
            {
                MAVLink.mavlink_set_position_target_local_ned_t mav_position = new MAVLink.mavlink_set_position_target_local_ned_t();
                /*Positions are relative to the current vehicle position in a frame based on the vehicle’s current heading.
                Use this to specify a position x metres forward from the current vehicle position, y metres to the right, and z metres down
                (forward, right and down are “positive” values).*/
                mav_position.coordinate_frame = 8;
                //set max velocity to 0.1 meters per second 
                mav_position.vx = 0.2F;
                mav_position.vy = 0.2F;
                mav_position.vz = 0.2F;
                //4088 = 0b0000111111111000 - Use Position Only
                //4039 = 0b0000111111000111 - Use Velocity Only
                mav_position.type_mask = (ushort)4088;

                switch (e.KeyChar)
                {
                    case 'o':
                        //arm quad
                        InteroperabilityCallback(new Interop_Callback_Struct(Interoperability.Interop_Action.MAV_Command_Arm_Disarm,
                            new Mavlink_Command(MAVLink.MAV_CMD.COMPONENT_ARM_DISARM, 1, 21196, 0, 0, 0, 0, 0)));
                        break;
                    case 'p':
                        //disarm quad
                        InteroperabilityCallback(new Interop_Callback_Struct(Interoperability.Interop_Action.MAV_Command_Arm_Disarm,
                            new Mavlink_Command(MAVLink.MAV_CMD.COMPONENT_ARM_DISARM, 0, 21196, 0, 0, 0, 0, 0)));
                        break;
                    //Make the quad move in 1 meter increments -- Hopefully??
                    case 'w':
                        //forward
                        mav_position.x = 1F;
                        InteroperabilityCallback(new Interop_Callback_Struct(Interoperability.Interop_Action.MAV_Command_Set_Position, mav_position));
                        break;
                    case 'a':
                        //left
                        mav_position.y = -1F;
                        InteroperabilityCallback(new Interop_Callback_Struct(Interoperability.Interop_Action.MAV_Command_Set_Position, mav_position));
                        break;
                    case 's':
                        //backwards
                        mav_position.x = -1F;
                        InteroperabilityCallback(new Interop_Callback_Struct(Interoperability.Interop_Action.MAV_Command_Set_Position, mav_position));
                        break;
                    case 'd':
                        //right
                        mav_position.y = 1F;
                        InteroperabilityCallback(new Interop_Callback_Struct(Interoperability.Interop_Action.MAV_Command_Set_Position, mav_position));
                        break;
                    default:
                        //Don't do anything!!!!!!!!!!!!
                        break;
                }
            }
        }

        //-------------------------------------------//
        //                 Menu Strip                //
        //-------------------------------------------//

        private void openSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Settings_GUI_Instance.isOpened)
            {
                Settings_GUI_Instance = new Settings_GUI(InteroperabilityCallback, InteroperabilityGUIAction, Settings);
                Settings_GUI_Instance.ShowDialog();
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Interoperability.getinstance().Current_Mission.unedited == true)
            {
                DialogResult result;
                result = MessageBox.Show("Are you sure you want to create a new file? \nYou will lose any unsaved changes to your mission", "Interoperability Control Panel", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                if (result == DialogResult.OK)
                {
                    Interoperability.Interoperability_Mutex.WaitOne();
                    Interoperability.getinstance().Current_Mission = new Mission();
                    Interoperability.Interoperability_Mutex.ReleaseMutex();

                }
                else
                {
                    //Do nothing
                }
            }
            else
            {
                Interoperability.Interoperability_Mutex.WaitOne();
                Interoperability.getinstance().Current_Mission = new Mission();
                Interoperability.Interoperability_Mutex.ReleaseMutex();

            }

            Interoperability.getinstance().Invalidate_Map();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("       UTAT UAV Interoperability Control Panel    \nDeveloped by Oliver Wu, Davis Wu, and Jesse Wang\nLogo designed by Andrew Ilersich", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            //Stopping and starting the threads causes issues sometimes, where callback 7 will infinitely loop because one on the threads refuses to stop (-_-)

                            //wait until all threads have stopped
                            //InteroperabilityCallback(7);

                            StreamReader reader = new StreamReader(myStream);
                            string text = reader.ReadToEnd();
                            Interoperability.Interoperability_Mutex.WaitOne();
                            Interoperability.getinstance().Current_Mission = new JavaScriptSerializer().Deserialize<Mission>(text);
                            Interoperability.Interoperability_Mutex.ReleaseMutex();

                            //restart all stopped threads 
                            //InteroperabilityCallback(6);

                            if (Settings["gui_format"] == "AUVSI")
                            {
                                this.Text = "UTAT UAV Interoperability Control Panel (AUVSI) - " + Interoperability.getinstance().Current_Mission.name;
                            }
                            else
                            {
                                this.Text = "UTAT UAV Interoperability Control Panel (USC) - " + Interoperability.getinstance().Current_Mission.name;
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                    //MessageBox.Show("Error, Invalid Mission File.\n" + ex.Message);
                    MessageBox.Show("Error, Invalid Mission File.\n" + ex.Message, "Interoperability Control Panel", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            Interoperability.getinstance().Invalidate_Map();
        }

        private void saveMissionAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    Interoperability.Interoperability_Mutex.WaitOne();
                    string thing = new JavaScriptSerializer().Serialize(Interoperability.getinstance().Current_Mission);
                    Interoperability.Interoperability_Mutex.ReleaseMutex();
                    byte[] byteArray = Encoding.UTF8.GetBytes(thing);
                    myStream.Write(byteArray, 0, byteArray.Count());
                    myStream.Close();
                }
            }
        }

        private void editMissionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Interoperability_Mission_Edit_Instance.isOpened)
            {
                Interoperability_Mission_Edit_Instance = new Interoperability_Mission_Edit();
                Interoperability_Mission_Edit_Instance.ShowDialog();
            }
            Interoperability.getinstance().Invalidate_Map();
        }


        //-------------------------------------------//
        //         Right Click Context Menu          //
        //-------------------------------------------//

        private void showGeofenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showGeofenceToolStripMenuItem.Checked == true)
            {
                showGeofenceToolStripMenuItem.Checked = false;
                Geofence_Checkbox.Checked = false;
                Map_Bool_DrawGeofence = false;
            }
            else
            {
                showGeofenceToolStripMenuItem.Checked = true;
                Geofence_Checkbox.Checked = true;
                Map_Bool_DrawGeofence = true;
            }
            Settings["DrawGeofence"] = Map_Bool_DrawGeofence.ToString();
            Settings.Save();
            Interoperability.getinstance().Invalidate_Map();
        }

        private void showSearchAreaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showSearchAreaToolStripMenuItem.Checked == true)
            {
                showSearchAreaToolStripMenuItem.Checked = false;
                SearchArea_Checkbox.Checked = false;
                Map_Bool_DrawSearchArea = false;
            }
            else
            {
                showSearchAreaToolStripMenuItem.Checked = true;
                SearchArea_Checkbox.Checked = true;
                Map_Bool_DrawSearchArea = true;
            }
            Settings["DrawSearchArea"] = Map_Bool_DrawSearchArea.ToString();
            Settings.Save();
            Interoperability.getinstance().Invalidate_Map();
        }

        private void showSRICEmergentDropToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showOFATEmergentDropToolStripMenuItem.Checked == true)
            {
                showOFATEmergentDropToolStripMenuItem.Checked = false;
                OFAT_EM_DROP_CheckBox.Checked = false;
                Map_Bool_DrawOFAT_EM_DROP = false;
            }
            else
            {
                showOFATEmergentDropToolStripMenuItem.Checked = true;
                OFAT_EM_DROP_CheckBox.Checked = true;
                Map_Bool_DrawOFAT_EM_DROP = true;
            }
            Settings["DrawOFAT_EN_DROP"] = Map_Bool_DrawOFAT_EM_DROP.ToString();
            Settings.Save();
            Interoperability.getinstance().Invalidate_Map();
        }

        private void showObstaclesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showObstaclesToolStripMenuItem.Checked == true)
            {
                showObstaclesToolStripMenuItem.Checked = false;
                Obstacles_Checkbox.Checked = false;
                Map_Bool_DrawObstacles = false;
            }
            else
            {
                showObstaclesToolStripMenuItem.Checked = true;
                Obstacles_Checkbox.Checked = true;
                Map_Bool_DrawObstacles = true;
            }
            Settings["DrawObstacles"] = Map_Bool_DrawObstacles.ToString();
            Settings.Save();
            Interoperability.getinstance().Invalidate_Map();
        }

        private void showPlaneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showPlaneToolStripMenuItem.Checked == true)
            {
                showPlaneToolStripMenuItem.Checked = false;
                UASLoc_Checkbox.Checked = false;
                Map_Bool_DrawPlane = false;
            }
            else
            {
                showPlaneToolStripMenuItem.Checked = true;
                UASLoc_Checkbox.Checked = true;
                Map_Bool_DrawPlane = true;
            }
            Settings["DrawPlane"] = Map_Bool_DrawPlane.ToString();
            Settings.Save();
            Interoperability.getinstance().Invalidate_Map();
        }

        private void showWaypointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showWaypointsToolStripMenuItem.Checked == true)
            {
                showWaypointsToolStripMenuItem.Checked = false;
                Waypoints_Checkbox.Checked = false;
                MAP_Bool_DrawWP = false;
            }
            else
            {
                showWaypointsToolStripMenuItem.Checked = true;
                Waypoints_Checkbox.Checked = true;
                MAP_Bool_DrawWP = true;
            }
            Settings["DrawWP"] = MAP_Bool_DrawWP.ToString();
            Settings.Save();
            Interoperability.getinstance().Invalidate_Map();
        }

        private void showCameraTriggerLocation_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();
            openFileDialog1.Filter = "txt files (*.txt)|*.txt";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string lines = File.ReadAllText(openFileDialog1.FileName);
                    Interoperability.getinstance().Current_Mission.all_waypoints.Clear();
                    Interoperability.getinstance().Current_Mission.all_waypoints.AddRange(Interoperability_Mission_Edit.DDtoWaypoints(lines));
                }
                catch
                {

                }
            }
            Interoperability.getinstance().Invalidate_Map();
        }

        private void Update_FP_Button_Click(object sender, EventArgs e)
        {
            //Do not draw if targets are at 0,0
            GMapOverlay tempOverlay = new GMapOverlay();
            GMarkerGoogle marker;
            Mission Current_Mission = Interoperability.getinstance().Current_Mission;

            //Off axis target
            if (Current_Mission.off_axis_target_pos.latitude != 0 || Current_Mission.off_axis_target_pos.longitude != 0)
            {
                marker = new GMarkerGoogle(new PointLatLng(Current_Mission.off_axis_target_pos.latitude, Current_Mission.off_axis_target_pos.longitude), GMarkerGoogleType.yellow_pushpin);
                marker.ToolTipMode = MarkerTooltipMode.Always;
                marker.ToolTipText = "OFAT";
                tempOverlay.Markers.Add(marker);
            }
            else
            {
                //Console.WriteLine("Did not display off axis because coordinate at 0,0");
            }

            //Air Drop Location
            if (Current_Mission.air_drop_pos.latitude != 0 || Current_Mission.air_drop_pos.longitude != 0)
            {
                marker = new GMarkerGoogle(new PointLatLng(Current_Mission.air_drop_pos.latitude, Current_Mission.air_drop_pos.longitude), GMarkerGoogleType.yellow_pushpin);
                marker.ToolTipMode = MarkerTooltipMode.Always;
                marker.ToolTipText = "Air Drop";
                tempOverlay.Markers.Add(marker);
            }
            else
            {
                //Console.WriteLine("Did not display air drop because coordinate at 0,0");
            }

            if (Current_Mission.air_drop_pos.latitude != 0 || Current_Mission.air_drop_pos.longitude != 0)
            {
                marker = new GMarkerGoogle(new PointLatLng(Current_Mission.emergent_last_known_pos.latitude, Current_Mission.emergent_last_known_pos.longitude), GMarkerGoogleType.yellow_pushpin);
                marker.ToolTipMode = MarkerTooltipMode.Always;
                marker.ToolTipText = "Emergent Target";
                tempOverlay.Markers.Add(marker);
            }
            // Interoperability.getinstance().Host.FPGMapControl.Overlays.Remove(tempOverlay);
            Interoperability.getinstance().Host.FPGMapControl.Overlays.Add(tempOverlay);
        }

        private void getMavWaypoint_Button_Click(object sender, EventArgs e)
        {
            Interoperability.getinstance().Host.GetWPs();

            MAVLinkInterface port = new MAVLinkInterface();
            //port.
        }

        private void SDA_Import_WP_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            StreamReader reader = new StreamReader(myStream);
                            string text = reader.ReadToEnd();
                            string[] lines = text.Split(new[] { "/r/n" }, StringSplitOptions.RemoveEmptyEntries);

                        }
                    }
                }
                catch (Exception ex)
                {

                    MessageBox.Show("Error, Invalid Mission File.\n" + ex.Message, "Interoperability Control Panel", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
        }

        private void dronepv_writewp_button_Click(object sender, EventArgs e)
        {
            InteroperabilityCallback(new Interop_Callback_Struct(Interoperability.Interop_Action.TEST));
        }

        private void button7_Click(object sender, EventArgs e)
        {
            InteroperabilityCallback(new Interop_Callback_Struct(Interoperability.Interop_Action.Drone_Cleaning_Server_Start));
        }

        private void Auto_Drone_Control_Button_Click(object sender, EventArgs e)
        {
            InteroperabilityCallback(new Interop_Callback_Struct(Interoperability.Interop_Action.PV_Drone_Control_Start));
        }

        public void PV_Add_Status(string status)
        {
            this.PV_Status_TextBox.BeginInvoke((MethodInvoker)delegate ()
            {
                PV_Status_TextBox.AppendText(status);
            });
        }

        public void PV_Clear_Status()
        {
            PV_Status_TextBox.Text = "";
        }

        private void Start_PV_Power_Read_Button_Click(object sender, EventArgs e)
        {
            InteroperabilityCallback(new Interop_Callback_Struct(Interoperability.Interop_Action.PV_Power_Read));
        }

        public void Update_PV_Voltage(double voltage)
        {
            this.Char_Cell_Voltage_Text.BeginInvoke((MethodInvoker)delegate ()
            {
                Char_Cell_Voltage_Text.Text = "Chracterization Cell Voltage: " + voltage.ToString();
            });
        }

        public void Update_PV_Temperature(double temperature)
        {
            this.Solar_Cell_Temperature_Text.BeginInvoke((MethodInvoker)delegate ()
            {
                Solar_Cell_Temperature_Text.Text = "Solar Cell Temperature: " + temperature.ToString() + "C";
            });

        }

        public void Update_PV_Power_Output(double power)
        {
            this.panel_power_output_label.BeginInvoke((MethodInvoker)delegate ()
            {
                panel_power_output_label.Text = "Panel Power Output: " + power.ToString() + "W";
            });

        }

        public void Update_PV_Expected_Power_Output(double power)
        {
            this.expected_panel_power_output_label.BeginInvoke((MethodInvoker)delegate ()
            {
                expected_panel_power_output_label.Text = "Expected Panel Power Output: " + power.ToString("F2") + "W";
            });

        }

        public void Update_Last_Panel_Update(Int64 unixtime)
        {
            this.last_panel_update_label.BeginInvoke((MethodInvoker)delegate ()
            {
                System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddSeconds(unixtime).ToLocalTime();
                last_panel_update_label.Text = "Last Panel Update: " + dtDateTime.ToString();
            });
        }

        //-------------------------------------------//
        //             Bottle Drop Tab               //
        //-------------------------------------------//
        private void bottle_drop_button_Click(object sender, EventArgs e)
        {
            if (bottle_drop_start_button.Text == "Start Bottle Drop")
            {
                InteroperabilityCallback(new Interop_Callback_Struct(Interoperability.Interop_Action.Bottle_Drop_Start));
                bottle_drop_start_button.Text = "Stop Bottle Drop";
                Bottle_Drop_WP_No.ReadOnly = true;
                Bottle_Drop_Lat.ReadOnly = true;
                Bottle_Drop_Long.ReadOnly = true;
            }
            else
            {
                InteroperabilityCallback(new Interop_Callback_Struct(Interoperability.Interop_Action.Bottle_Drop_Stop));
                bottle_drop_start_button.Text = "Start Bottle Drop";
                Bottle_Drop_WP_No.ReadOnly = false;
                Bottle_Drop_Lat.ReadOnly = false;
                Bottle_Drop_Long.ReadOnly = false;
            }

        }

        public string bottle_drop_get_Lat()
        {
            return Bottle_Drop_Lat.Text;
        }

        public string bottle_drop_get_Long()
        {
            return Bottle_Drop_Long.Text;
        }

        public int bottle_drop_get_WP_NO()
        {
            return (int)Bottle_Drop_WP_No.Value;
        }

        public void update_bottle_drop_status(string status)
        {
            Bottle_Drop_Status.Text = status;
            this.Bottle_Drop_Status.BeginInvoke((MethodInvoker)delegate ()
            {
                Bottle_Drop_Status.Text = status;
            });
        }

        //Image Upload Code
        async private void Image_Upload_Upload_Button_Click(object sender, EventArgs e)
        {
            string address = Settings["address"];
            string username = Settings["username"];
            string password = Settings["password"];

            CookieContainer cookies = new CookieContainer();

            try
            {
                using (var client = new HttpClient())
                {

                    TimeSpan timeout = new TimeSpan(0, 0, 0, 10);
                    //client.Timeout = timeout;

                    client.BaseAddress = new Uri(address); // This seems to change every time

                    // Log in.
                    Console.WriteLine("---INITIAL LOGIN---");
                    var v = new Dictionary<string, string>();
                    v.Add("username", username);
                    v.Add("password", password);
                    var auth = new FormUrlEncodedContent(v);
                    //Get authentication cookie. Cookie is automatically sent after being sent
                    HttpResponseMessage resp = await client.PostAsync("/api/login", auth);
                    Console.WriteLine("Login POST result: " + resp.Content.ReadAsStringAsync().Result);
                    Console.WriteLine("---LOGIN FINISHED---");

                    if (!resp.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Invalid Credentials");
                        return;

                    }
                    else
                    {
                        Console.WriteLine("Credentials Valid");
                    }


                    Image_Uploads_Target_ID temp_deseralizer = new Image_Uploads_Target_ID();

                    foreach (Image_Uploads i in Target_List)
                    {

                        //Upload Target 
                        string thing = new JavaScriptSerializer().Serialize(i);

                        var content = new StringContent(thing, Encoding.UTF8, "application/json");
                        var result = await client.PostAsync("/api/odlcs", content);

                        Console.WriteLine("Server_info GET result: " + result.Content.ReadAsStringAsync().Result);
                        temp_deseralizer = new JavaScriptSerializer().Deserialize<Image_Uploads_Target_ID>(result.Content.ReadAsStringAsync().Result);

                        Console.WriteLine("TARGET ID:" + temp_deseralizer.id.ToString());



                        //Upload Image

                        //convert filestream to byte array
                        MemoryStream stream = new MemoryStream();
                        i.image.Save(stream, ImageFormat.Jpeg);

                        byte[] fileBytes;
                        fileBytes = stream.ToArray();


                        //load the image byte[] into a System.Net.Http.ByteArrayContent
                        var imageBinaryContent = new ByteArrayContent(fileBytes);

                        //create a System.Net.Http.MultiPartFormDataContent
                        ByteArrayContent content2 = new ByteArrayContent(fileBytes);
                        content2.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

                        //make the POST request using the URI enpoint and the MultiPartFormDataContent
                        result = await client.PostAsync("/api/odlcs/" + temp_deseralizer.id.ToString() + "/image", content2);
                        Console.WriteLine(result.StatusCode.ToString());
                        Console.WriteLine(result.Content.ReadAsStringAsync().Result);



                    }

                }
            }

            //If this exception is thrown, then the thread will end soon after. Have no way to restart manually unless I get the loop working
            catch (Exception c)
            {
                Console.WriteLine("Error, exception thrown in Image upload thread");
                Console.WriteLine(c.Message);
                Console.WriteLine(c.InnerException);
            }
        }

        private void Image_Upload_Download_Button_Click(object sender, EventArgs e)
        {
            //Don't do anything here yet
        }

        bool Image_Upload_Ignore = false;

        private void Image_Upload_Add_Target_Click(object sender, EventArgs e)
        {
            Image_Upload_Target_Select.Items.Insert(Image_Upload_Target_Select.Items.Count, "Target" + (Image_Upload_Target_Select.Items.Count + 1).ToString());
            Target_List.Add(new Image_Uploads());

            //prevent infinite loops 
            Image_Upload_Ignore = true;

            Image_Upload_Target_Select.SelectedIndex = Image_Upload_Target_Select.Items.Count - 1;

            Image_Upload_Ignore = false;

            Image_Upload_Latitude.Text = "";
            Image_Upload_Longitude.Text = "";
            Image_Upload_Orientation.SelectedIndex = 0;
            Image_Upload_Shape.SelectedIndex = 0;
            Image_Upload_Type.SelectedIndex = 0;
            Image_Upload_Alphanumeric.Text = "";
            Image_Upload_Alphanumeric_Colour.SelectedIndex = 0;
            Image_Upload_Background_Colour.SelectedIndex = 0;




        }

        private void Image_Upload_Target_Select_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Image_Upload_Ignore)
            {
                return;
            }
            int index = Image_Upload_Target_Select.SelectedIndex;

            //We need to create a new target 
            /*if (index == Image_Upload_Target_Select.Items.Count - 1)
            {
                Target_List.Add(new Image_Uploads());
                Image_Upload_Latitude.Text = "0";
                Image_Upload_Longitude.Text = "0";
                Image_Upload_Orientation.SelectedIndex = 0;
                Image_Upload_Shape.SelectedIndex = 0; ;
                Image_Upload_Type.SelectedIndex = 0; ;
                Image_Upload_Alphanumeric.Text = "";
                Image_Upload_Alphanumeric_Colour.SelectedIndex = 0;
                Image_Upload_Background_Colour.SelectedIndex = 0;
            }
            //show a previously created target
            else
            {
                Target_List[index].latitude = 0;
                Target_List[index].longitude = 0;
                Target_List[index].orientation = "";
                Target_List[index].background_colour = "";
                Target_List[index].alphanumeric = "";
                Target_List[index].alphanumeric_colour = "";
                Target_List[index].shape = "";
                Target_List[index].description = "";
            }*/

            Image_Upload_Latitude.Text = Target_List[index].latitude;
            Image_Upload_Longitude.Text = Target_List[index].longitude;
            Image_Upload_Orientation.SelectedIndex = Image_Upload_Orientation_to_Index(Target_List[index].orientation);
            Image_Upload_Shape.SelectedIndex = Image_Upload_Shape_to_Index(Target_List[index].shape);
            Image_Upload_Type.SelectedIndex = Image_Upload_Type_to_Index(Target_List[index].type);
            Image_Upload_Alphanumeric.Text = Target_List[index].alphanumeric;
            Image_Upload_Alphanumeric_Colour.SelectedIndex = Image_Upload_Colour_to_Index(Target_List[index].alphanumeric_colour); ;
            Image_Upload_Background_Colour.SelectedIndex = Image_Upload_Colour_to_Index(Target_List[index].background_colour);
            Image_Upload_Description.Text = Target_List[index].description;
            Image_Upload_Picture.Image = Target_List[index].image;


            if (Image_Upload_Type.SelectedIndex != 2)
            {
                Image_Upload_Description.ReadOnly = true;
            }
            else
            {
                Image_Upload_Description.ReadOnly = false;
            }


            // Stretches the image to fit the pictureBox.
            /*pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            MyImage = new Bitmap(fileToDisplay);
            pictureBox1.ClientSize = new Size(xSize, ySize);
            pictureBox1.Image = (Image)MyImage;*/

        }

        private int Image_Upload_Type_to_Index(string colour)
        {
            switch (colour)
            {
                case "standard":
                    return 0;
                case "off_axis":
                    return 1;
                case "emergent":
                    return 2;
                default:
                    return -1;
            }
        }

        private int Image_Upload_Shape_to_Index(string colour)
        {
            switch (colour)
            {
                case "circle":
                    return 0;
                case "semicircle":
                    return 1;
                case "quarter_circle":
                    return 2;
                case "triangle":
                    return 3;
                case "square":
                    return 4;
                case "rectangle":
                    return 5;
                case "trapezoid":
                    return 6;
                case "pentagon":
                    return 7;
                case "hexagon":
                    return 8;
                case "heptagon":
                    return 9;
                case "octagon":
                    return 10;
                case "star":
                    return 11;
                case "cross":
                    return 12;
                default:
                    return -1;
            }
        }

        private int Image_Upload_Orientation_to_Index(string colour)
        {
            switch (colour)
            {
                case "N":
                    return 0;
                case "NE":
                    return 1;
                case "E":
                    return 2;
                case "SE":
                    return 3;
                case "S":
                    return 4;
                case "SW":
                    return 5;
                case "W":
                    return 6;
                case "NW":
                    return 7;
                default:
                    return -1;
            }
        }

        private int Image_Upload_Colour_to_Index(string colour)
        {
            switch (colour)
            {
                case "white":
                    return 0;
                case "black":
                    return 1;
                case "gray":
                    return 2;
                case "red":
                    return 3;
                case "blue":
                    return 4;
                case "green":
                    return 5;
                case "yellow":
                    return 6;
                case "purple":
                    return 7;
                case "brown":
                    return 8;
                case "orange":
                    return 9;
                default:
                    return -1;
            }
        }

        private void Image_Upload_Latitude_TextChanged(object sender, EventArgs e)
        {
            int index = Image_Upload_Target_Select.SelectedIndex;
            Target_List[index].latitude = Image_Upload_Latitude.Text;
        }

        private void Image_Upload_Longitude_TextChanged(object sender, EventArgs e)
        {
            int index = Image_Upload_Target_Select.SelectedIndex;
            Target_List[index].longitude = Image_Upload_Longitude.Text;
        }

        private void Image_Upload_Orientation_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = Image_Upload_Target_Select.SelectedIndex;
            Target_List[index].orientation = Image_Upload_Orientation.Text;
        }

        private void Image_Upload_Background_Colour_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = Image_Upload_Target_Select.SelectedIndex;
            Target_List[index].background_colour = Image_Upload_Background_Colour.Text;

        }

        private void Image_Upload_Alphanumeric_TextChanged(object sender, EventArgs e)
        {
            int index = Image_Upload_Target_Select.SelectedIndex;
            Target_List[index].alphanumeric = Image_Upload_Alphanumeric.Text;
        }

        private void Image_Upload_Alphanumeric_Colour_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = Image_Upload_Target_Select.SelectedIndex;
            Target_List[index].alphanumeric_colour = Image_Upload_Alphanumeric_Colour.Text;
        }

        private void Image_Upload_Shape_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = Image_Upload_Target_Select.SelectedIndex;
            Target_List[index].shape = Image_Upload_Shape.Text;
        }

        private void Image_Upload_Description_TextChanged(object sender, EventArgs e)
        {
            int index = Image_Upload_Target_Select.SelectedIndex;
            Target_List[index].description = Image_Upload_Description.Text;
        }

        private void Image_Upload_Type_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = Image_Upload_Target_Select.SelectedIndex;
            Target_List[index].type = Image_Upload_Type.Text;

            if (Image_Upload_Type.SelectedIndex != 2)
            {
                Image_Upload_Description.ReadOnly = true;
            }
            else
            {
                Image_Upload_Description.ReadOnly = false;
            }
        }

        private void Image_Upload_Select_Image_Button_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "jpg files (*.jpg)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            int index = Image_Upload_Target_Select.SelectedIndex;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Bitmap MyImage;
                    Image_Upload_Picture.SizeMode = PictureBoxSizeMode.StretchImage;
                    MyImage = new Bitmap(openFileDialog1.FileName);
                    Image_Upload_Picture.Image = MyImage;
                    Target_List[index].image = MyImage;
                }
                catch (Exception ex)
                {

                    MessageBox.Show("Error, Invalid Image File.\n" + ex.Message, "Interoperability Control Panel", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
        }

        private void Image_Upload_Latitude_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != '-'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '-') && ((sender as TextBox).Text.IndexOf('-') > -1))
            {
                e.Handled = true;
            }
        }

        private void Image_Upload_Longitude_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != '-'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '-') && ((sender as TextBox).Text.IndexOf('-') > -1))
            {
                e.Handled = true;
            }
        }

    }

}
