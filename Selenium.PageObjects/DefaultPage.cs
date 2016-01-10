using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using NUnit.Framework;

namespace Selenium.PageObjects
{
	/// <summary>
	/// Default responsive webpage properties/actions.
	/// </summary>
	public class DefaultPage : BasePage
	{
        #region Privates
        #endregion //Privates

        #region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Selenium.AreaLibrary.DefaultPage"/> class.
		/// </summary>
        public DefaultPage(bool fWaitForStatic = true) : base(false)
		{
            if (fWaitForStatic) {
                this.WaitForReadyForStatic();
            } else {
                this.WaitForDocumentLoaded();
            }
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="Selenium.AreaLibrary.DefaultPage"/> class.
        /// </summary>
        /// <param name="url">URL.</param>
		public DefaultPage(string url, bool fWaitForStatic = true) : base(false)
		{
			this.NavigateTo(url);
			if (fWaitForStatic) {
				this.WaitForReadyForStatic();
			} else {
				this.WaitForDocumentLoaded();
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Selenium.AreaLibrary.DefaultPage"/> class.
		/// </summary>
		/// <param name="localUrl">Local URL.</param>
		/// <param name="url">URL.</param>
		/// <param name="fWaitForStatic">If set to <c>true</c> f wait for static.</param>
		public DefaultPage(string localUrl, string url, bool fWaitForStatic = true) : base(false)
        {
			this.NavigateTo(url, localUrl);
            if (fWaitForStatic) {
                this.WaitForReadyForStatic();
            } else {
                this.WaitForDocumentLoaded();
            }
        }
        #endregion //Constructors

        #region Properties
        #endregion //Properties

        #region Actions
        #endregion //Actions

        #region Verifications
   
        /// <summary>
        /// Verifies the social shares.
        /// </summary>
        /// <returns><c>true</c>, if social shares was verifyed, <c>false</c> otherwise.</returns>
        /// <param name="exceptions">Exceptions.</param>
        /// <param name="expected">Expected.</param>
        public bool VerifySocialShares(out string exceptions, int expected)
        {
            bool result = true;
            exceptions = string.Empty;
            ReadOnlyCollection<IWebElement> links = this.FindElements(By.CssSelector(".social"));
            int actual = links.Count;
            string message = string.Format("Social shares expected: '{0}' actual: '{1}'", expected, actual);
            Console.WriteLine(message);
            try {
                Assert.GreaterOrEqual(actual, expected, message);
            }
            catch (Exception e)
            {
                exceptions = e.ToString();
                result = false;
            }
            return result;
        }

        #endregion //Verifications
	}
}

