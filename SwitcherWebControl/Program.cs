using SwitcherWebControl.Devices.BTools;
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
            dev.Test();
            Console.ReadLine();
        }
    }
}
