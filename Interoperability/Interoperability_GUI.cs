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
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

//For javascript serializer
using System.Web.Script.Serialization;

namespace Interoperability_GUI_Forms
{
    public partial class Interoperability_GUI_Main : Form
    {
        Action<Interoperability.Interop_Action> InteroperabilityCallback;

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
        GMapOverlay g_OFAT_EN_DROP_Overlay;


        //Used for map control thread
        protected bool MAP_Bool_DrawWP = true;
        protected bool Map_Bool_DrawObstacles = true;
        protected bool Map_Bool_DrawPlane = true;
        protected bool Map_Bool_DrawGeofence = true;
        protected bool Map_Bool_DrawSearchArea = true;
        protected bool Map_Bool_DrawOFAT_EN_DROP = true;
        protected bool Map_Bool_UAS_FixedSize = false;
        protected bool MAP_Bool_Autopan_Enable = false;


        protected string gui_format = "AUVSI";
        private List<TabPage> TabList = new List<TabPage>();

        //GMAP Zoom
        private int zoom = 0;

        //Container for all possible targets
        List<PointLatLng> PossibleTargets;  //Targets that are found through the FPV camera
        List<PointLatLng> FoundTargets;     //Targets found through Davis's algorithm

        public Interoperability_GUI_Main(Action<Interoperability.Interop_Action> _InteroperabilityCallback, Interoperability_Settings _Settings)
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
            MAP_Settings_Init_Bool(ref Settings, ref Map_Bool_DrawOFAT_EN_DROP, "DrawOFAT_EN_DROP");
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
            g_OFAT_EN_DROP_Overlay = new GMapOverlay("OFAT_EN_DROP_Overlay");

        }

