using SwitcherWebControl.Devices.BTools;
using SwitcherWebControl.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitcherWebControl
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BTools82Plus dev = new BTools82Plus(new Config.SerialPortConfig
            {
                PortName = "COM1",
                BaudRate = 9600,
                TimeoutMs = 10000
            }, 0);
            dev.Initialize();

            //Start server
            Console.WriteLine("Devices initialized. Starting server...");
            WebController server = new WebController("http://192.168.0.205:54544/", new WebControlDevice[]
            {
                new WebControlDevice("test", dev)
            });
            server.RunServer();
        }
    }
}
