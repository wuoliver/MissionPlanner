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
        Action<int> InteroperabilityCallback;

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
        GMapOverlay Static_Overlay;
        GMapOverlay Stationary_Obstacle_Overlay;
        GMapOverlay Moving_Obstacle_Overlay;
        GMapOverlay Plane_Overlay;
        GMapOverlay WP_Overlay;
        GMapOverlay OFAT_EN_DROP_Overlay;


        //Used for map control thread
        protected bool DrawWP = true;
        protected bool DrawObstacles = true;
        protected bool DrawPlane = true;
        protected bool DrawGeofence = true;
        protected bool DrawSearchArea = true;
        protected bool DrawOFAT_EN_DROP = true;
        protected bool UAS_FixedSize = false;
        protected bool MAP_Autopan = false;


        protected string gui_format = "AUVSI";
        private List<TabPage> TabList = new List<TabPage>();

        //GMAP Zoom
        private int zoom = 0;

        //Container for all possible targets
        List<PointLatLng> PossibleTargets;  //Targets that are found through the FPV camera
        List<PointLatLng> FoundTargets;     //Targets found through Davis's algorithm

        public Interoperability_GUI_Main(Action<int> _InteroperabilityCallback, Interoperability_Settings _Settings)
        {
            Console.WriteLine("Created GUI");
            InitializeComponent();
            InteroperabilityCallback = _InteroperabilityCallback;

            //Get Settings object, used for server settings
            Settings = _Settings;

            //Must be called after settings
            MAP_Settings_Init_Bool(ref Settings, ref DrawWP, "DrawWP");
            MAP_Settings_Init_Bool(ref Settings, ref DrawObstacles, "DrawObstacles");
            MAP_Settings_Init_Bool(ref Settings, ref DrawPlane, "DrawPlane");
            MAP_Settings_Init_Bool(ref Settings, ref DrawGeofence, "DrawGeofence");
            MAP_Settings_Init_Bool(ref Settings, ref DrawSearchArea, "DrawSearchArea");
            MAP_Settings_Init_Bool(ref Settings, ref DrawOFAT_EN_DROP, "DrawOFAT_EN_DROP");
            MAP_Settings_Init_Bool(ref Settings, ref UAS_FixedSize, "UAS_Fixedsize");
            MAP_Settings_Init_Bool(ref Settings, ref MAP_Autopan, "MAP_Autopan");


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

            Static_Overlay = new GMapOverlay("Static_Overlays");
            Stationary_Obstacle_Overlay = new GMapOverlay("Stationary_Obstacles");
            Plane_Overlay = new GMapOverlay("Plane_Overlay");
            Moving_Obstacle_Overlay = new GMapOverlay("Moving_Obstacle");
            WP_Overlay = new GMapOverlay("Waypoints");
            OFAT_EN_DROP_Overlay = new GMapOverlay("OFAT_EN_DROP_Overlay");

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
                    //Interoperability_GUI_Tab.TabPages.Add(TabList[0]); //Telemtry Upload Tab
                    //Interoperability_GUI_Tab.TabPages.Add(TabList[1]); //SDA Tab
                    Interoperability_GUI_Tab.TabPages.Add(TabList[2]); //Map control tab
                    //Interoperability_GUI_Tab.TabPages.Add(TabList[3]); //Image Tab
                    Interoperability_GUI_Tab.TabPages.Add(TabList[4]); //Callout Tab
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
            showGeofenceToolStripMenuItem.Checked = DrawGeofence;
            Geofence_Checkbox.Checked = DrawGeofence;

            showSearchAreaToolStripMenuItem.Checked = DrawSearchArea;
            SearchArea_Checkbox.Checked = DrawSearchArea;

            showObstaclesToolStripMenuItem.Checked = DrawObstacles;
            Obstacles_Checkbox.Checked = DrawObstacles;

            showPlaneToolStripMenuItem.Checked = DrawPlane;
            UASLoc_Checkbox.Checked = DrawPlane;

            showWaypointsToolStripMenuItem.Checked = DrawWP;
            Waypoints_Checkbox.Checked = DrawWP;

            showOFATEmergentDropToolStripMenuItem.Checked = DrawOFAT_EN_DROP;
            OFAT_EM_DROP_CheckBox.Checked = DrawOFAT_EN_DROP;

            UAS_Trackbar.Value = UAS_Scale;
            Fixed_UAS_Size_Checkbox.Checked = UAS_FixedSize;

            AutoPan_Checkbox.Checked = MAP_Autopan;

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

        public ToolStripItem getContextMenu()
        {
            return MissionPlanner_ContextMenuStrip.Items[0];
        }
        public ToolStripItem getMenuStrip()
        {
            return MissionPlannerMenuAddon.Items[0];
        }

        public int getTelemPollRate()
        {
            return telemPollRate;
        }

        public int getMapRefreshRate()
        {
            return mapRefreshRate;
        }
        public int getsdaPollRate()
        {
            return sdaPollRate;
        }

        public int getCalloutPeriod()
        {
            return Callout_period;
        }

        public int getCalloutminAirspeed()
        {
            return Callout_period;
        }
        public Settings_GUI getSettings_GUI()
        {
            return Settings_GUI_Instance;
        }

        public bool getSpeechRecognition_Enabled()
        {
            return SpeechRecognition_Enabled;
        }

        public string getCalloutMode()
        {
            return Callout_Mode_ComboBox.Text;
        }


        /*private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }
        */

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
            if (resp != this.TelemServerResp.Text)
            {
                this.TelemServerResp.BeginInvoke((MethodInvoker)delegate ()
                {
                    this.TelemServerResp.Text = resp;
                });
            }
        }

        public void setFlightTimerLabel(long elapsedmiliseconds)
        {
            this.FlightTimeLabel.BeginInvoke((MethodInvoker)delegate ()
            {
                TimeSpan t = TimeSpan.FromMilliseconds(elapsedmiliseconds);
                FlightTimeLabel.Text = string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
            });
        }

        public void SDAResp(string resp)
        {
            if (resp != this.SDA_ServerResponseTextBox.Text)
            {
                this.SDA_ServerResponseTextBox.BeginInvoke((MethodInvoker)delegate ()
                {
                    this.SDA_ServerResponseTextBox.Text = resp;
                });
            }
        }

        public int getPlaneSimulationAirspeed()
        {
            return Convert.ToInt32(Plane_Simulated_Airspeed_Select.Value);
        }

        //Function is called when the user clicks a button thing
        public void AddTarget()
        {

        }



        public void setObstacles(Obstacles _Obstacles)
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

        private void Server_Settings_Click(object sender, EventArgs e)
        {

        }

        private void openSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Settings_GUI_Instance.isOpened)
            {
                Settings_GUI_Instance = new Settings_GUI(InteroperabilityCallback, InteroperabilityGUIAction, Settings);
                Settings_GUI_Instance.ShowDialog();
            }
        }

        /*private void Mission_ImportExport_Button_Click(object sender, EventArgs e)
        {
            if (!Interoperability_Mission_Import_Instance.isOpened)
            {
                Interoperability_Mission_Import_Instance = new Interoperability_Mission_Import(Server_Mission_List);
                Interoperability_Mission_Import_Instance.ShowDialog();
            }

        }*/

        private void Reset_Stats_Click(object sender, EventArgs e)
        {
            //Restarts the Telemetry Upload Stats
            InteroperabilityCallback(4);
        }

        private void SDA_Start_Stop_Button_Click(object sender, EventArgs e)
        {
            if (SDA_Start_Stop_Button.Text == "Start SDA Polling")
            {
                //Start
                InteroperabilityCallback(1);
                SDA_Start_Stop_Button.Text = "Stop SDA Polling";
            }
            else
            {
                //Stop
                InteroperabilityCallback(2);
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

        private void Mission_Enable_Click(object sender, EventArgs e)
        {
            InteroperabilityCallback(3);
        }

        //Function called when the map is loaded
        private void gMapControl1_Load(object sender, EventArgs e)
        {
            gMapControl1.DragButton = MouseButtons.Left;
            gMapControl1.Position = new PointLatLng(38.145228, -76.427938); //AUVSI 
            gMapControl1.Zoom = 15;
            gMapControl1.MapProvider = GMap.NET.MapProviders.GoogleSatelliteMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache;
        }

        //Used to add polygons to define things such as: 
        // -Mission Area
        // -Search Area
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
                Static_Overlay.Polygons.Add(Static_Polygon);
            });
        }

        //Adds polygons for the stationary obstacles
        //All units in meters 
        public void MAP_addSObstaclePoly(double radius, double altitude, double Lat, double Lon)
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                //Add obstacle
                GMapPolygon polygon = new GMapPolygon(getCirclePoly(radius, Lat, Lon), "polygon");
                polygon.Stroke = new Pen(Color.Red, 2);
                polygon.Fill = new SolidBrush(Color.FromArgb(100, Color.RoyalBlue));
                Stationary_Obstacle_Overlay.Polygons.Add(polygon);

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

                Moving_Obstacle_Overlay.Markers.Add(marker);
            });

        }

        //All units in meters 
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

        //All units in meters  
        public void MAP_addMObstaclePoly(double radius, double altitude, double Lat, double Lon, string name)
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                GMapPolygon polygon = new GMapPolygon(getCirclePoly(radius, Lat, Lon), "name");
                polygon.Stroke = new Pen(Color.Red, 2);
                polygon.Fill = new SolidBrush(Color.FromArgb(100, Color.RoyalBlue));
                Moving_Obstacle_Overlay.Polygons.Add(polygon);

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
                Moving_Obstacle_Overlay.Markers.Add(marker);
            });

        }

        public void MAP_updatePlaneLoc(PointLatLng location, float altitude, float heading, float cog, float nav_bearing, float target, float radius)
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                if(location.Lat != 0 || location.Lng != 0)
                {
                    GMapMarkerPlane marker = new GMapMarkerPlane(location, zoom, UAS_Scale, UAS_FixedSize, heading, cog, nav_bearing, target, radius);
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
                    Plane_Overlay.Markers.Add(marker);
                }
                
            });
        }

        public void MAP_updateGPSLabel(string label)
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                UAS_GPS_Label.Text = label;
            });
            //gMapControl1.
        }

        public void MAP_updateAltLabel(string altitude, string delta_altutide)
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                UAS_Altitude_ASL_Label.Text = altitude;
                UAS_D_Altitude_Label.Text = delta_altutide;
            });
        }

        public void MAP_updateWP(List<Waypoint> waypoints)
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                GMapMarkerWP marker;
                for (int i = 0; i < waypoints.Count(); i++)
                {
                    marker = new GMapMarkerWP(new PointLatLng(waypoints[i].latitude, waypoints[i].longitude), i.ToString("0"));
                    //marker.ToolTipMode = MarkerTooltipMode.Always;
                    //marker.ToolTipText = i.ToString();
                    WP_Overlay.Markers.Add(marker);
                }
            });
        }

        public void MAP_updateWPRoute(List<Waypoint> waypoints)
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                for (int i = 0; i < waypoints.Count() - 1; i++)
                {
                    List<PointLatLng> list = new List<PointLatLng>();
                    list.Add(new PointLatLng(waypoints[i].latitude, waypoints[i].longitude));
                    list.Add(new PointLatLng(waypoints[i + 1].latitude, waypoints[i + 1].longitude));

                    WP_Overlay.Routes.Add(new GMapRoute(list, "route") { Stroke = new System.Drawing.Pen(System.Drawing.Color.Yellow, 4) });
                }
            });
        }

        public void MAP_updateOFAT_EN_DROP(Mission Current_Mission)
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                //Do not draw if targets are at 0,0

                GMarkerGoogle marker;
               
                //Off axis target
                if(Current_Mission.off_axis_target_pos.latitude != 0 || Current_Mission.off_axis_target_pos.longitude != 0)
                {
                    marker = new GMarkerGoogle(new PointLatLng(Current_Mission.off_axis_target_pos.latitude, Current_Mission.off_axis_target_pos.longitude), GMarkerGoogleType.yellow_pushpin);
                    marker.ToolTipMode = MarkerTooltipMode.Always;
                    marker.ToolTipText = "OFAT";
                    OFAT_EN_DROP_Overlay.Markers.Add(marker);
                }
                else
                {
                    //Console.WriteLine("Did not display off axis because coordinate at 0,0");
                }

                //Air Drop Location
                if(Current_Mission.air_drop_pos.latitude != 0 || Current_Mission.air_drop_pos.longitude != 0)
                {
                    marker = new GMarkerGoogle(new PointLatLng(Current_Mission.air_drop_pos.latitude, Current_Mission.air_drop_pos.longitude), GMarkerGoogleType.yellow_pushpin);
                    marker.ToolTipMode = MarkerTooltipMode.Always;
                    marker.ToolTipText = "Air Drop";
                    OFAT_EN_DROP_Overlay.Markers.Add(marker);
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
                    OFAT_EN_DROP_Overlay.Markers.Add(marker);
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
                gMapControl1.Overlays.Add(Static_Overlay);
                gMapControl1.Overlays.Add(WP_Overlay);
                gMapControl1.Overlays.Add(Moving_Obstacle_Overlay);
                gMapControl1.Overlays.Add(Stationary_Obstacle_Overlay);
                gMapControl1.Overlays.Add(OFAT_EN_DROP_Overlay);
                gMapControl1.Overlays.Add(Plane_Overlay);
                gMapControl1.Invalidate();
            });
        }

        public void MAP_Clear_Overlays()
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                Moving_Obstacle_Overlay.Clear();
                Static_Overlay.Clear();
                Stationary_Obstacle_Overlay.Clear();
                OFAT_EN_DROP_Overlay.Clear();
                Plane_Overlay.Clear();
                WP_Overlay.Clear();
            });
        }

        public void MAP_ChangeLoc(PointLatLng point)
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                gMapControl1.Position = point;
            });
        }

        //public void MAP_Update

        private void Telem_Start_Stop_Button_Click(object sender, EventArgs e)
        {
            if (Telem_Start_Stop_Button.Text == "Start")
            {
                //Start
                InteroperabilityCallback(0);
                Telem_Start_Stop_Button.Text = "Stop";
            }
            else
            {
                //Stop
                InteroperabilityCallback(5);
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

        public bool getDrawWP()
        {
            return DrawWP;
        }
        public bool getDrawObstacles()
        {
            return DrawObstacles;
        }
        public bool getDrawPlane()
        {
            return DrawPlane;
        }
        public bool getDrawGeofence()
        {
            return DrawGeofence;
        }
        public bool getDrawSearchArea()
        {
            return DrawSearchArea;
        }

        public bool getDrawOFAT_EN_DROP()
        {
            return DrawOFAT_EN_DROP;
        }

        public bool getAutopan()
        {
            return MAP_Autopan;
        }

        private void showGeofenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showGeofenceToolStripMenuItem.Checked == true)
            {
                showGeofenceToolStripMenuItem.Checked = false;
                Geofence_Checkbox.Checked = false;
                DrawGeofence = false;
            }
            else
            {
                showGeofenceToolStripMenuItem.Checked = true;
                Geofence_Checkbox.Checked = true;
                DrawGeofence = true;
            }
            Settings["DrawGeofence"] = DrawGeofence.ToString();
            Settings.Save();
        }

        private void showSearchAreaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showSearchAreaToolStripMenuItem.Checked == true)
            {
                showSearchAreaToolStripMenuItem.Checked = false;
                SearchArea_Checkbox.Checked = false;
                DrawSearchArea = false;
            }
            else
            {
                showSearchAreaToolStripMenuItem.Checked = true;
                SearchArea_Checkbox.Checked = true;
                DrawSearchArea = true;
            }
            Settings["DrawSearchArea"] = DrawSearchArea.ToString();
            Settings.Save();
        }

        private void showSRICEmergentDropToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showOFATEmergentDropToolStripMenuItem.Checked == true)
            {
                showOFATEmergentDropToolStripMenuItem.Checked = false;
                OFAT_EM_DROP_CheckBox.Checked = false;
                DrawOFAT_EN_DROP = false;
            }
            else
            {
                showOFATEmergentDropToolStripMenuItem.Checked = true;
                OFAT_EM_DROP_CheckBox.Checked = true;
                DrawOFAT_EN_DROP = true;
            }
            Settings["DrawOFAT_EN_DROP"] = DrawOFAT_EN_DROP.ToString();
            Settings.Save();
        }

        private void showObstaclesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showObstaclesToolStripMenuItem.Checked == true)
            {
                showObstaclesToolStripMenuItem.Checked = false;
                Obstacles_Checkbox.Checked = false;
                DrawObstacles = false;
            }
            else
            {
                showObstaclesToolStripMenuItem.Checked = true;
                Obstacles_Checkbox.Checked = true;
                DrawObstacles = true;
            }
            Settings["DrawObstacles"] = DrawObstacles.ToString();
            Settings.Save();
        }

        private void showPlaneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showPlaneToolStripMenuItem.Checked == true)
            {
                showPlaneToolStripMenuItem.Checked = false;
                UASLoc_Checkbox.Checked = false;
                DrawPlane = false;
            }
            else
            {
                showPlaneToolStripMenuItem.Checked = true;
                UASLoc_Checkbox.Checked = true;
                DrawPlane = true;
            }
            Settings["DrawPlane"] = DrawPlane.ToString();
            Settings.Save();
        }

        private void showWaypointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showWaypointsToolStripMenuItem.Checked == true)
            {
                showWaypointsToolStripMenuItem.Checked = false;
                Waypoints_Checkbox.Checked = false;
                DrawWP = false;
            }
            else
            {
                showWaypointsToolStripMenuItem.Checked = true;
                Waypoints_Checkbox.Checked = true;
                DrawWP = true;
            }
            Settings["DrawWP"] = DrawWP.ToString();
            Settings.Save();
        }

        private void Geofence_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!Geofence_Checkbox.Checked)
            {
                //Geofence_Checkbox.Checked = false;
                showGeofenceToolStripMenuItem.Checked = false;
                DrawGeofence = false;
            }
            else
            {
                //Geofence_Checkbox.Checked = true;
                showGeofenceToolStripMenuItem.Checked = true;
                DrawGeofence = true;
            }
            Settings["DrawGeofence"] = DrawGeofence.ToString();
            Settings.Save();
        }

        private void SearchArea_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!SearchArea_Checkbox.Checked)
            {
                showSearchAreaToolStripMenuItem.Checked = false;
                DrawSearchArea = false;
            }
            else
            {
                showSearchAreaToolStripMenuItem.Checked = true;
                DrawSearchArea = true;
            }
            Settings["DrawSearchArea"] = DrawSearchArea.ToString();
            Settings.Save();
        }

        private void Obstacles_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!Obstacles_Checkbox.Checked)
            {
                showObstaclesToolStripMenuItem.Checked = false;
                DrawObstacles = false;
            }
            else
            {
                showObstaclesToolStripMenuItem.Checked = true;
                DrawObstacles = true;
            }
            Settings["DrawObstacles"] = DrawObstacles.ToString();
            Settings.Save();
        }

        private void UASLoc_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!UASLoc_Checkbox.Checked)
            {
                showPlaneToolStripMenuItem.Checked = false;
                DrawPlane = false;
            }
            else
            {
                showPlaneToolStripMenuItem.Checked = true;
                DrawPlane = true;
            }
            Settings["DrawPlane"] = DrawPlane.ToString();
            Settings.Save();
        }

        private void Waypoints_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!Waypoints_Checkbox.Checked)
            {
                showWaypointsToolStripMenuItem.Checked = false;
                DrawWP = false;
            }
            else
            {
                showWaypointsToolStripMenuItem.Checked = true;
                DrawWP = true;
            }
            Settings["DrawWP"] = DrawWP.ToString();
            Settings.Save();
        }

        private void OFAT_EM_DROP_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!OFAT_EM_DROP_CheckBox.Checked)
            {
                showOFATEmergentDropToolStripMenuItem.Checked = false;
                DrawOFAT_EN_DROP = false;
            }
            else
            {
                showOFATEmergentDropToolStripMenuItem.Checked = true;
                DrawOFAT_EN_DROP = true;
            }
            Settings["DrawOFAT_EN_DROP"] = DrawOFAT_EN_DROP.ToString();
            Settings.Save();
        }

        private void gMapControl1_OnMapZoomChanged()
        {
            zoom = Convert.ToInt32(gMapControl1.Zoom);
        }

        private void UAS_Trackbar_Scroll(object sender, EventArgs e)
        {
            UAS_Scale = UAS_Trackbar.Value;
            Settings["UAS_Scale"] = UAS_Scale.ToString();
            Settings.Save();
        }

        private void Fixed_UAS_Size_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            UAS_FixedSize = Fixed_UAS_Size_Checkbox.Checked;
            Settings["UAS_Fixedsize"] = UAS_FixedSize.ToString();
            Settings.Save();
        }

        private void AutoPan_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            MAP_Autopan = AutoPan_Checkbox.Checked;
            Settings["MAP_Autopan"] = MAP_Autopan.ToString();
            Settings.Save();
        }

        private void gMapControl1_OnMapDrag()
        {
            MAP_Autopan = false;
            AutoPan_Checkbox.Checked = false;
            Settings["MAP_Autopan"] = MAP_Autopan.ToString();
            Settings.Save();
        }

        private void gMapControl1_KeyPress(object sender, KeyPressEventArgs e)
        {
            /*if(e == "t")
            {
                InteroperabilityCallback(7);
            }*/

            Console.Beep(433, 100);

        }

        private void Interoperability_GUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            InteroperabilityCallback(7);
            isOpened = false;
        }

        private void showInteroperabilityControlPanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Opening Interoperability Control Panel");
            InteroperabilityCallback(8);
        }

        private void Interoperability_GUI_Main_Shown(object sender, EventArgs e)
        {
            isOpened = true;
        }

        //not used anymore
        private void InteropGUIButton_Click(object sender, EventArgs e)
        {

        }

        private void InteropGUIButton_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                InteroperabilityCallback(9);
            }
            else if (e.Button == MouseButtons.Left)
            {
                Console.WriteLine("Opening Interoperability Control Panel");
                InteroperabilityCallback(8);
            }

        }

        private void Callout_StartStop_Button_Click(object sender, EventArgs e)
        {
            InteroperabilityCallback(10);
        }

        private void Flight_Time_Label_DoubleClick(object sender, EventArgs e)
        {
            InteroperabilityCallback(11);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(Interoperability.getinstance().Current_Mission.unedited == true)
            {
                DialogResult result;
                result = MessageBox.Show("Are you sure you want to create a new file? \nYou will lose any unsaved changes to your mission", "Interoperability Control Panel", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                
                if(result == DialogResult.OK)
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

        private void SDA_Plane_Simulation_Start_Button_Click(object sender, EventArgs e)
        {
            if(SDA_Plane_Simulation_Start_Button.Text == "Start Simulation")
            {
                SDA_Plane_Simulation_Start_Button.Text = "Stop Simulation";
            }
            else
            {
                SDA_Plane_Simulation_Start_Button.Text = "Start Simulation";
            }
            InteroperabilityCallback(13);
        }
    }

    public static class MercatorProjection
    {
        private static readonly double R_MAJOR = 6378137.0;
        private static readonly double R_MINOR = 6356752.3142;
        private static readonly double RATIO = R_MINOR / R_MAJOR;
        private static readonly double ECCENT = Math.Sqrt(1.0 - (RATIO * RATIO));
        private static readonly double COM = 0.5 * ECCENT;

        private static readonly double DEG2RAD = Math.PI / 180.0;
        private static readonly double RAD2Deg = 180.0 / Math.PI;
        private static readonly double PI_2 = Math.PI / 2.0;

        public static double[] toPixel(double lon, double lat)
        {
            return new double[] { lonToX(lon), latToY(lat) };
        }

        public static double[] toGeoCoord(double x, double y)
        {
            return new double[] { xToLon(x), yToLat(y) };
        }

        public static double lonToX(double lon)
        {
            return R_MAJOR * DegToRad(lon);
        }

        public static double latToY(double lat)
        {
            lat = Math.Min(89.5, Math.Max(lat, -89.5));
            double phi = DegToRad(lat);
            double sinphi = Math.Sin(phi);
            double con = ECCENT * sinphi;
            con = Math.Pow(((1.0 - con) / (1.0 + con)), COM);
            double ts = Math.Tan(0.5 * ((Math.PI * 0.5) - phi)) / con;
            return 0 - R_MAJOR * Math.Log(ts);
        }

        public static double xToLon(double x)
        {
            return RadToDeg(x) / R_MAJOR;
        }

        public static double yToLat(double y)
        {
            double ts = Math.Exp(-y / R_MAJOR);
            double phi = PI_2 - 2 * Math.Atan(ts);
            double dphi = 1.0;
            int i = 0;
            while ((Math.Abs(dphi) > 0.000000001) && (i < 15))
            {
                double con = ECCENT * Math.Sin(phi);
                dphi = PI_2 - 2 * Math.Atan(ts * Math.Pow((1.0 - con) / (1.0 + con), COM)) - phi;
                phi += dphi;
                i++;
            }
            return RadToDeg(phi);
        }

        private static double RadToDeg(double rad)
        {
            return rad * RAD2Deg;
        }

        private static double DegToRad(double deg)
        {
            return deg * DEG2RAD;
        }
    }

    //Marker so we can add payload images to the map
    [Serializable]
    public class GMapMarkerImage : GMapMarker
    {
        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        //private readonly Bitmap icon = global::MissionPlanner.Properties.Resources.planeicon;
        private readonly Bitmap icon; // icon = new Bitmap(path)
        float heading = 0;
        float altitude = 0;

        public GMapMarkerImage(PointLatLng p, float heading, float altitude, string path)
            : base(p)
        {
            this.heading = heading;
            this.altitude = altitude;
            icon = new Bitmap(path);
            Size = icon.Size;
        }

        public override void OnRender(Graphics g)
        {
            Matrix temp = g.Transform;
            g.TranslateTransform(LocalPosition.X, LocalPosition.Y);

            g.RotateTransform(-Overlay.Control.Bearing);
            try
            {
                g.RotateTransform(heading);
            }
            catch
            {
            }
            g.DrawImageUnscaled(icon, icon.Width / -2, icon.Height / -2);

            g.Transform = temp;
        }
    }
    //Marker to show the current location of the plane
    [Serializable]
    public class GMapMarkerPlane : GMapMarker
    {
        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        //private readonly Bitmap icon = global::MissionPlanner.Properties.Resources.planeicon;
        //private readonly Bitmap icon = interoperability.Properties.Resources.UT_X2B;
        private Bitmap icon;
        float heading = 0;
        float cog = -1;
        float target = -1;
        float nav_bearing = -1;
        float radius = -1;

        public GMapMarkerPlane(PointLatLng p, int zoom, int scale, bool fixedscale, float heading, float cog, float nav_bearing, float target, float radius)
            : base(p)
        {
            this.heading = heading;
            this.cog = cog;
            this.target = target;
            this.nav_bearing = nav_bearing;
            this.radius = radius;
            int gmapscale;

            //int scale = 8 * Convert.ToInt32(gmap.Zoom);
            if (!fixedscale)
            {
                gmapscale = Convert.ToInt32(scale * 1 / (156543.03392 * Math.Cos(p.Lat * Math.PI / 180) / Math.Pow(2, zoom)));
            }
            else
            {
                gmapscale = scale;
            }

            //Be careful not to make the image too large, or Size will throw an invalid paramter exception 
            if (gmapscale == 0)
            {
                gmapscale = 1;
            }
            if (gmapscale > 1500)
            {
                gmapscale = 1500;
            }

            icon = new Bitmap(interoperability.Properties.Resources.UT_X2B, new Size(gmapscale, gmapscale));
            Size = icon.Size;
            //Size = new Size(10, 10);
        }

        public GMapMarkerPlane(PointLatLng p, float heading)
            : base(p)
        {
            this.heading = heading;
            Size = icon.Size;
        }

        public override void OnRender(Graphics g)
        {
            Matrix temp = g.Transform;
            g.TranslateTransform(LocalPosition.X, LocalPosition.Y);

            g.RotateTransform(-Overlay.Control.Bearing);

            int length = 500;
            // anti NaN
            try
            {
                g.DrawLine(new Pen(Color.Red, 2), 0.0f, 0.0f, (float)Math.Cos((heading - 90) * deg2rad) * length,
                    (float)Math.Sin((heading - 90) * deg2rad) * length);
            }
            catch
            {
            }
            g.DrawLine(new Pen(Color.Green, 2), 0.0f, 0.0f, (float)Math.Cos((nav_bearing - 90) * deg2rad) * length,
                (float)Math.Sin((nav_bearing - 90) * deg2rad) * length);
            g.DrawLine(new Pen(Color.Black, 2), 0.0f, 0.0f, (float)Math.Cos((cog - 90) * deg2rad) * length,
                (float)Math.Sin((cog - 90) * deg2rad) * length);
            g.DrawLine(new Pen(Color.Orange, 2), 0.0f, 0.0f, (float)Math.Cos((target - 90) * deg2rad) * length,
                (float)Math.Sin((target - 90) * deg2rad) * length);
            // anti NaN
            try
            {
                float desired_lead_dist = 100;

                double width =
                    (Overlay.Control.MapProvider.Projection.GetDistance(Overlay.Control.FromLocalToLatLng(0, 0),
                        Overlay.Control.FromLocalToLatLng(Overlay.Control.Width, 0)) * 1000.0);
                double m2pixelwidth = Overlay.Control.Width / width;

                float alpha = ((desired_lead_dist * (float)m2pixelwidth) / radius) * rad2deg;

                if (radius < -1)
                {
                    // fixme 

                    float p1 = (float)Math.Cos((cog) * deg2rad) * radius + radius;

                    float p2 = (float)Math.Sin((cog) * deg2rad) * radius + radius;

                    g.DrawArc(new Pen(Color.HotPink, 2), p1, p2, Math.Abs(radius) * 2, Math.Abs(radius) * 2, cog, alpha);
                }

                else if (radius > 1)
                {
                    // correct

                    float p1 = (float)Math.Cos((cog - 180) * deg2rad) * radius + radius;

                    float p2 = (float)Math.Sin((cog - 180) * deg2rad) * radius + radius;

                    g.DrawArc(new Pen(Color.HotPink, 2), -p1, -p2, radius * 2, radius * 2, cog - 180, alpha);
                }
            }
            catch
            {
            }

            try
            {
                g.RotateTransform(heading);
            }
            catch
            {
            }
            g.DrawImageUnscaled(icon, icon.Width / -2, icon.Height / -2);
            //g.DrawImageUnscaled(icon, icon.Width / -2, icon.Height / -2, 50, 50);
            //g.DrawImage(icon, icon.Width / -2, icon.Height / 02, 50, 50);
            g.Transform = temp;
        }
    }

    //Marker to show waypoints 
    [Serializable]
    public class GMapMarkerWP : GMarkerGoogle
    {
        string wpno = "";
        public bool selected = false;
        SizeF txtsize = SizeF.Empty;
        static Dictionary<string, Bitmap> fontBitmaps = new Dictionary<string, Bitmap>();
        static Font font;

        public GMapMarkerWP(PointLatLng p, string wpno)
            : base(p, GMarkerGoogleType.green)
        {
            this.wpno = wpno;
            if (font == null)
                font = SystemFonts.DefaultFont;

            if (!fontBitmaps.ContainsKey(wpno))
            {
                Bitmap temp = new Bitmap(100, 40, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(temp))
                {
                    txtsize = g.MeasureString(wpno, font);

                    g.DrawString(wpno, font, Brushes.Black, new PointF(0, 0));
                }
                fontBitmaps[wpno] = temp;
            }
        }

        public override void OnRender(Graphics g)
        {
            if (selected)
            {
                g.FillEllipse(Brushes.Red, new Rectangle(this.LocalPosition, this.Size));
                g.DrawArc(Pens.Red, new Rectangle(this.LocalPosition, this.Size), 0, 360);
            }

            base.OnRender(g);

            var midw = LocalPosition.X + 10;
            var midh = LocalPosition.Y + 3;

            if (txtsize.Width > 15)
                midw -= 4;

            //if (Overlay.Control.Zoom > 16 || IsMouseOver)
            g.DrawImageUnscaled(fontBitmaps[wpno], midw, midh);
        }
    }
}
