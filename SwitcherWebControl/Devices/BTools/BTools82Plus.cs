using SwitcherWebControl.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitcherWebControl.Devices.BTools
{
    public class BTools82Plus : SerialControlDevice
    {
        public BTools82Plus(SerialPortConfig portInfo, int deviceId) : base(portInfo)
        {
            this.deviceId = deviceId;
        }

        private readonly int deviceId;

        public void Test()
        {
            Console.WriteLine(SendBToolsCommandGetResponse("SL"));
        }

        /// <summary>
        /// Sends a command, automatically prefixing it with the header.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        protected string SendBToolsCommandGetResponse(string command)
        {
            //Flush input/output buffers
            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();

            //Send
            return SendSerialCommandGetResponse("*" + deviceId + command);
        }
    }
}
