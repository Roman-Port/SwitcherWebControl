using SwitcherWebControl.Devices.BTools;
using SwitcherWebControl.Exceptions;
using SwitcherWebControl.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitcherWebControl
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Decode arguments
            if (args.Length < 1)
            {
                Console.WriteLine("ERROR: Must pass config filename as argument. Exiting...");
                return;
            }

            //Set up inflator and register classes
            ConfigInflator inflator = new ConfigInflator();
            inflator.RegisterControlDevice("BTools.8x2Plus", BTools82Plus.InflateDevice);

            //Load config
            string configFilename = args[0];
            Console.WriteLine($"Loading config from \"{configFilename}\"...");
            WebController server;
            try
            {
                server = inflator.Load(configFilename);
            }
            catch (FormattedException ex)
            {
                Console.WriteLine($"Error initializing config: {ex.Message}");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unknown error initializing config: {ex.Message}{ex.StackTrace}");
                return;
            }
            Console.WriteLine($"Inflated config successfully; {server.Devices.Length} devices loaded.");

            //Initialize all devices
            Console.WriteLine("Initializing devices...");
            foreach (var d in server.Devices)
            {
                try
                {
                    d.Device.Initialize();
                } catch (FormattedException ex)
                {
                    Console.WriteLine($"Error initializing \"{d.Id}\" device: {ex.Message}");
                    return;
                } catch (Exception ex)
                {
                    Console.WriteLine($"Unknown error initializing \"{d.Id}\" device: {ex.Message}{ex.StackTrace}");
                    return;
                }
            }
            Console.WriteLine("Devices initialized. Starting server...");

            //Run server
            server.RunServer();
        }
    }
}
