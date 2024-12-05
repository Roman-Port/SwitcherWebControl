using Newtonsoft.Json;
using SwitcherWebControl.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SwitcherWebControl.Web
{
    internal static class WebUtilities
    {
        public static string BitmaskToString(ulong bitmask, int bits)
        {
            char[] result = new char[bits];
            for (int i = 0; i < bits; i++)
                result[i] = ((bitmask & (1UL << i)) == 0) ? '0' : '1';
            return new string(result);
        }

        public static ulong StringToBitmask(string str)
        {
            char[] chars = str.ToCharArray();
            ulong result = 0;
            for (int i = 0; i < chars.Length; i++)
            {
                //Validate
                if (chars[i] != '0' && chars[i] != '1')
                    throw new FormattedException($"Invalid character in bitmask at index {i}.", 400);

                //Set if 1
                if (chars[i] == '1')
                    result |= 1UL << i;
            }
            return result;
        }

        /// <summary>
        /// Gets one or the other to use
        /// </summary>
        /// <param name="bitmask"></param>
        /// <param name="bitmaskString"></param>
        /// <returns></returns>
        public static ulong UlongOrStringToBitmask(ulong? bitmask, string bitmaskString)
        {
            if ((bitmask == null) == (bitmaskString == null))
                throw new FormattedException("Either the bitmask or bitmask string must be set exclusively.", 400);
            if (bitmask.HasValue)
                return bitmask.Value;
            return StringToBitmask(bitmaskString);
        }

        public static T DeserializePostJson<T>(this HttpListenerRequest request)
        {
            //Check content type
            if (request.ContentType.ToLower() != "application/json")
                throw new FormattedException($"Expected application/json", 400);

            //Read
            string json;
            using (StreamReader sr = new StreamReader(request.InputStream))
                json = sr.ReadToEnd();

            //Deserialize
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
