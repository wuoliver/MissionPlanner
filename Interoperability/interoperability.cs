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

// One or both of these is for HTTP requests. I forget which one
using System.Net;
using System.Net.Http;

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
    public class Interoperability : Plugin
    {
        double c = 0;
        int loop_rate_hz = 10;

        String address = "http://10.10.130.10"; //"http://100.65.92.156";
        String username = "U_Toronto";
        String password = "1646641666";        

        double pollfreq = 40; // This is temporary

        // FileStream fs;

        DateTime nextrun;

        // public Assembly Assembly = null;

        // public PluginHost Host { get; internal set; }

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
            Console.WriteLine("Loop rate is " + pollfreq + " Hz.");

            /*Console.WriteLine("---INITIAL LOGIN---");
            var v = new Dictionary<string, string>();
            v.Add("username", username);
            v.Add("password", password);
            var auth = new FormUrlEncodedContent(v);
            HttpResponseMessage resp = await client.PostAsync("/api/login", auth);
            Console.WriteLine("Login POST result: " + resp.Content.ReadAsStringAsync().Result);*/

            c = 0;

            nextrun = DateTime.Now.Add(new TimeSpan(0, 0, 1));

            // FileStream fs;
            // fs = File.Create("C:\\Jesse\\utat\\VARIOUSDEBUGLOG.txt");
            // Byte[] info = new UTF8Encoding(true).GetBytes("TIME -> " + DateTime.Now.ToString("MM/dd/yyyy h:mm tt"));
            // Add some information to the file.
            // fs.Write(info, 0, info.Length);

            // Fuck everything
            Thread bg = new Thread(new ThreadStart(this.troll));
            bg.Start();

            return (true);
        }

        // Time for some anarchy
        public async void troll()
        {

            Stopwatch t = new Stopwatch();
            t.Start();

            //String path = "C:/Users/adrom/Documents/GitHub/Interoperability/VERYDEBUGLOG2.txt";

            // StreamWriter writer = new StreamWriter(
                // new FileStream(path, (/*File.Exists(path)) ?*/ FileMode.Append /*: FileMode.Create*/), FileAccess.Write));
            //     File.Open(path, FileMode.Open));
            //FileStream fs = File.Create("VERYDEBUGLOG.txt");
            /*using (StreamWriter w = new StreamWriter(
                new FileStream(path, (File.Exists(path) ? FileMode.Append : FileMode.Create), FileAccess.Write)))
            {
                // w.WriteLine("LOOP TIME -> " + DateTime.Now.ToString("MM/dd/yyyy h:mm tt"));
                w.WriteLine("Troll started: " + DateTime.Now.ToString());
            }*/

            int count = 0;
            CookieContainer cookies = new CookieContainer();


            try
            {
                /*using (var writer = new StreamWriter(
                    new FileStream(path, (File.Exists(path) ? FileMode.Append : FileMode.Create), FileAccess.Write)))*/
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
                    // string cookie = 
                    // Console.WriteLine("Session cookie: " + );
      
                    /*** GET COOKIE ***/

                    CurrentState csl = this.Host.cs;

                    
                    double lat = csl.lat, lng = csl.lng, alt = csl.altasl, hng = csl.yaw;
                    double oldlat = 0, oldlng = 0, oldalt = 0, oldhng = 0;
                    int uniquedata_count = 0;
                    double averagedata_count = 0;


                    //this.Host.FDMenuHud.
                   





                    /*
                     * For some reason the code for getting the heading is commented out in CurrentState.cs.
                     * So in order to get this to work, that file probably has to be modified, unless there's
                     * another way to do this of which I am unaware.
                     */

                    for (;;)
                    {
                        if (t.ElapsedMilliseconds > (1000 / pollfreq)) //(DateTime.Now >= nextrun)
                        {
                            // this.nextrun = DateTime.Now.Add(new TimeSpan(0, 0, 1));
                            csl = this.Host.cs;
                            lat = csl.lat;
                            lng = csl.lng;
                            alt = csl.altasl;
                            hng = csl.yaw;
                            if (lat!= oldlat || lng != oldlng || alt!= oldalt || hng != oldhng)
                            {
                                uniquedata_count++;
                                averagedata_count++;
                                oldlat = csl.lat;
                                oldlng = csl.lng;
                                oldalt = csl.altasl;
                                oldhng = csl.yaw;
                            }
                            if (count % pollfreq == 0)
                            {
                                Console.WriteLine("-----------------------------------------Unique telemtry upload rate: " + uniquedata_count + "Hz----------------------------------------------");
                                Console.WriteLine("-----------------------------------------Average telemtry uplaod rate: " + averagedata_count/(count/ pollfreq) + "Hz---------------------" );
                                //Console.Title = "-----------------------------------------Unique telemtry upload rate: " + uniquedata_count + "Hz----------------------------------------------";
                                //Console.Beep(433, 2000);  //This causes the thread to stop while it's playing
                                uniquedata_count = 0;
                            }

                            t.Restart();
                            Console.WriteLine("RUN " + count);
                            this.TrollLoop(/*writer,*/client);
                            count++;
                        }
                    }
                }
            }
            catch (HttpRequestException hre)
            {
                // Why doesn't this catch?
                Console.WriteLine("HttpRequestException: " + hre.GetBaseException()); // What the hell does this
            }
        }

        // BE CAREFUL, THIS IS SKETCHY AS FUCK
        public /*virtual int*/ async void TrollLoop(/*StreamWriter writer,*/ HttpClient client) 
        {
            // Console.WriteLine("Loop: current time is " + DateTime.Now);

            // Byte[] info = new UTF8Encoding(true).GetBytes("TIME -> " + DateTime.Now.ToString("MM/dd/yyyy h:mm tt"));
            // Add some information to the file.
            /*using (StreamWriter w = new StreamWriter(
                new FileStream(path, (File.Exists(path) ? FileMode.Append : FileMode.Create), FileAccess.Write)))
            {*/
               // writer.WriteLine("LOOP TIME -> " + DateTime.Now.ToString());

                Console.WriteLine("LOOP TIME -> " + DateTime.Now.ToString());

                /*
                try
                {
                    using (var client = new HttpClient())
                    {*/
                        // Set address.
                        // client.BaseAddress = new Uri("http://192.168.1.24/");
                        // client.BaseAddress = new Uri(address); // This seems to change every time

                        HttpResponseMessage resp;
                        // Get something for fun.
                        // resp = client.GetAsync("/").Result;
                        // Console.WriteLine("GET result: " + resp.Content.ReadAsStringAsync());
                        // Console.WriteLine(hrm.Content.ReadAsStringAsync().Result);

                        // Log in.
                        // var v = new Dictionary<string, string>();
                        // v.Add("username", username);
                        // v.Add("password", password);
                        // var auth = new FormUrlEncodedContent(v);
                        // new StringContent(/*"username=\"" + username + "\"&password=\"" + password + "\""*/
                        //         "username=testadmin&password=testpass
                        // resp = await client.PostAsync("/api/login", auth);
                        // Console.WriteLine("Login POST result: " + resp.Content.ReadAsStringAsync().Result);

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

                        //resp = await client.GetAsync("/api/server_info");
                       // Console.WriteLine("Server_info GET result: " + resp.Content.ReadAsStringAsync().Result);




                        // resp = await client.GetAsync("/api/obstacles");
                        // Console.WriteLine("Obstacles GET result: " + resp.Content.ReadAsStringAsync().Result);

                        // REQUIRED POSTS (all are FLOAT): latitude (+/-DEG), longitude (+/-DEG), altitude_msl (FT?), uas_heading (DEG)

                        // POST to /api/login login data: username, password
                        /*client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        // New code:
                        HttpResponseMessage response = await client.GetAsync("api/products/1");
                        if (response.IsSuccessStatusCode)
                        {
                            Product product = await response.Content.ReadAsAsync>Product>();
                            Console.WriteLine("{0}\t${1}\t{2}", product.Name, product.Price, product.Category);
                        }*/
                /*    }
                }
                catch (HttpRequestException hre)
                {
                    // Why doesn't this catch?
                    Console.WriteLine("HttpRequestException: " + hre.GetBaseException()); // What the hell does this do
                    
                }*/

                // client.PostAsync("_STATIC_IP_" + "/api/login", content);

                // WebRequest wr = new WebRequest();

                // do something trivial

                // Also, use something like the HTTP request plugin on Firefox to verify that Windows can connect to the VM Server            
            //}
            
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
