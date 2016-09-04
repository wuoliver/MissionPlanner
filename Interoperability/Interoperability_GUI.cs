using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Interoperability;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace Interoperability_GUI
{
    public partial class Interoperability_GUI : Form
    {
        Action<int> InteroperabilityCallback;
        protected int telemPollRate = 10;
        protected int mapRefreshRate = 20;
        protected int sdaPollRate = 10;
        protected int UAS_Scale = 2;
        global::Interoperability_GUI.Settings_GUI settings_gui;
        Interoperability_Settings Settings;

        //Put on top or something afterwards
        GMapOverlay Static_Overlay;
        GMapOverlay Stationary_Obstacle_Overlay;
        GMapOverlay Moving_Obstacle_Overlay;
        GMapOverlay Plane_Overlay;
        GMapOverlay WP_Overlay;


        //Used for map control thread
        protected bool DrawWP = true;
        protected bool DrawObstacles = true;
        protected bool DrawPlane = true;
        protected bool DrawGeofence = true;
        protected bool DrawSearchArea = true;
        protected bool UAS_FixedSize = false;

        //GMAP Zoom
        private int zoom = 0;

        //Container for all possible targets
        List<PointLatLng> PossibleTargets;  //Targets that are found through the FPV camera
        List<PointLatLng> FoundTargets;     //Targets found through Davis's algorithm

        public Interoperability_GUI(Action<int> _InteroperabilityCallback, Interoperability_Settings _Settings)
        {
            Console.WriteLine("Created GUI");

            InitializeComponent();
            InteroperabilityCallback = _InteroperabilityCallback;

            //Get Settings object, used for server settings
            Settings = _Settings;

            //Must be called after settings
            MAP_OverlaySettings(ref Settings, ref DrawWP, "DrawWP");
            MAP_OverlaySettings(ref Settings, ref DrawObstacles, "DrawObstacles");
            MAP_OverlaySettings(ref Settings, ref DrawPlane, "DrawPlane");
            MAP_OverlaySettings(ref Settings, ref DrawGeofence, "DrawGeofence");
            MAP_OverlaySettings(ref Settings, ref DrawSearchArea, "DrawSearchArea");
            MAP_OverlaySettings(ref Settings, ref UAS_FixedSize, "UAS_Fixedsize");


            MAP_UAS_ScaleSettings(ref Settings, ref UAS_Scale, "UAS_Scale");

            InitializeGUI_States();

            //Set poll Rate text 
            Telemetry_pollRateInput.Text = telemPollRate.ToString();
            SDA_pollrateInput.Text = sdaPollRate.ToString();
            Map_RefreshRateInput.Text = mapRefreshRate.ToString();

            Static_Overlay = new GMapOverlay("Static_Overlays");
            Stationary_Obstacle_Overlay = new GMapOverlay("Stationary_Obstacles");
            Plane_Overlay = new GMapOverlay("Plane_Overlay");
            Moving_Obstacle_Overlay = new GMapOverlay("Moving_Obstacle");
            WP_Overlay = new GMapOverlay("Waypoints");
        }

        public void MAP_UAS_ScaleSettings(ref Interoperability_Settings Settings, ref int value, string key)
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
        public void MAP_OverlaySettings(ref Interoperability_Settings Settings, ref bool value, string key)
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

            UAS_Trackbar.Value = UAS_Scale;
            Fixed_UAS_Size_Checkbox.Checked = UAS_FixedSize;

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
            if (resp != this.TelemServerResp.Text)
            {
                this.TelemServerResp.BeginInvoke((MethodInvoker)delegate ()
                {
                    this.TelemServerResp.Text = resp;
                });
            }
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
            if (!Settings_GUI.isOpened)
            {
                settings_gui = new Settings_GUI(InteroperabilityCallback, Settings);
                settings_gui.Show();
            }
        }

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
                marker.ToolTipText = altitude.ToString("0");
                Moving_Obstacle_Overlay.Markers.Add(marker);
            });

        }


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
                marker.ToolTipText = altitude.ToString("0");
                Moving_Obstacle_Overlay.Markers.Add(marker);
            });

        }

        public void MAP_updatePlaneLoc(PointLatLng location, float altitude, float heading, float cog, float nav_bearing, float target, float radius)
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                GMapMarkerPlane marker = new GMapMarkerPlane(location, zoom, UAS_Scale, UAS_FixedSize, heading, cog, nav_bearing, target, radius);
                //Show the altitude always
                marker.ToolTipMode = MarkerTooltipMode.Always;
                marker.ToolTipText = altitude.ToString("0");
                Plane_Overlay.Markers.Add(marker);
            });
        }

        public void MAP_updateWP(List<PointLatLng> waypoints)
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                for (int i = 0; i < waypoints.Count(); i++)
                {
                    WP_Overlay.Markers.Add(new GMapMarkerWP(waypoints[i], i.ToString("0")));
                }
            });
        }

        public void MAP_updateWPRoute(List<PointLatLng> waypoints)
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                for (int i = 0; i < waypoints.Count() - 1; i++)
                {
                    List<PointLatLng> list = new List<PointLatLng>();
                    list.Add(waypoints[i]);
                    list.Add(waypoints[i + 1]);

                    WP_Overlay.Routes.Add(new GMapRoute(list, "route") { Stroke = new System.Drawing.Pen(System.Drawing.Color.Yellow, 4) });
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
                Plane_Overlay.Clear();
                WP_Overlay.Clear();
            });
        }

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
            if(gmapscale > 1500)
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

            if (Overlay.Control.Zoom > 16 || IsMouseOver)
                g.DrawImageUnscaled(fontBitmaps[wpno], midw, midh);
        }
    }
}
