using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Management;

namespace MicronApplicationSpy
{
    class Program
    {
        private static string machine = null;
        private static string application = null;
        private static bool debug = false;
        static void Main(string[] args)
        {
            string[] lines = System.IO.File.ReadAllLines(@"spy-config.txt");
            machine = lines[0].Trim(" "[0]);
            application = lines[1].Trim(" "[0]);
            if (lines.Length >= 3 && lines[2].Trim(" "[0]).Equals("debug"))
            {
                Console.WriteLine("Debug mode");
                debug = true;
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
            Match match = Regex.Match(processName, @"" + application, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                Console.WriteLine("***Right process stopped!***");
            }
            if (match.Success && !debug)
            {
                using (var wb = new WebClient())
                {
                    var data = new NameValueCollection();
                    data["machine"] = machine;
                    data["application"] = application;

                    var response = wb.UploadValues("https://launch-tracker.herokuapp.com/api/application_closed", "POST", data);
                }    
            }
        }

        static void startWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            string processName = (string) e.NewEvent.Properties["ProcessName"].Value;
            Console.WriteLine("Process started: {0}", processName);
            Match match = Regex.Match(processName, @"" + application, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                Console.WriteLine("Right process started!");
            }
            if (match.Success && !debug)
            {
                using (var wb = new WebClient())
                {
                    var data = new NameValueCollection();
                    data["machine"] = machine;
                    data["application"] = application;

                    var response = wb.UploadValues("https://launch-tracker.herokuapp.com/api/application_launched", "POST", data);
                }
            }
        }
    }
}
