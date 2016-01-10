using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Interactions;

namespace Selenium.PageObjects
{
    /// <summary>
    /// Singleton Factory class for WebDrivers.
    /// </summary>
    public class Factory
    {
        public static double DefaultTimeoutValue = 45;
        private static volatile IWebDriver _instance = null;
        private static object syncRoot = new Object();

        private Factory()
        {
        }

        public static IWebDriver Instance {
            get {
                if (_instance == null) {
                    lock (syncRoot) {
                        if (_instance == null) {
                            _instance = _CreateInstance(null, Drivers.ChromeDriver);
                        }
                    }
                }

                return _instance;
            }
        }

        public enum Drivers
        {
            IEDriver,
            ChromeDriver,
            FirefoxDriver,
            BrowserStack,
            LocalStack,
        }

        public static void CreateInstance(DesiredCapabilities caps, Drivers driver, double timeout)
        {
            DefaultTimeoutValue = timeout;
            lock (syncRoot) {
                _instance = _CreateInstance(caps, driver);
            }

        }

        private static IWebDriver _CreateInstance(DesiredCapabilities caps, Drivers driver)
        {
            IWebDriver _webdriver = null;
            if (caps == null) {
                //
                // Loaded default caps for RemoteWebDriver from Selenium.Tests.dll.config in current Dll directory
                // See brian.ku about user/key
                //
                caps = new DesiredCapabilities();
                caps.SetCapability("browserstack.user", ConfigurationManager.AppSettings["browserstack.user"]);
                caps.SetCapability("browserstack.key", ConfigurationManager.AppSettings["browserstack.key"]);
                caps.SetCapability("browser", ConfigurationManager.AppSettings["browser"]);
                caps.SetCapability("browser_version", ConfigurationManager.AppSettings["browser_version"]);
                caps.SetCapability("os", ConfigurationManager.AppSettings["os"]);
                caps.SetCapability("os_version", ConfigurationManager.AppSettings["os_version"]);
                caps.SetCapability("resolution", ConfigurationManager.AppSettings["resolution"]);
                caps.SetCapability("browserstack.debug", ConfigurationManager.AppSettings["browserstack.debug"]);
                caps.SetCapability("browserstack.local", ConfigurationManager.AppSettings["browserstack.local"]);
            }
            switch (driver) {
            case Drivers.IEDriver:
                InternetExplorerDriverService ieService = InternetExplorerDriverService.CreateDefaultService();
                _webdriver = new InternetExplorerDriver(ieService, new InternetExplorerOptions(), TimeSpan.FromSeconds(3 * 60));
                break;
            case Drivers.ChromeDriver:
                ChromeOptions chromeOptions = new ChromeOptions();
                chromeOptions.AddExcludedArgument("ignore-certifcate-errors");
                chromeOptions.AddArgument("test-type");
                _webdriver = new ChromeDriver(chromeOptions);
                _webdriver.Manage().Window.Maximize();
                break;
            case Drivers.FirefoxDriver:
                FirefoxProfile profile = new FirefoxProfile();
                _webdriver = new FirefoxDriver(profile);
                _webdriver.Manage().Window.Maximize();
                break;
            case Drivers.BrowserStack:
                caps.SetCapability("ensureCleanSession", true);
                _webdriver = new RemoteWebDriver(new Uri(ConfigurationManager.AppSettings["remotewebdriver_url"]), caps);
                //_webdriver.Manage().Window.Maximize();
                break;
            case Drivers.LocalStack:
                //
                // Start the local BrowserStack proxy for cloud browser access
                //
                bool bConnected = false;
                do {
                    //
                    // Reset the stdout capture
                    //
                    sbOutput = new StringBuilder();

                    //
                    // check if connection succeeeded
                    //
                    bConnected = LaunchBrowserStackLocalProcess();
                } while (!bConnected);

                _webdriver = new RemoteWebDriver(new Uri(ConfigurationManager.AppSettings["remotewebdriver_url"]), caps, TimeSpan.FromMinutes(5.0));
                break;
            }
            //
            // Set an implicit timeout for all FindElements
            //
            _webdriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(Factory.DefaultTimeoutValue));

            return _webdriver;
        }

        //
        // BrowserStackLocal connection settings
        //
        static StringBuilder sbOutput;
        static protected int IOFound = 0;
        static protected AutoResetEvent _IOSyncEvent = new AutoResetEvent(false);

