using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Management;
using Newtonsoft.Json;

namespace MicronApplicationSpy
{
    class Program
    {
        private static string machine = null;
        private static List<string> applications = new List<string>();
        private static bool debug = false;
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
            ManagementEventWatcher startWatch = new ManagementEventWatcher(
  new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            startWatch.EventArrived += new EventArrivedEventHandler(startWatch_EventArrived);
            startWatch.Start();
            ManagementEventWatcher stopWatch = new ManagementEventWatcher(
              new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));
            stopWatch.EventArrived += new EventArrivedEventHandler(stopWatch_EventArrived);
            stopWatch.Start();
            Console.WriteLine("***Press any key to exit***");
            while (!Console.KeyAvailable) System.Threading.Thread.Sleep(50);
            startWatch.Stop();
            stopWatch.Stop();
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
                        if (!debug)
                        {
                            var response = wb.UploadValues("https://launch-tracker.herokuapp.com/api/application_launched", "POST", data);
                        }
                    }
                }
            }
        }
    }
}
