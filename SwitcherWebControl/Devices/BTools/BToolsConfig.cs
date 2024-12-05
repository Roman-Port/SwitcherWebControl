using Newtonsoft.Json;
using SwitcherWebControl.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitcherWebControl.Devices.BTools
{
    /// <summary>
    /// Config object for the BTools switchers.
    /// </summary>
    public class BToolsConfig
    {
        /// <summary>
        /// Details about the serial port to use.
        /// </summary>
        [JsonProperty("port")]
        public ConfigSerialPort Port { get; set; }

        /// <summary>
        /// The device on the chain of devices on the port. Typically zero.
        /// </summary>
        [JsonProperty("device_id")]
        public int DeviceId { get; set; }
    }
}
