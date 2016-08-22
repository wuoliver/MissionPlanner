using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
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

using interoperability;

/* NOTES TO SELF
 * 
 * 1. All members inherited from abstracts need an "override" tag added in front
 * 2. Basically everything -inherited from abstracts- is temporary right now (eg. "return true")
 * 3. It seems that WebRequest is the right thing to use (ie. it isn't deprecated) THIS IS VERY FALSE
 * 
 */


namespace Interoperability
{


    public class Moving_Obstacle
    {
        public float altitude_msl { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public float sphere_radius { get; set; }

        public void printall()
        {
            Console.WriteLine("Altitude_MSL: " + altitude_msl + "\nLatitude: " + latitude + "\nLongitude: " + longitude + "\nSphere_Radius: " + sphere_radius);
        }
    }

    public class Stationary_Obstacle
    {
        public float cylinder_height { get; set; }
        public float cylinder_radius { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }


        public void printall()
        {
            Console.WriteLine("Cylinder Height: " + cylinder_height + "\nLatitude: " + latitude + "\nLongitude: " + longitude + "\nCylinder radius: " + cylinder_radius);
        }
    }

    public class Obstacles
    {
        public List<Moving_Obstacle> moving_obstacles;
        public List<Stationary_Obstacle> stationary_obstacles;
    }

    public class Interoperability : Plugin
    {
        double c = 0;
        int loop_rate_hz = 10;

        //Default credentials if credentials file does not exist
        String Default_address = "http://192.168.56.101";
        String Default_username = "testuser";
        String Default_password = "testpass";

        DateTime nextrun;
        private Thread Telemetry_Thread;        //Endabled by default
        private Thread Obstacle_SDA_Thread;     //Disabled by default
        bool Telemetry_Upload_shouldStop = false;
        bool Obstacle_SDA_shouldStop = false;

        bool resetUploadStats = false;



        //Instantiate windows forms
        interoperability.Interoperability Interoperability_GUI;

        override public string Name
        {
            get { return ("Interoperability"); }
        }
        override public string Version
        {
            get { return ("0.0.0"); }
        }
        override public string Author
        {
            get { return ("Nope"); }
        }

        /// <summary>
        /// this is the datetime loop will run next and can be set in loop, to override the loophzrate
        /// </summary>
        // Commented because it's just easier to use loopratehz
        override public DateTime NextRun
        {
            get
            {
                return nextrun;
            }
            set
            {
                //nextrun = value;
                nextrun = DateTime.Now;
            }
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

            // Start interface
            Interoperability_GUI = new interoperability.Interoperability(this.interoperabilityAction);
            Interoperability_GUI.Show();


            Console.WriteLine("Loop rate is " + Interoperability_GUI.getPollRate() + " Hz.");

            c = 0;
            nextrun = DateTime.Now.Add(new TimeSpan(0, 0, 1));

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
                    break;
                //Stop Obstacle_SDA Thread
                case 2:
                    Obstacle_SDA_shouldStop = true;
                    break;
                case 3:
                    break;
                //Reset Telemetry Upload Rate Stats
                case 4:
                    resetUploadStats = true;
                    break;
                default:
                    break;
            }

        }

