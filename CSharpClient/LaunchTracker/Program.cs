using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Management;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace MicronApplicationSpy
{
    class Program
    {
        private static string machine = null;
        private static List<string> applications = new List<string>();
        private static bool debug = false;
        public static ContextMenu menu;
        public static MenuItem mnuExit;
        public static NotifyIcon notificationIcon;
        static ManagementEventWatcher startWatch;
        static ManagementEventWatcher stopWatch;

        static void Main(string[] args)
        {
            string text = System.IO.File.ReadAllText(@"spy-config.txt");
            dynamic json = JsonConvert.DeserializeObject(text);

            machine = json.machine.Value.Trim(" "[0]);
            foreach (var app in json.applications)
            {
                applications.Add(app.Value);
            }
            debug = json.debug.Value;
            if (debug)
            {
                Console.WriteLine("Debug mode");
            }
            Thread notifyThread = new Thread(
                  delegate()
                  {
                      menu = new ContextMenu();
                      mnuExit = new MenuItem("Exit");
                      menu.MenuItems.Add(0, mnuExit);
                      mnuExit.Click += new EventHandler(mnuExit_Click);

                      notificationIcon = new NotifyIcon();
                      notificationIcon.Text = "LaunchTracker";
                      notificationIcon.Icon = new Icon(SystemIcons.Application, 40, 40);
                      notificationIcon.ContextMenu = menu;

                      notificationIcon.Visible = true;
                      Application.Run();
                  }
              );
            
            startWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            startWatch.EventArrived += new EventArrivedEventHandler(startWatch_EventArrived);
            startWatch.Start();
            stopWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));
            stopWatch.EventArrived += new EventArrivedEventHandler(stopWatch_EventArrived);
            stopWatch.Start();

            notifyThread.Start();
            Console.ReadLine();  
            
        }
        static void stopWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            string processName = (string)e.NewEvent.Properties["ProcessName"].Value;
            Console.WriteLine("Process stopped: {0}", processName);
            foreach (var application in applications)
            {
                Match match = Regex.Match(processName, @"" + application, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    Console.WriteLine("***Right process stopped!***");
                    using (var wb = new WebClient())
                    {
                        var data = new NameValueCollection();
                        data["machine"] = machine;
                        data["application"] = application;
                        data["time"] = NowString();
                        if (!debug)
                        {
                            var response = wb.UploadValues("https://launch-tracker.herokuapp.com/api/application_closed", "POST", data);                            
                        }
                    }
                }
            }
        }

        static void startWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            string processName = (string) e.NewEvent.Properties["ProcessName"].Value;
            Console.WriteLine("Process started: {0}", processName);
            foreach (var application in applications)
            {
                Match match = Regex.Match(processName, @"" + application, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    Console.WriteLine("Right process started!");
                    using (var wb = new WebClient())
                    {
                        var data = new NameValueCollection();
                        data["machine"] = machine;
                        data["application"] = application;
                        data["time"] = NowString();
                        if (!debug)
                        {
                            var response = wb.UploadValues("https://launch-tracker.herokuapp.com/api/application_launched", "POST", data);
                        }
                    }
                }
            }
        }
        static string NowString() 
        {
            DateTime now = DateTime.UtcNow;
            return now.Year + "-" + now.Month.ToString("D2") + "-" + now.Day.ToString("D2") + 
                "T" + now.Hour.ToString("D2") + ":" + now.Minute.ToString("D2") + ":" + now.Second.ToString("D2");
        }

        static void mnuExit_Click(object sender, EventArgs e)
        {
            notificationIcon.Dispose();
            startWatch.Stop();
            stopWatch.Stop();
            Application.Exit();
        }
    }
}
