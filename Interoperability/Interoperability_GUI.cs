﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        protected int mapPollRate = 60;
        global::Interoperability_GUI.Settings_GUI settings_gui;
        Interoperability_Settings Settings;

        //Put on top or something afterwards
        GMapOverlay Static_Overlay;
        GMapOverlay Stationary_Obstacle_Overlay;
        GMapOverlay polyOverlay;


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
            polyOverlay = new GMapOverlay("Polygon");
        }

        public int getTelemPollRate()
        {
            return telemPollRate;
        }

        public int getMapPollRate()
        {
            return mapPollRate;
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
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
        }


        public void MAP_addStaticPoly(List<PointLatLng> points, string name, Color Border_Color, Color Fill_Color, int width, int alpha)
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                GMapPolygon Static_Polygon = new GMapPolygon(points, name);
                Static_Overlay.Polygons.Add(Static_Polygon);
                gMapControl1.Overlays.Add(Static_Overlay);
                //gMapControl1.UpdatePolygonLocalPosition(polygon);
            });
        }

        public void MAP_addSObstaclePoly(double radius, double Lat, double Lon)
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {

                GMapPolygon polygon = new GMapPolygon(getCirclePoly(radius, Lat, Lon), "polygon");
                polygon.Stroke = new Pen(Color.Red, 2);
                polygon.Fill = new SolidBrush(Color.FromArgb(100, Color.RoyalBlue));

                Stationary_Obstacle_Overlay.Polygons.Add(polygon);
                gMapControl1.Overlays.Add(Stationary_Obstacle_Overlay);
                gMapControl1.UpdatePolygonLocalPosition(polygon);
            });

        }


        public List<PointLatLng> getCirclePoly(double radius, double Lat, double Lon)
        {
            int numPoints = 20;
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
                //gMapControl1.Overlays.Add(polyOverlay);


                //gMapControl1.Overlays[0].Id;


                //gMapControl1.UpdatePolygonLocalPosition(polygon);
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
            });
        }
        public void MAP_Clear_Overlays()
        {
            this.gMapControl1.BeginInvoke((MethodInvoker)delegate ()
            {
                polyOverlay.Clear();
                Static_Overlay.Clear();
                Stationary_Obstacle_Overlay.Clear();
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
}
