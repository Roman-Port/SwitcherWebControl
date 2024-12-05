using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitcherWebControl.Web.Entities
{
    public class WebReadAudioOutputs
    {
        public WebReadAudioOutputs(ulong[] outputs, int inputsCount)
        {
            this.outputs = outputs;
            this.inputsCount = inputsCount;
        }

        private readonly ulong[] outputs;
        private readonly int inputsCount;

        [JsonProperty("outputs")]
        public WebReadBitmask[] Outputs => outputs.Select(x => new WebReadBitmask(x, inputsCount)).ToArray();
    }
}
