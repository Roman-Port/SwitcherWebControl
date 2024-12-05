using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitcherWebControl.Config
{
    /// <summary>
    /// Root of the config file to load.
    /// </summary>
    public class ConfigRoot
    {
        /// <summary>
        /// The address that the web server will listen on.
        /// </summary>
        [JsonProperty("listen_address")]
        public string ListenAddress { get; set; }

        /// <summary>
        /// A list of devices that will be added to the web service.
        /// </summary>
        [JsonProperty("devices")]
        public ConfigDevice[] Devices { get; set; } = new ConfigDevice[0];
    }
}
