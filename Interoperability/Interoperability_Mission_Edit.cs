using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using interoperability;
using Interoperability_GUI_Forms;

namespace interoperability
{
    public partial class Interoperability_Mission_Edit : Form
    {
        //The edit button will open up a human readable text box, with line by line coordinates, and tags, and everything.
        //So humans can edit each mission if they want. 

        public bool isOpened = false;

        GMapOverlay Map_Overlay;
        Mission Temporary_Mission;

        //These should really be in a class or stuct.
        //Too lazy to fix, get frosh to fix it
        int FlyZone_Index = 0;
        int Max_Alt_MSL_Index = 0;
        int Min_Alt_MSL_Index = 0;
        List<string> FlyZone_List = new List<string>();
        List<float> Max_Alt_MSL_List = new List<float>();
        List<float> Min_Alt_MSL_List = new List<float>();
        List<Color> Border_Colour_List = new List<Color>();
        List<Color> Fill_Colour_List = new List<Color>();

        bool FlyZone_Index_Changed_doNothing = false;


        public Interoperability_Mission_Edit()
        {
            InitializeComponent();
        }

        private void Interoperability_Mission_Import_Shown(object sender, EventArgs e)
        {
            isOpened = true;

            Map_Overlay = new GMapOverlay("Map_Overlay");
            

            FlyZone_Textbox.Text = "";
            Search_Area_Textbox.Text = "";
            Waypoint_Textbox.Text = "";

            Mission_Name_Textbox.Text = Interoperability.getinstance().Current_Mission.name;
            if (Interoperability.getinstance().Settings["geo_cords"] == "DD.DDDDDD")
            {
                for (int i = 0; i < Interoperability.getinstance().Current_Mission.fly_zones.Count; i++)
                {
                    FlyZone_Select_Combobox.Items.Insert(i, Interoperability.getinstance().Current_Mission.fly_zones[i].name);
                    Max_Alt_MSL_List.Add(Interoperability.getinstance().Current_Mission.fly_zones[i].altitude_msl_max);
                    Min_Alt_MSL_List.Add(Interoperability.getinstance().Current_Mission.fly_zones[i].altitude_msl_min);
                    Border_Colour_List.Add(Interoperability.getinstance().Current_Mission.fly_zones[i].border_color);
                    Fill_Colour_List.Add(Interoperability.getinstance().Current_Mission.fly_zones[i].fill_color);

                    FlyZone_List.Add("");
                    for (int j = 0; j < Interoperability.getinstance().Current_Mission.fly_zones[0].boundary_pts.Count; j++)
                    {
                        FlyZone_List[i] += Interoperability.getinstance().Current_Mission.fly_zones[i].boundary_pts[j].latitude.ToString("00.000000");
                        FlyZone_List[i] += " ";
                        FlyZone_List[i] += Interoperability.getinstance().Current_Mission.fly_zones[i].boundary_pts[j].longitude.ToString("00.000000") + "\r\n";
                    }
                }
                for (int i = 0; i < Interoperability.getinstance().Current_Mission.search_grid_points.Count; i++)
                {
                    Search_Area_Textbox.AppendText(Interoperability.getinstance().Current_Mission.search_grid_points[i].latitude.ToString("00.000000"));
                    Search_Area_Textbox.AppendText(" ");
                    Search_Area_Textbox.AppendText(Interoperability.getinstance().Current_Mission.search_grid_points[i].longitude.ToString("00.000000") + "\r\n");
                }
                for (int i = 0; i < Interoperability.getinstance().Current_Mission.mission_waypoints.Count; i++)
                {
                    Waypoint_Textbox.AppendText(Interoperability.getinstance().Current_Mission.mission_waypoints[i].latitude.ToString("00.000000"));
                    Waypoint_Textbox.AppendText(" ");
                    Waypoint_Textbox.AppendText(Interoperability.getinstance().Current_Mission.mission_waypoints[i].longitude.ToString("00.000000"));
                    Waypoint_Textbox.AppendText(" ");
                    Waypoint_Textbox.AppendText(Interoperability.getinstance().Current_Mission.mission_waypoints[i].altitude_msl.ToString("000") + "\r\n");
                }
                if (Interoperability.getinstance().Current_Mission.air_drop_pos.latitude != 0 || Interoperability.getinstance().Current_Mission.air_drop_pos.longitude != 0)
                {
                    Airdrop_Textbox.Text = Interoperability.getinstance().Current_Mission.air_drop_pos.latitude.ToString("00.000000") + " "
                   + Interoperability.getinstance().Current_Mission.air_drop_pos.longitude.ToString("00.000000");
                }
                if (Interoperability.getinstance().Current_Mission.emergent_last_known_pos.latitude != 0 || Interoperability.getinstance().Current_Mission.emergent_last_known_pos.longitude != 0)
                {
                    Emergent_Target_Textbox.Text = Interoperability.getinstance().Current_Mission.emergent_last_known_pos.latitude.ToString("00.000000") + " "
                    + Interoperability.getinstance().Current_Mission.emergent_last_known_pos.longitude.ToString("00.000000");
                }
                if (Interoperability.getinstance().Current_Mission.off_axis_target_pos.latitude != 0 || Interoperability.getinstance().Current_Mission.off_axis_target_pos.longitude != 0)
                {
                    Off_Axis_Target_Textbox.Text = Interoperability.getinstance().Current_Mission.off_axis_target_pos.latitude.ToString("00.000000") + " "
                    + Interoperability.getinstance().Current_Mission.off_axis_target_pos.longitude.ToString("00.000000");
                }
            }
            else
            {
                for (int i = 0; i < Interoperability.getinstance().Current_Mission.fly_zones.Count; i++)
                {
                    FlyZone_Select_Combobox.Items.Insert(i, Interoperability.getinstance().Current_Mission.fly_zones[i].name);
                    Max_Alt_MSL_List.Add(Interoperability.getinstance().Current_Mission.fly_zones[i].altitude_msl_max);
                    Min_Alt_MSL_List.Add(Interoperability.getinstance().Current_Mission.fly_zones[i].altitude_msl_min);
                    Border_Colour_List.Add(Interoperability.getinstance().Current_Mission.fly_zones[i].border_color);
                    Fill_Colour_List.Add(Interoperability.getinstance().Current_Mission.fly_zones[i].fill_color);
                    FlyZone_List.Add("");
                    for (int j = 0; j < Interoperability.getinstance().Current_Mission.fly_zones[0].boundary_pts.Count; j++)
                    {
                        FlyZone_List[i] += Interoperability.DDtoDMS(Interoperability.getinstance().Current_Mission.fly_zones[i].boundary_pts[j].latitude,
                            Interoperability.getinstance().Current_Mission.fly_zones[i].boundary_pts[j].longitude) + "\r\n";
                    }
                }
                for (int i = 0; i < Interoperability.getinstance().Current_Mission.search_grid_points.Count; i++)
                {
                    Search_Area_Textbox.AppendText(Interoperability.DDtoDMS(Interoperability.getinstance().Current_Mission.search_grid_points[i].latitude,
                        Interoperability.getinstance().Current_Mission.search_grid_points[i].longitude) + "\r\n");
                }
                for (int i = 0; i < Interoperability.getinstance().Current_Mission.mission_waypoints.Count; i++)
                {
                    Waypoint_Textbox.AppendText(Interoperability.DDtoDMS(Interoperability.getinstance().Current_Mission.mission_waypoints[i].latitude,
                        Interoperability.getinstance().Current_Mission.mission_waypoints[i].longitude));
                    Waypoint_Textbox.AppendText(" ");
                    Waypoint_Textbox.AppendText(Interoperability.getinstance().Current_Mission.mission_waypoints[i].altitude_msl.ToString("000") + "\r\n");

                }
                if (Interoperability.getinstance().Current_Mission.air_drop_pos.latitude != 0 || Interoperability.getinstance().Current_Mission.air_drop_pos.longitude != 0)
                {
                    Airdrop_Textbox.Text = Interoperability.DDtoDMS(Interoperability.getinstance().Current_Mission.air_drop_pos.latitude,
                        Interoperability.getinstance().Current_Mission.air_drop_pos.longitude);
                }
                if (Interoperability.getinstance().Current_Mission.emergent_last_known_pos.latitude != 0 || Interoperability.getinstance().Current_Mission.emergent_last_known_pos.longitude != 0)
                {
                    Emergent_Target_Textbox.Text = Interoperability.DDtoDMS(Interoperability.getinstance().Current_Mission.emergent_last_known_pos.latitude,
                        Interoperability.getinstance().Current_Mission.emergent_last_known_pos.longitude);
                }
                if (Interoperability.getinstance().Current_Mission.off_axis_target_pos.latitude != 0 || Interoperability.getinstance().Current_Mission.off_axis_target_pos.longitude != 0)
                {
                    Off_Axis_Target_Textbox.Text = Interoperability.DDtoDMS(Interoperability.getinstance().Current_Mission.off_axis_target_pos.latitude,
                        Interoperability.getinstance().Current_Mission.off_axis_target_pos.longitude);
                }
            }

            FlyZone_Index_Changed_doNothing = true;
            FlyZone_Select_Combobox.SelectedIndex = 0;
            FlyZone_Index_Changed_doNothing = false;
            Max_Alt_MSL_Index = 0;
            Min_Alt_MSL_Index = 0;
            Max_Alt_MSL_Box.Value = Convert.ToDecimal(Max_Alt_MSL_List[0]);
            Min_Alt_MSL_Box.Value = Convert.ToDecimal(Min_Alt_MSL_List[0]);
            FlyZone_Textbox.Text = FlyZone_List[0];
        }

