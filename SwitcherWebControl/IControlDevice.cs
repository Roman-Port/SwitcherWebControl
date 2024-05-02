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
    }
}
