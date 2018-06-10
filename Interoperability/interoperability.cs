﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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

using System.IO.Ports;

using System.Speech.Synthesis;
using System.Speech.Recognition;

using MissionPlanner.Plugin;
using MissionPlanner.Comms;

using Interoperability_GUI_Forms;

// Davis was here
// One or both of these is for HTTP requests. I forget which one
using System.Net;
using System.Net.Http;

//For spline interpolation
using Spline;

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

namespace interoperability
{
    public class Interoperability : Plugin
    {
        //Default credentials if credentials file does not exist
        private string address = "http://192.168.56.101";
        private string username = "testuser";
        private string password = "testpass";

        private string dist_units = "Metres";
        private string airspd_units = "Metres per Second";
        private string geo_cords = "DD.DDDDDD";

        //Constants 
        //public const double PositiveInfinity;

        //SDA Simulator Values
        private double sim_lat = 0;
        private double sim_lng = 0;
        private float sim_alt = 0;
        private float sim_yaw = 0;
        private int sim_next_wp = 0;

        private static Mutex Interoperability_Action_Mutex = new Mutex();

        private Thread Telemetry_Thread;
        private Thread Obstacle_SDA_Thread;
        private Thread Mission_Thread;
        private Thread Map_Control_Thread;
        private Thread Callout_Thread;
        private Thread SDA_Plane_Simulator_Thread;
        private Thread SDA_Avoidance_Algorithm_Thread;
        private Thread Drone_Server_Thread;
        private Thread PV_Drone_Control_Thread;
        private Thread PV_Power_Read_Thread;
        private Thread Bottle_Drop_Thread;

        private bool Telemetry_Thread_shouldStop = true;        //Used to start/stop the telemtry thread
        private bool Obstacle_SDA_Thread_shouldStop = true;     //Used to start/stop the SDA thread
        private bool Mission_Thread_shouldStop = true;          //Used to start/stop the misison thread
        private bool Map_Control_Thread_shouldStop = true;      //Used to start/stop the map control thread
        private bool Callout_Thread_shouldStop = true;          //Used to start/stop the callout thread
        private bool SDA_Plane_Simulator_Thread_shouldStop = true;
        private bool SDA_Avoidance_Algorithm_Thread_shouldStop = true;
        private bool PV_Drone_Control_Thread_shouldStop = true;
        private bool PV_Power_Read_Thread_shouldStop = true;
        private bool Bottle_Drop_Thread_shouldStop = true;

        private int ImportantCounter = 0;
        private long ImporantTimeCount = 0;
        private Stopwatch ImportantTimer = new Stopwatch();

        private double Panel_Voltage = -1;
        private double Panel_Temp = -1;

        bool Obstacles_Downloaded = false;                  //Used to tell the map control thread we can access obstaclesList 
        bool resetUploadStats = false;                      //Used to reset telemetry upload stats
        bool resetFlightTimer = false;                      //Used to reset the flight timer
        bool usePlaneSimulator = false;                     //Used for the plane simulator


        public bool mapinvalidateWaypoints = true;
        public bool mapinvalidateGeofence = true;
        public bool mapinvalidateSearchArea = true;
        public bool mapinvalidateObstacle = true;
        public bool mapinvalidateOFAT_EM_DROP = true;
        public bool mapinvalidateImage = true;

        Obstacles obstaclesList = new Obstacles();          //Instance that holds all SDA Obstacles 
        public Interoperability_Settings Settings;          //Instance that holds all Interoperability Settings

        public List<Mission> Mission_List { get; set; }   //Holds a list of all missions from interoperability + Server
        public Mission Current_Mission { get; set; }      //The current mission open in the program  

        private static Interoperability Instance = null;

        public static Mutex Interoperability_Mutex = new Mutex();

        //Instantiate windows forms
        Interoperability_GUI_Main Interoperability_GUI;

        override public string Name
        {
            get { return ("Interoperability"); }
        }
        override public string Version
        {
            get { return ("0.7.6"); }
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
            Instance = this;
            // System.Windows.Forms.MessageBox.Show("Pong");
            Console.Write("* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\n"
                + "*                                   UTAT UAV                                  *\n"
                + "*                            Interoperability 0.7.6                           *\n"
                + "* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\n");


            //Set up settings object, and load from xml file
            Settings = new Interoperability_Settings();
            Settings.Load();
            getSettings();

            //Set up host settings to include distance units. For some reason the distunits is not loaded
            //at startup on some programs unless you modify the setting
            if (Host.config["distunits"] == null)
            {
                Console.Write("Error, can't find distance unit in settings. Setting to Meters");
                Host.config["distunits"] = "Meters";
            }

            //Instantiate all threads, but do not start
            Telemetry_Thread = new Thread(new ThreadStart(this.Telemetry_Upload));
            Obstacle_SDA_Thread = new Thread(new ThreadStart(this.Obstacle_SDA));
            Mission_Thread = new Thread(new ThreadStart(this.Mission_Download));
            Callout_Thread = new Thread(new ThreadStart(this.Callouts));
            SDA_Plane_Simulator_Thread = new Thread(new ThreadStart(this.SDA_Plane_Simulator));
            SDA_Avoidance_Algorithm_Thread = new Thread(new ThreadStart(this.SDA_Avoidance_Algorithm));
            Map_Control_Thread = new Thread(new ThreadStart(this.Map_Control));
            Drone_Server_Thread = new Thread(new ThreadStart(this.drone_cleaning_server));
            PV_Drone_Control_Thread = new Thread(new ThreadStart(this.pv_drone_control));
            PV_Power_Read_Thread = new Thread(new ThreadStart(this.pv_power_read));
            Bottle_Drop_Thread = new Thread(new ThreadStart(this.Bottle_Drop));

            //Instantiate Mission_List
            Mission_List = new List<Mission>();
            Current_Mission = new Mission();

            // Start interface
            Interoperability_GUI = new Interoperability_GUI_Forms.Interoperability_GUI_Main(this.interoperabilityAction, Settings);
            if (Convert.ToBoolean(Settings["showInteroperability_GUI"]) == true)
            {
                Interoperability_GUI.Show();
                //Start map thread
                Map_Control_Thread = new Thread(new ThreadStart(this.Map_Control));
                Map_Control_Thread_shouldStop = false;
                Map_Control_Thread.Start();
            }

            loopratehz = 0.25F;

            //Add item to flight data context menu 
            Host.FDMenuMap.Items.Add(Interoperability_GUI.getContextMenu());
            Host.MainForm.MainMenu.Items.Insert(2, Interoperability_GUI.getMenuStrip());
            if (Settings["gui_format"] == "Drone PV Cleaning")
            {
                Host.MainForm.MainMenu.Items[2].Name = "Drone PV";
            }

            // MyView.AddScreen(new MainSwitcher.Screen("FlightData", FlightData, true));
            //Start Important Timer
            ImportantTimer.Start();


            Console.WriteLine("End of init()");

            return (true);
        }

        /// <summary>
        /// Returns the current interoperability instance
        /// </summary>
        /// <returns></returns>
        public static Interoperability getinstance()
        {
            return Instance;
        }

        public enum Interop_Action
        {
            Telemtry_Thread_Start,
            Telemtry_Thread_Stop,
            Obstacle_SDA_Thread_Start,
            Obstacle_SDA_Thread_Stop,
            Mission_Download_Run,
            Telemetry_Thread_Reset_Stats,
            Restart_Threads_Settings,
            Stop_All_Threads_Quit,
            Show_Interop_GUI,
            Easter_Egg_Action,
            Callout_Thread_Start,
            Callout_Thread_Stop,
            Map_Thread_Restart_Flight_Timer,
            Clear_Cur_Mission,
            SDA_Plane_Simulator_Thread_Start,
            SDA_Plane_Simulator_Thread_Stop,
            SDA_Avoidance_Algorithm_Thread_Start,
            SDA_Avoidance_Algorithm_Thread_Stop,
            MAV_Command_Arm_Disarm,
            MAV_Command_Set_Position,
            Drone_Cleaning_Server_Start,
            PV_Drone_Control_Start,
            PV_Power_Read,
            Bottle_Drop_Start,
            Bottle_Drop_Stop,
            TEST
        }

