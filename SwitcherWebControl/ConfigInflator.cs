using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwitcherWebControl.Config;
using SwitcherWebControl.Exceptions;
using SwitcherWebControl.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitcherWebControl
{
    /// <summary>
    /// Class responsible for loading a config.
    /// </summary>
    class ConfigInflator
    {
        public delegate IControlDevice CreateControlDevice(JObject info);

        public ConfigInflator()
        {

        }

        private readonly Dictionary<string, CreateControlDevice> deviceClasses = new Dictionary<string, CreateControlDevice>();

        /// <summary>
        /// Registers a control device that can be created from a given class ID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="func"></param>
        public void RegisterControlDevice(string id, CreateControlDevice func)
        {
            deviceClasses.Add(id, func);
        }

        /// <summary>
        /// Initializes a controller from a config filename.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public WebController Load(string filename)
        {
            //Check if it exists
            if (!File.Exists(filename))
                throw new FormattedException("Config file does not exist.");

            //Read file
            string json;
            try
            {
                json = File.ReadAllText(filename);
            } catch
            {
                throw new FormattedException("Failed to read file.");
            }

            //Deserialize
            ConfigRoot config;
            try
            {
                config = JsonConvert.DeserializeObject<ConfigRoot>(json);
            } catch (Exception ex)
            {
                throw new FormattedException($"Failed to deserialize config file: {ex.Message}");
            }

            return Load(config);
        }

        /// <summary>
        /// Initializes a controller from a config file. Raises an exception if failed.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public WebController Load(ConfigRoot config)
        {
            //Check root
            if (config == null || config.Devices == null || config.ListenAddress == null)
                throw new FormattedException("Config is null, or missing required fields. Refer to the documentation.");

            //Create all control devices.
            WebControlDevice[] devices = new WebControlDevice[config.Devices.Length];
            for (int i = 0; i < devices.Length; i++)
            {
                //Get and check data
                ConfigDevice deviceInfo = config.Devices[i];
                if (deviceInfo == null || deviceInfo.Id == null || deviceInfo.DeviceType == null)
                    throw new FormattedException($"Device #{i + 1} is missing required fields. Refer to the documentation.");

                //Resolve the device type to a registered class
                CreateControlDevice createDevice = null;
                if (!deviceClasses.TryGetValue(deviceInfo.DeviceType, out createDevice))
                    throw new FormattedException($"Device \"{deviceInfo.Id}\" uses unknown device type \"{deviceInfo.DeviceType}\".");

                //Create the control device
                IControlDevice control;
                try
                {
                    control = createDevice(deviceInfo.DeviceInfo);
                } catch (Exception ex)
                {
                    throw new FormattedException($"Failed to create control device \"{deviceInfo.Id}\": {ex.Message}{ex.StackTrace}");
                }
                if (control == null)
                    throw new FormattedException($"Failed to create control device \"{deviceInfo.Id}\": Device was null.");

                //Create the web wrapper for it
                devices[i] = new WebControlDevice(deviceInfo.Id, control);
            }

            //Create web service
            return new WebController(config.ListenAddress, devices);
        }
    }
}