        public async void Telemetry_Upload()
        {
            Console.WriteLine("Telemetry_Upload Thread Started");
            Stopwatch t = new Stopwatch();
            t.Start();

            int count = 0;
            CookieContainer cookies = new CookieContainer();

            string address, username, password;

            //Set up file paths to save default login information 
            string path = Directory.GetCurrentDirectory() + @"\Interoperability";
            Directory.CreateDirectory(path);
            Console.WriteLine("The credentials directory is {0}", path);
            //Do not change file name. 
            path += @"\credentials.txt";
            
            try
            {

                if (File.Exists(path))
                {
                    //Create new filestream for streamreader to read
                    using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        Console.WriteLine("Credential File Exists, Opening File");
                        using (StreamReader sr = new StreamReader(fs))
                        {
                            //Assuming nobody messed with the original file, we read each line into their respective variables
                            String[] credentials = new String[3];
                            for (int i = 0; i < 3; i++)
                            {
                                //Going to do some error checking in the future, in case people mess with file
                                credentials[i] = sr.ReadLine();
                            }
                            address = credentials[0];
                            username = credentials[1];
                            password = credentials[2];
                            Console.WriteLine("Address: " + address + "\nUsername: " + username + "\nPassword: " + password);
                            sr.Close();
                        }
                        fs.Close();
                    }
                }
                //File does not exist, we create a new file, and write the default server credentials 
                else
                {
                    using (FileStream fs = File.Open(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                    {
                        Console.WriteLine("Credential File Does not Exist, Creating File");

                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            sw.WriteLine(Default_address);
                            sw.WriteLine(Default_username);
                            sw.WriteLine(Default_password);

                            address = Default_address;
                            username = Default_username;
                            password = Default_password;
                            sw.Close();
                        }
                        fs.Close();
                    }
                }
            }
            catch
            {
                Console.WriteLine("File opening fail");
                //Use default credentials for now
                address = Default_address;
                username = Default_username;
                password = Default_password;
                //Should do something...not sure what for now
            }


            try
            {
                using (var client = new HttpClient())
                {
                    //bool successful_login = false;
                    //while (!successful_login)
                    // {
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

                    /*
                     * For some reason the code for getting the heading is commented out in CurrentState.cs.
                     * So in order to get this to work, that file probably has to be modified, unless there's
                     * another way to do this of which I am unaware.
                     */

                    while (!Telemetry_Upload_shouldStop)
                    //while(false)
                    {
                        //Doesn't work, need another way to do this
                        if (Interoperability_GUI.getPollRate() != 0)
                        {
                            if (t.ElapsedMilliseconds > (1000 / Math.Abs(Interoperability_GUI.getPollRate()))) //(DateTime.Now >= nextrun)
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
                                if (count % Interoperability_GUI.getPollRate() == 0)
                                {
                                    Interoperability_GUI.setAvgTelUploadText((averagedata_count / (count / Interoperability_GUI.getPollRate())) + "Hz");
                                    Interoperability_GUI.setUniqueTelUploadText(uniquedata_count + "Hz");
                                    uniquedata_count = 0;
                                }
                                if (resetUploadStats)
                                {
                                    uniquedata_count = 0;
                                    averagedata_count = 0;
                                    resetUploadStats = false;
                                }


                                t.Restart();
                                //Console.WriteLine("RUN " + count);
                                //this.TrollLoop(/*writer,*/client);

                                var vthing = new Dictionary<string, string>();

                                CurrentState cs = this.Host.cs;
                                // double lat = cs.lat, lng = cs.lng, alt = cs.altasl, yaw = cs.yaw;

                                // var v = new Dictionary<string, string>();
                                vthing.Add("latitude", lat.ToString("F10"));
                                vthing.Add("longitude", lng.ToString("F10"));
                                vthing.Add("altitude_msl", alt.ToString("F10"));
                                vthing.Add("uas_heading", yaw.ToString("F10"));
                                //Console.WriteLine("Latitude: " + lat + "\nLongitude: " + lng + "\nAltitude_MSL: " + alt + "\nHeading: " + yaw);

                                var telem = new FormUrlEncodedContent(vthing);
                                HttpResponseMessage telemresp = await client.PostAsync("/api/telemetry", telem);
                                Console.WriteLine("Server_info GET result: " + telemresp.Content.ReadAsStringAsync().Result);

                                count++;
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

            string address, username, password;

            //Set up file paths to save default login information 
            string path = Directory.GetCurrentDirectory() + @"\Interoperability";
            Directory.CreateDirectory(path);
            Console.WriteLine("The credentials directory is {0}", path);
            //Do not change file name. 
            path += @"\credentials.txt";

            try
            {
                //Create new filestream for streamreader to read
                using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    Console.WriteLine("Credential File Exists, Opening File");
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        //Assuming nobody messed with the original file, we read each line into their respective variables
                        String[] credentials = new String[3];
                        for (int i = 0; i < 3; i++)
                        {
                            //Going to do some error checking in the future, in case people mess with file
                            credentials[i] = sr.ReadLine();
                        }
                        address = credentials[0];
                        username = credentials[1];
                        password = credentials[2];

                        address = Default_address;
                        username = Default_username;
                        password = Default_password;
                        sr.Close();
                    }
                    fs.Close();
                }
            }
            catch
            {
                Console.WriteLine("File opening fail");
                //Use default credentials for now
                address = Default_address;
                username = Default_username;
                password = Default_password;
                //Should do something...not sure what for now
            }

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
                        Interoperability_GUI.setAvgTelUploadText("Error, Invalid Credentials.");
                        Interoperability_GUI.setUniqueTelUploadText("Error, Invalid Credentials");
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

                        HttpResponseMessage SDAresp = await client.GetAsync("/api/obstacles");
                        Console.WriteLine(SDAresp.Content.ReadAsStringAsync().Result);
                        count++;

                        // the code that you want to measure comes here
                        Console.WriteLine("outputting formatted data");
                        var watch = System.Diagnostics.Stopwatch.StartNew();
                        Obstacles obstaclesList = new JavaScriptSerializer().Deserialize<Obstacles>(SDAresp.Content.ReadAsStringAsync().Result);

                        watch.Stop();
                        var elapsedMs = watch.ElapsedMilliseconds;
                        Console.WriteLine("Elapsed Miliseconds: " + elapsedMs);

                        Console.WriteLine("\tPRINTING MOVING OBSTACLES");
                        for (int i = 0; i < obstaclesList.moving_obstacles.Count(); i++)
                        {
                            obstaclesList.moving_obstacles[i].printall();
                        }
                        Console.WriteLine("\tPRINTING STATIONARY OBSTACLES");
                        for (int i = 0; i < obstaclesList.stationary_obstacles.Count(); i++)
                        {
                            obstaclesList.stationary_obstacles[i].printall();
                        }


                        Interoperability_GUI.setObstacles(ref obstaclesList);

                        //Need to figure out how to draw polygons on MP map
                        //this.Host.FPDrawnPolygon.Points.Add(new PointLatLng(43.834281, -79.240994));
                        //this.Host.FPDrawnPolygon.Points.Add(new PointLatLng(43.834290, -79.240994));
                        //this.Host.FPDrawnPolygon.Points.Add(new PointLatLng(43.834281, -79.240999));
                        //this.Host.FPDrawnPolygon.Points.Add(new PointLatLng(43.834261, -79.240991));

                        Obstacle_SDA_shouldStop = false;
                    }
                }
            }
            catch
            {
                Console.WriteLine("Error, exception thrown in Obstacle_SDA Thread");
            }
        }


        // BE CAREFUL, THIS IS SKETCHY AS FUCK
        // We also don't need this until later :) 
        public /*virtual int*/ async void TrollLoop(/*StreamWriter writer,*/ HttpClient client)
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

            return (false);
        }

        /// <summary>
        /// for future expansion
        /// </summary>
        /// <param name="gui"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual bool SetupUI(int gui = 0, object data = null)
        {
            // Figure this out later. Would be useful to indicate on MP that the plugin is doing something

            return true;
        }

        /// <summary>
        /// Run at NextRun time - loop is run in a background thread. and is shared with other plugins
        /// </summary>
        /// <returns></returns>

        override public bool Loop()
        {
            // Do nothing, because this is broken.
            Console.WriteLine("The actual loop function worked??");
            return true;
        }


        /// <summary>
        /// run at a specific hz rate.
        /// </summary>
        override public /*virtual*/ float loopratehz
        {
            get { return (loop_rate_hz); }

            set { loopratehz = loop_rate_hz; }
        }

        /// <summary>
        /// Exit is only called on plugin unload. not app exit
        /// </summary>
        /// <returns></returns>
        override public bool Exit()
        {
            return (true);
        }
    }
}