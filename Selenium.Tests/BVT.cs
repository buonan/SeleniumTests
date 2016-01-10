using System;
using System.Configuration;
using NUnit.Framework;
using OpenQA.Selenium.Remote;
using Selenium.PageObjects;

namespace Selenium.Tests
{
	/// <summary>
	/// This test verifies this framework is working.
	/// 
    public class BVT : BaseTestSuite
	{
		/// <summary>>
		/// Tests basic navigation using Selenium. Verifies title is correct.
		/// </summary>	
		[Test()]
        [Category("BVT")]
		public void BVT_Test()
		{
			Browsers.ChromeInstance instance = new Browsers.ChromeInstance(base.BrowserCapabilities);
			string url =  @"http://www.google.com";
			instance.NavigateTo(url);
			DefaultPage page = new DefaultPage();	
            string title = page.Title;
			instance.Quit();
		}
	}
}

