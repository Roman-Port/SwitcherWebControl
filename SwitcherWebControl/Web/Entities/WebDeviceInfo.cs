using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitcherWebControl.Web.Entities
{
    public class WebDeviceInfo
    {
        public WebDeviceInfo(string id, IControlDevice device)
        {
            this.id = id;
            this.device = device;
        }

        private readonly string id;
        private readonly IControlDevice device;

        [JsonProperty("id")]
        public string Id => id;

        [JsonProperty("type")]
        public string Type => device.Label;

        [JsonProperty("gpi_count")]
        public int GPICount => device.GPICount;

        [JsonProperty("gpo_count")]
        public int GPOCount => device.GPOCount;

        [JsonProperty("audio_input_count")]
        public int AudioInputCount => device.AudioInputCount;

        [JsonProperty("audio_output_count")]
        public int AudioOutputCount => device.AudioOutputCount;
    }
}
