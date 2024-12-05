using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitcherWebControl.Config
{
    /// <summary>
    /// A device to be added to the server
    /// </summary>
    public class ConfigDevice
    {
        /// <summary>
        /// The ID of the device that will be used to reference it in the web interface.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// The type of device that is being communicated with.
        /// </summary>
        [JsonProperty("device_type")]
        public string DeviceType { get; set; }

        /// <summary>
        /// Additional info that is device-specific.
        /// </summary>
        [JsonProperty("device_info")]
        public JObject DeviceInfo { get; set; }
    }
}
