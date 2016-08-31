using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using MissionPlanner;
using MissionPlanner.Utilities;
using MissionPlanner.GCSViews;


using MissionPlanner.Plugin;

// Davis was here
// One or both of these is for HTTP requests. I forget which one
using System.Net;
using System.Net.Http;

//For javascript serializer
using System.Web.Script.Serialization;

using System.IO; // For logging

using System.Threading; // Trololo
using System.Diagnostics; // For stopwatch


/* NOTES TO SELF
 * 
 * 1. All members inherited from abstracts need an "override" tag added in front
 * 2. Basically everything -inherited from abstracts- is temporary right now (eg. "return true")
 * 3. It seems that WebRequest is the right thing to use (ie. it isn't deprecated) THIS IS VERY FALSE
 * 
 */


namespace Interoperability
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
        public Waypoint(float _latitude, float _longitude)
        {
            latitude = _latitude;
            longitude = _longitude;
        }
        public Waypoint(double _latitude, double _longitude)
        {
            latitude = (float)_latitude;
            longitude = (float)_longitude;
        }

        public Waypoint(float _altitude_msl, float _latitude, float _longitude)
        {
            latitude = _latitude;
            longitude = _longitude;
        }

        public Waypoint(float _altitude_msl, float _latitude, float _longitude, int order)
        {
            latitude = _latitude;
            longitude = _longitude;
        }
    }

    public class GPS_Position
    {
        public float latitude { get; set; }
        public float longitude { get; set; }
    }

    public class FlyZone
    {
        public float altitude_msl_max { get; set; }
        public float altitude_msl_min { get; set; }
        List<Waypoint> boundary_pts { get; set; }
    }

    //The class that holds a single mission
    public class Mission
    {
        public int id { get; set; }
        public bool active { get; set; }
        public GPS_Position air_drop_pos { get; set; }
        public FlyZone fly_zones { get; set; }
        public GPS_Position home_pos { get; set; }
        public List<Waypoint> mission_waypoints { get; set; }
        public GPS_Position off_axis_target_pos { get; set; }
        public List<Waypoint> search_grid_points { get; set; }
        public GPS_Position sric_pos { get; set; }
    }

    //Holds a list of missions
    public class Mission_List
    {
        public List<Mission> List { get; set; }
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


    public class Interoperability : Plugin
    {
        //Default credentials if credentials file does not exist
        String Default_address = "http://192.168.56.101";
        String Default_username = "testuser";
        String Default_password = "testpass";

        private Thread Telemetry_Thread;        //Endabled by default
        private Thread Obstacle_SDA_Thread;     //Disabled by default
        private Thread Mission_Thread;          //Disabled by default
        private Thread Map_Thread;              //Enabled by default

        bool Telemetry_Upload_shouldStop = false;   //Used to start/stop the telemtry thread
        bool Obstacle_SDA_shouldStop = false;       //Used to start/stop the SDA thread
        bool Mission_Download_shouldStop = false;   //Used to start/stop the Misison thread
        bool resetUploadStats = false;              //Used to reset telemetry upload stats
        bool Obstacles_Downloaded = false;          //Used to tell the map control thread we can access obstaclesList 

        Obstacles obstaclesList;                    //Instance that holds all SDA Obstacles 
        Interoperability_Settings Settings;         //Instance that holds all Interoperability Settings


        //Instantiate windows forms
        global::Interoperability_GUI.Interoperability_GUI Interoperability_GUI;

        override public string Name
        {
            get { return ("Interoperability"); }
        }
        override public string Version
        {
            get { return ("0.2.1"); }
        }
        override public string Author
        {
            get { return ("Jesse, Davis, Oliver"); }
        }

        
        /// <summary>
        /// Run First, checking plugin
        /// </summary>
        /// <returns></returns>
        override public bool Init()
        {
            // System.Windows.Forms.MessageBox.Show("Pong");
            Console.Write("* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\n"
                + "*                                   UTAT UAV                                  *\n"
                + "*                            Interoperability 0.0.1                           *\n"
                + "* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\n");

            //Set up settings object, and load from xml file
            Settings = new Interoperability_Settings();
            Settings.Load();

            // Start interface
            Interoperability_GUI = new global::Interoperability_GUI.Interoperability_GUI(this.interoperabilityAction, Settings);
            Interoperability_GUI.Show();


            Console.WriteLine("Loop rate is " + Interoperability_GUI.getTelemPollRate() + " Hz.");

            loopratehz = 10;

            //Must have this started, or bad things will happen
            Telemetry_Thread = new Thread(new ThreadStart(this.Telemetry_Upload));
            Telemetry_Thread.Start();




            return (true);
        }

        public void interoperabilityAction(int action)
        {
            switch (action)
            {
                //Stop then restart Telemetry_Upload Thread
                case 0:
                    Telemetry_Upload_shouldStop = true;
                    Telemetry_Thread = new Thread(new ThreadStart(this.Telemetry_Upload));
                    Telemetry_Upload_shouldStop = false;
                    Telemetry_Thread.Start();
                    break;
                //Start Obstacle_SDA Thread
                //Fix so that you can only start 1 thread at a time
                case 1:
                    Obstacle_SDA_Thread = new Thread(new ThreadStart(this.Obstacle_SDA));
                    Obstacle_SDA_Thread.Start();
                    Map_Thread = new Thread(new ThreadStart(this.Map_Control));
                    Map_Thread.Start();
                    //test_function();
                    break;
                //Stop Obstacle_SDA Thread
                case 2:
                    Obstacle_SDA_shouldStop = true;
                    break;
                case 3:
                    Mission_Thread = new Thread(new ThreadStart(this.Mission_Download));
                    Mission_Thread.Start();
                    break;
                //Reset Telemetry Upload Rate Stats
                case 4:
                    resetUploadStats = true;
                    break;
                default:
                    break;
            }

        }


        public void test_function()
        {

            //Doesn't seem to work. Need to modify FlightData.cs or ConfigPlanner.cs
            MissionPlanner.Utilities.Settings test = this.Host.config;
            test["CMB_rateattitude"] = "1";
            test.Save();

            //this.Host.FDMenuMap.
        }

        public void getLogin(ref string address, ref string username, ref string password)
        {
            if (Settings.ContainsKey("address") && Settings.ContainsKey("username") && Settings.ContainsKey("username"))
            {
                address = Settings["address"];
                username = Settings["username"];
                password = Settings["password"];
            }
            else
            {
                Settings["address"] = Default_address;
                Settings["username"] = Default_username;
                Settings["password"] = Default_password;
                address = Default_address;
                username = Default_username;
                password = Default_password;
            }
            Settings.Save();
        }


        public async void Telemetry_Upload()
        {
            Console.WriteLine("Telemetry_Upload Thread Started");
            Stopwatch t = new Stopwatch();
            t.Start();

            int count = 0;
            CookieContainer cookies = new CookieContainer();

            string address = "", username = "", password = "";

            getLogin(ref address, ref username, ref password);

            try
            {
                using (var client = new HttpClient())
                {

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
                        Interoperability_GUI.setAvgTelUploadText("Error, Invalid Credentials.");
                        Interoperability_GUI.setUniqueTelUploadText("Error, Invalid Credentials");
                        Interoperability_GUI.TelemResp(resp.Content.ReadAsStringAsync().Result);
                        Telemetry_Upload_shouldStop = true;

                    }
                    else
                    {
                        Console.WriteLine("Credentials Valid");
                        Telemetry_Upload_shouldStop = false;

                    }



                    CurrentState csl = this.Host.cs;
                    double lat = csl.lat, lng = csl.lng, alt = csl.altasl, yaw = csl.yaw;
                    double oldlat = 0, oldlng = 0, oldalt = 0, oldyaw = 0;
                    int uniquedata_count = 0;
                    double averagedata_count = 0;


                    while (!Telemetry_Upload_shouldStop)
                    {
                        //Doesn't work, need another way to do this
                        //If person sets speed to 0, then GUI crashes 
                        if (Interoperability_GUI.getTelemPollRate() != 0)
                        {
                            if (t.ElapsedMilliseconds > (1000 / Math.Abs(Interoperability_GUI.getTelemPollRate()))) //(DateTime.Now >= nextrun)
                            {
                                // this.nextrun = DateTime.Now.Add(new TimeSpan(0, 0, 1));
                                csl = this.Host.cs;
                                lat = csl.lat;
                                lng = csl.lng;
                                alt = csl.altasl;
                                yaw = csl.yaw;
                                if (lat != oldlat || lng != oldlng || alt != oldalt || yaw != oldyaw)
                                {
                                    uniquedata_count++;
                                    averagedata_count++;
                                    oldlat = csl.lat;
                                    oldlng = csl.lng;
                                    oldalt = csl.altasl;
                                    oldyaw = csl.yaw;
                                }
                                if (count % Interoperability_GUI.getTelemPollRate() == 0)
                                {
                                    Interoperability_GUI.setAvgTelUploadText((averagedata_count / (count / Interoperability_GUI.getTelemPollRate())) + "Hz");
                                    Interoperability_GUI.setUniqueTelUploadText(uniquedata_count + "Hz");
                                    uniquedata_count = 0;
                                }
                                if (resetUploadStats)
                                {
                                    uniquedata_count = 0;
                                    averagedata_count = 0;
                                    count = 0;
                                    resetUploadStats = false;
                                }


                                t.Restart();

                                var telemData = new Dictionary<string, string>();

                                CurrentState cs = this.Host.cs;

                                telemData.Add("latitude", lat.ToString("F10"));
                                telemData.Add("longitude", lng.ToString("F10"));
                                telemData.Add("altitude_msl", alt.ToString("F10"));
                                telemData.Add("uas_heading", yaw.ToString("F10"));
                                //Console.WriteLine("Latitude: " + lat + "\nLongitude: " + lng + "\nAltitude_MSL: " + alt + "\nHeading: " + yaw);

                                var telem = new FormUrlEncodedContent(telemData);
                                HttpResponseMessage telemresp = await client.PostAsync("/api/telemetry", telem);
                                Console.WriteLine("Server_info GET result: " + telemresp.Content.ReadAsStringAsync().Result);
                                Interoperability_GUI.TelemResp(telemresp.Content.ReadAsStringAsync().Result);
                                count++;
                                Interoperability_GUI.setTotalTelemUpload(count);
                            }
                        }
                    }
                }
            }

            //If this exception is thrown, then the thread will end soon after. Have no way to restart manually unless I get the loop working
            catch//(HttpRequestException)
            {
                //<h1>403 Forbidden</h1> 
                Interoperability_GUI.setAvgTelUploadText("Error, Unable to Connect to Server");
                Interoperability_GUI.setUniqueTelUploadText("Error, Unable to Connect to Server");
                Interoperability_GUI.TelemResp("Error, Unable to Connect to Server");
                Console.WriteLine("Error, exception thrown in telemtry upload thread");
            }


        }

        //This is where we periodically download the obstacles from the server 
        public async void Obstacle_SDA()
        {
            Stopwatch t = new Stopwatch();
            t.Start();

            int count = 0;
            CookieContainer cookies = new CookieContainer();

            string address = "", username = "", password = "";

            getLogin(ref address, ref username, ref password);

            try
            {
                using (var client = new HttpClient())
                {

                    client.BaseAddress = new Uri(address); // This seems to change every time

                    // Log in.
                    Console.WriteLine("---INITIAL LOGIN---");
                    var v = new Dictionary<string, string>();
                    v.Add("username", username);
                    v.Add("password", password);
                    var auth = new FormUrlEncodedContent(v);
                    HttpResponseMessage resp = await client.PostAsync("/api/login", auth);
                    Console.WriteLine("Login POST result: " + resp.Content.ReadAsStringAsync().Result);
                    Console.WriteLine("---LOGIN FINISHED---");
                    //resp.IsSuccessStatusCode;
                    if (!resp.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Invalid Credentials");
                        //Interoperability_GUI.setAvgTelUploadText("Error, Invalid Credentials.");
                        //Interoperability_GUI.setUniqueTelUploadText("Error, Invalid Credentials");
                        Obstacle_SDA_shouldStop = true;
                        //successful_login = false;
                    }
                    else
                    {
                        Console.WriteLine("Credentials Valid");
                        Obstacle_SDA_shouldStop = false;
                        //successful_login = true;
                    }

                    while (!Obstacle_SDA_shouldStop)
                    {
                        if (t.ElapsedMilliseconds > (1000 / Math.Abs(Interoperability_GUI.getsdaPollRate())))
                        {

                            HttpResponseMessage SDAresp = await client.GetAsync("/api/obstacles");
                            //Console.WriteLine(SDAresp.Content.ReadAsStringAsync().Result);
                            count++;

                            // the code that you want to measure comes here
                            Console.WriteLine("outputting formatted data");
                            obstaclesList = new JavaScriptSerializer().Deserialize<Obstacles>(SDAresp.Content.ReadAsStringAsync().Result);

                            Obstacles_Downloaded = true;

                            /*Console.WriteLine("\tPRINTING MOVING OBSTACLES");
                            for (int i = 0; i < obstaclesList.moving_obstacles.Count(); i++)
                            {
                                obstaclesList.moving_obstacles[i].printall();
                            }
                            Console.WriteLine("\tPRINTING STATIONARY OBSTACLES");
                            for (int i = 0; i < obstaclesList.stationary_obstacles.Count(); i++)
                            {
                                obstaclesList.stationary_obstacles[i].printall();
                            }*/

                            Interoperability_GUI.setObstacles(obstaclesList);

                            System.Threading.Thread.Sleep(100);

                            t.Restart();
                            Obstacle_SDA_shouldStop = false;
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine("Error, exception thrown in Obstacle_SDA Thread");
            }
        }

        public async void Mission_Download()
        {
            Stopwatch t = new Stopwatch();
            t.Start();

            int count = 0;
            CookieContainer cookies = new CookieContainer();

            string address = "", username = "", password = "";

            getLogin(ref address, ref username, ref password);

            try
            {
                using (var client = new HttpClient())
                {

                    client.BaseAddress = new Uri(address); // This seems to change every time

                    // Log in.
                    var v = new Dictionary<string, string>();
                    v.Add("username", username);
                    v.Add("password", password);
                    var auth = new FormUrlEncodedContent(v);
                    HttpResponseMessage resp = await client.PostAsync("/api/login", auth);
                    //resp.IsSuccessStatusCode;
                    if (!resp.IsSuccessStatusCode)
                    {
                        Mission_Download_shouldStop = true;
                        //successful_login = false;
                    }
                    else
                    {
                        Console.WriteLine("Credentials Valid");
                        Mission_Download_shouldStop = false;
                        //successful_login = true;
                    }



                    while (!Mission_Download_shouldStop)
                    {

                        HttpResponseMessage SDAresp = await client.GetAsync("/api/missions");
                        Console.WriteLine(SDAresp.Content.ReadAsStringAsync().Result);
                        count++;

                        //Mission_List missionList = new JavaScriptSerializer().Deserialize<Mission_List>(SDAresp.Content.ReadAsStringAsync().Result);
                        Mission_List missionList = new JavaScriptSerializer().Deserialize<Mission_List>(Settings["test"]);

                        Mission_Download_shouldStop = true;
                    }
                }
            }
            catch
            {
                Console.WriteLine("Error, exception thrown in Obstacle_SDA Thread");
            }
        }

        public /*async*/ void Map_Control()
        {
            Stopwatch t = new Stopwatch();
            t.Start();
            bool Static_Overlays_Drawn = false;
            string PolyName;


            //Add static overlays:
            //Issue because need to wait until obstaclesList has loaded or been instantiated

            //For testing right now. Will update when server has misison functionality added
            List<Waypoint> Op_Area = new List<Waypoint>();
            Op_Area.Add(new Waypoint(38.1462694, -76.4277778));
            Op_Area.Add(new Waypoint(38.151625,  -76.4286833));
            Op_Area.Add(new Waypoint(38.1518889, -76.4314667));
            Op_Area.Add(new Waypoint(38.1505944, -76.4353611));
            Op_Area.Add(new Waypoint(38.1475667, -76.4323417));
            Op_Area.Add(new Waypoint(38.1446667, -76.4329472));
            Op_Area.Add(new Waypoint(38.1432556, -76.4347667));
            Op_Area.Add(new Waypoint(38.1404639, -76.4326361));
            Op_Area.Add(new Waypoint(38.1407194, -76.4260139));
            Op_Area.Add(new Waypoint(38.1437611, -76.4212056));
            Op_Area.Add(new Waypoint(38.1473472, -76.4232111));
            Op_Area.Add(new Waypoint(38.1461306, -76.4266528));

            List<Waypoint> Search_Area = new List<Waypoint>();
            Search_Area.Add(new Waypoint(38.1457306, -76.4295972));
            Search_Area.Add(new Waypoint(38.1431861, -76.4338917));
            Search_Area.Add(new Waypoint(38.1410028, -76.4322333));
            Search_Area.Add(new Waypoint(38.1411917, -76.4269806));
            Search_Area.Add(new Waypoint(38.1422194, -76.4261111));


            while (true)
            {
                //Console.WriteLine("Elapsed Miliseconds: " + t.ElapsedMilliseconds);
                if (t.ElapsedMilliseconds > (1000 / Math.Abs(Interoperability_GUI.getMapRefreshRate())))
                {
                    Interoperability_GUI.MAP_Clear_Overlays();
                    if (Obstacles_Downloaded)
                    {
                        if (!Static_Overlays_Drawn)
                        {
                            for (int i = 0; i < obstaclesList.stationary_obstacles.Count(); i++)
                            {
                                Interoperability_GUI.MAP_addSObstaclePoly(obstaclesList.stationary_obstacles[i].cylinder_radius* 0.3048,
                                    obstaclesList.stationary_obstacles[i].latitude, obstaclesList.stationary_obstacles[i].longitude);
                            }
                            Static_Overlays_Drawn = false;
                        }
                        
                        for (int i = 0; i < obstaclesList.moving_obstacles.Count(); i++)
                        {
                            //PolyName = "StationaryObject" + i.ToString();
                            Interoperability_GUI.MAP_addCirclePoly(obstaclesList.moving_obstacles[i].sphere_radius * 0.3048,
                                obstaclesList.moving_obstacles[i].latitude, obstaclesList.moving_obstacles[i].longitude, "polygon");
                        }

                        Interoperability_GUI.MAP_addStaticPoly(Op_Area, "Operation_Area", Color.Red, Color.Transparent, 3, 50);
                        Interoperability_GUI.MAP_addStaticPoly(Search_Area, "Search_Area", Color.Green, Color.Green, 3, 90);
                        Interoperability_GUI.MAP_updatePlaneLoc(new PointLatLng(Host.cs.lat, Host.cs.lng), Host.cs.alt, Host.cs.yaw);
                    }
                    Interoperability_GUI.MAP_Update_Overlay();
                    t.Restart();
                }
            }
             
        }
         
        // BE CAREFUL, THIS IS SKETCHY AS FUCK
        // We also don't need this until later :) 
        public /*virtual int*/
                async void TrollLoop(/*StreamWriter writer,*/ HttpClient client)
        {
            //Console.WriteLine("LOOP TIME -> " + DateTime.Now.ToString());

            CurrentState cs = this.Host.cs;
            double lat = cs.lat, lng = cs.lng, alt = cs.altasl, yaw = cs.yaw;

            var v = new Dictionary<string, string>();
            v.Add("latitude", lat.ToString("F10"));
            v.Add("longitude", lng.ToString("F10"));
            v.Add("altitude_msl", alt.ToString("F10"));
            v.Add("uas_heading", yaw.ToString("F10"));
            //Console.WriteLine("Latitude: " + lat + "\nLongitude: " + lng + "\nAltitude_MSL: " + alt + "\nHeading: " + yaw);

            var telem = new FormUrlEncodedContent(v);
            HttpResponseMessage telemresp = await client.PostAsync("/api/telemetry", telem);
            Console.WriteLine("Server_info GET result: " + telemresp.Content.ReadAsStringAsync().Result);

            return;
        }

        /// <summary>
        /// Load your own code here, this is only run once on loading
        /// </summary>
        /// <returns></returns>
        override public bool Loaded()
        {
            Console.WriteLine("* * * * * Interoperability plugin loaded. * * * * *");

            // Attempt to login to server?

            return (true);
        }


        /// <summary>
        /// Run at NextRun time - loop is run in a background thread. and is shared with other plugins
        /// </summary>
        /// <returns></returns>
        override public bool Loop()
        {

            return true;
        }

        /// <summary>
        /// Exit is only called on plugin unload. not app exit
        /// </summary>
        /// <returns></returns>
        override public bool Exit()
        {
            Map_Thread.Abort();
            Telemetry_Thread.Abort();
            Obstacle_SDA_Thread.Abort();
            return (true);
        }
    }


    public class Interoperability_Settings
    {
        static Interoperability_Settings _instance;

        public static Interoperability_Settings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Interoperability_Settings();
                }
                return _instance;
            }
        }

        public Interoperability_Settings()
        {
        }

        /// <summary>
        /// use to store all internal config
        /// </summary>
        public static Dictionary<string, string> config = new Dictionary<string, string>();

        const string FileName = "Interoperability_Config.xml";

        public string this[string key]
        {
            get
            {
                string value = null;
                config.TryGetValue(key, out value);
                return value;
            }

            set
            {
                config[key] = value;
            }
        }

        public IEnumerable<string> Keys
        {
            // the "ToArray" makes this safe for someone to add items while enumerating.
            get { return config.Keys.ToArray(); }
        }
        public bool ContainsKey(string key)
        {
            return config.ContainsKey(key);
        }



        public int Count { get { return config.Count; } }


        internal int GetInt32(string key)
        {
            int result = 0;
            string value = null;
            if (config.TryGetValue(key, out value))
            {
                int.TryParse(value, out result);
            }
            return result;
        }

        internal bool GetBoolean(string key)
        {
            bool result = false;
            string value = null;
            if (config.TryGetValue(key, out value))
            {
                bool.TryParse(value, out result);
            }
            return result;
        }

        internal float GetFloat(string key)
        {
            float result = 0f;
            string value = null;
            if (config.TryGetValue(key, out value))
            {
                float.TryParse(value, out result);
            }
            return result;
        }

        internal double GetDouble(string key)
        {
            double result = 0D;
            string value = null;
            if (config.TryGetValue(key, out value))
            {
                double.TryParse(value, out result);
            }
            return result;
        }

        internal byte GetByte(string key)
        {
            byte result = 0;
            string value = null;
            if (config.TryGetValue(key, out value))
            {
                byte.TryParse(value, out result);
            }
            return result;
        }

        public static string GetFullPath()
        {
            string directory = Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            return Path.Combine(directory, FileName);
        }

        public void Load()
        {
            using (XmlTextReader xmlreader = new XmlTextReader(GetFullPath()))
            {
                while (xmlreader.Read())
                {
                    if (xmlreader.NodeType == XmlNodeType.Element)
                    {
                        try
                        {
                            switch (xmlreader.Name)
                            {
                                case "Config":
                                    break;
                                case "xml":
                                    break;
                                default:
                                    config[xmlreader.Name] = xmlreader.ReadString();
                                    break;
                            }
                        }
                        // silent fail on bad entry
                        catch (Exception)
                        {
                        }
                    }
                }
            }
        }

        public void Save()
        {
            string filename = GetFullPath();

            using (XmlTextWriter xmlwriter = new XmlTextWriter(filename, Encoding.UTF8))
            {
                xmlwriter.Formatting = Formatting.Indented;

                xmlwriter.WriteStartDocument();

                xmlwriter.WriteStartElement("Config");

                foreach (string key in config.Keys)
                {
                    try
                    {
                        if (key == "" || key.Contains("/")) // "/dev/blah"
                            continue;

                        xmlwriter.WriteElementString(key, "" + config[key]);
                    }
                    catch
                    {
                    }
                }

                xmlwriter.WriteEndElement();

                xmlwriter.WriteEndDocument();
                xmlwriter.Close();
            }
        }

        public void Remove(string key)
        {
            if (config.ContainsKey(key))
            {
                config.Remove(key);
            }
        }

    }
}