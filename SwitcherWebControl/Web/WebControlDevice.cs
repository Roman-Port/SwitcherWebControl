using SwitcherWebControl.Exceptions;
using SwitcherWebControl.Web.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SwitcherWebControl.Web
{
    class WebControlDevice
    {
        public WebControlDevice(string id, IControlDevice device)
        {
            this.id = id;
            this.device = device;
        }

        private readonly string id;
        private readonly IControlDevice device;

        public string Id => id;
        public IControlDevice Device => device;

        public object HandleRoot(HttpListenerRequest e, string path)
        {
            switch (path.ToLower())
            {
                case "":
                case "/":
                    return HandleInfo(e);
                case "/relays":
                    return HandleRelays(e);
                case "/optos":
                    return HandleOptos(e);
                case "/audio":
                    return HandleAudio(e);
                default:
                    throw new FormattedException($"Bad endpoint.", 404);
            }
        }

        private object HandleAudio(HttpListenerRequest e)
        {
            switch (e.HttpMethod.ToUpper())
            {
                case "GET":
                    return new WebReadAudioOutputs(device.ReadAudioOutputs(), device.AudioInputCount);
                case "POST":
                    return HandleSetAudio(e);
                default:
                    throw new FormattedException($"Unknown request method: {e.HttpMethod}", 400);
            }
        }

        private object HandleSetAudio(HttpListenerRequest e)
        {
            //Decode request body
            WebSetAudioOutputsRequest request = e.DeserializePostJson<WebSetAudioOutputsRequest>();

            //Read outputs to manipulate
            ulong[] outputs = device.ReadAudioOutputs();

            //Loop through and set
            foreach (var o in request.Outputs)
            {
                //Check ID
                if (o.Index < 0 || o.Index >= outputs.Length)
                    throw new FormattedException($"Out of bounds output index {o.Index}.", 400);

                //Decode and set
                outputs[o.Index] = WebUtilities.UlongOrStringToBitmask(o.Bitmask, o.BitmaskString);
            }

            //Apply
            return new WebReadAudioOutputs(device.SetAudioOutputs(outputs), device.AudioInputCount);
        }

        private object HandleInfo(HttpListenerRequest e)
        {
            switch (e.HttpMethod.ToUpper())
            {
                case "GET":
                    return new WebDeviceInfo(id, device);
                default:
                    throw new FormattedException($"Unknown request method: {e.HttpMethod}", 400);
            }
        }

        private object HandleRelays(HttpListenerRequest e)
        {
            switch (e.HttpMethod.ToUpper())
            {
                case "GET":
                    return new WebReadBitmask(device.ReadRelays(), device.GPOCount);
                default:
                    throw new FormattedException($"Unknown request method: {e.HttpMethod}", 400);
            }
        }

        private object HandleOptos(HttpListenerRequest e)
        {
            switch (e.HttpMethod.ToUpper())
            {
                case "GET":
                    return new WebReadBitmask(device.ReadOptos(), device.GPICount);
                default:
                    throw new FormattedException($"Unknown request method: {e.HttpMethod}", 400);
            }
        }
    }
}
