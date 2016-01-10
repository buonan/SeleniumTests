using System;
using System.Linq;
using System.Configuration;
using System.Drawing;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Selenium.PageObjects;
using NUnit_retry;

namespace Selenium.Tests
{
    /// <summary>
    /// This test verifies we can run remote tests in BrowserStack.com. 
    /// User and Key are provided by BrowserStack account.
    /// </summary>
    [TestFixture()]
    public class BrowserStackTestSuite : BaseTestSuite
    {
        #pragma warning disable 0414		
 
		/// <summary>
		/// Desktop Browsers for testcase ValueSource.
		/// </summary>
		public class DesktopBrowsers
		{
			public string browser; 
			public string browser_version;
			public string os;
			public string os_version;
			public string resolution;

			public DesktopBrowsers(string browser, string browser_version, string os, string os_version, string resolution)
			{
				this.browser = browser;
				this.browser_version = browser_version;
				this.os = os;
				this.os_version = os_version;
				this.resolution = resolution;
			}
		}

		/// <summary>
		/// Desktop Browsers factory for testcase ValueSource.
		/// </summary>
		public class DesktopBrowsersFactory
		{
			public IEnumerable<DesktopBrowsers> DesktopBrowsers()
			{
				yield return new DesktopBrowsers("Chrome", "44", "OS X", "Mavericks", "1600x1200");
				yield return new DesktopBrowsers("IE", "11", "Windows", "8.1", "1600x1200");
				yield return new DesktopBrowsers("Safari", "8", "OS X", "Yosemite", "1600x1200");
				yield return new DesktopBrowsers("Firefox", "40", "OS X", "Mavericks", "1600x1200");
			}
		}
                     
        #pragma warning restore 0414

        /// <summary>
        /// TestCase base initialization
        /// </summary>
        [SetUp]
        public void Init()
        {
			Console.WriteLine("Init");
            base.TestCaseBegin();
        }

        /// <summary>
        /// Cleanup this instance.
        /// </summary>
        [TearDown]
        public void Cleanup()
        {
			Console.WriteLine("Cleanup");
            base.TestCaseEnd();
        }

        /// <summary>DeviceMatrix
        /// BVTs for browser stack.  Run this to verify our handshake is setup.
        /// </summary>
        [Test()]
        [Category("BrowserStack")]
		[Retry(Times = 3, RequiredPassCount = 1)]
        public void BVT_BrowserStack()
        {
            IWebDriver driver;
            driver = new RemoteWebDriver(
                new Uri("http://hub.browserstack.com/wd/hub/"), base.BrowserCapabilities);
            driver.Navigate().GoToUrl("http://www.google.com/ncr");
            Console.WriteLine(driver.Title);

            IWebElement query = driver.FindElement(By.Name("q"));
            query.SendKeys("Browserstack");
            query.Submit();
            Console.WriteLine(driver.Title);

            if (false == driver.Title.Contains("Google")) {
                Assert.Fail("Expected: '{0}' and Actual: '{1}'", "Google", driver.Title);
            } else {
                Console.WriteLine("Expected: '{0}' and Actual: '{1}'", "Google", driver.Title);
            }
            driver.Quit();
        }     

        /// <summary>
        /// Subnav social
        /// </summary>
        [Test()]
        [TestCaseSource("DesktopMatrix")]
        [Category("BrowserStack")]
        [Retry(Times = 3, RequiredPassCount = 1)]
        public void SocialLinks(string browser, string browser_version, string os, string os_version)
        {            
            base.BrowserCapabilities.SetCapability("project", "SocialLinks");
            base.BrowserCapabilities.SetCapability("browser", browser);
            base.BrowserCapabilities.SetCapability("browser_version", browser_version);
            base.BrowserCapabilities.SetCapability("os", os);
            base.BrowserCapabilities.SetCapability("os_version", os_version);
            this.instance = new Selenium.PageObjects.Browsers.BrowserStackInstance(base.BrowserCapabilities);
            CommonTests.SocialLinks(this);
        }

        /// <summary>
        /// Performance BenchMark.
        /// </summary>
        public class PerformanceBenchMark
        {
            public string url; 
            public int benchmark;          

            public PerformanceBenchMark(string url, int benchmark)
            {
                this.url = url;
                this.benchmark = benchmark;
            }
        }

        /// <summary>
        /// Performance Benchmark factory.
        /// </summary>
        public class PerformanceBenchMarkFactory
        {
            public IEnumerable<PerformanceBenchMark> PerformanceBenchMark()
            {
                yield return new PerformanceBenchMark("http://www.google.com/", 15000);
                yield return new PerformanceBenchMark("http://www.yahoo.com/", 15000);
            }
        }
        [Test()]
        [Category("BrowserStack")]
        public void PerformancePageLoadTest([ValueSource(typeof(DesktopBrowsersFactory), "DesktopBrowsers")]DesktopBrowsers browser,
            [ValueSource(typeof(PerformanceBenchMarkFactory), "PerformanceBenchMark")]PerformanceBenchMark performance)
        {   
            base.BrowserCapabilities.SetCapability("project", "PerformancePageLoadTest");
            base.BrowserCapabilities.SetCapability("browser", browser.browser);
            base.BrowserCapabilities.SetCapability("browser_version", browser.browser_version);
            base.BrowserCapabilities.SetCapability("os", browser.os);
            base.BrowserCapabilities.SetCapability("os_version", browser.os_version);
            this.instance = new Selenium.PageObjects.Browsers.BrowserStackInstance(base.BrowserCapabilities);   
            CommonTests.PerformancePageLoadTest(this, performance.url, performance.benchmark);
        }       
    }
}

