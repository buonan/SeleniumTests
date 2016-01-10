using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using NUnit.Framework;

namespace Scout.Selenium.Tests
{
    [TestFixture()]
    public class PerformanceTests : BaseTestSuite
    {
        /// <summary>
        /// TestCase base initialization
        /// </summary>
        [SetUp]
        public void Init()
        {
            base.TestCaseBegin();
        }

        /// <summary>
        /// Cleanup this instance.
        /// </summary>
        [TearDown]
        public void Cleanup()
        {
            base.TestCaseEnd();
        }

        /// <summary>
        /// Page load performance test.
        /// </summary>
        [Test]
        [Category("Performance")]
        [TestCase("http://www.scout.com/", 15000)]
        [TestCase("http://master.scout.com/", 15000)]
        [TestCase("http://stage.scout.com/", 15000)]
        public void PerformancePageLoadTest(string url, int benchmark)
        {
            this.BrowserCapabilities.SetCapability("browser", "Chrome");
            this.BrowserCapabilities.SetCapability("browser_version", "36.0");  
            this.instance = new AreaLibrary.Browsers.ChromeInstance(this.BrowserCapabilities);
            CommonTests.PerformancePageLoadTest(this, url, benchmark);
        }
    }
}