        private void Interoperability_GUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            InteroperabilityCallback(Interoperability.Interop_Action.Stop_All_Threads_Quit);
            isOpened = false;
        }

        private void showInteroperabilityControlPanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Opening Interoperability Control Panel");
            InteroperabilityCallback(Interoperability.Interop_Action.Show_Interop_GUI);
        }

        private void Interoperability_GUI_Main_Shown(object sender, EventArgs e)
        {
            isOpened = true;
        }


        private void InteropGUIButton_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                InteroperabilityCallback(Interoperability.Interop_Action.Easter_Egg_Action);
            }
            else if (e.Button == MouseButtons.Left)
            {
                Console.WriteLine("Opening Interoperability Control Panel");
                InteroperabilityCallback(Interoperability.Interop_Action.Show_Interop_GUI);
            }

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

            showOFATEmergentDropToolStripMenuItem.Checked = Map_Bool_DrawOFAT_EN_DROP;
            OFAT_EM_DROP_CheckBox.Checked = Map_Bool_DrawOFAT_EN_DROP;

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
            else
            {
                InteroperabilityGUIAction(1);
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
            InteroperabilityCallback(Interoperability.Interop_Action.Telemetry_Thread_Reset_Stats);
        }

        private void Telem_Start_Stop_Button_Click(object sender, EventArgs e)
        {
            if (Telem_Start_Stop_Button.Text == "Start")
            {
                //Start
                InteroperabilityCallback(Interoperability.Interop_Action.Telemtry_Thread_Start);
                Telem_Start_Stop_Button.Text = "Stop";
            }
            else
            {
                //Stop
                InteroperabilityCallback(Interoperability.Interop_Action.Telemtry_Thread_Stop);
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
                InteroperabilityCallback(Interoperability.Interop_Action.Obstacle_SDA_Thread_Start);
                SDA_Start_Stop_Button.Text = "Stop SDA Polling";
            }
            else
            {
                //Stop
                InteroperabilityCallback(Interoperability.Interop_Action.Obstacle_SDA_Thread_Stop);
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
                InteroperabilityCallback(Interoperability.Interop_Action.SDA_Plane_Simulator_Thread_Start);
            }
            else
            {
                SDA_Plane_Simulation_Start_Button.Text = "Start Simulation";
                InteroperabilityCallback(Interoperability.Interop_Action.SDA_Plane_Simulator_Thread_Stop);
            }

        }

        private void SDA_Start_Algorithm_Button_Click(object sender, EventArgs e)
        {
            if (SDA_Start_Algorithm_Button.Text == "Start Algorithm")
            {
                SDA_Start_Algorithm_Button.Text = "Stop Algorithm";
                InteroperabilityCallback(Interoperability.Interop_Action.SDA_Avoidance_Algorithm_Thread_Start);
            }
            else
            {
                SDA_Start_Algorithm_Button.Text = "Start Algorithm";
                InteroperabilityCallback(Interoperability.Interop_Action.SDA_Avoidance_Algorithm_Thread_Stop);
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
        }

        private void OFAT_EM_DROP_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!OFAT_EM_DROP_CheckBox.Checked)
            {
                showOFATEmergentDropToolStripMenuItem.Checked = false;
                Map_Bool_DrawOFAT_EN_DROP = false;
            }
            else
            {
                showOFATEmergentDropToolStripMenuItem.Checked = true;
                Map_Bool_DrawOFAT_EN_DROP = true;
            }
            Settings["DrawOFAT_EN_DROP"] = Map_Bool_DrawOFAT_EN_DROP.ToString();
            Settings.Save();
        }

        private void UAS_Trackbar_Scroll(object sender, EventArgs e)
        {
            UAS_Scale = UAS_Trackbar.Value;
            Settings["UAS_Scale"] = UAS_Scale.ToString();
            Settings.Save();
        }

        private void Fixed_UAS_Size_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            Map_Bool_UAS_FixedSize = Fixed_UAS_Size_Checkbox.Checked;
            Settings["UAS_Fixedsize"] = Map_Bool_UAS_FixedSize.ToString();
            Settings.Save();
        }

        private void AutoPan_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            MAP_Bool_Autopan_Enable = AutoPan_Checkbox.Checked;
            Settings["MAP_Autopan"] = MAP_Bool_Autopan_Enable.ToString();
            Settings.Save();
        }

        private void gMapControl1_OnMapDrag()
        {
            MAP_Bool_Autopan_Enable = false;
            AutoPan_Checkbox.Checked = false;
            Settings["MAP_Autopan"] = MAP_Bool_Autopan_Enable.ToString();
            Settings.Save();
        }

        private void Mission_Download_Click(object sender, EventArgs e)
        {
            InteroperabilityCallback(Interoperability.Interop_Action.Mission_Download_Run);
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

        public bool getDrawOFAT_EN_DROP()
        {
            return Map_Bool_DrawOFAT_EN_DROP;
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
                InteroperabilityCallback(Interoperability.Interop_Action.Callout_Thread_Start);
            }
            else
            {
                Callout_StartStop_Button.Text = "Start";
                InteroperabilityCallback(Interoperability.Interop_Action.Callout_Thread_Stop);
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
                    GMapMarkerPlane marker = new GMapMarkerPlane(location, zoom, UAS_Scale, Map_Bool_UAS_FixedSize, heading, cog, nav_bearing, target, radius);
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
                GMapMarkerWP marker;
                for (int i = 0; i < waypoints.Count(); i++)
                {
                    marker = new GMapMarkerWP(new PointLatLng(waypoints[i].latitude, waypoints[i].longitude), i.ToString("0"));
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
        public void MAP_addWPRoute(List<Waypoint> waypoints)
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
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
                    g_OFAT_EN_DROP_Overlay.Markers.Add(marker);
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
                    g_OFAT_EN_DROP_Overlay.Markers.Add(marker);
                }
                else
                {
                    //Console.WriteLine("Did not display air drop because coordinate at 0,0");
                }

                if (Current_Mission.air_drop_pos.latitude != 0 || Current_Mission.air_drop_pos.longitude != 0)
                {
                    marker = new GMarkerGoogle(new PointLatLng(Current_Mission.emergent_lkp.latitude, Current_Mission.emergent_lkp.longitude), GMarkerGoogleType.yellow_pushpin);
                    marker.ToolTipMode = MarkerTooltipMode.Always;
                    marker.ToolTipText = "Emergent Target";
                    g_OFAT_EN_DROP_Overlay.Markers.Add(marker);
                }
                else
                {

                }
            });
        }

        public void MAP_Update_Overlay()
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                gMapControl1.Overlays.Clear();
                gMapControl1.Overlays.Add(g_Static_Overlay);
                gMapControl1.Overlays.Add(g_Image_Overlay);
                gMapControl1.Overlays.Add(g_WP_Overlay);
                gMapControl1.Overlays.Add(g_Moving_Obstacle_Overlay);
                gMapControl1.Overlays.Add(g_Stationary_Obstacle_Overlay);
                gMapControl1.Overlays.Add(g_OFAT_EN_DROP_Overlay);
                gMapControl1.Overlays.Add(g_Plane_Overlay);
                gMapControl1.Invalidate();
            });
        }

        public void MAP_Clear_Overlays()
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                g_Moving_Obstacle_Overlay.Clear();
                g_Image_Overlay.Clear();
                g_Static_Overlay.Clear();
                g_Stationary_Obstacle_Overlay.Clear();
                g_OFAT_EN_DROP_Overlay.Clear();
                g_Plane_Overlay.Clear();
                g_WP_Overlay.Clear();
            });
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
            InteroperabilityCallback(Interoperability.Interop_Action.Map_Thread_Restart_Flight_Timer);
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

            Console.Beep(433, 100);

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
        }

        private void showSRICEmergentDropToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showOFATEmergentDropToolStripMenuItem.Checked == true)
            {
                showOFATEmergentDropToolStripMenuItem.Checked = false;
                OFAT_EM_DROP_CheckBox.Checked = false;
                Map_Bool_DrawOFAT_EN_DROP = false;
            }
            else
            {
                showOFATEmergentDropToolStripMenuItem.Checked = true;
                OFAT_EM_DROP_CheckBox.Checked = true;
                Map_Bool_DrawOFAT_EN_DROP = true;
            }
            Settings["DrawOFAT_EN_DROP"] = Map_Bool_DrawOFAT_EN_DROP.ToString();
            Settings.Save();
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
        }

        
    }

}
