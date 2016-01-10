using System;
using System.Collections.ObjectModel;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace Selenium.PageObjects
{
    /// <summary>
    /// Base webpage class for new response site.
    /// </summary>
    public class BasePage : Instance
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Selenium.PageObjects.BasePage"/> class.
        /// </summary>
        /// <param name="fWait">If set to <c>true</c> f wait.</param>
        public BasePage(bool fWait = true)
        {
            // Some pages dont have readyForStatic yet so we dont wait.
            if (fWait) {
                this.WaitForDocumentLoaded();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Selenium.PageObjects.BasePage"/> class.
        /// </summary>
        /// <param name="url">URL.</param>
        public BasePage(string url)
        {
            Console.WriteLine("{0}", url);
            IWebDriver driver = Factory.Instance;
            Console.WriteLine("GoToUrl url = '{0}'", url);

            // We will not call NavigateTo here because some tests navigate to API not pages (HealthTest)
            driver.Navigate().GoToUrl(url);
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title {
            get {
                return Factory.Instance.Title;
            }
        }

        /// <summary>
        /// Gets the inner text.
        /// </summary>
        /// <value>The inner text.</value>
        public string InnerText {
            get {
                return this.WaitForElement(By.TagName("body"), 5.0).Text;
            }
        }

        /// <summary>
        /// Gets the inner HTM.
        /// </summary>
        /// <value>The inner HTM.</value>
        public string InnerHTML {
            get {
                return Factory.Instance.PageSource;
            }
        }            

        #endregion //Properties
    }
}