        /// <summary>
        /// Launches the browser stack local process.
        /// </summary>
        /// <returns><c>true</c>, if browser stack local process was launched, <c>false</c> otherwise.</returns>
        private static bool LaunchBrowserStackLocalProcess()
        {
            // 
            // Check if an instance of BrowserStackLocal is already running
            //
            if (!IsBrowserStackLocalProcessRunning()) {
                Process p = new Process();
                p.StartInfo.FileName = string.Format("./BrowserStackLocal");
                p.StartInfo.Arguments = string.Format("{0}", ConfigurationManager.AppSettings["browserstacklocal_settings"]);
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                Console.WriteLine("Starting ./BrowserStackLocal {0}", p.StartInfo.Arguments); 
                p.Start();
                Process.GetProcessById(p.Id);

                // Set our event handler to asynchronously read the sort output.
                p.OutputDataReceived += new DataReceivedEventHandler(StdOutputHandler);
                p.BeginOutputReadLine();

                //
                // Block this thread until signaled in StdOutputHandler
                //            
                _IOSyncEvent.WaitOne(3 * 60 * 1000); 

                Console.WriteLine("Started ./BrowserStackLocal {0}", p.StartInfo.Arguments); 

                //
                // Check if we found the 'access' text in stdout
                //
                if (IOFound > 0) {
                    return true;
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether this instance is browser stack local process running.
        /// </summary>
        /// <returns><c>true</c> if this instance is browser stack local process running; otherwise, <c>false</c>.</returns>
        public static bool IsBrowserStackLocalProcessRunning()
        {
            bool bRunning = false;
            foreach (var process in Process.GetProcessesByName("BrowserStackLocal")) {
                bRunning = true;
                break;
            }
            return bRunning;
        }

        /// <summary>
        /// Stds the output handler.
        /// </summary>
        /// <param name="sendingProcess">Sending process.</param>
        /// <param name="outLine">Out line.</param>
        public static void StdOutputHandler(object sendingProcess, 
            DataReceivedEventArgs outLine)
        {         
            //
            // Keep appending outLine 
            //
            if (!String.IsNullOrEmpty(outLine.Data)) {
                sbOutput.Append(outLine.Data);
            }

            //
            // Check the sbOutput strings
            //
            if (sbOutput.ToString().ToLower().Contains("ctrl-c") == true) {
                //
                // Increment and signal
                //
                Interlocked.Increment(ref IOFound);
                _IOSyncEvent.Set();
            }           
        }

        /// <summary>
        /// Finds the element.
        /// </summary>
        /// <returns>The element.</returns>
        /// <param name="by">By.</param>
        /// <param name="timeoutInSeconds">Timeout in seconds.</param>
        public static IWebElement FindElement(By by, double timeoutInSeconds = 0.0)
        {
            IWebDriver driver = Factory.Instance;
            Console.WriteLine("FindElement by '{0}'", by.ToString());
            if (timeoutInSeconds > 0) {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(drv => drv.FindElement(by));
            }
            return driver.FindElement(by);
        }

        /// <summary>
        /// Finds the element and does not throw an exception.
        /// </summary>
        /// <param name="by">By.</param>
        public static IWebElement FindElementNotThrow(By by, double timeoutInSeconds = 0.0)
        {
            Console.WriteLine("FindElementNotThrow by '{0}'", by.ToString());
            try {
                IWebDriver driver = Factory.Instance;
                if (timeoutInSeconds > 0){
                    new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                }
                return driver.FindElement(by);
            } 
            catch (Exception) {
                return null;
            }
        }

        /// <summary>
        /// Finds the element, wait until they exists.
        /// </summary>
        /// <returns>The element.</returns>
        /// <param name="by">By.</param>
        /// <param name="timeoutInSeconds">Timeout in seconds.</param>
        public static ReadOnlyCollection<IWebElement> FindElements(By by, double timeoutInSeconds = 15.0)
        {
            IWebDriver driver = Factory.Instance;
            Console.WriteLine("FindElements by '{0}' timeout '{1}'", by.ToString(), timeoutInSeconds);               
            if (timeoutInSeconds > 0) {                
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                wait.Until(ExpectedConditions.ElementExists(by));
                return driver.FindElements(by);
            }
            return driver.FindElements(by); 
        }

        /// <summary>
        /// Finds the elements, wait until they're visible.
        /// </summary>
        /// <returns>The elements visible.</returns>
        /// <param name="by">By.</param>
        /// <param name="timeoutInSeconds">Timeout in seconds.</param>
        public static ReadOnlyCollection<IWebElement> FindElementsVisible(By by, double timeoutInSeconds = 15.0)
        {
            IWebDriver driver = Factory.Instance;
            Console.WriteLine("FindElements by '{0}' timeout '{1}'", by.ToString(), timeoutInSeconds);               
            if (timeoutInSeconds > 0) {                
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                wait.Until(ExpectedConditions.ElementIsVisible(by));
                return driver.FindElements(by);
            }
            return driver.FindElements(by); 
        }

        /// <summary>
        /// Waits for element visible.
        /// </summary>
        /// <returns>The for element visible.</returns>
        /// <param name="by">By.</param>
        public static IWebElement WaitForElementVisible(By by)
        {
            IWebDriver driver = Factory.Instance;
            double timeoutInSeconds = Factory.DefaultTimeoutValue;
            Console.WriteLine("WaitForElementVisible by '{0}'", by.ToString());     
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            return wait.Until(ExpectedConditions.ElementIsVisible(by));
        }

        /// <summary>
        /// Determines if is element in view port the specified el.
        /// </summary>
        /// <returns><c>true</c> if is element in view port the specified el; otherwise, <c>false</c>.</returns>
        /// <param name="el">El.</param>
        public static bool IsElementInViewPort(IWebElement el)
        {
            Size window = Factory.Instance.Manage().Window.Size;
            bool fFound = false;
            Point pt = el.Location;
            //Point ptInViewPort = ((ILocatable)el).Coordinates.LocationInViewport;
            long scrollPos = Factory.GetCurrentScrollPosition();
            if (pt.X >= 0 &&
                pt.Y >= 0 &&
                pt.X <= window.Width) {
                // Case where element is above current scollpos
                if (pt.Y < scrollPos) {
                    fFound = false;
                }
                else if ((pt.Y - scrollPos) <= window.Height && el.Displayed) {
                    fFound = true;               
                }
            }
            Console.WriteLine("Element X = '{0}', Y = '{1}' \t window.Width = '{2}', window.Height = '{3}' \t scrollpos = '{4}'", pt.X, (pt.Y - scrollPos), window.Width, window.Height, scrollPos);
            return fFound;
        }

        /// <summary>
        /// Gets the current scroll position.
        /// </summary>
        /// <returns>The current scroll position.</returns>
        public static long GetCurrentScrollPosition()
        {
            IJavaScriptExecutor executor = (IJavaScriptExecutor)Factory.Instance;
            long value = (long)executor.ExecuteScript("return window.scrollY;");
            return value;
        }
    }
}