        /// <summary>
        /// Allows other functions to control the interoperability threads.
        /// </summary>
        /// <param name="action"></param>
        public void interoperabilityAction(Interop_Callback_Struct action)
        {
            Interoperability_Action_Mutex.WaitOne();
            switch (action.action)
            {
                //Start Telemetry_Upload Thread
                case Interop_Action.Telemtry_Thread_Start:
                    Stop_Thread(ref Telemetry_Thread, ref Telemetry_Thread_shouldStop);
                    Telemetry_Thread = new Thread(new ThreadStart(this.Telemetry_Upload));
                    Telemetry_Thread_shouldStop = false;
                    Telemetry_Thread.Start();
                    break;

                //Stop telemtry upload thread
                case Interop_Action.Telemtry_Thread_Stop:
                    Stop_Thread(ref Telemetry_Thread, ref Telemetry_Thread_shouldStop);
                    break;

                //Start Obstacle_SDA Thread
                case Interop_Action.Obstacle_SDA_Thread_Start:
                    Stop_Thread(ref Obstacle_SDA_Thread, ref Obstacle_SDA_Thread_shouldStop);
                    Obstacle_SDA_Thread = new Thread(new ThreadStart(this.Obstacle_SDA));
                    Obstacle_SDA_Thread_shouldStop = false;
                    Obstacle_SDA_Thread.Start();
                    break;

                //Stop Obstacle_SDA Thread
                case Interop_Action.Obstacle_SDA_Thread_Stop:
                    Stop_Thread(ref Obstacle_SDA_Thread, ref Obstacle_SDA_Thread_shouldStop);
                    break;

                //Run mission download thread
                case Interop_Action.Mission_Download_Run:
                    Stop_Thread(ref Mission_Thread, ref Mission_Thread_shouldStop);
                    Mission_Thread = new Thread(new ThreadStart(this.Mission_Download));
                    Mission_Thread_shouldStop = false;
                    Mission_Thread.Start();
                    break;

                //Reset Telemetry Upload Rate Stats
                case Interop_Action.Telemetry_Thread_Reset_Stats:
                    resetUploadStats = true;
                    if (Telemetry_Thread.IsAlive == false)
                    {
                        Interoperability_GUI.setAvgTelUploadText("0Hz");
                        Interoperability_GUI.setUniqueTelUploadText("0Hz");
                        Interoperability_GUI.setTotalTelemUpload(0);
                    }
                    break;

                //Restart all running threads that rely on server credentials or unit settings
                case Interop_Action.Restart_Threads_Settings:
                    getSettings();
                    //No need to reset the map control thread
                    if (Map_Control_Thread.IsAlive == false)
                    {
                        Stop_Thread(ref Map_Control_Thread, ref Map_Control_Thread_shouldStop);
                        Map_Control_Thread = new Thread(new ThreadStart(this.Map_Control));
                        Map_Control_Thread_shouldStop = false;
                        Map_Control_Thread.Start();
                    }
                    //If GUI format is not AUVSI, disable all AUVSI Threads
                    if (Settings["gui_format"] != "AUVSI")
                    {
                        Stop_Thread(ref Telemetry_Thread, ref Telemetry_Thread_shouldStop);
                        Stop_Thread(ref Obstacle_SDA_Thread, ref Obstacle_SDA_Thread_shouldStop);
                        Stop_Thread(ref Mission_Thread, ref Mission_Thread_shouldStop);
                        Stop_Thread(ref SDA_Avoidance_Algorithm_Thread, ref SDA_Avoidance_Algorithm_Thread_shouldStop);
                        Stop_Thread(ref SDA_Plane_Simulator_Thread, ref SDA_Plane_Simulator_Thread_shouldStop);
                    }
                    else
                    {
                        if (Telemetry_Thread.IsAlive)
                        {
                            Stop_Thread(ref Telemetry_Thread, ref Telemetry_Thread_shouldStop);
                            Telemetry_Thread = new Thread(new ThreadStart(this.Telemetry_Upload));
                            Telemetry_Thread_shouldStop = false;
                            Telemetry_Thread.Start();
                        }
                        if (Obstacle_SDA_Thread.IsAlive)
                        {
                            Stop_Thread(ref Obstacle_SDA_Thread, ref Obstacle_SDA_Thread_shouldStop);
                            Obstacle_SDA_Thread = new Thread(new ThreadStart(this.Obstacle_SDA));
                            Obstacle_SDA_Thread_shouldStop = false;
                            Obstacle_SDA_Thread.Start();
                        }
                        if (Mission_Thread.IsAlive)
                        {
                            Stop_Thread(ref Mission_Thread, ref Mission_Thread_shouldStop);
                            Mission_Thread = new Thread(new ThreadStart(this.Mission_Download));
                            Mission_Thread_shouldStop = false;
                            Mission_Thread.Start();
                        }
                    }

                    break;

                //Stop all threads, when loop until all threads are done
                case Interop_Action.Stop_All_Threads_Quit:

                    Telemetry_Thread_shouldStop = true;
                    Mission_Thread_shouldStop = true;
                    Obstacle_SDA_Thread_shouldStop = true;
                    Map_Control_Thread_shouldStop = true;
                    Callout_Thread_shouldStop = true;
                    SDA_Plane_Simulator_Thread_shouldStop = true;
                    SDA_Avoidance_Algorithm_Thread_shouldStop = true;
                    PV_Drone_Control_Thread_shouldStop = true;
                    PV_Power_Read_Thread_shouldStop = true;
                    Bottle_Drop_Thread_shouldStop = true;

                    Stopwatch t = new Stopwatch();
                    t.Start();
                    while (Mission_Thread.IsAlive || Obstacle_SDA_Thread.IsAlive || Telemetry_Thread.IsAlive || Map_Control_Thread.IsAlive ||
                        Callout_Thread.IsAlive || SDA_Plane_Simulator_Thread.IsAlive || SDA_Avoidance_Algorithm_Thread.IsAlive || PV_Drone_Control_Thread.IsAlive
                        || PV_Power_Read_Thread.IsAlive || Bottle_Drop_Thread.IsAlive)
                    {
                        //If all threads haven't quit in 1 seconds, force quit
                        if (t.ElapsedMilliseconds > 1000)
                        {
                            Telemetry_Thread.Abort();
                            Mission_Thread.Abort();
                            Obstacle_SDA_Thread.Abort();
                            Map_Control_Thread.Abort();
                            Callout_Thread.Abort();
                            SDA_Avoidance_Algorithm_Thread.Abort();
                            SDA_Plane_Simulator_Thread.Abort();
                            PV_Drone_Control_Thread.Abort();
                            PV_Power_Read_Thread.Abort();
                            Bottle_Drop_Thread.Abort();
                            break;
                        }
                        //Wait until all threads have stopped
                    }
                    break;

                //Show the interoperability control panel
                case Interop_Action.Show_Interop_GUI:
                    if (!Interoperability_GUI.isOpened)
                    {
                        Interoperability_GUI = new Interoperability_GUI_Forms.Interoperability_GUI_Main(this.interoperabilityAction, Settings);
                        Interoperability_GUI.Show();
                        //Start map thread
                        Stop_Thread(ref Map_Control_Thread, ref Map_Control_Thread_shouldStop);
                        Map_Control_Thread = new Thread(new ThreadStart(this.Map_Control));
                        Map_Control_Thread_shouldStop = false;
                        Map_Control_Thread.Start();
                    }
                    else
                    {
                        Interoperability_GUI.BringToFront();
                        if (Interoperability_GUI.WindowState == FormWindowState.Minimized)
                        {
                            Interoperability_GUI.WindowState = FormWindowState.Normal;
                        }
                    }
                    break;

                //Easter egg
                case Interop_Action.Easter_Egg_Action:
                    if (ImportantTimer.ElapsedMilliseconds - ImporantTimeCount > 100)
                    {
                        switch (ImportantCounter)
                        {
                            case 0:
                                Host.MainForm.MainMenu.Items[2].Image = Properties.Resources.Interop_Icon_Oliver;
                                break;
                            case 1:
                                Host.MainForm.MainMenu.Items[2].Image = Properties.Resources.Interop_Icon_Yih_Tang;
                                break;
                            case 2:
                                Host.MainForm.MainMenu.Items[2].Image = Properties.Resources.Interop_Icon_Erik;
                                break;
                            case 3:
                                Host.MainForm.MainMenu.Items[2].Image = Properties.Resources.Interop_Icon_Jesse;
                                break;
                            case 4:
                                Host.MainForm.MainMenu.Items[2].Image = Properties.Resources.Interop_Icon_Rikky;
                                break;
                            default:
                                Host.MainForm.MainMenu.Items[2].Image = Properties.Resources.Interop_Icon;
                                ImportantCounter = -1;
                                break;
                        }
                        ImporantTimeCount = ImportantTimer.ElapsedMilliseconds;
                        ImportantCounter++;
                    }
                    break;

                //Start callout thread
                case Interop_Action.Callout_Thread_Start:
                    Stop_Thread(ref Callout_Thread, ref Callout_Thread_shouldStop);
                    Callout_Thread = new Thread(new ThreadStart(this.Callouts));
                    Callout_Thread_shouldStop = false;
                    Callout_Thread.Start();
                    break;

                //Stop callout thread
                case Interop_Action.Callout_Thread_Stop:
                    Stop_Thread(ref Callout_Thread, ref Callout_Thread_shouldStop);
                    break;

                //Reset Flight Timer
                case Interop_Action.Map_Thread_Restart_Flight_Timer:
                    resetFlightTimer = true;
                    break;

                //Clear current mission
                case Interop_Action.Clear_Cur_Mission:
                    Current_Mission = new Mission();
                    Current_Mission.name = "TEST RESET";
                    break;

                //Start simulator
                case Interop_Action.SDA_Plane_Simulator_Thread_Start:
                    Stop_Thread(ref SDA_Plane_Simulator_Thread, ref SDA_Plane_Simulator_Thread_shouldStop);
                    SDA_Plane_Simulator_Thread = new Thread(new ThreadStart(this.SDA_Plane_Simulator));
                    SDA_Plane_Simulator_Thread_shouldStop = false;
                    usePlaneSimulator = true;
                    SDA_Plane_Simulator_Thread.Start();
                    break;

                //Stop simulator
                case Interop_Action.SDA_Plane_Simulator_Thread_Stop:
                    usePlaneSimulator = false;
                    Stop_Thread(ref SDA_Plane_Simulator_Thread, ref SDA_Plane_Simulator_Thread_shouldStop);
                    break;

                //Start or stop SDA avoidance algorithm
                case Interop_Action.SDA_Avoidance_Algorithm_Thread_Start:
                    Stop_Thread(ref SDA_Avoidance_Algorithm_Thread, ref SDA_Avoidance_Algorithm_Thread_shouldStop);
                    SDA_Avoidance_Algorithm_Thread = new Thread(new ThreadStart(this.SDA_Avoidance_Algorithm));
                    SDA_Avoidance_Algorithm_Thread_shouldStop = false;
                    SDA_Avoidance_Algorithm_Thread.Start();
                    break;

                case Interop_Action.SDA_Avoidance_Algorithm_Thread_Stop:
                    Stop_Thread(ref SDA_Avoidance_Algorithm_Thread, ref SDA_Avoidance_Algorithm_Thread_shouldStop);
                    break;
                case Interop_Action.MAV_Command_Arm_Disarm:
                    //Check to see if there is a connection to the quad
                    if (!Host.comPort.BaseStream.IsOpen)
                    {
                        return;
                    }
                    //Check to see if we have an invalid mavlink command
                    if (action.mav_command.actionid == MAVLink.MAV_CMD.ENUM_END)
                    {
                        MessageBox.Show("Error, invalid mavlink command", Strings.ERROR);
                        return;
                    }

                    try
                    {
                        if (action.mav_command.actionid == MAVLink.MAV_CMD.COMPONENT_ARM_DISARM)
                        {
                            if (!Host.comPort.MAV.cs.armed)
                                if (MessageBox.Show("Are you sure you want to Arm?", "Arm?", MessageBoxButtons.YesNo) !=
                                    DialogResult.Yes)
                                    return;
                        }

                        bool ans = Host.comPort.doCommand(action.mav_command.actionid, action.mav_command.p1, action.mav_command.p2,
                            action.mav_command.p3, action.mav_command.p4, action.mav_command.p5, action.mav_command.p6,
                            action.mav_command.p7, action.mav_command.requireack);

                        if (ans == false)
                            MessageBox.Show(Strings.ErrorRejectedByMAV, Strings.ERROR);
                    }
                    catch
                    {
                        MessageBox.Show(Strings.ErrorNoResponce, Strings.ERROR);
                    }

                    break;

                case Interop_Action.MAV_Command_Set_Position:
                    //Check to see if there is a connection to the quad
                    if (!Host.comPort.BaseStream.IsOpen)
                    {
                        return;
                    }

                    try
                    {
                        MAVLink.mavlink_set_position_target_local_ned_t packet;
                        packet = action.mav_position;
                        packet.target_system = Host.comPort.MAV.sysid;
                        packet.target_component = Host.comPort.MAV.compid;
                        packet.time_boot_ms = 0;
                        packet.afx = 0;
                        packet.afy = 0;
                        packet.afz = 0;

                        /*MAVLink.mavlink_rc_channels_override_t rc_override = new MAVLink.mavlink_rc_channels_override_t();
                        rc_override.target_component = Host.comPort.MAV.sysid;
                        rc_override.target_component = Host.comPort.MAV.compid;

                        rc_override.chan1_raw = 0;
                        rc_override.chan2_raw = 0;
                        rc_override.chan3_raw = (ushort)(1600);
                        rc_override.chan4_raw = 0; 
                        rc_override.chan5_raw = 0;
                        rc_override.chan6_raw = 0; 
                        rc_override.chan7_raw = 0;
                        rc_override.chan8_raw = 0;*/

                        //send the command
                        //Host.comPort.sendPacket(rc_override, Host.comPort.MAV.sysid, Host.comPort.MAV.compid);
                        Host.comPort.sendPacket(packet, Host.comPort.MAV.sysid, Host.comPort.MAV.compid);

                        //Reset rc_override back to the RC controller after 200ms 
                        //Thread.Sleep(200);
                        //rc_override.chan3_raw = 0;
                        //Host.comPort.sendPacket(rc_override, Host.comPort.MAV.sysid, Host.comPort.MAV.compid);
                    }
                    catch
                    {
                        MessageBox.Show(Strings.ErrorNoResponce, Strings.ERROR);
                    }
                    //MAVLink.mavlink_command_int_t command = new MAVLink.mavlink_command_int_t();
                    //Host.comPort.

                    break;
                case Interop_Action.Bottle_Drop_Start:
                    Bottle_Drop_Thread = new Thread(new ThreadStart(this.Bottle_Drop));
                    Bottle_Drop_Thread.Start();
                    break;
                case Interop_Action.Bottle_Drop_Stop:
                    Bottle_Drop_Thread_shouldStop = true;
                    break;

                case Interop_Action.TEST:
                    List<Waypoint> temp = new List<Waypoint>();
                    temp.Add(new Waypoint(300, 38.149220, -76.429480));
                    temp.Add(new Waypoint(300, 38.150140, -76.430850));
                    temp.Add(new Waypoint(300, 38.148950, -76.432290));
                    temp.Add(new Waypoint(400, 38.147010, -76.430640));
                    temp.Add(new Waypoint(200, 38.143780, -76.431990));

                    List<MAVLink.mavlink_mission_item_t> temp_waypoints = create_mission(MAVLink.MAV_TYPE.QUADROTOR, temp);
                    export_mission_file(temp_waypoints, temp_waypoints[0]);
                    /*
                    foreach (MAVLink.mavlink_mission_item_t i in temp_waypoints)
                    {
                        MAVLink.MAV_MISSION_RESULT result =  Host.comPort.setWP(i);
                        if(result == MAVLink.MAV_MISSION_RESULT.MAV_MISSION_ACCEPTED)
                        {
                            Host.comPort.setWPACK();
                        }
                        else
                        {
                            MessageBox.Show("Error sending waypoints");
                        }
                    }
                    Host.comPort.setWPTotal((ushort)(temp_waypoints.Count() - 1)); //because first waypoint is the home.
                    //Host.comPort.setWP()
                    */
                    break;
                case Interop_Action.Drone_Cleaning_Server_Start:
                    Drone_Server_Thread = new Thread(new ThreadStart(this.drone_cleaning_server));                   
                    Drone_Server_Thread.Start();
                    break;
                case Interop_Action.PV_Drone_Control_Start:
                    PV_Drone_Control_Thread = new Thread(new ThreadStart(pv_drone_control));
                    PV_Drone_Control_Thread_shouldStop = false;
                    PV_Drone_Control_Thread.Start();
                    break;
                case Interop_Action.PV_Power_Read:
                    PV_Power_Read_Thread = new Thread(new ThreadStart(pv_power_read));
                    PV_Power_Read_Thread_shouldStop = false;
                    PV_Power_Read_Thread.Start();
                    break;
                default:
                    break;
            }
            Interoperability_Action_Mutex.ReleaseMutex();
            return;
        }

        private void Stop_Thread(ref Thread _thread, ref bool shouldStop_Variable)
        {
            shouldStop_Variable = true;
            if (_thread.IsAlive)
            {
                shouldStop_Variable = true;
                _thread.Join(50);
            }

            if (_thread.IsAlive)
            {
                _thread.Abort();
            }
            return;
        }

        public void export_mission_file(List<MAVLink.mavlink_mission_item_t> mission, MAVLink.mavlink_mission_item_t home)
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
                    string mission_string = "";

                    //< INDEX > < CURRENT WP > < COORD FRAME > < COMMAND > < PARAM1 > < PARAM2 > < PARAM3 > < PARAM4 > < PARAM5 / X / LONGITUDE > < PARAM6 / Y / LATITUDE > < PARAM7 / Z / ALTITUDE > < AUTOCONTINUE >
                    mission_string += "QGC WPL 110\n";
                    for (int i = 0; i < mission.Count(); i++)
                    {
                        mission_string += i + "\t" + "0\t" + "0\t" + mission[i].command + "\t" + mission[i].param1 + "\t" + mission[i].param2 + "\t"
                            + mission[i].param3 + "\t" + mission[i].param4 + "\t" + mission[i].x + "\t" + mission[i].y + "\t" + mission[i].z + "\n";
                    }

