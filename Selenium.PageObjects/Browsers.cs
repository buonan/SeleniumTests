using System;
using OpenQA.Selenium.Remote;

namespace Selenium.PageObjects
{
    /// <summary>
    /// Browser instances.
    /// </summary>
    public class Browsers
    {
        /// <summary>
        /// The default timeout in seconds.
        /// </summary>
        public static double DefaultTimeoutInSeconds = 30;

        /// <summary>
        /// IE.
        /// </summary>
        public class IEInstance : Instance
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Selenium.AreaLibrary.Browsers+IEInstance"/> class.
            /// </summary>
            /// <param name="caps">Caps.</param>
			public IEInstance(DesiredCapabilities caps, double timeout = 30)
            {
                Factory.CreateInstance(caps, Factory.Drivers.IEDriver, timeout);
            }
        }

        /// <summary>
        /// Chrome.
        /// </summary>
        public class ChromeInstance : Instance
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Selenium.AreaLibrary.Browsers+ChromeInstance"/> class.
            /// </summary>
            /// <param name="caps">Caps.</param>
			public ChromeInstance(DesiredCapabilities caps, double timeout = 30)
            {
				DefaultTimeoutInSeconds = timeout;
                Factory.CreateInstance(caps, Factory.Drivers.ChromeDriver, timeout);
            }
        }

        /// <summary>
        /// Firefox.
        /// </summary>
        public class FirefoxInstance : Instance
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Selenium.AreaLibrary.Browsers+FirefoxInstance"/> class.
            /// </summary>
            /// <param name="caps">Caps.</param>
			public FirefoxInstance(DesiredCapabilities caps, double timeout = 30)
            {
				DefaultTimeoutInSeconds = timeout;
                Factory.CreateInstance(caps, Factory.Drivers.FirefoxDriver, timeout);
            }
        }

        /// <summary>
        /// BrowserStack Remote Instance.
        /// </summary>
        public class BrowserStackInstance : Instance
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Selenium.AreaLibrary.Browsers+BrowserStackInstance"/> class.
            /// </summary>
            /// <param name="caps">Caps.</param>
			public BrowserStackInstance(DesiredCapabilities caps, double timeout = 30)
            {
				DefaultTimeoutInSeconds = timeout;
                Factory.CreateInstance(caps, Factory.Drivers.BrowserStack, timeout);
            }
        }

        /// <summary>
        /// BrowserStack Localhost Instance.
        /// </summary>
        public class BrowserStackLocalInstance : Instance
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Selenium.AreaLibrary.Browsers+BrowserStackLocalInstance"/> class.
            /// </summary>
            /// <param name="caps">Caps.</param>
			public BrowserStackLocalInstance(DesiredCapabilities caps, double timeout = 30)
            {
				DefaultTimeoutInSeconds = timeout;
                Factory.CreateInstance(caps, Factory.Drivers.LocalStack, timeout);
            }
        }
    }
}

