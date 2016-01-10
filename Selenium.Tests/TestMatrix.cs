using System;

namespace Selenium.Tests
{
    public class TestMatrix
    {
        #pragma warning disable 0414

        /// <summary>
        /// The browser stack device matrix.
        /// </summary>
        public static object[] BrowserStackDeviceMatrix =
        {
            /* browserName, platform, device */
            new object[] { "iPhone" , "MAC" , "iPhone 5" },
            new object[] { "iPad" , "MAC" , "iPad Air" } 
        };

        /// <summary>
        /// The browser stack platform matrix.
        /// </summary>
        public static object[] BrowserStackPlatformMatrix = 
        {
            /* browserName, browser_version, os, os_version */
            new string[] { "Chrome" , "43" , "OS X" , "Mavericks" },            
            new string[] { "IE" , "11" , "Windows" , "8.1" },
        };

        /// <summary>
        /// The browser stack page load platform matrix.
        /// </summary>
        public static object[] BrowserStackPageLoadDesktopMatrix = 
        {
            /* browserName, browser_version, os, os_version */
            new string[] { "Chrome" , "43" , "OS X" , "Mavericks", "15" }, 
            new string[] { "Chrome" , "42" , "Windows" , "8.1", "147" }, 
            new string[] { "IE" , "11" , "Windows" , "10", "101" },
            new string[] { "IE" , "10" , "Windows" , "8", "181" },
            new string[] { "IE" , "9" , "Windows" , "7", "78" },
            new string[] { "Safari" , "9" , "OS X" , "El Capitan", "128" },   
            new string[] { "Safari" , "8" , "OS X" , "Yosemite", "128" },   
            new string[] { "Firefox" , "41" , "OS X" , "Yosemite", "523" },  
            new string[] { "Firefox" , "40" , "Windows" , "8.1", "523" }  
        };

        //// <summary>
        /// The browser stack page load device matrix.
        /// </summary>
        public static object[] BrowserStackPageLoadDeviceMatrix =
        {
            /* browserName, platform, device */
            //new object[] { "iPhone" , "MAC" , "iPhone 6", "15" },
            new object[] { "iPhone" , "MAC" , "iPhone 5", "15" },
            //new object[] { "iPhone" , "MAC" , "iPhone 5S", "147" },
            new object[] { "iPad" , "MAC" , "iPad Air", "101" },
            new object[] { "android" , "ANDROID" , "Samsung Galaxy S5", "128" },
            //new object[] { "android" , "ANDROID" , "Samsung Galaxy Tab 4 10.1", "78" },
            //new object[] { "android" , "ANDROID" , "Google Nexus 6", "523" } 
        };


        /// <summary>
        /// The local platform matrix.
        /// </summary>
        public static object[] IntegrationPlatformMatrix = 
        {            
            new string[] { "Chrome" , "43" , "OS X" , "Mavericks" } 
        };

        #pragma warning restore 0414
    }
}

