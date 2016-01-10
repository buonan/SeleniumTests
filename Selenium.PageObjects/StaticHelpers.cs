using System;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;

namespace Selenium.PageObjects
{
    /// <summary>
    /// Static helpers.
    /// </summary>
    static public class StaticHelpers
    {            
        /// <summary>
        /// Gets the sub domain.
        /// </summary>
        /// <returns>The sub domain.</returns>
        /// <param name="url">URL.</param>
        public static string GetSubDomain(Uri url)
        {
            if (url.HostNameType == UriHostNameType.Dns)
            {
                string host = url.Host;
                var nodes = host.Split('.');
                return string.Format("{0}", nodes[0]);
            }
            return null; 
        }
    }
}