                    byte[] byteArray = Encoding.UTF8.GetBytes(mission_string);
                    myStream.Write(byteArray, 0, byteArray.Count());
                    myStream.Close();
                }
            }
        }


        /// <summary>
        /// Creates a mission given a set of waypoints. Inserts a takeoff and land command if quadcopter.
        /// </summary>
        public List<MAVLink.mavlink_mission_item_t> create_mission(MAVLink.MAV_TYPE frametype, List<Waypoint> waypoints)
        {
            List<MAVLink.mavlink_mission_item_t> mission = new List<MAVLink.mavlink_mission_item_t>();

            MAVLink.mavlink_mission_item_t temp_command = new MAVLink.mavlink_mission_item_t();
            temp_command.param1 = 0;    //Copter hold time (seconds)
            temp_command.param2 = 20;   //Acceptance Radius (metres)
            temp_command.param3 = 0;
            temp_command.param4 = 0;
            temp_command.target_component = Host.comPort.MAV.compid;
            temp_command.target_system = Host.comPort.MAV.sysid;
            temp_command.autocontinue = 1;
            temp_command.command = (ushort)MAVLink.MAV_CMD.WAYPOINT;
            temp_command.current = 0;
            temp_command.frame = (byte)MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT;
            temp_command.seq = 0;

            if (frametype == MAVLink.MAV_TYPE.FIXED_WING)
            {
                for (int i = 0; i < waypoints.Count(); i++)
                {
                    temp_command.x = waypoints[i].latitude;
                    temp_command.y = waypoints[i].longitude;
                    temp_command.z = waypoints[i].altitude_msl; //Be careful, I don't know how to set absolute vs relative altitude
                    temp_command.seq = (ushort)(i);
                    mission.Add(temp_command);
                }
                //p2 - acceptance radius (metres)
                //p5 - lat
                //p6 - lon
                //p7 - alt
            }

            else if (frametype == MAVLink.MAV_TYPE.QUADROTOR)
            {
                for (int i = 0; i < waypoints.Count(); i++)
                {
                    temp_command.x = waypoints[i].latitude;
                    temp_command.y = waypoints[i].longitude;
                    temp_command.z = waypoints[i].altitude_msl; //Be careful, I don't know how to set absolute vs relative altitude
                    temp_command.seq = (ushort)(i);
                    mission.Add(temp_command);
                }
                //Be careful, I don't know how to set absolute vs relative altitude
                //p1 - hold time (seconds)
                //p5 - lat
                //p6 - lon
                //p7 - alt
            }
            return mission;
        }


        /// <summary>
        /// Concatinates a series of waypoints 
        /// </summary>
        public void concat_mission()
        {

        }

        /// <summary>
        /// Contorls the quadcopter for solar panel cleaning
        /// </summary>
        public void pv_drone_control()
        {
            Console.WriteLine("\n\n\n------------------Starting Automatic Drone Control------------------\n\n\n");
            Console.WriteLine("***********************************************************************");
            Interoperability_GUI.PV_Add_Status("Starting Drone Control\n");
            Interoperability_GUI.PV_Add_Status("**********************\n");
            /*
            Features:
                -Continuously poll the module that checks the power levels. If it needs cleaning, then the module will return true, or some coordinates. 
                -Get home position, and compute optimal path to solar panel using waypoints. 
                -Write waypoints
                -Set mode to auto
                -Arm quadcopter and start flying 
                -Check to see if it is at the panel (within 5m or something). 
                -Switch to guided mode and clean panel (need a module or something for this)
                -Once clean, move to next panel, or go home. 
                -Switch to auto and conitnue mission
                -Once done, go home and land. 
                -Disarm quadcopter
                -Go back to top
            */
            Interoperability_GUI.PV_Add_Status("Checking Panel Voltage:\n");
            while(Panel_Voltage > 1.0)
            {
                Interoperability_GUI.PV_Add_Status("Panel Voltage: " + Panel_Voltage.ToString() + "V\n");
                Thread.Sleep(1000);
            }
            Interoperability_GUI.PV_Add_Status("Voltage Threshold Reached\n");

            //Temp waypoints until we get the waypoint generator working. 
            Interoperability_GUI.PV_Add_Status("Generating Waypoints...");
            List<Waypoint> temp = new List<Waypoint>();
            temp.Add(new Waypoint(300, 38.149220, -76.429480));
            temp.Add(new Waypoint(300, 38.150140, -76.430850));
            temp.Add(new Waypoint(300, 38.148950, -76.432290));
            temp.Add(new Waypoint(400, 38.147010, -76.430640));
            temp.Add(new Waypoint(200, 38.143780, -76.431990));
            Interoperability_GUI.PV_Add_Status("Done\n");

            //Get list of missions 
            List<MAVLink.mavlink_mission_item_t> mission = create_mission(MAVLink.MAV_TYPE.QUADROTOR, temp);

            //Write waypoints'
            Interoperability_GUI.PV_Add_Status("Writing Waypoints...");
            Thread.Sleep(1500);
            Interoperability_GUI.PV_Add_Status("Done\n");

            //Set mode
            Interoperability_GUI.PV_Add_Status("Setting mode to Loiter...");
            Host.comPort.setMode("LOITER");
            Thread.Sleep(1500);
            if(Host.cs.mode != "Loiter")
            {
                Interoperability_GUI.PV_Add_Status("\nError, could not set mode. Quitting\n");
                return;
            }
            Interoperability_GUI.PV_Add_Status("Done\n");

            Thread.Sleep(5000);

            Interoperability_GUI.PV_Add_Status("Arming UAV...");
            
            Interoperability_GUI.PV_Add_Status("Done\n");

            if (!Host.comPort.MAV.cs.armed)
            {
                arm_disarm_uav(MAV_ARM_DISARM.ARM);
            }
            else
            {
                //If it's armed, don't do anything. Something probably went wrong
                Interoperability_GUI.PV_Add_Status("Error, UAV is already armed. Quitting\n");
                return;
            }
            Thread.Sleep(1500);
            if (Host.cs.armed != true)
            {
                Interoperability_GUI.PV_Add_Status("\nError, could not arm quad. Quitting\n");
                return;
            }

            Interoperability_GUI.PV_Add_Status("Done\n");


            Interoperability_GUI.PV_Add_Status("Setting mode to Auto in 5 Seconds...");
            Thread.Sleep(5000);
            //Set mode 
            Host.comPort.setMode("Auto");
            if (Host.cs.mode != "Auto")
            {
                Interoperability_GUI.PV_Add_Status("\nError, could not set mode. Quitting\n");
                return;
            }
            Interoperability_GUI.PV_Add_Status("Done\n");
            //Start mission 
            //Might need to "hack" ardupilot, as it needs a pilot to initiate flight by raising throttle. 
            //But it's also a safety feature 
            Interoperability_GUI.PV_Add_Status("Pilot, please raise throttle to begin.\n");
            while (Host.cs.landed)
            {
            }
            Interoperability_GUI.PV_Add_Status("Mission Starting...\n");

            //This waypoint will be the one we use when we get to the 
            while (Host.cs.wpno != 6)
            {
                //Poll until we have reached the waypoint. 
            }
            Interoperability_GUI.PV_Add_Status("Setting mode to Loiter...");
            Host.comPort.setMode("Loiter");
            Thread.Sleep(1500);
            if (Host.cs.mode != "Loiter")
            {
                Interoperability_GUI.PV_Add_Status("\nError, could not set mode. Quitting\n");
                return;
            }

            //Loop here to start cleaning pattern
            Thread.Sleep(10000);

            Interoperability_GUI.PV_Add_Status("Setting waypoint to 7\n");
            Host.comPort.setWPCurrent(7);

             
            Interoperability_GUI.PV_Add_Status("Setting mode to Auto\n");
            Host.comPort.setMode("Auto");
            Thread.Sleep(1500);
            if (Host.cs.mode != "Auto")
            {
                Interoperability_GUI.PV_Add_Status("\nError, could not set mode. Quitting\n");
                return;
            }

            //Quadcopter should land once the cleaning pattern is done. 
        }

        void pv_power_read()
        {
            string message;
            string[] words;
            StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;

            //Create new thread to handle webserver handling
            Thread Enlighten_Thread = new Thread(new ThreadStart(this.pv_enlighten_connect));
            Enlighten_Thread.Start();

            Thread Characterization_Server_Thread = new Thread(new ThreadStart(this.characterization_server));
            Characterization_Server_Thread.Start();

            // Create a new SerialPort object with default settings.
            System.IO.Ports.SerialPort _serialPort = new System.IO.Ports.SerialPort();

            // Allow the user to set the appropriate properties.
            _serialPort.PortName = "COM5";
            _serialPort.BaudRate = 9600;
            _serialPort.Parity = Parity.None;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = StopBits.One;
            _serialPort.Handshake = Handshake.None;

            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;

            try
            {
                _serialPort.Open();
                while (!PV_Power_Read_Thread_shouldStop)
                {
                    _serialPort.DiscardInBuffer();
                    Thread.Sleep(100);
                    message = _serialPort.ReadLine();
                    Console.WriteLine(message);
                    words = message.Split(' ');
                    if (words[0] == "Temp")
                    {
                        Interoperability_GUI.Update_PV_Voltage(Convert.ToDouble(words[4]));
                        Interoperability_GUI.Update_PV_Temperature(Convert.ToDouble(words[2]));
                        Panel_Voltage = Convert.ToDouble(words[4]);
                        char_voltage = Convert.ToDouble(words[4]);
                        Panel_Temp = Convert.ToDouble(words[2]);
                        char_temperature = Convert.ToDouble(words[2]);
                    }

                    Thread.Sleep(100);
                }
                
            }

            catch (Exception e)
            {
                Console.WriteLine("Error in Panel Read Function: \n");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.InnerException);
            }

            Enlighten_Thread.Abort();
            Characterization_Server_Thread.Abort();
            return;
        }

        private double char_voltage = 0;
        private double char_temperature = 0;


        async void pv_enlighten_connect()
        {
            //user id: 4f4455354e6a67310a
            try
            {
                using (var client = new HttpClient())
                {
                    TimeSpan timeout = new TimeSpan(0, 0, 0, 10);
                    client.Timeout = timeout;
                    //client.BaseAddress = new Uri("https://api.enphaseenergy.com/api/v2"); // This seems to change every time


                    //"https://api.enphaseenergy.com/api/v2/systems/67/summary?key=96a7de32fabc1dd8ff68ec43eca21c06&user_id=4d7a45774e6a41320a"
                    //HttpResponseMessage resp = await client.GetAsync("https://api.enphaseenergy.com/api/v2/systems?key=c8e5316cff14b89ba39a495f46dd8a9b&user_id=4f4455354e6a67310a");
                    //Console.WriteLine("Login POST result: " + resp.Content.ReadAsStringAsync().Result);

                    //HttpResponseMessage resp = await client.GetAsync("https://api.enphaseenergy.com/api/v2/systems/1138299/summary?key=c8e5316cff14b89ba39a495f46dd8a9b&user_id=4f4455354e6a67310a");
                    //Console.WriteLine("Login POST result: " + resp.Content.ReadAsStringAsync().Result);

                    while (true)
                    {
                        HttpResponseMessage resp = await client.GetAsync("https://api.enphaseenergy.com/api/v2/systems/1138299/summary?key=c8e5316cff14b89ba39a495f46dd8a9b&user_id=4f4455354e6a67310a");
                        Console.WriteLine("Login POST result: " + resp.Content.ReadAsStringAsync().Result);

                        Enlighten_Object data = new JavaScriptSerializer().Deserialize<Enlighten_Object>(resp.Content.ReadAsStringAsync().Result);
                        Interoperability_GUI.Update_PV_Power_Output(data.current_power);
                        Interoperability_GUI.Update_Last_Panel_Update(data.last_report_at);


                        Thread.Sleep(60000);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error, exception thrown in Connecting to Enlighten");
                Console.WriteLine("Error" + e.Message);
                Console.WriteLine("Error" + e.InnerException);

            }

        }

        public void characterization_server()
        {
            IPAddress ipaddr = IPAddress.Any;
            int port = 9905;
            IPEndPoint ip = new IPEndPoint(ipaddr, port);


            // TcpListener server = null;
            Socket socket = null;
            Socket conn = null;
            try
            {
                socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(ip);
                socket.Listen(1);

                Console.WriteLine("*\n*\n*\n*\n*\nWaiting for connection...\n");

                conn = socket.Accept();

                Console.WriteLine("*\n*\n*\n*\n*\nReceived Connection from Client\n");
                Stopwatch timer = new Stopwatch();
                timer.Start();
                double average_expected_power = 0;
                int num_polled = 0;
                while (true)
                {
                    var characterizationDict = new Dictionary<String, double>();
                    characterizationDict["\"Voltage\""] = char_voltage;
                    characterizationDict["\"Temperature\""] = char_temperature;
                    // String t = "Test string 12345\r\n\0";
                    String t = "{" + String.Join(", ", characterizationDict.Select(x => x.Key + ":" + x.Value.ToString()).ToArray()) + "}< ";
                    byte[] tb = Encoding.ASCII.GetBytes(t);
                    // stream.Write(tb, 0, tb.Length);
                    conn.Send(tb);
                    Thread.Sleep(2000);
                    byte[] recv = new byte[100];
                    conn.Receive(recv);
                    Console.WriteLine("\n\n\n\n******************\n\n\n\n");
                    
                    string text = Encoding.ASCII.GetString(recv);
                    string[] split_text = Encoding.ASCII.GetString(recv).Split( ':', '}');
                    average_expected_power += Convert.ToDouble(split_text[2]);
                    num_polled++;

                    if (timer.ElapsedMilliseconds > 300000)
                    {
                        Interoperability_GUI.Update_PV_Expected_Power_Output(average_expected_power/num_polled);
                        average_expected_power = 0;
                        num_polled = 0;
                        timer.Restart();
                    } 
                }
            } // try
            catch (Exception e)
            {
                Console.WriteLine("*\n*\n*\n*\n*\nDrone cleaning server: SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                // Shutdown and end connection
                conn.Close();
                socket.Close();
            }


            Console.WriteLine("Drone cleaning server: exit");
        }


        public void drone_cleaning_server()
        {
            IPAddress ipaddr = IPAddress.Any;
            int port = 9903;
            IPEndPoint ip = new IPEndPoint(ipaddr, port);


            // TcpListener server = null;
            Socket socket = null;
            Socket conn = null;
            try
            {
                socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(ip);
                socket.Listen(1);

                Console.WriteLine("*\n*\n*\n*\n*\nWaiting for connection...\n");

                conn = socket.Accept();

                Console.WriteLine("*\n*\n*\n*\n*\nReceived Connection from Client\n");

                while (true)
                {
                    var telemDict = new Dictionary<String, double>();
                    telemDict["\"lat\""] = Host.cs.lat; 
                    telemDict["\"lon\""] = Host.cs.lng;
                    telemDict["\"alt\""] = Host.cs.altasl;
                    telemDict["\"hdg\""] = Host.cs.yaw;
                    telemDict["\"spd\""] = Host.cs.groundspeed;
                    // String t = "Test string 12345\r\n\0";
                    String t = "{" + String.Join(", ", telemDict.Select(x => x.Key + ":" + x.Value.ToString()).ToArray()) + "}\n";
                    byte[] tb = Encoding.ASCII.GetBytes(t);
                    // stream.Write(tb, 0, tb.Length);
                    conn.Send(tb);

                    Thread.Sleep(1000);
                }
            } // try
            catch (Exception e)
            {
                Console.WriteLine("*\n*\n*\n*\n*\nDrone cleaning server: SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                // Shutdown and end connection
                conn.Close();
                socket.Close();
            }


            Console.WriteLine("Drone cleaning server: exit");
        }

        /// <summary>
        /// Arms or disarms the connected UAV. If the UAV is armed, it will disarm. If it is disarmed, it will arm. 
        /// Warning. If aircraft is airborne, this may cause it to fall out of the sky if the checks fail.
        /// </summary>
        public void arm_disarm_uav(MAV_ARM_DISARM choice)
        {
            //Check to see if there is a connection to the quad
            if (!Host.comPort.BaseStream.IsOpen)
            {
                return;
            }

            try
            {
                Mavlink_Command command;
                if (choice == MAV_ARM_DISARM.ARM)
                {
                    //arm
                    command = new Mavlink_Command(MAVLink.MAV_CMD.COMPONENT_ARM_DISARM, 1, 21196, 0, 0, 0, 0, 0);
                }
                else
                {
                    //Ensure aircraft is not airborne. 
                    if (!Host.cs.landed)
                    {
                        Console.WriteLine("Error, cannot disarm. UAV is airborne");
                        return;
                    }
                    //disarm
                    command = new Mavlink_Command(MAVLink.MAV_CMD.COMPONENT_ARM_DISARM, 0, 21196, 0, 0, 0, 0, 0);
                }



                bool ans = Host.comPort.doCommand(command.actionid, command.p1, command.p2, command.p3,
                    command.p4, command.p5, command.p6, command.p7, command.requireack);

                if (ans == false)
                    MessageBox.Show(Strings.ErrorRejectedByMAV, Strings.ERROR);
            }
            catch
            {
                MessageBox.Show(Strings.ErrorNoResponce, Strings.ERROR);
            }
        }

        public enum MAV_ARM_DISARM
        {
            ARM,
            DISARM
        }

        

        public int get_power()
        {
            return 1;
        }

        public void test_function()
        {

            //Doesn't seem to work. Need to modify FlightData.cs or ConfigPlanner.cs
            MissionPlanner.Utilities.Settings test = this.Host.config;
            test["CMB_rateattitude"] = "1";
            test.Save();

            //this.Host.FDMenuMap.
        }


        /// <summary>
        /// Loads all the settings from the Interoperability_Config.xml
        /// </summary>
        public void getSettings()
        {
            if (Settings.ContainsKey("dist_units") && Settings.ContainsKey("airspd_units") && Settings.ContainsKey("geo_cords"))
            {
                dist_units = Settings["dist_units"];
                airspd_units = Settings["airspd_units"];
                geo_cords = Settings["geo_cords"];
            }
            else
            {
                Settings["dist_units"] = dist_units;
                Settings["airspd_units"] = airspd_units;
                Settings["geo_cords"] = geo_cords;
            }
            if (Settings.ContainsKey("address") && Settings.ContainsKey("username") && Settings.ContainsKey("password"))
            {
                address = Settings["address"];
                username = Settings["username"];
                password = Settings["password"];
            }
            else
            {
                Settings["address"] = address;
                Settings["username"] = username;
                Settings["password"] = password;
            }
            if (!Settings.ContainsKey("showInteroperability_GUI"))
            {
                Settings["showInteroperability_GUI"] = false.ToString();
            }
            Settings.Save();
        }

        /// <summary>
        /// Runs in a loop, uploading telemtry data to the interoperability server
        /// </summary>
        public async void Telemetry_Upload()
        {
            Console.WriteLine("Telemetry_Upload Thread Started");
            Stopwatch t = new Stopwatch();
            t.Start();

            int count = 0;
            CookieContainer cookies = new CookieContainer();

            string telemetry_uploaded_data = "";
            try
            {
                using (var client = new HttpClient())
                {

                    TimeSpan timeout = new TimeSpan(0, 0, 0, 10);
                    //client.Timeout = timeout;

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
                        Interoperability_GUI.setTelemResp(resp.Content.ReadAsStringAsync().Result);
                        Interoperability_GUI.Telem_Start_Stop_Button_Off();
                        //interoperabilityAction(Interop_Action.Telemtry_Thread_Stop);
                        return;

                    }
                    else
                    {
                        Console.WriteLine("Credentials Valid");
                        //interoperabilityAction(Interop_Action.Telemtry_Thread_Stop);
                    }

                    CurrentState csl = this.Host.cs;

                    double lat = 0, lng = 0, alt = 0, yaw = 0;

                    double oldlat = 0, oldlng = 0, oldalt = 0, oldyaw = 0;
                    int uniquedata_count = 0;
                    double averagedata_count = 0;

                    while (Telemetry_Thread_shouldStop == false)
                    {
                        //Doesn't work, need another way to do this
                        //If person sets speed to 0, then GUI crashes 
                        if (Interoperability_GUI.getTelemPollRate() != 0)
                        {
                            lat = csl.lat;
                            lng = csl.lng;
                            alt = csl.altasl;
                            yaw = csl.yaw;
                            Host.config.Load();
                            if (Host.config["distunits"].ToString() == "Meters")
                            {
                                alt *= 3.28084;
                            }

                            if (lat != oldlat || lng != oldlng || alt != oldalt || yaw != oldyaw)
                            {
                                uniquedata_count++;
                                averagedata_count++;
                                oldlat = lat;
                                oldlng = lng;
                                oldalt = alt;
                                oldyaw = yaw;
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


                            telemData.Add("latitude", lat.ToString("F10"));
                            telemData.Add("longitude", lng.ToString("F10"));
                            telemData.Add("altitude_msl", alt.ToString("F10"));
                            telemData.Add("uas_heading", yaw.ToString("F10"));

                            telemetry_uploaded_data = "Latitude: " + lat.ToString("F10");
                            telemetry_uploaded_data += "\r\nLongitude: " + lng.ToString("F10");
                            telemetry_uploaded_data += "\r\nAltitude_MSL: " + alt.ToString("F10");
                            telemetry_uploaded_data += "\r\nUAS_Heading: " + yaw.ToString("F10");

                            //Interoperability_GUI.set_telemetry_data_textbox("");
                            Interoperability_GUI.set_telemetry_data_textbox(telemetry_uploaded_data);

                            //Console.WriteLine("Latitude: " + lat + "\nLongitude: " + lng + "\nAltitude_MSL: " + alt + "\nHeading: " + yaw);

                            var telem = new FormUrlEncodedContent(telemData);
                            HttpResponseMessage telemresp = await client.PostAsync("/api/telemetry", telem);
                            Console.WriteLine("Server_info GET result: " + telemresp.Content.ReadAsStringAsync().Result);
                            Interoperability_GUI.setTelemResp(telemresp.Content.ReadAsStringAsync().Result);
                            count++;
                            Interoperability_GUI.setTotalTelemUpload(count);

                            Thread.Sleep(1000 / Interoperability_GUI.getTelemPollRate());
                        }
                        //Thread.Sleep(1000);
                    }
                }
            }

            //If this exception is thrown, then the thread will end soon after. Have no way to restart manually unless I get the loop working
            catch (Exception e)
            {
                //<h1>403 Forbidden</h1> 
                Interoperability_GUI.setAvgTelUploadText("Error, Unable to Connect to Server");
                Interoperability_GUI.setUniqueTelUploadText("Error, Unable to Connect to Server");
                Interoperability_GUI.setTelemResp("Error, Unable to Connect to Server");
                Interoperability_GUI.Telem_Start_Stop_Button_Off();
                Console.WriteLine("Error, exception thrown in telemtry upload thread");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.InnerException);
            }
            Console.WriteLine("Telemetry_Upload Thread Stopped");
            Interoperability_GUI.Telem_Start_Stop_Button_Off();
        }


        public void addImage()
        {

        }

        /// <summary>
        /// Runs in a loop, downloading current obstacle locaitons from the server 
        /// </summary>
        public async void Obstacle_SDA()
        {
            Console.WriteLine("Obstacle_SDA Thread Started");
            CookieContainer cookies = new CookieContainer();

            try
            {
                using (var client = new HttpClient())
                {
                    TimeSpan timeout = new TimeSpan(0, 0, 0, 10);
                    //client.Timeout = timeout;
                    client.BaseAddress = new Uri(address); // This seems to change every time

                    // Log in.
                    Console.WriteLine("---INITIAL LOGIN---");
                    var v = new Dictionary<string, string>();
                    v.Add("username", username);
                    v.Add("password", password);
                    var auth = new FormUrlEncodedContent(v);
                    HttpResponseMessage resp = await client.PostAsync("/api/login", auth);
                    Console.WriteLine("Login POST result: " + resp.Content.ReadAsStringAsync().Result);


                    if (!resp.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Invalid Credentials");
                        Interoperability_GUI.setSDAResp(resp.Content.ReadAsStringAsync().Result);
                        Interoperability_GUI.SetSDAStart_StopButton_Off();
                        //interoperabilityAction(Interop_Action.Obstacle_SDA_Thread_Stop);
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Credentials Valid");
                        Interoperability_GUI.setSDAResp(resp.Content.ReadAsStringAsync().Result);
                    }
                    Console.WriteLine("---LOGIN FINISHED---");

                    while (!Obstacle_SDA_Thread_shouldStop)
                    {
                        HttpResponseMessage SDAresp = await client.GetAsync("/api/obstacles");

                        obstaclesList = new JavaScriptSerializer().Deserialize<Obstacles>(SDAresp.Content.ReadAsStringAsync().Result);
                        Obstacles_Downloaded = true;
                        Interoperability_GUI.WriteObstacles(obstaclesList);

                        mapinvalidateObstacle = true;

                        Thread.Sleep(1000 / Interoperability_GUI.getsdaPollRate());
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error, exception thrown in Obstacle_SDA Thread");
                Console.WriteLine(e.Message);
                Interoperability_GUI.setSDAResp("Error, Unable to Connect to Server");
                Interoperability_GUI.SetSDAStart_StopButton_Off();

            }
            Interoperability_GUI.SetSDAStart_StopButton_Off();
            Console.WriteLine("Obstacle_SDA Thread Stopped");
        }

        public void Bottle_Drop()
        {
            //float target_altitude_agl = (float)srtm.getAltitude(., Host.cs.lng).alt;
            float plane_altitude_agl, groundspeed, time, drop_distance, plane_distance, time_drop;
            string drop_lat, drop_lng;
            double drop_x, drop_y;
            double plane_x, plane_y;
            int waypoint_drop;

            string status;

            while (Bottle_Drop_Thread_shouldStop)
            {
                plane_altitude_agl = Host.cs.altasl;
                groundspeed = Host.cs.groundspeed;
                waypoint_drop = Interoperability_GUI.bottle_drop_get_WP_NO();

                //This does not account for drag of the bottle
                time = (float)Math.Sqrt((double)(2 * plane_altitude_agl) / 9.81); //Time it takes for bottle to hit ground
                drop_distance = groundspeed * time;    //Distance the bottle will travel

                drop_lat = Interoperability_GUI.bottle_drop_get_Lat();
                drop_lng = Interoperability_GUI.bottle_drop_get_Long();

                drop_y = MercatorProjection.latToY(Convert.ToDouble(drop_lat));
                drop_x = MercatorProjection.lonToX(Convert.ToDouble(drop_lng));

                plane_y = MercatorProjection.latToY(Host.cs.lat);
                plane_x = MercatorProjection.lonToX(Host.cs.lng);

                plane_distance = (float)Math.Sqrt(Math.Pow((drop_x - plane_x), 2) + Math.Pow((drop_y - plane_y), 2));
                time_drop = (plane_distance - drop_distance) / groundspeed;

                status = "Current Waypoint: " + Host.cs.wpno.ToString() + "\r\n" + "Distance to Target: " + plane_distance.ToString()
                    + "\r\n" + "Drop Distance: " + drop_distance.ToString() + "\r\n" + "Estimated time to drop: " + time_drop.ToString() 
                    + "\r\n" + "Bottle Status: Not Dropped";

                Interoperability_GUI.update_bottle_drop_status(status);

                //Send command to drop thing
                if (drop_distance >=  plane_distance)
                {
                    for(int i=0; i < 20; i++)
                    {
                        //Host.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_SERVO, 9, 1750, 0, 0, 0, 0, 0);
                    }
                    status = "Bottle Status: Dropped :)";
                    Interoperability_GUI.update_bottle_drop_status(status);
                    break;
                }

                
            }
        }

        private List<Waypoint> Simulator_Path;


        /// <summary>
        /// SDA algorithm. Runs periodically to avoid obstacles
        /// </summary>
        public void SDA_Avoidance_Algorithm()
        {

            while (SDA_Avoidance_Algorithm_Thread_shouldStop == false)
            {
                /*Write your algorithm here
                You have access to: 
                    sim_lat         --Simulated latitude of the airplane
                    sim_lng         --Simulated longitude of the airplane
                    sim_alt         --Simulated altitude of the plane
                    sim_yaw         --Simulated heading of the plane (compass bearing)
                    sim_next_wp     --The next waypoint the plane will be moving towards 

                    obstaclesList   --Holds a list of all moving and stationary obstacles (updated constantly)
                        obstaclesList.moving_obstacles          --List of type 'Moving_Obstacle' 
                        obstaclesList.stationary_obstacles      --List of type 'Stationary_Obstacle'
                    Current_Mission.all_waypoints               --List of all waypoints the plane will be flying, type of 'Waypoint'
                    Interoperability_GUI.getPlaneSimulationAirspeed() --The plane's simulated airspeed (in meters per second)
                */


                List<int> intersectingWaypoints = new List<int>();

                //Find all waypoints that intersect with an obstacle
                for (int i = 0; i < Current_Mission.all_waypoints.Count() - 1; i++)
                {
                    double y1 = MercatorProjection.latToY(Current_Mission.all_waypoints[i].latitude);
                    double y2 = MercatorProjection.latToY(Current_Mission.all_waypoints[i + 1].latitude);
                    double x1 = MercatorProjection.lonToX(Current_Mission.all_waypoints[i].longitude);
                    double x2 = MercatorProjection.lonToX(Current_Mission.all_waypoints[i + 1].longitude);
                    //Centre of the obstacle
                    double y0 = 0;
                    double x0 = 0;
                    double distance = 0;


                    foreach (Stationary_Obstacle o in obstaclesList.stationary_obstacles)
                    {
                        double length = 0;
                        double currX = x1;
                        double currY = y1;
                        double dx = x2 - currX;
                        double dy = y2 - currY;

                        y0 = MercatorProjection.latToY(o.latitude);
                        x0 = MercatorProjection.lonToX(o.longitude);

                        do
                        {
                            dx = x2 - currX;
                            dy = y2 - currY;
                            length = Math.Sqrt(dx * dx + dy * dy);

                            double conversionRatio = 1 / length;

                            if (1 < length)
                            {
                                dx = dx * conversionRatio;
                                dy = dy * conversionRatio;
                            }

                            currY += dy;
                            currX += dx;

                            distance = Math.Sqrt(Math.Pow(currY - y0, 2) + Math.Pow(currX - x0, 2));
                            if (distance <= (o.cylinder_radius * 0.3048))
                            {
                                intersectingWaypoints.Add(i);
                                break;
                            }
                        } while (length > 1.5);
                    }
                }



                List<List<PointLatLng>> newPaths = new List<List<PointLatLng>>();
                for (int w = 0; w < intersectingWaypoints.Count(); w++)
                {
                    List<PointLatLng> path = Lazy_Theta(new PointLatLng(Current_Mission.all_waypoints[intersectingWaypoints[w]].latitude, Current_Mission.all_waypoints[intersectingWaypoints[w]].longitude),
                      new PointLatLng(Current_Mission.all_waypoints[intersectingWaypoints[w] + 1].latitude, Current_Mission.all_waypoints[intersectingWaypoints[w] + 1].longitude),
                      obstaclesList, new List<PointLatLng>());
                    newPaths.Add(path);
                }

                int oldSize = Current_Mission.all_waypoints.Count();

                //Update the current plane waypoints to avoid the obstacles
                for (int i = 0; i < newPaths.Count(); i++)
                {
                    //if path not found, figure something out
                    if (newPaths[i] != null)
                    {
                        int delta = Current_Mission.all_waypoints.Count() - oldSize;

                        //Calculate altitude differences between the two waypoints that we're adding waypoints
                        float dAlt = Current_Mission.all_waypoints[intersectingWaypoints[i] + delta + 1].altitude_msl - Current_Mission.all_waypoints[intersectingWaypoints[i] + delta].altitude_msl;
                        float distance = 0;
                        List<float> altitudes = new List<float>();
                        float prevAlt = Current_Mission.all_waypoints[intersectingWaypoints[i] + delta].altitude_msl;

                        //Calculate total distance of the new path
                        for (int j = 0; j < newPaths[i].Count() - 1; j++)
                        {
                            distance += (float)Math.Sqrt(Math.Pow(newPaths[i][j].Lat - newPaths[i][j + 1].Lat, 2) + Math.Pow(newPaths[i][j].Lng - newPaths[i][j + 1].Lng, 2));
                        }
                        //Set the altitudes of the new generated waypoitns
                        for (int j = 0; j < newPaths[i].Count() - 1; j++)
                        {
                            float temp = (float)Math.Sqrt(Math.Pow(newPaths[i][j].Lat - newPaths[i][j + 1].Lat, 2) + Math.Pow(newPaths[i][j].Lng - newPaths[i][j + 1].Lng, 2));
                            altitudes.Add((temp / distance * dAlt) + prevAlt);
                            prevAlt = altitudes[j];
                        }

                        //Remove start and end points waypoints becuase they will be slightly off due to how we do the grid
                        newPaths[i].RemoveAt(newPaths[i].Count() - 1);
                        newPaths[i].RemoveAt(0);

                        //Insert new waypoints into the flight path
                        for (int j = 0; j < newPaths[i].Count(); j++)
                        {
                            Current_Mission.all_waypoints.Insert(intersectingWaypoints[i] + j + 1 + delta, new Waypoint(altitudes[j], newPaths[i][j].Lat, newPaths[i][j].Lng));
                        }
                    }
                }

                Invalidate_Map();
                Invalidate_Map();

                Thread.Sleep(500);  //Change depending on how often you want to compute the algorithm
                SDA_Avoidance_Algorithm_Thread_shouldStop = true;
            }
        }


        List<List<Vertex>> vertices = new List<List<Vertex>>();
        PriorityQueue<Vertex> open;
        PriorityQueue<Vertex> closed;


        /// <summary>
        /// Calculates the optimal path between two points given stationary obstacles and geofence
        /// </summary>
        public List<PointLatLng> Lazy_Theta(PointLatLng Start, PointLatLng End, Obstacles Mission_Obstacles, List<PointLatLng> Geofence)
        {

            //http://aigamedev.com/open/tutorial/lazy-theta-star/
            /*Issues: 
                Doesn't check for illegal angles (>90 degrees)
                Doesn't check if we start in an occupied block? 
            */

            //Default 10, 5
            int gridSize = 10; //How far apart each vertex is in the x or y direction (in metres)
            double distanceMultiplier = 5; //Multiplier to make the search area bigger

            //Get start and end coordinates for the two points
            double startX = MercatorProjection.lonToX(Start.Lng);
            double startY = MercatorProjection.latToY(Start.Lat);
            double endX = MercatorProjection.lonToX(End.Lng);
            double endY = MercatorProjection.latToY(End.Lat);

            double deltaX = endX - startX;
            double deltaY = endY - startY;
            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);


            double centreX = (startX + endX) / 2;
            double centreY = (startY + endY) / 2;

            double gridStartX = centreX - (distance / 2 * distanceMultiplier); //Want to make the search area bigger so we can see eveything
            double gridStartY = centreY - (distance / 2 * distanceMultiplier); //Same as above comment

            double searchSize = distance * distanceMultiplier;

            //Index starts at 0,0 at top left corner, and increaess to the right and down
            // E is start, and S is end
            // 5 X E X X X 
            // 4 X X X X X 
            // 3 X X X X X 
            // 2 X X X X X 
            // 1 X X X S X 
            // 0 1 2 3 4 5

            //Need to map the start and end to the grid we made
            int indexStartX = (int)((startX - gridStartX) / gridSize);
            int indexStartY = (int)((startY - gridStartY) / gridSize);
            int indexEndX = (int)((endX - gridStartX) / gridSize);
            int indexEndY = (int)((endY - gridStartY) / gridSize);



            VertexCoords vertexStart = new VertexCoords(indexStartX, indexStartY);
            VertexCoords vertexEnd = new VertexCoords(indexEndX, indexEndY);


            vertices.Clear();
            //Iterate through all the X indices and and Y
            for (int i = 0; i < searchSize / gridSize; i++)
            {
                List<Vertex> vertexList = new List<Vertex>();
                vertexList.Clear();
                double X = gridStartX + gridSize * i;
                double Y = gridStartY;
                double h = 0;
                double g = 0;

                //Adds the rows (Y) starting from top to bottom
                for (int j = 0; j < searchSize / gridSize; j++)
                {
                    Y = gridStartY + gridSize * j;
                    g = 10000000000; //magic number. Bad. Should not use but don't know yet. 
                    h = Math.Sqrt(Math.Pow(i - vertexEnd.x, 2) + Math.Pow(j - vertexEnd.y, 2));
                    vertexList.Add(new Vertex(new VertexCoords(i, j), new VertexGPSCoords(X, Y), new VertexCoords(0, 0), g, h));
                }
                vertices.Add(vertexList);
            }

            //Used to help debug the grid search size
            if (true)
            {
                List<Waypoint> grid = new List<Waypoint>();
                int count = vertices.Count() - 1;
                grid.Add(new Waypoint(vertices[0][0].gpsCoords.latY, vertices[0][0].gpsCoords.lngX));
                grid.Add(new Waypoint(vertices[count][0].gpsCoords.latY, vertices[count][0].gpsCoords.lngX));
                grid.Add(new Waypoint(vertices[0][count].gpsCoords.latY, vertices[0][count].gpsCoords.lngX));
                grid.Add(new Waypoint(vertices[count][count].gpsCoords.latY, vertices[count][count].gpsCoords.lngX));

                FlyZone debugGrid = new FlyZone(100, 50, grid);
                Simulator_Path.Clear();
                Simulator_Path.AddRange(grid);
            }


            //Start algorithm
            VertexComp compare = new VertexComp();
            open = new PriorityQueue<Vertex>();
            closed = new PriorityQueue<Vertex>();


            vertices[vertexStart.x][vertexStart.y].parentCoords = vertexStart;
            vertices[vertexStart.x][vertexStart.y].isStart = true;
            vertices[vertexStart.x][vertexStart.y].g = 0;
            open.Enqueue(vertices[vertexStart.x][vertexStart.y]);

            Vertex currentVertex;
            bool pathFound = false;
            while (open.Count() != 0)
            {
                do
                {
                    currentVertex = open.Dequeue();
                    if (currentVertex == null)
                    {
                        return null;
                    }
                    vertices[currentVertex.selfCoords.x][currentVertex.selfCoords.y].open = false;
                } while (vertices[currentVertex.selfCoords.x][currentVertex.selfCoords.y].closed == true);

                currentVertex = new Vertex(SetVertex(currentVertex));
                if (currentVertex.selfCoords.x == indexEndX && currentVertex.selfCoords.y == indexEndY)
                {
                    pathFound = true;
                    break;
                }
                closed.Enqueue(currentVertex);
                currentVertex.closed = true;
                vertices[currentVertex.selfCoords.x][currentVertex.selfCoords.y].closed = true;
                foreach (Vertex v in getNeighboursVis(currentVertex))
                {
                    if (!v.closed)
                    {
                        if (!v.open)
                        {
                            v.g = 10000000000000000000;
                            v.parentCoords = null;
                            vertices[v.selfCoords.x][v.selfCoords.y].g = 10000000000000000000;
                            vertices[v.selfCoords.x][v.selfCoords.y].parentCoords = null;

                        }
                        Update_Vertex(currentVertex, v);
                    }
                }
            }

            if (pathFound)
            {
                return getThetaStarPath(new VertexCoords(indexEndX, indexEndY));
            }

            return null;
        }

        public List<PointLatLng> getThetaStarPath(VertexCoords v)
        {
            if (vertices[v.x][v.y].isStart)
            {
                List<PointLatLng> path = new List<PointLatLng>();
                path.Add(new PointLatLng(MercatorProjection.yToLat(vertices[v.x][v.y].gpsCoords.latY), MercatorProjection.xToLon(vertices[v.x][v.y].gpsCoords.lngX)));
                return path;
            }
            if (vertices[v.x][v.y].parentCoords.x == v.x && vertices[v.x][v.y].parentCoords.y == v.y)
            {
                //Something broke in the algorithm. This should not happen
                MessageBox.Show("Something went wrong in the Theta* algorithm. Algorithm say path found, but we cannot find a path");
                return null;
            }

            List<PointLatLng> tempPath = new List<PointLatLng>();
            tempPath.AddRange(getThetaStarPath(vertices[v.x][v.y].parentCoords));
            tempPath.Add(new PointLatLng(MercatorProjection.yToLat(vertices[v.x][v.y].gpsCoords.latY), MercatorProjection.xToLon(vertices[v.x][v.y].gpsCoords.lngX)));
            return tempPath;
        }

        public List<Vertex> getNeighboursVis(Vertex S)
        {
            int x = S.selfCoords.x;
            int y = S.selfCoords.y;

            //According to harry, we only need to do 4 directions, and it'll be much faster. 8 neighbours -> 4
            //X0X
            //000
            //X0X

            //Can do the same thing in 3D, 26 neighbours -> 6. X is invalid, 0 is neighbour
            //Top   Middle  Bottom
            //XXX   X0X     XXX
            //X0X   000     X0X
            //XXX   X0X     XXX

            List<VertexCoords> list = new List<VertexCoords>();
            list.Add(new VertexCoords(x, y + 1)); //N
            //list.Add(new VertexCoords(x + 1, y + 1)); //NE
            list.Add(new VertexCoords(x + 1, y)); //E
            //list.Add(new VertexCoords(x + 1, y - 1)); //SE
            list.Add(new VertexCoords(x, y - 1)); //S
            //list.Add(new VertexCoords(x - 1, y - 1)); //SW
            list.Add(new VertexCoords(x - 1, y)); //W
            //list.Add(new VertexCoords(x - 1, y + 1)); //NW

            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i].x >= vertices.Count() || list[i].x < 0)
                {
                    list.RemoveAt(i);
                    i--;
                }
                else if (list[i].y >= vertices.Count() || list[i].y < 0)
                {
                    list.RemoveAt(i);
                    i--;
                }
            }
            List<Vertex> neighbours = new List<Vertex>();
            foreach (VertexCoords v in list)
            {
                if (LOS(S, vertices[v.x][v.y]) == true)
                {
                    neighbours.Add(vertices[v.x][v.y]);
                }
            }

            return neighbours;
        }

        public void Update_Vertex(Vertex S, Vertex S_prime)
        {
            Vertex oldVertex = new Vertex(S_prime);
            ComputeCost(ref S, ref S_prime);
            if (S_prime.g < oldVertex.g)
            {
                //We don't remove things from the queue because it's too difficult. 
                //Instead we discard them when we pop them off the queue in the main algorithm
                open.Enqueue(S_prime);
                vertices[S_prime.selfCoords.x][S_prime.selfCoords.y].open = true;
            }
        }

        public void ComputeCost(ref Vertex S, ref Vertex S_prime)
        {
            double newCost = vertices[S.parentCoords.x][S.parentCoords.y].g + Math.Sqrt(Math.Pow(S.parentCoords.x - S_prime.selfCoords.x, 2) + Math.Pow(S.parentCoords.y - S_prime.selfCoords.y, 2));
            if (newCost < S_prime.g)
            {
                S_prime.parentCoords = S.parentCoords;
                S_prime.g = newCost;
                vertices[S_prime.selfCoords.x][S_prime.selfCoords.y].parentCoords = new VertexCoords(S.parentCoords.x, S.parentCoords.y);
                vertices[S_prime.selfCoords.x][S_prime.selfCoords.y].g = newCost;
            }
        }

        public Vertex SetVertex(Vertex S)
        {
            if (!LOS(S, vertices[S.parentCoords.x][S.parentCoords.y]))
            {
                double minCost = Double.MaxValue;
                Vertex minVertex = null;
                foreach (Vertex v in getNeighboursVis(S))
                {
                    if (v.closed)
                    {
                        if (v.g + Math.Sqrt(Math.Pow(S.parentCoords.x - v.selfCoords.x, 2) + Math.Pow(S.parentCoords.y - v.selfCoords.y, 2)) < minCost)
                        {
                            minVertex = v;
                            minCost = v.g + Math.Sqrt(Math.Pow(S.parentCoords.x - v.selfCoords.x, 2) + Math.Pow(S.parentCoords.y - v.selfCoords.y, 2));
                        }
                    }
                }
                if (minVertex != null)
                {
                    S.parentCoords = minVertex.selfCoords;
                    S.g = minCost;
                    vertices[S.selfCoords.x][S.selfCoords.y].parentCoords = minVertex.selfCoords;
                    vertices[S.selfCoords.x][S.selfCoords.y].g = minCost;
                }
            }
            return S;
        }

        //Determine if we hit any obstacles. 
        public bool LOS(Vertex S, Vertex S_prime)
        {
            //Points of S and S_prime
            double y1 = S.gpsCoords.latY;
            double x1 = S.gpsCoords.lngX;
            double y2 = S_prime.gpsCoords.latY;
            double x2 = S_prime.gpsCoords.lngX;

            //Centre of the obstacle
            double y0 = 0;
            double x0 = 0;

            double distance = 0;

            //Check if line crosses obstacles
            foreach (Stationary_Obstacle o in obstaclesList.stationary_obstacles)
            {
                double length = 0;
                double currX = x1;
                double currY = y1;
                double dx = x2 - currX;
                double dy = y2 - currY;

                y0 = MercatorProjection.latToY(o.latitude);
                x0 = MercatorProjection.lonToX(o.longitude);

                do
                {
                    dx = x2 - currX;
                    dy = y2 - currY;
                    length = Math.Sqrt(dx * dx + dy * dy);

                    double conversionRatio = 1 / length;

                    if (1 < length)
                    {
                        dx = dx * conversionRatio;
                        dy = dy * conversionRatio;
                    }

                    currY += dy;
                    currX += dx;

                    distance = Math.Sqrt(Math.Pow(currY - y0, 2) + Math.Pow(currX - x0, 2));
                    if (distance <= (o.cylinder_radius * 0.3048 + 15)) //Add 50 foot safety radius 
                    {
                        return false;
                    }
                } while (length > 1.5);
            }


            int count = Current_Mission.fly_zones[0].boundary_pts.Count();
            LineIntersect.Vector intersectPoint;

            //Check if lines cross geofence. We make it easier by checking each line segment instead of the entire polygon

            //Currently doens't work because we treat the lines as infinite, instead of fixed. 
            //Need to check distance between each point of the line, and each point of the geofence.

            for (int i = 0; i < count; i++)
            {
                if (LineIntersect.LineSegementsIntersect(new LineIntersect.Vector(S.selfCoords.x, S.selfCoords.y), new LineIntersect.Vector(S.parentCoords.x, S.parentCoords.y),
                new LineIntersect.Vector(Current_Mission.fly_zones[0].boundary_pts[i % count].longitude, Current_Mission.fly_zones[0].boundary_pts[i % count].latitude),
                new LineIntersect.Vector(Current_Mission.fly_zones[0].boundary_pts[(i + 1) % count].longitude, Current_Mission.fly_zones[0].boundary_pts[(i + 1) % count].latitude),
                out intersectPoint, false))
                {
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Determines if the resulting path will collide with an obstacle or geofence
        /// </summary>
        /// <returns></returns>
        public bool TurnRadius()
        {
            return true;
        }


        public void SDA_Plane_Simulator()
        {
            //Save current waypoints stored
            List<Waypoint> Current_Waypoints = Current_Mission.all_waypoints;

            //Save all obstacles
            Obstacles Current_Obstacles = obstaclesList;
            bool obstacleDownloadState = Obstacles_Downloaded;

            obstaclesList.stationary_obstacles = new List<Stationary_Obstacle>();
            obstaclesList.moving_obstacles = new List<Moving_Obstacle>();
            Obstacles_Downloaded = true;


            //Add fake waypoints 
            Current_Mission.all_waypoints.Clear();


            Current_Mission.all_waypoints.Add(new Waypoint(100, 43.834473, -79.238323));
            Current_Mission.all_waypoints.Add(new Waypoint(100, 43.835200, -79.237374));
            Current_Mission.all_waypoints.Add(new Waypoint(100, 43.835924, -79.237685));
            Current_Mission.all_waypoints.Add(new Waypoint(100, 43.835490, -79.239801));
            Current_Mission.all_waypoints.Add(new Waypoint(100, 43.835030, -79.241869));
            Current_Mission.all_waypoints.Add(new Waypoint(100, 43.834093, -79.243033));
            Current_Mission.all_waypoints.Add(new Waypoint(100, 43.833691, -79.242824));
            Current_Mission.all_waypoints.Add(new Waypoint(100, 43.833664, -79.241896));

            obstaclesList.stationary_obstacles.Add(new Stationary_Obstacle(100, 50, (float)43.835270, (float)-79.240780));
            obstaclesList.stationary_obstacles.Add(new Stationary_Obstacle(100, 50, (float)43.835711, (float)-79.238709));


            //start working on an actual simulator. 
            Dictionary<int, MAVLink.mavlink_mission_item_t> mission = Host.comPort.MAV.wps;




            if (Current_Mission.all_waypoints.Count() < 3)
            {
                Console.WriteLine("Error, not enough waypoints to start simulator (Minimum 3)");
                return;
            }

            int target_waypoint = 1;
            int total_waypoints = Current_Mission.all_waypoints.Count();

            sim_lat = Current_Mission.all_waypoints[0].latitude;
            sim_lng = Current_Mission.all_waypoints[0].longitude;
            sim_alt = 0;

            double ddist = 0;


            //Spline Algorithm
            //--------------------------------------------------------------------------------
            float[] x = new float[total_waypoints + 1]; //Plus one so the plane goes back to waypoint 0
            float[] y = new float[total_waypoints + 1];
            float[] xs;
            float[] ys;

            for (int i = 0; i < total_waypoints; i++)
            {
                x[i] = Current_Mission.all_waypoints[i].longitude;
                y[i] = Current_Mission.all_waypoints[i].latitude;
            }
            x[total_waypoints] = Current_Mission.all_waypoints[0].longitude;
            y[total_waypoints] = Current_Mission.all_waypoints[0].latitude;

            CubicSpline.FitParametric(x, y, 1000, out xs, out ys);

            Simulator_Path = new List<Waypoint>();
            for (int i = 0; i < xs.Count(); i++)
            {
                Simulator_Path.Add(new Waypoint(ys[i], xs[i]));
            }

            int[] altitude_array = new int[total_waypoints + 1];

            //Start calculating altitude spline... kinda
            int altitude_count = 0;
            for (int i = 0; i < xs.Count(); i++)
            {
                double dd;
                if (altitude_count < Current_Mission.all_waypoints.Count())
                {
                    double dx = MercatorProjection.lonToX(Current_Mission.all_waypoints[altitude_count].longitude - Simulator_Path[i].longitude);
                    double dy = MercatorProjection.latToY(Current_Mission.all_waypoints[altitude_count].latitude - Simulator_Path[i].latitude);
                    dd = Math.Sqrt(dx * dx + dy * dy);
                }
                else
                {
                    double dx = MercatorProjection.lonToX(Current_Mission.all_waypoints[0].longitude - Simulator_Path[i].longitude);
                    double dy = MercatorProjection.latToY(Current_Mission.all_waypoints[0].latitude - Simulator_Path[i].latitude);
                    dd = Math.Sqrt(dx * dx + dy * dy);
                }

                if (dd < 5)
                {
                    if (altitude_count >= altitude_array.Count())
                    {
                        break;
                    }
                    altitude_array[altitude_count] = i;
                    altitude_count++;
                }
            }

            for (int i = 0; i <= total_waypoints - 2; i++)
            {
                //Get number of indexes between start and end altitude
                int delta_index = altitude_array[i + 1] - altitude_array[i];
                double delta_altitude = (Current_Mission.all_waypoints[i + 1].altitude_msl - Current_Mission.all_waypoints[i].altitude_msl) / delta_index;
                for (int j = altitude_array[i]; j < altitude_array[i + 1]; j++)
                {
                    Simulator_Path[j].altitude_msl = (float)(Current_Mission.all_waypoints[i].altitude_msl + (j - altitude_array[i]) * delta_altitude);
                }

            }

            double Total_Distance = 0;
            //Get total distance of spline 
            for (int i = 0; i < Simulator_Path.Count() - 1; i++)
            {
                double dx = MercatorProjection.lonToX(Simulator_Path[i + 1].longitude) - MercatorProjection.lonToX(Simulator_Path[i].longitude);
                double dy = MercatorProjection.latToY(Simulator_Path[i + 1].latitude) - MercatorProjection.latToY(Simulator_Path[i].latitude);
                Total_Distance += Math.Sqrt(dx * dx + dy * dy);
            }

            double current_dist = 0;

            //-------------------------------------------------------------------------------------------
            //End Spline Algorithm


            bool useSpline = false;

            //Invalidate waypoints and obstacles, to update for new paths
            mapinvalidateWaypoints = true;
            mapinvalidateObstacle = true;

            while (SDA_Plane_Simulator_Thread_shouldStop == false)
            {
                //If using straight moving lines
                if (!useSpline)
                {
                    total_waypoints = Current_Mission.all_waypoints.Count();

                    Simulator_Path.Clear();
                    foreach (Waypoint i in Current_Mission.all_waypoints)
                    {
                        Simulator_Path.Add(i);
                    }

                    if (target_waypoint == total_waypoints)
                    {
                        sim_next_wp = 0;
                        target_waypoint = 0;
                    }
                    Console.WriteLine("Target Waypoint: " + target_waypoint.ToString());
                    // Gets an actual dx and dy in meters
                    double currX = MercatorProjection.lonToX(sim_lng);
                    double currY = MercatorProjection.latToY(sim_lat);
                    double dx = MercatorProjection.lonToX(Current_Mission.all_waypoints[target_waypoint].longitude) - currX;
                    double dy = MercatorProjection.latToY(Current_Mission.all_waypoints[target_waypoint].latitude) - currY;
                    double dalt = 0;
                    if (Settings["dist_units"] == "Feet")
                    {
                        dalt = Current_Mission.all_waypoints[target_waypoint].altitude_msl * 3.28084 - sim_alt;
                    }
                    else
                    {
                        dalt = Current_Mission.all_waypoints[target_waypoint].altitude_msl - sim_alt;
                    }


                    //Since we're calling every 1/2 second, we move the plane AIRSPEED/2 meters per second
                    ddist = Interoperability_GUI.getPlaneSimulationAirspeed() / 10;

                    // Makes dx and dy the right length
                    double length = Math.Sqrt(dx * dx + dy * dy);
                    double conversionRatio = ddist / length;

                    if (ddist < length)
                    {
                        dx = dx * conversionRatio;
                        dy = dy * conversionRatio;
                        dalt = dalt * conversionRatio;
                    }

                    sim_yaw = (float)(90 - Math.Atan2(dy, dx) * 180 / Math.PI);
                    if (sim_yaw < 0)
                    {
                        sim_yaw += 360;
                    }

                    sim_lat = MercatorProjection.yToLat(currY + dy);
                    sim_lng = MercatorProjection.xToLon(currX + dx);
                    sim_alt += (float)dalt;


                    if (Math.Sqrt(Math.Pow(MercatorProjection.latToY(Current_Mission.all_waypoints[target_waypoint].latitude) - MercatorProjection.latToY(sim_lat), 2) +
                        Math.Pow(MercatorProjection.latToY(Current_Mission.all_waypoints[target_waypoint].longitude) - MercatorProjection.latToY(sim_lng), 2)) < 10)
                    {
                        target_waypoint++;
                        sim_next_wp++;
                    }
                }
                //Spline Interpolation
                //Not very accurate (assumes each point is the same distance apart, which it's not)
                else
                {
                    ddist = Interoperability_GUI.getPlaneSimulationAirspeed() / 10;
                    current_dist += ddist;

                    int spline_index = (int)Math.Ceiling(current_dist / (Total_Distance / Simulator_Path.Count()));

                    if (current_dist >= Total_Distance || spline_index >= Simulator_Path.Count() - 1)
                    {
                        current_dist = 0;
                        spline_index = 0;
                    }

                    sim_lat = Simulator_Path[spline_index].latitude;
                    sim_lng = Simulator_Path[spline_index].longitude;

                    if (Settings["dist_units"] == "Feet")
                    {
                        sim_alt = (int)(Simulator_Path[spline_index].altitude_msl * 3.28084);
                    }
                    else
                    {
                        sim_alt = Simulator_Path[spline_index].altitude_msl;
                    }



                    //Very stupid way of calculating target waypoint 
                    for (int i = 0; i <= total_waypoints; i++)
                    {
                        if (spline_index < altitude_array[i])
                        {
                            sim_next_wp = i;
                            break;
                        }
                        else if (spline_index > altitude_array[total_waypoints - 1])
                        {
                            sim_next_wp = 0;
                            break;
                        }
                    }


                    Console.WriteLine("Target Waypoint: " + sim_next_wp.ToString());

                    double dy = Simulator_Path[spline_index + 1].latitude - sim_lat;
                    double dx = Simulator_Path[spline_index + 1].longitude - sim_lng;

                    sim_yaw = (float)(90 - Math.Atan2(dy, dx) * 180 / Math.PI);
                    if (sim_yaw < 0)
                    {
                        sim_yaw += 360;
                    }
                }
                Thread.Sleep(100);
            }
            Current_Mission.all_waypoints = Current_Waypoints;
            Obstacles_Downloaded = obstacleDownloadState;
            obstaclesList = Current_Obstacles;
        }

        //Will be used to export to something.
        public void ExportMapData(List<Mission> missionList)
        {
            string thing = new JavaScriptSerializer().Serialize(missionList);
            Console.WriteLine(thing);
        }


        public async void Mission_Download()
        {
            Console.WriteLine("Mission_Download Thread Started");
            Stopwatch t = new Stopwatch();
            t.Start();
            CookieContainer cookies = new CookieContainer();
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
                    if (!resp.IsSuccessStatusCode)
                    {
                        Mission_Thread_shouldStop = true;
                    }
                    else
                    {
                        Console.WriteLine("Credentials Valid");
                        Mission_Thread_shouldStop = false;
                    }
                    HttpResponseMessage SDAresp = await client.GetAsync("/api/missions");
                    Console.WriteLine(SDAresp.Content.ReadAsStringAsync().Result);

                    List<Mission> Server_Mission = new JavaScriptSerializer().Deserialize<List<Mission>>(SDAresp.Content.ReadAsStringAsync().Result);

                    Console.WriteLine(SDAresp.Content.ReadAsStringAsync().Result.ToString());

                    int count = Mission_List.Count();
                    //Add obtained missions to the current list of missions 
                    for (int i = 0; i < Server_Mission.Count(); i++)
                    {
                        Server_Mission[i].name = "Server Mission_" + Convert.ToString(i + count);
                        Mission_List.Add(Server_Mission[i]);
                    }
                    Current_Mission = Server_Mission[0];

                    mapinvalidateWaypoints = true;
                    mapinvalidateGeofence = true;
                    mapinvalidateSearchArea = true;
                    mapinvalidateObstacle = true;
                    mapinvalidateOFAT_EM_DROP = true;
                    mapinvalidateImage = true;
                }
            }
            catch
            {
                Console.WriteLine("Error, exception thrown in the Mission_Download Thread");
            }
            Console.WriteLine("Mission_Download Thread Stopped");
        }

        public void Invalidate_Map()
        {
            mapinvalidateWaypoints = true;
            mapinvalidateGeofence = true;
            mapinvalidateSearchArea = true;
            mapinvalidateObstacle = true;
            mapinvalidateOFAT_EM_DROP = true;
            mapinvalidateImage = true;
        }

        public void Map_Control()
        {
            Console.WriteLine("Map_Control Thread Started");
            Stopwatch t = new Stopwatch();
            Stopwatch FlightTime = new Stopwatch();
            double GroundElevation;
            t.Start();

            //Add static overlays:
            //For testing right now. Will update when server has misison functionality added
            if (false)
            {
                List<Waypoint> Op_Area = new List<Waypoint>();
                Op_Area.Add(new Waypoint(38.1462694, -76.4277778));
                Op_Area.Add(new Waypoint(38.1516250, -76.4286833));
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

                //We clear becaues the construtor creates an empty flyzone.
                Current_Mission.fly_zones.Clear();
                Current_Mission.fly_zones.Add(new FlyZone(600, 100, Op_Area));

                Current_Mission.search_grid_points.Add(new Waypoint(38.1457306, -76.4295972));
                Current_Mission.search_grid_points.Add(new Waypoint(38.1431861, -76.4338917));
                Current_Mission.search_grid_points.Add(new Waypoint(38.1410028, -76.4322333));
                Current_Mission.search_grid_points.Add(new Waypoint(38.1411917, -76.4269806));
                Current_Mission.search_grid_points.Add(new Waypoint(38.1422194, -76.4261111));

                Current_Mission.air_drop_pos = new GPS_Position(38.145852, -76.426416);
                Current_Mission.off_axis_target_pos = new GPS_Position(38.147408, -76.433651);
                Current_Mission.emergent_last_known_pos = new GPS_Position(38.144187, -76.423645);
            }


            /*List<Waypoint> Waypoints = new List<Waypoint>();
            Waypoints.Add(new Waypoint(38.147720, -76.429610));
            Waypoints.Add(new Waypoint(38.150893, -76.432056));
            Waypoints.Add(new Waypoint(38.149559, -76.434159));
            Waypoints.Add(new Waypoint(38.145729, -76.430275));
            Waypoints.Add(new Waypoint(38.143147, -76.428344));
            Waypoints.Add(new Waypoint(38.142168, -76.429482));
            Waypoints.Add(new Waypoint(38.144176, -76.431584));
            Waypoints.Add(new Waypoint(38.143214, -76.433151));
            Waypoints.Add(new Waypoint(38.141898, -76.432593));
            Waypoints.Add(new Waypoint(38.143063, -76.429675));
            Waypoints.Add(new Waypoint(38.144531, -76.426542));
            Waypoints.Add(new Waypoint(38.146471, -76.422379));
            Waypoints.Add(new Waypoint(38.144801, -76.421757));
            Waypoints.Add(new Waypoint(38.143029, -76.421692));
            Waypoints.Add(new Waypoint(38.142084, -76.423817));
            Waypoints.Add(new Waypoint(38.142016, -76.425469));
            Waypoints.Add(new Waypoint(38.145189, -76.428537));*/

            while (!Map_Control_Thread_shouldStop)
            {
                //Map invalidation not fully set up yet. Since we need to set the variable every time we update
                //an object. For now, we will invalidate everything so performance will be the same. Once we enable invalidation,
                //it should reduce CPU usage

                /*mapinvalidateWaypoints = true;
                mapinvalidateGeofence = true;
                mapinvalidateSearchArea = true;
                mapinvalidateObstacle = true;
                mapinvalidateOFAT_EM_DROP = true;
                mapinvalidateImage = true;*/

                Interoperability_GUI.MAP_Clear_Overlays();
                //Draw Obstacles 
                if (Obstacles_Downloaded)
                {
                    if (Interoperability_GUI.getDrawObstacles() && mapinvalidateObstacle)
                    {
                        for (int i = 0; i < obstaclesList.stationary_obstacles.Count(); i++)
                        {
                            Interoperability_GUI.MAP_addSObstaclePoly(obstaclesList.stationary_obstacles[i].cylinder_radius * 0.3048,
                                obstaclesList.stationary_obstacles[i].cylinder_height * 0.3048, obstaclesList.stationary_obstacles[i].latitude,
                                obstaclesList.stationary_obstacles[i].longitude, "Static_Obstacle" + i.ToString());
                        }

                        for (int i = 0; i < obstaclesList.moving_obstacles.Count(); i++)
                        {
                            Interoperability_GUI.MAP_addMObstaclePoly(obstaclesList.moving_obstacles[i].sphere_radius * 0.3048,
                               obstaclesList.moving_obstacles[i].altitude_msl * 0.3048, obstaclesList.moving_obstacles[i].latitude,
                               obstaclesList.moving_obstacles[i].longitude, "Moving_Obstacle" + i.ToString());
                        }
                    }
                }

                //Draw geofence
                if (Interoperability_GUI.getDrawGeofence() && mapinvalidateGeofence)
                {
                    //Using first boundary point (assuming there is only one geofence) COME BACK TO THIS LATER
                    Interoperability_GUI.MAP_addStaticPoly(Current_Mission.fly_zones[0].boundary_pts, "Geofence", Color.Red, Color.Transparent, 3, 50);
                }

                if (Current_Mission.fly_zones.Count() > 1)
                {
                    //Interoperability_GUI.MAP_addWPRoute(Current_Mission.fly_zones[1].boundary_pts);
                    //Interoperability_GUI.MAP_addStaticPoly(Current_Mission.fly_zones[1].boundary_pts, "gridDebug", Color.Cyan, Color.Transparent, 3, 50);
                }

                //Draw search area
                if (Interoperability_GUI.getDrawSearchArea() && mapinvalidateSearchArea)
                {
                    Interoperability_GUI.MAP_addStaticPoly(Current_Mission.search_grid_points, "Search_Area", Color.Green, Color.Green, 3, 90);
                }

                //Draw plane location                   
                if (Interoperability_GUI.getDrawPlane())
                {
                    if (usePlaneSimulator == true)
                    {
                        Interoperability_GUI.MAP_updatePlaneLoc(new PointLatLng(sim_lat, sim_lng), sim_alt, sim_yaw, sim_yaw, sim_yaw, sim_yaw, 0);
                    }
                    else
                    {
                        //Account for feet vs meters
                        double alt = Host.cs.altasl;
                        if (Host.config["distunits"].ToString() == "Feet")
                        {

                            alt /= 3.28084;

                        }

                        Interoperability_GUI.MAP_updatePlaneLoc(new PointLatLng(Host.cs.lat, Host.cs.lng), (float)alt, Host.cs.yaw,
                                                Host.cs.groundcourse, Host.cs.nav_bearing, Host.cs.target_bearing, Host.cs.radius);
                    }
                }

                //Draw Waypoints
                if (Interoperability_GUI.getDrawWP() && mapinvalidateWaypoints)
                {
                    if (usePlaneSimulator == true)
                    {
                        Console.WriteLine("HELLO I AM SIMULATOR");
                        Interoperability_GUI.MAP_addWP(Current_Mission.all_waypoints);
                        Interoperability_GUI.MAP_addWPRoute(Simulator_Path);
                    }
                    else
                    {
                        //Draw waypoints
                        Interoperability_GUI.MAP_addWP(Current_Mission.all_waypoints);
                        //Draw lines between waypoints
                        Interoperability_GUI.MAP_addWPRoute(Current_Mission.all_waypoints);
                    }
                }

                //Draw off axis targets, emergent targets, and air drop location
                if (Interoperability_GUI.getDrawOFAT_EM_DROP() && mapinvalidateOFAT_EM_DROP)
                {
                    Interoperability_GUI.MAP_updateOFAT_EM_DROP(Current_Mission);
                }

                if (Interoperability_GUI.getAutopan())
                {
                    if (usePlaneSimulator == true)
                    {
                        Interoperability_GUI.MAP_Update_Loc(new PointLatLng(sim_lat, sim_lng));
                    }
                    else
                    {
                        Interoperability_GUI.MAP_Update_Loc(new PointLatLng(Host.cs.lat, Host.cs.lng));
                    }

                }

                //Draw Images if preset

                //foreach 

                Interoperability_GUI.MAP_Update_Overlay();

                //Update GPS Location label at bottom of interface
                switch (geo_cords)
                {
                    case "DD.DDDDDD":
                        Interoperability_GUI.MAP_updateGPSLabel(Host.cs.lat.ToString("00.000000") + " " + Host.cs.lng.ToString("00.000000"));
                        break;
                    case "DD MM SS.SS":
                        Interoperability_GUI.MAP_updateGPSLabel(DDtoDMS(Host.cs.lat, Host.cs.lng));
                        break;
                    default:
                        Interoperability_GUI.MAP_updateGPSLabel(Host.cs.lat.ToString("00.000000") + " " + Host.cs.lng.ToString("00.000000"));
                        break;
                }

                GroundElevation = srtm.getAltitude(Host.cs.lat, Host.cs.lng).alt;

                //Update altitude and delta altitude label at bottom of interface and AUVSI Label
                if (Host.config["distunits"].ToString() == "Meters")
                {
                    switch (dist_units)
                    {
                        case "Metres":
                            Interoperability_GUI.MAP_updateAltLabel(Host.cs.altasl.ToString("00.000") + "m",
                                (Host.cs.altasl - GroundElevation).ToString("00.000") + "m");
                            Interoperability_GUI.MAP_updateAUVSIAltLabel((3.28084 * Host.cs.altasl).ToString("00.000"));
                            break;
                        case "Feet":
                            Interoperability_GUI.MAP_updateAltLabel((3.28084 * Host.cs.altasl).ToString("00.000") + "ft",
                                (3.28084 * Host.cs.altasl - 3.28084 * GroundElevation).ToString("00.000") + "ft");
                            Interoperability_GUI.MAP_updateAUVSIAltLabel((3.28084 * Host.cs.altasl).ToString("00.000"));
                            break;
                        default:
                            break;
                    }
                }
                //Units are in feet
                else
                {
                    switch (dist_units)
                    {
                        case "Metres":
                            Interoperability_GUI.MAP_updateAltLabel((Host.cs.altasl / 3.28084).ToString("00.000") + "m",
                                (Host.cs.altasl / 3.28084 - GroundElevation / 3.28084).ToString("00.000") + "m");
                            Interoperability_GUI.MAP_updateAUVSIAltLabel((Host.cs.altasl).ToString("00.000"));
                            break;
                        case "Feet":
                            Interoperability_GUI.MAP_updateAltLabel((Host.cs.altasl).ToString("00.000") + "ft",
                                (Host.cs.altasl - GroundElevation).ToString("00.000") + "ft");
                            Interoperability_GUI.MAP_updateAUVSIAltLabel((Host.cs.altasl).ToString("00.000"));
                            break;
                        default:
                            break;
                    }
                }

                
                switch (Host.config["speedunits"].ToString())
                {
                    case "meters_per_second":
                        Interoperability_GUI.MAP_updateAUVSIArspdLabel((1.94384 * Host.cs.airspeed).ToString());
                        break;
                    case "knots":
                        Interoperability_GUI.MAP_updateAUVSIArspdLabel((Host.cs.airspeed).ToString());
                        break;
                    case "mph":
                        Interoperability_GUI.MAP_updateAUVSIArspdLabel((0.868976 * Host.cs.airspeed).ToString());
                        break;
                    case "fps":
                        Interoperability_GUI.MAP_updateAUVSIArspdLabel((0.5924836363625846 * Host.cs.airspeed).ToString());
                        break;
                    case "kph":
                        Interoperability_GUI.MAP_updateAUVSIArspdLabel((0.53995665314471419372 * Host.cs.airspeed).ToString());
                        break;
                    default:
                        break;
      
                }

                
                Interoperability_GUI.setFlightTimerLabel(FlightTime.ElapsedMilliseconds);
                //Console.WriteLine(srtm.getAltitude(Host.cs.lat, Host.cs.lng).alt.ToString());
                t.Restart();

                //It's okay if we call this multiple times, because it just resumes the flight timer
                if (Host.cs.airspeed > 10)
                {
                    FlightTime.Start();
                }
                else
                {
                    FlightTime.Stop();
                }
                if (resetFlightTimer == true)
                {
                    FlightTime.Reset();
                    resetFlightTimer = false;
                }
                Thread.Sleep(1000 / Interoperability_GUI.getMapRefreshRate());
            }
            Console.WriteLine("Map_Control Thread Stopped");
        }

        public void Callouts()
        {
            Stopwatch t = new Stopwatch();
            t.Start();
            //Set up speech output 
            SpeechSynthesizer Speech = new SpeechSynthesizer();

            Console.WriteLine("Callout Thread Started");

            using (SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US")))
            {
                Choices Modes = new Choices();
                Modes.Add(new string[] { "landing", "taking off", "airspeed", "altitude", "flight time" });

                // Create a GrammarBuilder object and append the Choices object.
                GrammarBuilder gb = new GrammarBuilder();
                gb.Append(Modes);
                Grammar g = new Grammar(gb);
                recognizer.LoadGrammar(g);
                //Voice recognition allows 
                recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized);
                recognizer.SetInputToDefaultAudioDevice();

                bool Recognizer_Enabled = false;
                string CalloutMode = "";

                while (Callout_Thread_shouldStop == false)
                {
                    if (Interoperability_GUI.getSpeechRecognition_Enabled() == true && !Recognizer_Enabled)
                    {
                        recognizer.RecognizeAsync(RecognizeMode.Multiple);
                        Recognizer_Enabled = true;
                    }
                    else if (Interoperability_GUI.getSpeechRecognition_Enabled() == false)
                    {
                        recognizer.RecognizeAsyncStop();
                        Recognizer_Enabled = false;
                    }

                    //Probably shouldn't be calling this in an infinite loop
                    CalloutMode = Interoperability_GUI.getCalloutMode();
                    if (CalloutMode == "Landing")
                    {

                    }
                    else if (CalloutMode == "Takeoff")
                    {

                    }
                    //Thread.Sleep(10000000);
                    if (t.ElapsedMilliseconds > (Interoperability_GUI.getCalloutPeriod()))
                    {
                        //If we speak asynchronously, then we might speak over the previous speach  
                        Speech.SpeakAsync(Convert.ToInt32(Host.cs.airspeed).ToString());
                        t.Restart();
                    }
                }

            }
            Console.WriteLine("Callout thread has stopped");
        }

        private void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            MessageBox.Show("Speech recognized: " + e.Result.Text);
        }


        /// <summary>
        /// Returns Degree Minute Seconds string given Decimal Degrees
        /// </summary>
        /// <param name="lat">Latitude</param>
        /// <param name="lng">Longitude</param>
        /// <returns></returns>
        public static string DDtoDMS(double lat, double lng)
        {
            string DMS;

            double minutes = Math.Floor((Math.Abs(lat) - Math.Floor(Math.Abs(lat))) * 60.0);
            double seconds = (Math.Abs(lat) - Math.Floor(Math.Abs(lat)) - minutes / 60) * 3600;


            if (lat > 0)
            {
                DMS = "N";
            }
            else
            {
                DMS = "S";
            }
            DMS += Convert.ToInt32(Math.Abs(lat)).ToString("00") + "-" + Math.Abs(minutes).ToString("00") + "-" + Math.Abs(seconds).ToString("00.00") + " ";// + "." + tenths.ToString("00") + " ";

            minutes = Math.Floor((Math.Abs(lng) - Math.Floor(Math.Abs(lng))) * 60.0);
            seconds = (Math.Abs(lng) - Math.Floor(Math.Abs(lng)) - minutes / 60) * 3600;

            if (lng > 0)
            {
                DMS += "E";
            }
            else
            {
                DMS += "W";
            }
            DMS += Convert.ToInt32(Math.Abs(lng)).ToString("000") + "-" + Math.Abs(minutes).ToString("00") + "-" + Math.Abs(seconds).ToString("00.00"); //+ "." + tenths.ToString("00");
            return DMS;
        }

        /// <summary>
        /// Returns Decimal Degrees given a Degree Minute Seconds string
        /// </summary>
        /// <param name="lat">Latitude in DMS "W36-28-45.67"</param>
        /// <param name="lng">Longitude in DMS "S038-94-12.89"</param>
        /// <returns></returns>
        public static Waypoint DMStoDD(string lat, string lng)
        {
            double DD_Lat, DD_Lng;
            Waypoint Converted_DD = new Waypoint();

            //Assuming the format is correct
            char[] delimiterChars = { '-' };

            string[] lat_split = lat.Split(delimiterChars);
            string[] lng_split = lng.Split(delimiterChars);
            //43.236403, -98.927366
            try
            {
                double lat_degrees = Convert.ToDouble(lat_split[0][1].ToString() + lat_split[0][2].ToString());
                double lat_minutes = Convert.ToDouble(lat_split[1].ToString()) / 60;
                double lat_seconds = Convert.ToDouble(lat_split[2]) / 3600;
                DD_Lat = lat_degrees + lat_minutes + lat_seconds;

                if (lat_split[0][0] == 'S')
                {
                    DD_Lat *= -1;
                }

                double lng_degrees = Convert.ToDouble(lng_split[0][1].ToString() + lng_split[0][2].ToString() + lng_split[0][3].ToString());
                double lng_minutes = Convert.ToDouble(lng_split[1].ToString()) / 60;
                double lng_seconds = Convert.ToDouble(lng_split[2]) / 3600;
                DD_Lng = lng_degrees + lng_minutes + lng_seconds;

                if (lng_split[0][0] == 'W')
                {
                    DD_Lng *= -1;
                }
            }
            catch
            {
                //Something went wrong with the format I think
                return Converted_DD;
            }

            return new Waypoint(DD_Lat, DD_Lng);
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

            return (true);
        }

        /// <summary>
        /// Run at NextRun time - loop is run in a background thread. and is shared with other plugins
        /// Ensure threads that unintentially die are revived
        /// </summary>
        /// <returns></returns>
        override public bool Loop()
        {
            /*if (Telemetry_Upload_shouldStop == false && !Telemetry_Upload_isAlive)
            {
                interoperabilityAction(0);
            }
            if (Obstacle_SDA_shouldStop == false && !Obstacle_SDA_isAlive)
            {
                interoperabilityAction(1);
            }
            if (Map_Control_shouldStop == false && !Map_Control_isAlive)
            {
                interoperabilityAction(6);
            }*/

            return true;
        }

        /// <summary>
        /// Exit is only called on plugin unload. not app exit
        /// </summary>
        /// <returns></returns>
        override public bool Exit()
        {
            interoperabilityAction(new Interop_Callback_Struct(Interop_Action.Stop_All_Threads_Quit));
            return (true);
        }
    }
}
