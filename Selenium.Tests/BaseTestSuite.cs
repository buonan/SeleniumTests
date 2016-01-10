using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Selenium.PageObjects;

namespace Selenium.Tests
{
    /// <summary>
    /// BaseTestSuite settings loaded from Selenium.Tests.dll.config in current Dll directory
    /// </summary>
    public class BaseTestSuite
    {
        public string TestUser;
        public string TestPassword;
        public string BaseUrl;
        public string LocalUrl;
        public string Host;
        public string ServerPort;
		public string ApiPort;
        public string CoverageReport;

        /// <summary>
        /// The Browser instance.
        /// </summary>
        protected Selenium.PageObjects.Instance instance = null;


        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public Selenium.PageObjects.Instance Instance {
            get {
                return this.instance;
            }
        }

        /// <summary>
        /// The browser capabilities.
        /// </summary>
        public DesiredCapabilities BrowserCapabilities = null;

        /// <summary>
        /// Init this instance and load from app.config browserstack default capabilities.
        /// </summary>]
        [TestFixtureSetUp]
        public void TestSuiteBegin()
        {
            Console.WriteLine("TestSuiteBegin");
            try {
                //
                // Load our default settings
                //
                this.TestUser = ConfigurationManager.AppSettings["TestUser"];
                this.TestPassword = ConfigurationManager.AppSettings["TestPassword"];
                this.BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];
                this.LocalUrl = ConfigurationManager.AppSettings["LocalUrl"];
                this.Host = new Uri(this.LocalUrl).Host;
                this.ServerPort = ConfigurationManager.AppSettings["ServerPort"];
                this.ApiPort = ConfigurationManager.AppSettings["ApiPort"];
                this.CoverageReport = ConfigurationManager.AppSettings["CoverageReport"];
                Console.WriteLine("LocalUrl is '{0}'", this.LocalUrl);
                Console.WriteLine("Host is '{0}'", this.Host);
                Console.WriteLine("ServerPort is '{0}'", this.ServerPort);
                Console.WriteLine("CoverageReport is '{0}'", this.CoverageReport);
                Console.WriteLine("ApiPort is '{0}'", this.ApiPort);

                // Set our global subdomain
                Selenium.PageObjects.Globals.SubDomain = Selenium.PageObjects.StaticHelpers.GetSubDomain(new Uri(this.BaseUrl));
            }
            catch {
            }
            //
            // Load our default capabilities
            //
            this.LoadDefaultCapabilities();

        }

        /// <summary>
        /// UnInit this instance and kill any browserstacklocal tunnel process.
        /// </summary>
        [TestFixtureTearDown]
        public void TestSuiteEnd()
        {
            // TODO: Talk to Damien if there's a better way start/stop BrowserStackLocal process outside of tests
            this.KillBrowserStackLocal();
            Console.WriteLine("TestSuiteEnd");
        }

        /// <summary>
        /// Testcase begin.
        /// </summary>
        public void TestCaseBegin()
        {
            Console.WriteLine("TestCaseBegin");
            //
            // Load our default capabilities to reset for each testcase
            //
            this.LoadDefaultCapabilities();
        }

        /// <summary>
        /// Testcase end.
        /// </summary>
        public void TestCaseEnd()
        {                     
            // Generate Coverage report if coverage run
            if (string.Compare(this.CoverageReport.ToLower(),"true") == 0) {
                try {               
                    const string coverFolder = @"json/";
                    string testcaseName = TestContext.CurrentContext.Test.FullName;
                    if (!System.IO.Directory.Exists(coverFolder)) {
                        System.IO.Directory.CreateDirectory(coverFolder);
                    }
                    string filename = string.Format("{0}/{1}.coverage.json", coverFolder, testcaseName);
                    string json = (string)((IJavaScriptExecutor)this.Instance.IWebDriver).ExecuteScript("return JSON.stringify(window.__coverage__);");
                    if (json.Length > 0) {
                        StreamWriter sw = new StreamWriter(filename);
                        sw.WriteLine(json);
                        sw.Close();
                        Console.WriteLine(string.Format("Writing coverage '{0}'", filename));
                    }
                }
                catch (Exception) 
                {
                    // Silently eat if no code coverage
                }           
            }

            // 
            // Quite the Browser session
            //
            if (this.instance != null) {
                this.instance.Quit();
                this.instance = null;
            }

            Console.WriteLine("TestCaseEnd");
        }

        /// <summary>
        /// Kills all browser stack local process used for localhost tunneling testing.
        /// </summary>
        public void KillBrowserStackLocal()
        {
            try {
                foreach (var process in Process.GetProcessesByName("BrowserStackLocal")) {
                    process.Kill();
                }
            } catch {
                // eat any exceptions we dont care
            }
            Console.WriteLine("Killed BrowserStackLocal processes");
        }

        /// <summary>
        /// Loads the default capabilities from Selenium.Tests.dll.config in current directory.
        /// </summary>
        public void LoadDefaultCapabilities()
        {
            Console.WriteLine("LoadDefaultCapabilities");
            this.BrowserCapabilities = new DesiredCapabilities();
            this.BrowserCapabilities.SetCapability("build", ConfigurationManager.AppSettings["build"]);
            this.BrowserCapabilities.SetCapability("project", ConfigurationManager.AppSettings["project"]);
            this.BrowserCapabilities.SetCapability("browserstack.user", ConfigurationManager.AppSettings["browserstack.user"]);
            this.BrowserCapabilities.SetCapability("browserstack.key", ConfigurationManager.AppSettings["browserstack.key"]);
            this.BrowserCapabilities.SetCapability("browserstack.debug", ConfigurationManager.AppSettings["browserstack.debug"]);
            this.BrowserCapabilities.SetCapability("browserstack.local", ConfigurationManager.AppSettings["browserstack.local"]);
            this.BrowserCapabilities.SetCapability("resolution", ConfigurationManager.AppSettings["resolution"]);
            this.BrowserCapabilities.SetCapability("requireWindowFocus", true);
            this.BrowserCapabilities.SetCapability("enablePersistentHover", false);
        }

		/// <summary>
		/// This performs an assert and catches the errors and returns them as a string so you test can continue without bailing after a failure.
		/// You can specify whatever type and the comparison logic in the caller.
		/// </summary>
		/// <returns>The check.</returns>
		/// <param name="first">First.</param>
		/// <param name="second">Second.</param>
		/// <param name="comparison">Comparison.</param>
		/// <param name="message">Message.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public string AssertCheck<T>(T first, T second, Func<T, T, bool> comparison, string message) //where  T : struct
		{
			string exceptionString = null;
			try {
				Assert.IsTrue(comparison(first, second), message);
			}
			catch (Exception e) {
				exceptionString = e.ToString();
			}
			return exceptionString;
		}
    }
}

