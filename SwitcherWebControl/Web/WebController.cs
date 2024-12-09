using Newtonsoft.Json;
using SwitcherWebControl.Exceptions;
using SwitcherWebControl.Web.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SwitcherWebControl.Web
{
    class WebController
    {
        public WebController(string prefix, WebControlDevice[] devices)
        {
            server = new HttpListener();
            server.Prefixes.Add(prefix);
            this.devices = devices;
        }

        private readonly HttpListener server;
        private readonly WebControlDevice[] devices;

        public WebControlDevice[] Devices => devices;

        public void RunServer()
        {
            server.Start();
            while (true)
            {
                HttpListenerContext e = server.GetContext();
                HandleRequest(e);
                e.Response.Close();
            }
        }

        private void HandleRequest(HttpListenerContext e)
        {
            try
            {
                //Process
                object response = HandleRoot(e);

                //If an object is returned, serialize it
                if (response != null)
                {
                    //Format as JSON
                    string responseJson = JsonConvert.SerializeObject(response, Formatting.Indented);

                    //Send response
                    SendText(e, responseJson, "application/json", 200);
                }
            } catch (FormattedException ex)
            {
                SendError(e, ex.Message, ex.HttpCode);
            } catch (Exception ex)
            {
                SendError(e, "Unexpected error: " + ex.Message, 500);
            }
        }

        private void SendError(HttpListenerContext e, string message, int code)
        {
            try
            {
                SendText(e, message, "text/plain", code);
            } catch
            {
                //Ignore...
            }
        }

        private void SendText(HttpListenerContext e, string message, string type, int code)
        {
            byte[] text = Encoding.UTF8.GetBytes(message + "\n");
            e.Response.StatusCode = code;
            e.Response.ContentType = type;
            e.Response.ContentLength64 = text.Length;
            e.Response.OutputStream.Write(text, 0, text.Length);
        }

        private void ProcessRootPage(HttpListenerContext e)
        {
            using (Stream src = Assembly.GetExecutingAssembly().GetManifestResourceStream("SwitcherWebControl.Web.index.html"))
            {
                //Set headers
                e.Response.StatusCode = 200;
                e.Response.ContentType = "text/html";
                e.Response.ContentLength64 = src.Length;

                //Copy
                using (Stream dst = e.Response.OutputStream)
                    src.CopyTo(dst);
            }
        }

        private object HandleRoot(HttpListenerContext e)
        {
            //Check for root and return a basic web controller page
            if (e.Request.Url.AbsolutePath == "/" && e.Request.HttpMethod.ToUpper() == "GET")
            {
                ProcessRootPage(e);
                return null;
            }

            //Check for info endpoint
            if (e.Request.Url.AbsolutePath == "/api/devices" && e.Request.HttpMethod.ToUpper() == "GET")
            {
                return new WebInfoResponse(devices);
            }

            //If the path goes to a device, pass control to that
            foreach (var d in devices)
            {
                if (e.Request.Url.AbsolutePath.StartsWith("/api/devices/" + d.Id))
                    return d.HandleRoot(e.Request, e.Request.Url.AbsolutePath.Substring(("/api/devices/" + d.Id).Length));
            }

            throw new FormattedException("Endpoint not found.", 404);
        }
    }
}