        private void Geofence_Select_Combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FlyZone_Index_Changed_doNothing == false)
            {
                FlyZone_Index_Changed_doNothing = true;
                //Save all current work 
                FlyZone_List[FlyZone_Index] = FlyZone_Textbox.Text;
                Max_Alt_MSL_List[Max_Alt_MSL_Index] = (float)Max_Alt_MSL_Box.Value;
                Min_Alt_MSL_List[Min_Alt_MSL_Index] = (float)Min_Alt_MSL_Box.Value;
                Border_Colour_List[FlyZone_Index] = Border_Colour_Button.BackColor;
                Fill_Colour_List[FlyZone_Index] = Fill_Colour_Button.BackColor;

                //Set new selected index 
                FlyZone_Index = FlyZone_Select_Combobox.SelectedIndex;
                Max_Alt_MSL_Index = FlyZone_Select_Combobox.SelectedIndex;
                Min_Alt_MSL_Index = FlyZone_Select_Combobox.SelectedIndex;


                //Selected "Add Flight Zone" 
                if (FlyZone_Select_Combobox.SelectedIndex == FlyZone_Select_Combobox.Items.Count - 1)
                {
                    //Make new index for everything
                    FlyZone_List.Add("");
                    FlyZone_Select_Combobox.Items.Insert(FlyZone_Index, "New Flyzone " + FlyZone_Index.ToString());
                    Max_Alt_MSL_List.Add(0);
                    Min_Alt_MSL_List.Add(0);
                    Border_Colour_List.Add(Color.Red);
                    Fill_Colour_List.Add(Color.White);   
                    Max_Alt_MSL_Box.Value = 0;
                    Min_Alt_MSL_Box.Value = 0;
                    FlyZone_Select_Combobox.SelectedIndex = FlyZone_Index;
                    FlyZone_Textbox.Text = "";
                }
                //Selected other flight zone
                else
                {
                    FlyZone_Textbox.Text = FlyZone_List[FlyZone_Index];
                    Max_Alt_MSL_Box.Value = Convert.ToDecimal(Max_Alt_MSL_List[Max_Alt_MSL_Index]);
                    Min_Alt_MSL_Box.Value = Convert.ToDecimal(Min_Alt_MSL_List[Min_Alt_MSL_Index]);
                    Border_Colour_Button.BackColor = Border_Colour_List[FlyZone_Index];
                    Fill_Colour_Button.BackColor = Fill_Colour_List [FlyZone_Index];
                }
                FlyZone_Index_Changed_doNothing = false;
            }
        }

        private void Interoperability_Mission_Import_FormClosed(object sender, FormClosedEventArgs e)
        {
            isOpened = false;
        }

        private void Save_Button_Click(object sender, EventArgs e)
        {
            isOpened = false;

            //Parse the fields, verify coordinate format, and save to Current_Mission
            Interoperability.getinstance().Current_Mission = new Mission(Parse_Mission());

            this.Close();
        }

        //---------------------------------------------------------------------------------------//
        //                                                                                       //
        //                                  MAP FUNCTIONS                                        //
        //                                                                                       //
        //---------------------------------------------------------------------------------------//

        private void gMapControl1_Load(object sender, EventArgs e)
        {
            gMapControl1.DragButton = MouseButtons.Left;
            gMapControl1.Position = new PointLatLng(38.145228, -76.427938); //AUVSI 
            gMapControl1.Zoom = 15;
            gMapControl1.MapProvider = GMap.NET.MapProviders.GoogleSatelliteMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache;
        }

        public void MAP_addStaticPoly(List<Waypoint> points, string name, Color Border_Color, Color Fill_Color, int width, int alpha)
        {
            List<PointLatLng> _points = new List<PointLatLng>();
            for (int i = 0; i < points.Count(); i++)
            {
                _points.Add(new PointLatLng(points[i].latitude, points[i].longitude));
            }

            GMapPolygon Static_Polygon = new GMapPolygon(_points, name);
            Static_Polygon.Stroke = new Pen(Border_Color, width);
            Static_Polygon.Fill = new SolidBrush(Color.FromArgb(alpha, Fill_Color));
            Map_Overlay.Polygons.Add(Static_Polygon);
        }

        public void MAP_updateOFAT_EN_DROP(Mission Temporary_Mission)
        {
            //Do not draw if targets are at 0,0

            GMarkerGoogle marker;

            //Off axis target
            if (Temporary_Mission.off_axis_target_pos.latitude != 0 || Temporary_Mission.off_axis_target_pos.longitude != 0)
            {
                marker = new GMarkerGoogle(new PointLatLng(Temporary_Mission.off_axis_target_pos.latitude, Temporary_Mission.off_axis_target_pos.longitude), GMarkerGoogleType.yellow_pushpin);
                marker.ToolTipMode = MarkerTooltipMode.Always;
                marker.ToolTipText = "OFAT";
                Map_Overlay.Markers.Add(marker);
            }
            else
            {
                //Console.WriteLine("Did not display off axis because coordinate at 0,0");
            }

            //Air Drop Location
            if (Temporary_Mission.air_drop_pos.latitude != 0 || Temporary_Mission.air_drop_pos.longitude != 0)
            {
                marker = new GMarkerGoogle(new PointLatLng(Temporary_Mission.air_drop_pos.latitude, Temporary_Mission.air_drop_pos.longitude), GMarkerGoogleType.yellow_pushpin);
                marker.ToolTipMode = MarkerTooltipMode.Always;
                marker.ToolTipText = "Air Drop";
                Map_Overlay.Markers.Add(marker);
            }
            else
            {
                //Console.WriteLine("Did not display air drop because coordinate at 0,0");
            }

            if (Temporary_Mission.air_drop_pos.latitude != 0 || Temporary_Mission.air_drop_pos.longitude != 0)
            {
                marker = new GMarkerGoogle(new PointLatLng(Temporary_Mission.emergent_last_known_pos.latitude, Temporary_Mission.emergent_last_known_pos.longitude), GMarkerGoogleType.yellow_pushpin);
                marker.ToolTipMode = MarkerTooltipMode.Always;
                marker.ToolTipText = "Emergent Target";
                Map_Overlay.Markers.Add(marker);
            }
            else
            {

            }
        }
        public void MAP_updateWP(List<Waypoint> waypoints)
        {
            GMapMarkerWP marker;
            for (int i = 0; i < waypoints.Count(); i++)
            {
                marker = new GMapMarkerWP(new PointLatLng(waypoints[i].latitude, waypoints[i].longitude), i.ToString("0"));
                //marker.ToolTipMode = MarkerTooltipMode.Always;
                //marker.ToolTipText = i.ToString();
                Map_Overlay.Markers.Add(marker);
            }
        }

        public void MAP_updateWPRoute(List<Waypoint> waypoints)
        {
            for (int i = 0; i < waypoints.Count() - 1; i++)
            {
                List<PointLatLng> list = new List<PointLatLng>();
                list.Add(new PointLatLng(waypoints[i].latitude, waypoints[i].longitude));
                list.Add(new PointLatLng(waypoints[i+1].latitude, waypoints[i+1].longitude));

                Map_Overlay.Routes.Add(new GMapRoute(list, "route") { Stroke = new System.Drawing.Pen(System.Drawing.Color.Yellow, 4) });
            }
        }
        public void MAP_Update_Overlay()
        {
            gMapControl1.Overlays.Clear();
            gMapControl1.Overlays.Add(Map_Overlay);
            gMapControl1.Refresh();
        }

        public void MAP_Clear_Overlays()
        {
            Map_Overlay.Clear();
        }

        private void Refresh_Map_Button_Click(object sender, EventArgs e)
        {
            Temporary_Mission = Parse_Mission();

            MAP_Update_Overlay();
            MAP_Clear_Overlays();


            for (int i = 0; i < Temporary_Mission.fly_zones.Count; i++)
            {
                MAP_addStaticPoly(Temporary_Mission.fly_zones[i].boundary_pts, "Geofence" + i.ToString(), Temporary_Mission.fly_zones[i].border_color,
                    Temporary_Mission.fly_zones[i].fill_color, 3, 50);
            }

            MAP_addStaticPoly(Temporary_Mission.search_grid_points, "Search_Area", Color.Green, Color.Green, 3, 90);

            MAP_updateWP(Temporary_Mission.mission_waypoints);
            MAP_updateWPRoute(Temporary_Mission.mission_waypoints);

            MAP_updateOFAT_EN_DROP(Temporary_Mission);

            MAP_Update_Overlay();

            List<double> lat = new List<double>();
            List<double> lng = new List<double>();
            for (int i = 0; i < Temporary_Mission.fly_zones.Count; i++)
            {
                for (int j = 0; j < Temporary_Mission.fly_zones[i].boundary_pts.Count; j++)
                {
                    lat.Add(Temporary_Mission.fly_zones[i].boundary_pts[j].latitude);
                    lng.Add(Temporary_Mission.fly_zones[i].boundary_pts[j].longitude);
                }
            }   
            for (int i = 0; i < Temporary_Mission.search_grid_points.Count; i++)
            {
                lat.Add(Temporary_Mission.search_grid_points[i].latitude);
                lng.Add(Temporary_Mission.search_grid_points[i].longitude);
            }
            if(Temporary_Mission.air_drop_pos.latitude != 0 || Temporary_Mission.air_drop_pos.longitude != 0)
            {
                lat.Add(Temporary_Mission.air_drop_pos.latitude);
                lng.Add(Temporary_Mission.air_drop_pos.longitude);
            }
            if (Temporary_Mission.emergent_last_known_pos.latitude != 0 || Temporary_Mission.emergent_last_known_pos.longitude != 0)
            {
                lat.Add(Temporary_Mission.emergent_last_known_pos.latitude);
                lng.Add(Temporary_Mission.emergent_last_known_pos.longitude);
            }
            if (Temporary_Mission.off_axis_target_pos.latitude != 0 || Temporary_Mission.off_axis_target_pos.longitude != 0)
            {
                lat.Add(Temporary_Mission.off_axis_target_pos.latitude);
                lng.Add(Temporary_Mission.off_axis_target_pos.longitude);
            }
             
            gMapControl1.SetZoomToFitRect(new RectLatLng((lat.Max() + lat.Min()) / 2, (lng.Max() + lng.Min()) / 2, lng.Max() - lng.Min(), lat.Max() - lat.Min()));
            gMapControl1.Position = new PointLatLng((lat.Max() + lat.Min()) / 2, (lng.Max() + lng.Min()) / 2);


            /*List<Waypoint> temp2 = new List<Waypoint>();
            temp2.Add(new Waypoint(lat.Max(), lng.Max()));
            temp2.Add(new Waypoint(lat.Max(), lng.Min()));
            temp2.Add(new Waypoint(lat.Min(), lng.Min()));
            temp2.Add(new Waypoint(lat.Min(), lng.Max()));
            MAP_addStaticPoly(temp2, "idk", Color.Red, Color.Transparent, 3, 0);*/

        }

        private Mission Parse_Mission()
        {
            Mission Parsed_Misison = new Mission(); 
            string Search_Area_Text, Waypoint_Text, Emergent_Target_Text, Airdrop_Text, Off_Axis_target_Text;

            FlyZone_List[FlyZone_Select_Combobox.SelectedIndex] = FlyZone_Textbox.Text;
            Search_Area_Text = Search_Area_Textbox.Text;
            Waypoint_Text = Waypoint_Textbox.Text;
            Emergent_Target_Text = Emergent_Target_Textbox.Text;
            Airdrop_Text = Airdrop_Textbox.Text;
            Off_Axis_target_Text = Off_Axis_Target_Textbox.Text;


            if (Interoperability.getinstance().Settings["geo_cords"] == "DD.DDDDDD")
            {
                Parsed_Misison.fly_zones.Clear();
                for (int i = 0; i < FlyZone_List.Count; i++)
                {
                    Parsed_Misison.fly_zones.Add(new FlyZone(Max_Alt_MSL_List[i], Min_Alt_MSL_List[i], DDtoWaypoints(FlyZone_List[i]),
                        Border_Colour_List[i], Fill_Colour_List[i]));
                }

                //Add Search Areas
                Parsed_Misison.search_grid_points = DDtoWaypoints(Search_Area_Text);

                //Add Waypoints
                Parsed_Misison.mission_waypoints = DDwithAltitudetoWaypoints(Waypoint_Text);


                if (DDtoWaypoints(Emergent_Target_Text).Count != 0)
                {
                    Parsed_Misison.emergent_last_known_pos.latitude = DDtoWaypoints(Emergent_Target_Text)[0].latitude;
                    Parsed_Misison.emergent_last_known_pos.longitude = DDtoWaypoints(Emergent_Target_Text)[0].longitude;
                }
                if (DDtoWaypoints(Airdrop_Text).Count != 0) 
                {
                    Parsed_Misison.air_drop_pos.latitude = DDtoWaypoints(Airdrop_Text)[0].latitude;
                    Parsed_Misison.air_drop_pos.longitude = DDtoWaypoints(Airdrop_Text)[0].longitude;
                }
                if (DDtoWaypoints(Off_Axis_target_Text).Count != 0)
                {
                    Parsed_Misison.off_axis_target_pos.latitude = DDtoWaypoints(Off_Axis_target_Text)[0].latitude;
                    Parsed_Misison.off_axis_target_pos.longitude = DDtoWaypoints(Off_Axis_target_Text)[0].longitude;
                }

                
            }
            else
            {
                //Add Fly Zones
                Parsed_Misison.fly_zones.Clear();
                for (int i = 0; i < FlyZone_List.Count; i++)
                {
                    Parsed_Misison.fly_zones.Add(new FlyZone(Max_Alt_MSL_List[i], Min_Alt_MSL_List[i], DMStoWaypoints(FlyZone_List[i])));
                }

                //Add Search Areas
                Parsed_Misison.search_grid_points = DMStoWaypoints(Search_Area_Text);

                //Add Waypoints
                Parsed_Misison.mission_waypoints = DMSwithAltitudetoWaypoints(Waypoint_Text);

                if (DMStoWaypoints(Emergent_Target_Text).Count != 0)
                {
                    Parsed_Misison.emergent_last_known_pos.latitude = DMStoWaypoints(Emergent_Target_Text)[0].latitude;
                    Parsed_Misison.emergent_last_known_pos.longitude = DMStoWaypoints(Emergent_Target_Text)[0].longitude;
                }
                if (DMStoWaypoints(Airdrop_Text).Count != 0)
                {
                    Parsed_Misison.air_drop_pos.latitude = DMStoWaypoints(Airdrop_Text)[0].latitude;
                    Parsed_Misison.air_drop_pos.longitude = DMStoWaypoints(Airdrop_Text)[0].longitude;
                }
                if (DMStoWaypoints(Off_Axis_target_Text).Count != 0)
                {
                    Parsed_Misison.off_axis_target_pos.latitude = DMStoWaypoints(Off_Axis_target_Text)[0].latitude;
                    Parsed_Misison.off_axis_target_pos.longitude = DMStoWaypoints(Off_Axis_target_Text)[0].longitude;
                }
            }

            return Parsed_Misison;
        }

        public static List<Waypoint> DMSwithAltitudetoWaypoints(string DMS)
        {
            List<Waypoint> Waypoint_List = new List<Waypoint>();
            //If string is empty, then don't do anything
            if (DMS.Replace(" ", "") == "")
            {
                return Waypoint_List;
            }

            Waypoint Temp_Waypoint;
            char[] delimiterChars = { ' ' };
            DMS = DMS.Replace("\r\n", " ").Replace("\r", "").Replace("\n", " ");
            string[] DMS_SPLIT = DMS.Split(delimiterChars);

            List<string> DMS_FILTERED = new List<string>();

            //Filter out all the "" items
            for (int i = 0; i < DMS_SPLIT.Count(); i++)
            {
                if (DMS_SPLIT[i] != "")
                {
                    DMS_FILTERED.Add(DMS_SPLIT[i]);
                }
            }

            int num_pairs = DMS_FILTERED.Count / 3;
            try {
                for (int i = 0; i < num_pairs; i++)
                {
                    Temp_Waypoint = Interoperability.DMStoDD(DMS_FILTERED[i * 3], DMS_FILTERED[i * 3 + 1]);
                    if (Temp_Waypoint.empty == false)
                    {
                        Temp_Waypoint.altitude_msl = (float)Convert.ToDouble(DMS_FILTERED[i * 3 + 2]);
                        Waypoint_List.Add(Temp_Waypoint);
                    }
                }
            }
            catch {
                //Do nothing
            }
            
            return Waypoint_List;
        }

        public static List<Waypoint> DDwithAltitudetoWaypoints(string DD)
        {
            List<Waypoint> Waypoint_List = new List<Waypoint>();
            if (DD.Replace(" ", "") == "")
            {
                return Waypoint_List;
            }
            Waypoint Temp_Waypoint;
            char[] delimiterChars = { ' ' };
            DD = DD.Replace("\r\n", " ").Replace("\r", "").Replace("\n", " ");
            string[] DD_SPLIT = DD.Split(delimiterChars);

            List<string> DD_FILTERED = new List<string>();

            //Filter out all the "" items
            for (int i = 0; i < DD_SPLIT.Count(); i++)
            {
                if (DD_SPLIT[i] != "")
                {
                    DD_FILTERED.Add(DD_SPLIT[i]);
                }
            }

            int num_pairs = DD_FILTERED.Count / 3;

            for (int i = 0; i < num_pairs; i++)
            {
                //Veify correct input
                if (Convert.ToDouble(DD_FILTERED[i * 3]) < 90 && Convert.ToDouble(DD_FILTERED[i * 3]) > -90 &&
                    Convert.ToDouble(DD_FILTERED[i * 3 + 1]) < 180 && Convert.ToDouble(DD_FILTERED[i * 3 + 1]) > -180)
                {
                    Temp_Waypoint = new Waypoint(Convert.ToDouble(DD_FILTERED[i * 3]), Convert.ToDouble(DD_FILTERED[i * 3 + 1]));
                    Temp_Waypoint.altitude_msl = (float)Convert.ToDecimal(DD_FILTERED[i * 3 + 2]);
                    Waypoint_List.Add(Temp_Waypoint);
                }
            }
            return Waypoint_List;
        }


        public static List<Waypoint> DMStoWaypoints(string DMS)
        {
            List<Waypoint> Waypoint_List = new List<Waypoint>();
            //If string is empty, then don't do anything
            if (DMS.Replace(" ", "") == "")
            {
                return Waypoint_List;
            }

            Waypoint Temp_Waypoint;
            char[] delimiterChars = { ' ' };
            DMS = DMS.Replace("\r\n", " ").Replace("\r", "").Replace("\n", " ");
            string[] DMS_SPLIT = DMS.Split(delimiterChars);

            List<string> DMS_FILTERED = new List<string>();

            //Filter out all the "" items
            for (int i = 0; i < DMS_SPLIT.Count(); i++)
            {
                if (DMS_SPLIT[i] != "")
                {
                    DMS_FILTERED.Add(DMS_SPLIT[i]);
                }
            }

            int num_pairs = DMS_FILTERED.Count / 2;

            for (int i = 0; i < num_pairs; i++)
            {
                Temp_Waypoint = Interoperability.DMStoDD(DMS_FILTERED[i * 2], DMS_FILTERED[i * 2 + 1]);
                if (Temp_Waypoint.empty == false)
                {
                    Waypoint_List.Add(Temp_Waypoint);
                }
            }
            return Waypoint_List;
        }

        public static List<Waypoint> DDtoWaypoints(String DD)
        {
            List<Waypoint> Waypoint_List = new List<Waypoint>();
            if (DD.Replace(" ", "") == "")
            {
                return Waypoint_List;
            }
            Waypoint Temp_Waypoint;
            char[] delimiterChars = { ' ' };
            DD = DD.Replace("\r\n", " ").Replace("\r", "").Replace("\n", " ");
            string[] DD_SPLIT = DD.Split(delimiterChars);

            List<string> DD_FILTERED = new List<string>();

            //Filter out all the "" items
            for (int i = 0; i < DD_SPLIT.Count(); i++)
            {
                if (DD_SPLIT[i] != "")
                {
                    DD_FILTERED.Add(DD_SPLIT[i]);
                }
            }

            int num_pairs = DD_FILTERED.Count / 2;

            for (int i = 0; i < num_pairs; i++)
            {
                //Veify correct input
                if (Convert.ToDouble(DD_FILTERED[i * 2]) < 90 && Convert.ToDouble(DD_FILTERED[i * 2]) > -90 &&
                    Convert.ToDouble(DD_FILTERED[i * 2 + 1]) < 180 && Convert.ToDouble(DD_FILTERED[i * 2 + 1]) > -180)
                {
                    Temp_Waypoint = new Waypoint(Convert.ToDouble(DD_FILTERED[i * 2]), Convert.ToDouble(DD_FILTERED[i * 2 + 1]));
                    Waypoint_List.Add(Temp_Waypoint);
                }
            }
            return Waypoint_List;
        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            //Save nothing
            isOpened = false;
            this.Close();
        }

        private void Geofence_Select_Rename_Click(object sender, EventArgs e)
        {
            if (FlyZone_Select_Rename.Text == "Rename")
            {
                FlyZone_Index = FlyZone_Select_Combobox.SelectedIndex;
                FlyZone_Select_Combobox.DropDownStyle = ComboBoxStyle.Simple;
                FlyZone_Select_Rename.Text = "Save";
            }
            else
            {
                FlyZone_Select_Combobox.Items[FlyZone_Index] = FlyZone_Select_Combobox.Text;
                FlyZone_Select_Combobox.DropDownStyle = ComboBoxStyle.DropDownList;
                FlyZone_Index = FlyZone_Select_Combobox.SelectedIndex = FlyZone_Index;
                FlyZone_Select_Rename.Text = "Rename";
            }

        }

        private void FlyZone_Delete_Button_Click(object sender, EventArgs e)
        {
            DialogResult result;
            result = MessageBox.Show("Are you sure you want to delete this Geofence?", "Interoperability Control Panel", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                FlyZone_Index_Changed_doNothing = true;

                int index_to_be_deleted = FlyZone_Select_Combobox.SelectedIndex;

                //Remove all indexes 
                FlyZone_Select_Combobox.Items.RemoveAt(index_to_be_deleted);
                FlyZone_List.RemoveAt(index_to_be_deleted);
                Max_Alt_MSL_List.RemoveAt(index_to_be_deleted);
                Min_Alt_MSL_List.RemoveAt(index_to_be_deleted);
                Border_Colour_List.RemoveAt(index_to_be_deleted);
                Fill_Colour_List.RemoveAt(index_to_be_deleted);

                //First and not only
                //First and only
                //Last
                //Neither

                //If no items left (except for the add items item), create new item
                if (FlyZone_Select_Combobox.Items.Count == 1)
                {
                    FlyZone_List.Add("");
                    FlyZone_Select_Combobox.Items.Insert(0, "New Flyzone 0");
                    Max_Alt_MSL_List.Add(0);
                    Min_Alt_MSL_List.Add(0);
                    Border_Colour_List.Add(Color.Red);
                    Fill_Colour_List.Add(Color.White);
                    Border_Colour_Button.BackColor = Border_Colour_List[0];
                    Fill_Colour_Button.BackColor = Fill_Colour_List[0];
                    FlyZone_Select_Combobox.SelectedIndex = 0;
                }
                else
                {
                    FlyZone_Index = 0;
                    Max_Alt_MSL_Index = 0;
                    Min_Alt_MSL_Index = 0;
                    Border_Colour_Button.BackColor = Border_Colour_List[FlyZone_Index];
                    Fill_Colour_Button.BackColor = Fill_Colour_List[FlyZone_Index];
                    FlyZone_Select_Combobox.SelectedIndex = 0;
                }

                FlyZone_Textbox.Text = FlyZone_List[FlyZone_Index];
                Max_Alt_MSL_Box.Value = Convert.ToDecimal(Max_Alt_MSL_List[Max_Alt_MSL_Index]);
                Min_Alt_MSL_Box.Value = Convert.ToDecimal(Min_Alt_MSL_List[Min_Alt_MSL_Index]);

                FlyZone_Index_Changed_doNothing = false;
            }
            else
            {
                //Do nothing
            }

        }

        private void Border_Button_Click(object sender, EventArgs e)
        {
            if(colorDialog1.ShowDialog() == DialogResult.OK)
            {
                Border_Colour_Button.BackColor = colorDialog1.Color;
                Border_Colour_List[FlyZone_Index] = colorDialog1.Color;
            }
        }

        private void Fill_Colour_Button_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                Fill_Colour_Button.BackColor = colorDialog1.Color;
                Fill_Colour_List[FlyZone_Index] = colorDialog1.Color;
            }
        }
    }
}
