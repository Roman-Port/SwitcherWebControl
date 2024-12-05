using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitcherWebControl
{
    /// <summary>
    /// A switcher device
    /// </summary>
    public interface IControlDevice : IDisposable
    {
        /// <summary>
        /// Initialize the device and get all settings. Call this before accessing anything.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Display label for identifying the device.
        /// </summary>
        string Label { get; }

        /// <summary>
        /// Gets the number of input optos the device supports. Should not change.
        /// </summary>
        int GPICount { get; }

        /// <summary>
        /// Gets the number of output relays the device supports. Should not change.
        /// </summary>
        int GPOCount { get; }

        /// <summary>
        /// Gets the number of audio inputs the device supports. Should not change.
        /// </summary>
        int AudioInputCount { get; }

        /// <summary>
        /// Gets the number of audio outputs the device supports. Should not change.
        /// </summary>
        int AudioOutputCount { get; }

        /// <summary>
        /// Reads the state of all input closures and returns a bitmask of each set.
        /// </summary>
        /// <returns></returns>
        ulong ReadOptos();

        /// <summary>
        /// Reads the current state of all output relays and returns a bitmask of each set.
        /// </summary>
        /// <returns></returns>
        ulong ReadRelays();

        /// <summary>
        /// Sets the status of all relays from the bitmask specified.
        /// </summary>
        /// <param name="relays"></param>
        void SetRelays(ulong relays);

        /// <summary>
        /// Sets the status of a single relay.
        /// </summary>
        /// <param name="index">The index of the relay, starting at zero.</param>
        /// <param name="set">True if the relay is to be closed.</param>
        void SetRelay(int index, bool set);

        /// <summary>
        /// Reads the audio matrix state. Returns an array with size AudioOutputCount, where each element is a bitmask of the current inputs active on that output. Each element represents an output.
        /// </summary>
        /// <returns></returns>
        ulong[] ReadAudioOutputs();

        /// <summary>
        /// Updates the current audio state. Input array must be size AudioOutputCount, where each element represents the state of each output. Each item is a bitmask of the current inputs active on that output.
        /// Returns the status recieved from the switcher.
        /// </summary>
        /// <param name="state"></param>
        ulong[] SetAudioOutputs(ulong[] state);
    }
}
