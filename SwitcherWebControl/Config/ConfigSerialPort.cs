using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitcherWebControl.Config
{
    public class ConfigSerialPort
    {
        [JsonProperty("port_name")]
        public string PortName { get; set; }

        [JsonProperty("baud_rate")]
        public int BaudRate { get; set; }

        [JsonProperty("timeout_ms")]
        public int TimeoutMs { get; set; }
    }
}
