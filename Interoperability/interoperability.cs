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
    public class Interoperability : Plugin
    {
        double c = 0;
        int loop_rate_hz = 10;

        //Default credentials if credentials file does not exist
        String address = "http://192.168.56.101"; 
        String username = "testuser";
        String password = "testpass";        

        DateTime nextrun;
        private Thread bg;
        bool _shouldStop = false;

        //Instantiate windows forms
        interoperability.Interoperability davis;

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
                nextrun = value;
                // nextrun = DateTime.Now;
            }
        }

        /// <summary>
        /// Run First, checking plugin
        /// </summary>
        /// <returns></returns>
        override public bool Init()
        {
            // System.Windows.Forms.MessageBox.Show("Pong");
            Console.Write(  "* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\n"
                +           "*                                   UTAT UAV                                  *\n"
                +           "*                            Interoperability 0.0.1                           *\n"
                +           "* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\n");

            // Start interface
            davis = new interoperability.Interoperability(this.interoperabilityAction);
            davis.Show();


            Console.WriteLine("Loop rate is " + davis.getPollRate() + " Hz.");

            c = 0;
            nextrun = DateTime.Now.Add(new TimeSpan(0, 0, 1));

            bg = new Thread(new ThreadStart(this.Telemetry_Upload));
            bg.Start();

            return (true);
        }

        public void interoperabilityAction(int action)
        {
            switch (action)
            {
                case 0:
                    Console.WriteLine("Telemetry_Upload Thread Stopped");
                     _shouldStop = true;
                    bg = new Thread(new ThreadStart(this.Telemetry_Upload));
                    bg.Start();
                    break;
                case 1:
                    Console.WriteLine("Thread Restarted");
                    bg = new Thread(new ThreadStart(this.Telemetry_Upload));
                    bg.Start();
                    break;
                default:
                    break;
            }
            
        }

        // Time for some anarchy
        public async void Telemetry_Upload()
        {
            Stopwatch t = new Stopwatch();
            t.Start();

            int count = 0;
            CookieContainer cookies = new CookieContainer();

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
                            Console.WriteLine(address + username + password);
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
                            sw.WriteLine(address);
                            sw.WriteLine(username);
                            sw.WriteLine(password);
                            sw.Close();
                        }
                        fs.Close();
                    }
                }
            }
            catch(FileNotFoundException)
            {
                Console.WriteLine("we have failed :(");
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
                        HttpResponseMessage resp = await client.PostAsync("/api/login", auth);
                        Console.WriteLine("Login POST result: " + resp.Content.ReadAsStringAsync().Result);
                        Console.WriteLine("---LOGIN FINISHED---");

                        if (resp.Content.ReadAsStringAsync().Result == "Invalid Credentials.")
                        {
                            Console.WriteLine("Invalid Credentials");
                            davis.setAvgTelUploadText("Error, Invalid Credentials.");
                            davis.setUniqueTelUploadText("Error, Invalid Credentials");
                            _shouldStop = true;
                        //successful_login = false;
                        }
                        else
                        {
                            Console.WriteLine("Credentials Valid");
                            _shouldStop = false;    
                        //successful_login = true;
                        }
                   // }
                    //I'm not sure how the cookie is being handled, but it works somehow. Need to ask Jesse how he managed to do it
                    /*** GET COOKIE ***/

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

                    while (!_shouldStop)
                    //while(false)
                    {
                        //Doesn't work, need another way to do this
                        if (davis.getPollRate() != 0)
                        {
                            if (t.ElapsedMilliseconds > (1000 / Math.Abs(davis.getPollRate()))) //(DateTime.Now >= nextrun)
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
                                if (count % davis.getPollRate() == 0)
                                {
                                    davis.setAvgTelUploadText((averagedata_count / (count / davis.getPollRate())) + "Hz");
                                    davis.setUniqueTelUploadText(uniquedata_count + "Hz");
                                    uniquedata_count = 0;
                                }

                                t.Restart();
                                Console.WriteLine("RUN " + count);
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
                    _shouldStop = false;
                }
            }
            catch//(HttpRequestException)
            {
                //<h1>403 Forbidden</h1> 
                Console.WriteLine("Error, problem authenticating");
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
            v.Add("altitude_msl",alt.ToString("F10"));
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

        public bool isStupid()
        {
            bool oliver = true;
            if (oliver)
            {
                return true;
            }
            else
            {
                return oliver && true;
            }
        }

        public bool isExtraStupid()
        {
            return isStupid();
        }

        public void bogusSort(int[] list)
        {
            for (int i = 0; i < 3; ++i)
            {
                list[i] = isExtraStupid() ? 1 : 0;
            }
        }
    }
}
