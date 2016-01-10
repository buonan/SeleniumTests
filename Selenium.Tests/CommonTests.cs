using System;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Selenium.PageObjects;
using System.Web.Script.Serialization;
using OpenQA.Selenium.Interactions;

namespace Selenium.Tests
{
    /// <summary>
    /// Common tests (can be run both local machine or browserstack).
    /// </summary>
    internal class CommonTests
    {
        #region SocialLinksTests
        /// <summary>
        /// Socials the links.
        /// </summary>
        /// <param name="ts">Ts.</param>
        /// <param name="instance">Instance.</param>
        public static void SocialLinks(BaseTestSuite ts)
        {            
            string url = "http://www.google.com/";
            Selenium.PageObjects.DefaultPage page = new Selenium.PageObjects.DefaultPage(url);
            string exceptions;
            const int expected = 5;
            StringBuilder failures = new StringBuilder();
            if (!page.VerifySocialShares(out exceptions, expected)) {
                failures.AppendLine(exceptions);
            }
            if (failures.ToString().Trim().Length > 0) {
                Assert.Fail(failures.ToString());
            }
        }
		#endregion

		#region PerformanceTests
		/// <summary>
		/// Page load performance test.
		/// </summary>
		/// <param name="ts">TestSuite</param>
		/// <param name="url">URL page</param>
		/// <param name="benchmark">Benchmark.</param>
		public static void PerformancePageLoadTest(BaseTestSuite ts, string url, int benchmark)
		{
			DateTime start = DateTime.Now;
			ts.Instance.NavigateToSkipReadyForStatic(url);
			ts.Instance.WaitForReadyForStatic();
			DateTime end = DateTime.Now;
			TimeSpan diff = end - start;
			int actual = (int)diff.TotalMilliseconds;
			int expected = benchmark;
			Console.WriteLine("Page loaded waiting for #readyForStatic in '{0}'ms", actual);
			Assert.Less(actual, expected, string.Format("Took longer than '{0}'ms to load, url '{1}'!", expected, url));
		}
		#endregion //PerformanceTests
	}
}
