using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitcherWebControl.Web.Entities
{
    public class WebSetAudioOutputsRequest
    {
        [JsonProperty("outputs")]
        public AudioOutputReq[] Outputs { get; set; }

        public class AudioOutputReq
        {
            [JsonProperty("index")]
            public int Index { get; set; }

            [JsonProperty("bitmask")]
            public ulong? Bitmask { get; set; }

            [JsonProperty("bitmask_string")]
            public string BitmaskString { get; set; }
        }
    }
}
