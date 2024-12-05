using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitcherWebControl.Web.Entities
{
    public class WebInfoResponse
    {
        internal WebInfoResponse(WebControlDevice[] devices)
        {
            this.devices = devices;
        }

        private readonly WebControlDevice[] devices;

        [JsonProperty("version_major")]
        public int VersionMajor => 0;

        [JsonProperty("version_minor")]
        public int VersionMinor => 1;

        [JsonProperty("devices")]
        public WebDeviceInfo[] Devices => devices.Select(x => new WebDeviceInfo(x.Id, x.Device)).ToArray();
    }
}
