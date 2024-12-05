using SwitcherWebControl.Config;
using SwitcherWebControl.Exceptions;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitcherWebControl.Devices
{
    public abstract class SerialControlDevice : IControlDevice
    {
        public SerialControlDevice(SerialPortConfig portInfo)
        {
            this.portInfo = portInfo;
        }

        private readonly SerialPortConfig portInfo;

        private SerialPort port;

        protected SerialPort Port => port;
        protected abstract string DeviceLabel { get; }

        public string Label => $"{DeviceLabel} (on {portInfo.PortName})";
        public abstract int GPICount { get; }
        public abstract int GPOCount { get; }
        public abstract int AudioInputCount { get; }
        public abstract int AudioOutputCount { get; }

        public virtual void Initialize()
        {
            //Attempt to open the serial port
            try
            {
                port = new SerialPort(portInfo.PortName, portInfo.BaudRate);
                port.ReadTimeout = portInfo.TimeoutMs;
                port.WriteTimeout = portInfo.TimeoutMs;
                port.Open();
            } catch
            {
                throw new FormattedException($"Failed to open port \"{port.PortName}\" at baud rate {portInfo.BaudRate}");
            }
        }

        public virtual void Dispose()
        {
            port.Dispose();
        }

        /// <summary>
        /// Sends command out the serial port.
        /// </summary>
        /// <param name="command"></param>
        protected void SendSerialCommand(string command)
        {
            //Encode request with ASCII
            byte[] request = Encoding.ASCII.GetBytes(command);

            //Send
            try
            {
                port.Write(request, 0, request.Length);
            }
            catch
            {
                throw new FormattedException("Failed to transmit serial command.");
            }
        }
        
        /// <summary>
        /// Sends a text command and waits for a response within the timeout. Throws an exception on timeout.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        protected string SendSerialCommandGetResponse(string command)
        {
            //Encode request with ASCII
            byte[] request = Encoding.ASCII.GetBytes(command);

            //Send
            try
            {
                port.Write(request, 0, request.Length);
            } catch
            {
                throw new FormattedException("Failed to transmit serial command.");
            }

            //Attempt to recieve data
            byte[] response = new byte[16384];
            int read;
            try
            {
                read = port.Read(response, 0, response.Length);
            } catch (TimeoutException)
            {
                throw new FormattedException("Timed out waiting for data from the device.");
            } catch
            {
                throw new FormattedException("Failed to read data from the serial port.");
            }

            return Encoding.ASCII.GetString(response, 0, read);
        }

        /// <summary>
        /// Reads a line from the serial port. Throws an exception on timeout.
        /// </summary>
        /// <returns></returns>
        protected string ReadSerialLine()
        {
            //Attempt to recieve data
            string response;
            try
            {
                response = port.ReadLine();
            }
            catch (TimeoutException)
            {
                throw new FormattedException("Timed out waiting for data from the device.");
            }
            catch
            {
                throw new FormattedException("Failed to read data from the serial port.");
            }

            return response.TrimEnd('\r');
        }

        public abstract ulong ReadOptos();
        public abstract ulong ReadRelays();
        public abstract void SetRelays(ulong relays);
        public abstract void SetRelay(int index, bool set);
        public abstract ulong[] ReadAudioOutputs();
        public abstract ulong[] SetAudioOutputs(ulong[] state);
    }
}
