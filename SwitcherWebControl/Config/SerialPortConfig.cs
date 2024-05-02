using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitcherWebControl.Config
{
    public class SerialPortConfig
    {
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public int TimeoutMs { get; set; }
    }
}
