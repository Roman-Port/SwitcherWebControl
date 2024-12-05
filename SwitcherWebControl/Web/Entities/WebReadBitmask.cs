using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitcherWebControl.Web.Entities
{
    public class WebReadBitmask
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmask">The bitmask to return.</param>
        /// <param name="bits">The number of bits to show in the bitmask string.</param>
        public WebReadBitmask(ulong bitmask, int bits)
        {
            this.bitmask = bitmask;
            this.bits = bits;
        }

        private readonly ulong bitmask;
        private readonly int bits;

        [JsonProperty("bitmask")]
        public ulong Bitmask => bitmask;

        [JsonProperty("bitmask_string")]
        public string BitmaskString => WebUtilities.BitmaskToString(bitmask, bits); // 0-1 string for JavaScript clients
    }
}
