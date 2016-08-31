using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        Action<int> restartInteroperabilityCallback;
        protected int telemPollRate = 10;
        protected int mapRefreshRate = 20;
        protected int sdaPollRate = 10;
        global::Interoperability_GUI.Settings_GUI settings_gui;
        Interoperability_Settings Settings;

        //Put on top or something afterwards
        GMapOverlay Static_Overlay;
        GMapOverlay Stationary_Obstacle_Overlay;
        GMapOverlay polyOverlay;
        GMapOverlay Plane_Overlay;


        public Interoperability_GUI(Action<int> _restartInteroperabilityCallback, Interoperability_Settings _Settings)
        {
            InitializeComponent();
            restartInteroperabilityCallback = _restartInteroperabilityCallback;

            //Get Settings object, used for server settings
            Settings = _Settings;
            //Set poll Rate text 
            pollRateInput.Text = telemPollRate.ToString();

            Static_Overlay = new GMapOverlay("Static_Overlays");
            Stationary_Obstacle_Overlay = new GMapOverlay("Stationary_Obstacles");
            Plane_Overlay = new GMapOverlay("Plane_Overlay");
            polyOverlay = new GMapOverlay("Polygon");
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
                telemPollRate = Int32.Parse(this.pollRateInput.Text);
            }
            catch
            {
                this.pollRateInput.Text = telemPollRate.ToString();
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

        //Function called when the map is loaded
        private void gMapControl1_Load(object sender, EventArgs e)
        {
            gMapControl1.DragButton = MouseButtons.Left;
            gMapControl1.Position = new PointLatLng(38.145228, -76.427938); //AUVSI 
            gMapControl1.Zoom = 15;
            gMapControl1.MapProvider = GMap.NET.MapProviders.GoogleSatelliteMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache;
            //GMap.NET.GMaps.Instance.
        }

        //Used to add polygons to define things such as: 
        // -Mission Area
        // -Search Area
        public void MAP_addStaticPoly(List<Waypoint> points, string name, Color Border_Color, Color Fill_Color, int width, int alpha)
        {
            List<PointLatLng> _points = new List<PointLatLng>();
            for (int i = 0; i < points.Count(); i++)
            {
                _points.Add(new PointLatLng(points[i].latitude, points[i].longitude));
            }

            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                GMapPolygon Static_Polygon = new GMapPolygon(_points, name);
                Static_Polygon.Stroke = new Pen(Border_Color, width);
                Static_Polygon.Fill = new SolidBrush(Color.FromArgb(alpha, Fill_Color));
                Static_Overlay.Polygons.Add(Static_Polygon);
                //gMapControl1.Overlays.Add(Static_Overlay);
                //gMapControl1.UpdatePolygonLocalPosition(polygon);
            });
        }

        //Adds polygons for the stationary obstacles
        public void MAP_addSObstaclePoly(double radius, double Lat, double Lon)
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {

                GMapPolygon polygon = new GMapPolygon(getCirclePoly(radius, Lat, Lon), "polygon");
                polygon.Stroke = new Pen(Color.Red, 2);
                polygon.Fill = new SolidBrush(Color.FromArgb(100, Color.RoyalBlue));

                //GMapMarker thing = new GMapMarker();
                //thing.
                //marker;

                Stationary_Obstacle_Overlay.Polygons.Add(polygon);
                //gMapControl1.Overlays.Add(Stationary_Obstacle_Overlay);
                //gMapControl1.UpdatePolygonLocalPosition(polygon);
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

        public void MAP_addCirclePoly(double radius, double Lat, double Lon, string name)
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                //polyOverlay = new GMapOverlay("polygons");

                //gMapControl1.Overlays.Clear(); 
                GMapPolygon polygon = new GMapPolygon(getCirclePoly(radius, Lat, Lon), "name");
                polygon.Stroke = new Pen(Color.Red, 2);
                polygon.Fill = new SolidBrush(Color.FromArgb(100, Color.RoyalBlue));
                polyOverlay.Polygons.Add(polygon);
                
                //GMarkerGoogle Test = new GmarkerGoogle();
                //Test.Tag
                //gMapControl1.Overlays.Add(polyOverlay);


                //gMapControl1.Overlays[0].Id;


                //gMapControl1.UpdatePolygonLocalPosition(polygon);
            });

        }

        public void MAP_updatePlaneLoc(PointLatLng Location, float altitude, float Heading)
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                GMapMarkerPlane marker = new GMapMarkerPlane(Location, Heading);
                //Show the altitude always
                marker.ToolTipMode = MarkerTooltipMode.Always;
                marker.ToolTipText = altitude.ToString("0");
                Plane_Overlay.Markers.Add(marker);
            });
        }

        public void MAP_Update_Overlay()
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                gMapControl1.Overlays.Clear();
                gMapControl1.Overlays.Add(polyOverlay);
                gMapControl1.Overlays.Add(Static_Overlay);
                gMapControl1.Overlays.Add(Stationary_Obstacle_Overlay);
                gMapControl1.Overlays.Add(Plane_Overlay);
                gMapControl1.Invalidate();
            });
        }
        public void MAP_Clear_Overlays()
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                polyOverlay.Clear();
                Static_Overlay.Clear();
                Stationary_Obstacle_Overlay.Clear();
                Plane_Overlay.Clear();
            });
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

    public class GMapMarkerPlane : GMapMarker
    {
        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        private readonly Bitmap icon =  global::MissionPlanner.Properties.Resources.planeicon;
        //private readonly Bitmap icon = new Bitmap(@"C:\Trapezoid.bmp");
        float heading = 0;
        //float cog = -1;
        //float target = -1;
        //float nav_bearing = -1;
        //float radius = -1;

        /*public GMapMarkerPlane(PointLatLng p, float heading, float cog, float nav_bearing, float target, float radius)
            : base(p)
        {
            this.heading = heading;
            this.cog = cog;
            this.target = target;
            this.nav_bearing = nav_bearing;
            this.radius = radius;
            Size = icon.Size;
        }*/

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
            /*g.DrawLine(new Pen(Color.Green, 2), 0.0f, 0.0f, (float)Math.Cos((nav_bearing - 90) * deg2rad) * length,
                (float)Math.Sin((nav_bearing - 90) * deg2rad) * length);
            g.DrawLine(new Pen(Color.Black, 2), 0.0f, 0.0f, (float)Math.Cos((cog - 90) * deg2rad) * length,
                (float)Math.Sin((cog - 90) * deg2rad) * length);
            g.DrawLine(new Pen(Color.Orange, 2), 0.0f, 0.0f, (float)Math.Cos((target - 90) * deg2rad) * length,
                (float)Math.Sin((target - 90) * deg2rad) * length);*/
            // anti NaN
           /* try
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
            */
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
}
