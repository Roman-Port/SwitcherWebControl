using Newtonsoft.Json.Linq;
using SwitcherWebControl.Config;
using SwitcherWebControl.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitcherWebControl.Devices.BTools
{
    public class BTools82Plus : SerialControlDevice
    {
        public BTools82Plus(ConfigSerialPort portInfo, int deviceId) : base(portInfo)
        {
            this.deviceId = deviceId;
        }

        public static BTools82Plus InflateDevice(JObject rawInfo)
        {
            //Convert the raw info to our config
            if (rawInfo == null)
                throw new FormattedException("device_info was null.");
            BToolsConfig info = rawInfo.ToObject<BToolsConfig>();

            //Check fields
            if (info == null || info.Port == null)
                throw new FormattedException("Missing fields in config.");

            //Create
            return new BTools82Plus(info.Port, info.DeviceId);
        }

        private readonly int deviceId;

        protected override string DeviceLabel => "BTools 8.2";

        public override int GPICount => 16;

        public override int GPOCount => 8;

        public override int AudioInputCount => 8;

        public override int AudioOutputCount => 2;

        /* SERIAL UTILS */

        /// <summary>
        /// Clears input buffer, then sends a command to the switcher, automatically prefixing it with the header.
        /// </summary>
        /// <param name="command"></param>
        protected void SendBToolsCommand(string command)
        {
            //Flush input buffer
            Port.DiscardInBuffer();

            //Send
            SendSerialCommand("*" + deviceId + command);
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

        /// <summary>
        /// Reads and discards lines until a line with the requested header matches. Useful for reading responses but throwing away status updates between them.
        /// </summary>
        /// <param name="header">The start of the line that must match for it to be counted.</param>
        /// <param name="attempts">The number of lines to attempt to read before failing.</param>
        /// <returns></returns>
        protected string ReadCommandResponse(string header, int attempts = 5)
        {
            string line;
            while (attempts > 0)
            {
                //Read line
                line = ReadSerialLine();

                //Check
                if (line.StartsWith(header))
                    return line;

                //Decrement counter and try again
                attempts--;
            }
            throw new FormattedException("Used all attempts to read response from BTools switcher.");
        }

        /* MISC UTILS */

        /// <summary>
        /// Parses values of '0' or '1' chars in the array to be a bitfield. For example, "0,1,1,0,0" would return as 0b01100.
        /// </summary>
        /// <param name="values">The values to read from.</param>
        /// <param name="offset">The offset into the values to begin reading from.</param>
        /// <param name="length">The number of items to read. If not all are available, throws an exception.</param>
        private ulong ParseStateToBitfield(string[] values, int offset, int length)
        {
            //Check prameters
            if (length > 64)
                throw new Exception("Unable to store more than 64 values.");
            if (offset + length > values.Length)
                throw new Exception($"Invalid data; Attempted to parse bitfield of {values.Length} from offset {offset}, length {length} but there weren't enough values.");
            if (offset < 0 || length < 0)
                throw new ArgumentException("Invalid offset or length.");

            //Loop
            ulong result = 0;
            string cell;
            for (int i = 0; i < length; i++)
            {
                //Read
                cell = values[offset + i];

                //Make sure it's valid
                if (cell != "0" && cell != "1")
                    throw new Exception($"Invalid data; \"{cell}\" at {offset + i} is not a valid boolean for bitfield parsing.");

                //If it is 1, set bit
                if (cell == "1")
                    result |= 1UL << i;
            }

            return result;
        }

        /// <summary>
        /// Reads the S0L<x> lines that are sent when setting or getting audio state
        /// </summary>
        /// <returns></returns>
        private ulong[] ReadAudioStateResponse()
        {
            ulong[] result = new ulong[AudioOutputCount];
            for (int i = 0; i < result.Length; i++)
            {
                //Read the line with the output index (plus 1)
                string[] line = ReadCommandResponse($"S{deviceId}L{i + 1},").Split(',');

                //Parse
                result[i] = ParseStateToBitfield(line, 1, 8);
            }
            return result;
        }

        /* API */

        public override ulong ReadOptos()
        {
            //Transmit request for all
            SendBToolsCommand("SPA");

            //Read the response line
            string[] line = ReadCommandResponse($"S{deviceId}P,A,").Split(',');

            //Parse
            return ParseStateToBitfield(line, 2, GPICount);
        }

        public override ulong ReadRelays()
        {
            //Transmit request for all
            SendBToolsCommand("SR");

            //Read the response line
            string[] line = ReadCommandResponse($"S{deviceId}R,").Split(',');

            //Parse
            return ParseStateToBitfield(line, 1, GPOCount);
        }

        public override void SetRelays(ulong relays)
        {
            throw new NotImplementedException();
        }

        public override void SetRelay(int index, bool set)
        {
            throw new NotImplementedException();
        }

        public override ulong[] ReadAudioOutputs()
        {
            //Transmit
            SendBToolsCommand("SL");

            //Read response state lines
            return ReadAudioStateResponse();
        }

        public override ulong[] SetAudioOutputs(ulong[] state)
        {
            //Validate state array size
            if (state == null || state.Length != AudioOutputCount)
                throw new ArgumentException($"State array size must be exactly {AudioOutputCount}.");

            //Request current state and check if it matches what we're setting it to.
            //If it matches the requested state, don't do anything as the switcher will respond with nothing.
            ulong[] oldState = ReadAudioOutputs();
            bool oldStateChanged = false;
            for (int i = 0; i < Math.Min(oldState.Length, state.Length); i++)
                oldStateChanged = oldStateChanged || oldState[i] != state[i];
            if (!oldStateChanged)
                return oldState;

            //Build the command. BTools uses A-D for the state of each input which can be interpreted as a kind of table
            string cmd = "B";
            int temp;
            ulong mask;
            for (int i = 0; i < AudioInputCount; i++)
            {
                //Create the mask to check
                mask = 1UL << i;

                //The character can be created by setting these bits...
                temp = 0;
                for (int o = 0; o < AudioOutputCount; o++)
                {
                    if ((state[o] & mask) != 0)
                        temp |= 1 << o;
                }

                //Now add this to ASCII A and push to the command
                cmd += "," + (char)((int)'A' + temp);
            }

            //Transmit
            SendBToolsCommand(cmd);

            //Read response state lines
            return ReadAudioStateResponse();
        }
    }
}
