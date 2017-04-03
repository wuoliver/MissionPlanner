using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace interoperability
{
    //SDA Classes 
    public class Moving_Obstacle
    {
        //Altitude in feet
        public float altitude_msl { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        //radius in feet
        public float sphere_radius { get; set; }

        public void printall()
        {
            Console.WriteLine("Altitude_MSL: " + altitude_msl + "\nLatitude: " + latitude +
                "\nLongitude: " + longitude + "\nSphere_Radius: " + sphere_radius);
        }
    }

    public class Stationary_Obstacle
    {   //In feet
        public float cylinder_height { get; set; }
        //In feet
        public float cylinder_radius { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }

        public Stationary_Obstacle(float _cylinder_height, float _cylinder_radius, float _latitude, float _longitude)
        {
            cylinder_height = _cylinder_height;
            cylinder_radius = _cylinder_radius;
            latitude = _latitude;
            longitude = _longitude;
        }
        public void printall()
        {
            Console.WriteLine("Cylinder Height: " + cylinder_height + "\nLatitude: " + latitude +
                "\nLongitude: " + longitude + "\nCylinder radius: " + cylinder_radius);
        }
    }

    public class Obstacles
    {
        public List<Moving_Obstacle> moving_obstacles;
        public List<Stationary_Obstacle> stationary_obstacles;
    }

    //Mission Classes
    public class Waypoint
    {
        public float altitude_msl { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public int order { get; set; }
        public bool empty { get; set; }

        public Waypoint()
        {
            empty = true;
        }
        public Waypoint(float _latitude, float _longitude)
        {
            latitude = _latitude;
            longitude = _longitude;
            empty = false;
        }
        public Waypoint(PointLatLng coordinates)
        {
            latitude = (float)coordinates.Lat;
            longitude = (float)coordinates.Lng;
        }
        public Waypoint(double _latitude, double _longitude)
        {
            latitude = (float)_latitude;
            longitude = (float)_longitude;
            empty = false;
        }

        public Waypoint(float _altitude_msl, float _latitude, float _longitude)
        {
            latitude = _latitude;
            longitude = _longitude;
            altitude_msl = _altitude_msl;
            empty = false;
        }

        public Waypoint(double _altitude_msl, double _latitude, double _longitude)
        {
            latitude = (float)_latitude;
            longitude = (float)_longitude;
            altitude_msl = (float)_altitude_msl;
            empty = false;
        }

        public Waypoint(float _altitude_msl, float _latitude, float _longitude, int _order)
        {
            latitude = _latitude;
            longitude = _longitude;
            altitude_msl = _altitude_msl;
            order = _order;
            empty = false;
        }
    }

    public class GPS_Position
    {
        public float latitude { get; set; }
        public float longitude { get; set; }
        public GPS_Position()
        {
            latitude = 0;
            longitude = 0;
        }
        public GPS_Position(GPS_Position _GPS_Position)
        {
            latitude = _GPS_Position.latitude;
            longitude = _GPS_Position.longitude;
        }
        public GPS_Position(float _latitude, float _longitude)
        {
            latitude = _latitude;
            longitude = _longitude;
        }

        public GPS_Position(double _latitude, double _longitude)
        {
            latitude = (float)_latitude;
            longitude = (float)_longitude;
        }
    }

    public class FlyZone
    {
        public float altitude_msl_max { get; set; }
        public float altitude_msl_min { get; set; }
        public List<Waypoint> boundary_pts { get; set; }
        public string name { get; set; }

        public Color border_color { get; set; }
        public Color fill_color { get; set; }

        public FlyZone(float _altitude_msl_max, float _altitude_msl_min, List<Waypoint> _boundary_pts)
        {
            altitude_msl_max = _altitude_msl_max;
            altitude_msl_min = _altitude_msl_min;
            boundary_pts = _boundary_pts;
            border_color = Color.Red;
            fill_color = Color.White;
            name = "Geofence";
        }
        public FlyZone(float _altitude_msl_max, float _altitude_msl_min, List<Waypoint> _boundary_pts, Color _border_color, Color _fill_color)
        {
            altitude_msl_max = _altitude_msl_max;
            altitude_msl_min = _altitude_msl_min;
            boundary_pts = _boundary_pts;
            border_color = _border_color;
            fill_color = _fill_color;
            name = "Geofence";
        }
        public FlyZone(float _altitude_msl_max, float _altitude_msl_min, string _name, List<Waypoint> _boundary_pts)
        {
            altitude_msl_max = _altitude_msl_max;
            altitude_msl_min = _altitude_msl_min;
            name = _name;
            border_color = Color.Red;
            fill_color = Color.White;
            boundary_pts = _boundary_pts;
        }
        public FlyZone()
        {
            altitude_msl_max = 0;
            altitude_msl_min = 0;
            name = "Geofence";
            boundary_pts = new List<Waypoint>();
            border_color = Color.Red;
            fill_color = Color.White;
        }
    }

    //The class that holds a single mission
    public class Mission
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool unedited { get; set; }
        public bool active { get; set; }
        //Position of the air drop location 
        public GPS_Position air_drop_pos { get; set; }
        //Last known position of the emergent target
        public GPS_Position emergent_lkp { get; set; }
        //A list of flyzones (geofence)
        public List<FlyZone> fly_zones { get; set; }
        public GPS_Position home_pos { get; set; }
        //Waypoints we must fly as part of the mission
        public List<Waypoint> mission_waypoints { get; set; }
        //A list of all waypoints we will be flying in our mission
        public List<Waypoint> all_waypoints { get; set; }
        public GPS_Position off_axis_target_pos { get; set; }
        public List<Waypoint> search_grid_points { get; set; }

        //SRIC removed for AUVSI 2017
        //public GPS_Position sric_pos { get; set; }

        public Mission()
        {
            id = 0;
            name = "New Mission";
            unedited = true;
            active = false;
            air_drop_pos = new GPS_Position();
            emergent_lkp = new GPS_Position();
            fly_zones = new List<FlyZone>();
            fly_zones.Add(new FlyZone());
            home_pos = new GPS_Position();
            mission_waypoints = new List<Waypoint>();
            all_waypoints = new List<Waypoint>();
            off_axis_target_pos = new GPS_Position();
            search_grid_points = new List<Waypoint>();
        }

        public Mission(Mission _Mission)
        {
            id = 0;
            name = _Mission.name;
            unedited = _Mission.unedited;
            active = _Mission.active;
            air_drop_pos = new GPS_Position(_Mission.air_drop_pos);
            emergent_lkp = new GPS_Position(_Mission.emergent_lkp);
            fly_zones = new List<FlyZone>(_Mission.fly_zones);
            home_pos = new GPS_Position(_Mission.home_pos);
            mission_waypoints = new List<Waypoint>(_Mission.mission_waypoints);
            all_waypoints = new List<Waypoint>(_Mission.all_waypoints);
            off_axis_target_pos = new GPS_Position(_Mission.off_axis_target_pos);
            search_grid_points = new List<Waypoint>(_Mission.search_grid_points);
        }
    }

    //SDA Vertex Classes
    public class Vertex : IComparable<Vertex>
    {
        public VertexCoords selfCoords; //Coordinates of this vertex
        public VertexGPSCoords gpsCoords; //GPS coordinates of the vertex
        public VertexCoords parentCoords; //Coordinates of the parent vertex
        public double g; //Goal Value - Shortest distance from start vertex the current vertex
        public double h; //Heuristic - distance from vertex to end vertex
        public bool closed = false;
        public bool open = false;
        public bool isStart = false;
        public Vertex(VertexCoords _selfCoords, VertexGPSCoords _gpsCoords, VertexCoords _parentCoords, double _g, double _h)
        {
            selfCoords = _selfCoords;
            gpsCoords = _gpsCoords;
            parentCoords = _parentCoords;
            g = _g;
            h = _h;
        }

        public Vertex(double _g, double _h)
        {
            g = _g;
            h = _h;
        }
        public Vertex(Vertex v)
        {
            selfCoords = v.selfCoords;
            gpsCoords = v.gpsCoords;
            parentCoords = v.parentCoords;
            g = v.g;
            h = v.h;
            closed = v.closed;
            open = v.open;
            isStart = v.isStart;
        }
        public int CompareTo(Vertex other)
        {
            if ((g + h) < (other.g + other.h)) return -1;
            else if ((g + h) > (other.g + other.h)) return 1;
            else return 0;
        }
    }

    public class VertexComp : IComparer<Vertex>
    {
        // Compares by Height, Length, and Width.
        public int Compare(Vertex x, Vertex y)
        {
            if (x.g + x.h > y.g + y.h)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }

    public class PriorityQueue<T> where T : IComparable<T>
    {
        private List<T> data;

        public PriorityQueue()
        {
            this.data = new List<T>();
        }

        public void Enqueue(T item)
        {
            data.Add(item);
            int ci = data.Count - 1; // child index; start at end
            while (ci > 0)
            {
                int pi = (ci - 1) / 2; // parent index
                if (data[ci].CompareTo(data[pi]) >= 0) break; // child item is larger than (or equal) parent so we're done
                T tmp = data[ci]; data[ci] = data[pi]; data[pi] = tmp;
                ci = pi;
            }
        }

        public T Dequeue()
        {
            // assumes pq is not empty; up to calling code
            int li = data.Count - 1; // last index (before removal)
            T frontItem = data[0];   // fetch the front
            data[0] = data[li];
            data.RemoveAt(li);

            --li; // last index (after removal)
            int pi = 0; // parent index. start at front of pq
            while (true)
            {
                int ci = pi * 2 + 1; // left child index of parent
                if (ci > li) break;  // no children so done
                int rc = ci + 1;     // right child
                if (rc <= li && data[rc].CompareTo(data[ci]) < 0) // if there is a rc (ci + 1), and it is smaller than left child, use the rc instead
                    ci = rc;
                if (data[pi].CompareTo(data[ci]) <= 0) break; // parent is smaller than (or equal to) smallest child so done
                T tmp = data[pi]; data[pi] = data[ci]; data[ci] = tmp; // swap parent and child
                pi = ci;
            }
            return frontItem;
        }

        public T Peek()
        {
            T frontItem = data[0];
            return frontItem;
        }

        public int Count()
        {
            return data.Count;
        }

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < data.Count; ++i)
                s += data[i].ToString() + " ";
            s += "count = " + data.Count;
            return s;
        }

        public bool IsConsistent()
        {
            // is the heap property true for all data?
            if (data.Count == 0) return true;
            int li = data.Count - 1; // last index
            for (int pi = 0; pi < data.Count; ++pi) // each parent index
            {
                int lci = 2 * pi + 1; // left child index
                int rci = 2 * pi + 2; // right child index

                if (lci <= li && data[pi].CompareTo(data[lci]) > 0) return false; // if lc exists and it's greater than parent then bad.
                if (rci <= li && data[pi].CompareTo(data[rci]) > 0) return false; // check the right child too.
            }
            return true; // passed all checks
        } // IsConsistent
    } // PriorityQueue


    public class VertexCoords
    {
        public int x;
        public int y;
        public VertexCoords(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
    }

    public class VertexGPSCoords
    {
        public double lngX;
        public double latY;
        public VertexGPSCoords(double _lngX, double _latY)
        {
            lngX = _lngX;
            latY = _latY;
        }
    }

    

    //Target Classes
    public class Target
    {
        public int id { get; set; }
        public string type { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public string orientation { get; set; }
        public float shape { get; set; }
        public float background_color { get; set; }
        public float alphanumeric { get; set; }
        public float alphanumeric_colour { get; set; }
        public float description { get; set; }
        public bool autonomous { get; set; }

        public void printall()
        {
            Console.WriteLine("Target ID: " + id + "\nType: " + type + "\nLatitude: " + latitude +
                "\nLongitude: " + longitude + "\nOrientation: " + orientation + "\nShape: " + shape +
                "\nBackground Colour: " + background_color + "\nAlphanumeric: " + alphanumeric +
                "\nAlphanumeric Colour : " + alphanumeric_colour + "\nDescription: " + description +
                "\nAutonomous: " + autonomous);
        }
    }

    public class Target_List
    {
        public List<Target> List;
    }


    public class imageOverlay
    {
        public string imagePath;
        public float heading;
        public float size;


        public imageOverlay()
        {
            imagePath = "";
            heading = 0;
            size = 0;
        }
        public imageOverlay(string _imagePath, float _heading, float _size){
            imagePath = _imagePath;
            heading = _heading;
            size = _size;
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
