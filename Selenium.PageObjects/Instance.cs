using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

namespace Selenium.PageObjects
{
	/// <summary>
	/// Browser Instance members.
	/// </summary>
    abstract public class Instance : IDisposable
	{
        // Flag: Has Dispose already been called? 
        bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers. 
        public void Dispose()
        { 
            Dispose(true);
            GC.SuppressFinalize(this);           
        }

        // Protected implementation of Dispose pattern. 
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return; 

            //
            // We're using Dispose to quit the Browser session because NUnit-Retry doesn't call TestCaseEnd
            //
            if (disposing) {
                if (Factory.Instance != null) {
                    Factory.Instance.Quit();
                 }
            }

            //
            // Free any unmanaged objects here. 
            //
            disposed = true;
        }

		/// <summary>
		/// Gets the URL.
		/// </summary>
		/// <value>The URL.</value>
		public string Url {
			get {
				return Factory.Instance.Url;
			}
		}

        public IWebDriver IWebDriver {
            get {
                return Factory.Instance;
            }
        }

        /// <summary>
        /// Sets the implicit timeout.
        /// </summary>
        /// <param name="timeoutInSeconds">Timeout in seconds.</param>
        public void SetImplicitTimeout(double timeoutInSeconds)
        {
            Factory.Instance.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(timeoutInSeconds));
        }

		/// <summary>
		/// Close this instance.
		/// </summary>
		public void Close() 
		{
			Factory.Instance.Close();
		}

		/// <summary>
		/// Waits for page/document loaded.
		/// </summary>
		public void WaitForPageLoaded()
		{
			this.WaitForDocumentLoaded();
		}

		/// <summary>
		/// Waits for document loaded.
		/// </summary>
		public void WaitForDocumentLoaded()
		{
            Console.WriteLine("WaitForDocumentLoaded");
            try {
                this._RetryWaitForDocumentLoaded();
            }
            catch (TimeoutException) {
                this._RetryWaitForDocumentLoaded();
            }
		}

        /// <summary>
        /// Retries the wait for document loaded.
        /// </summary>
        private void _RetryWaitForDocumentLoaded()
        {
            IWebDriver driver = Factory.Instance;
            IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(Browsers.DefaultTimeoutInSeconds));
            wait.Until(d => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        }

        /// <summary>
        /// Waits for ajax.
        /// </summary>
        public void WaitForAjax()
        {
            double timeout = Browsers.DefaultTimeoutInSeconds;
            IWebDriver driver = Factory.Instance;
            while (timeout > 0)
            {
                var ajaxIsComplete = (bool)(driver as IJavaScriptExecutor).ExecuteScript("return jQuery.active == 0");
                if (ajaxIsComplete)
                    break;
                System.Threading.Thread.Sleep(100);
                timeout -= 100;
            }
        }

		/// <summary>
		/// Waits for ready for static.
		/// </summary>
		public void WaitForReadyForStatic()
		{
            try 
            {
			    this.WaitForElement("readyForStatic");
            }
            catch (TimeoutException) 
            {                
                Console.WriteLine("WaitForReadyForStatic timedout!");   
            }
		}

		/// <summary>
		/// Waits for element.
		/// </summary>
		/// <param name="elemId">Element identifier.</param>
		public void WaitForElement(string elemId)
		{
			IWebDriver driver = Factory.Instance;
            Console.WriteLine("WaitForElement id '{0}'", elemId);
			IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(Browsers.DefaultTimeoutInSeconds));
			wait.Until(d => d.FindElement(By.Id(elemId)));
		}

		/// <summary>
		/// Waits for element.
		/// </summary>
		/// <returns>The for element.</returns>
		/// <param name="by">By.</param>
		/// <param name="timeoutInSeconds">Timeout in seconds.</param>
		public IWebElement WaitForElement(By by, double timeoutInSeconds = 0)
		{
			IWebDriver driver = Factory.Instance; 
            Console.WriteLine("WaitForElement by '{0}'", by.ToString());
			if (timeoutInSeconds > 0)
			{
                // Clear implicit timeout
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(0));
                try 
                {
    				var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
    				return wait.Until(drv => drv.FindElement(by));
                }
                finally {
                    // Restore implicit timeout
                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(Factory.DefaultTimeoutValue));
                }
			}
			return driver.FindElement(by);
		}

        /// <summary>
        /// Waits for element attribute changed after action click.
        /// </summary>
        /// <param name="by">By.</param>
        /// <param name="attribute">Attribute.</param>
        /// <param name="elAction">El action.</param>
        /// <param name="timeoutInSeconds">Timeout in seconds.</param>
        public void WaitForElementAttributeChangedAfterActionClick(By by, string attribute, IWebElement elAction, double timeoutInSeconds = 0)
        {
            IWebDriver driver = Factory.Instance; 
            Console.WriteLine("WaitForElement by '{0}'", by.ToString());
            if (timeoutInSeconds > 0)
            {
                // Clear implicit timeout
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(0));
                try 
                {
                    string value = driver.FindElement(by).GetAttribute(attribute);
                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                    elAction.Clicks();
                    wait.Until(drv => (value != drv.FindElement(by).GetAttribute(attribute)));
                }
                finally {
                    // Restore implicit timeout
                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(Factory.DefaultTimeoutValue));
                }
            }           
        }

		/// <summary>
		/// Navigates to url.
		/// </summary>
		/// <param name="url">URL.</param>
		public void NavigateTo(string url, string LocalUrl="") 
		{
			IWebDriver driver = Factory.Instance;
            Console.WriteLine("NavigateTo url = '{0}'", url);
			driver.Navigate().GoToUrl(url);
		}

		/// <summary>
		/// Navigates to url.
		/// </summary>
		/// <param name="url">URL.</param>
		public void NavigateToSkipReadyForStatic(string url, string LocalUrl="") 
		{
			IWebDriver driver = Factory.Instance;
			Console.WriteLine("NavigateTo url = '{0}'", url);
			driver.Navigate().GoToUrl(url);
		}
            	
		/// <summary>
		/// Gets the inner text by identifier.
		/// </summary>
		/// <returns>The inner text by identifier.</returns>
		/// <param name="by">By.</param>
		/// <param name="timeout">Timeout.</param>
		public string GetText(By by, int timeout)
		{
			IWebElement el = null;
			IWebDriver driver = Factory.Instance;
			if (timeout > 0)
			{
				var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
				el = wait.Until(drv => drv.FindElement(by));
				return el.Text;
			}
			el = driver.FindElement(by);
			return el.Text;
		}

		/// <summary>
		/// Determines whether this instance is title valid the specified expected.
		/// </summary>
		/// <returns><c>true</c> if this instance is title valid the specified expected; otherwise, <c>false</c>.</returns>
		/// <param name="expected">Expected.</param>
		public bool IsTitleValid(string expected)
		{
			bool bFound = false;
            WebDriverWait wait = new WebDriverWait(Factory.Instance, TimeSpan.FromSeconds(10.00));
			wait.Until((d) =>
				{
					bFound = d.Title.ToLower().Contains(expected.ToLower());
					return bFound;
				});
			return bFound;
		}

		/// <summary>
		/// Gets the page title.
		/// </summary>
		/// <returns>The page title.</returns>
		public string GetPageTitle()
		{
			string title = string.Empty;
            WebDriverWait wait = new WebDriverWait(Factory.Instance, TimeSpan.FromSeconds(10.00));
			wait.Until((d) =>
				{
					title = d.Title;
					return title;
				});
			return title;
		}

        /// <summary>
        /// Waits for element visible.
        /// </summary>
        /// <returns>The for element visible.</returns>
        /// <param name="by">By.</param>
        /// <param name="timeoutInSeconds">Timeout in seconds.</param>
        public IWebElement WaitForElementVisible(By by, double timeoutInSeconds = 5.0)
        {
            IWebDriver driver = Factory.Instance;
            Console.WriteLine("WaitForElementVisible by '{0}'", by.ToString());
            if (timeoutInSeconds > 0)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(ExpectedConditions.ElementIsVisible(by));
            }
            return driver.FindElement(by);
        }           

        /// <summary>
        /// Waits for elements visible, specifically first element.
        /// </summary>
        /// <returns>The for elements visible.</returns>
        /// <param name="by">By.</param>
        /// <param name="timeoutInSeconds">Timeout in seconds.</param>
        public ReadOnlyCollection<IWebElement> WaitForElementsVisible(By by, double timeoutInSeconds = 5.0)
        {
            IWebDriver driver = Factory.Instance;
            ReadOnlyCollection<IWebElement> result = null;
            try {
                this.SetImplicitTimeout(0);
                var wait = WebDriverWaitWithMessage(driver, by, TimeSpan.FromSeconds(timeoutInSeconds));                   
                wait.Until( (drv) => {
                    ReadOnlyCollection<IWebElement> res = drv.FindElements(by);
                    if (res != null && res.Count > 0) { 
                        return res[0].Displayed;
                    }
                    return false; });
                result = driver.FindElements(by);
            }
            catch (WebDriverTimeoutException ex) 
            {
                Console.WriteLine(ex.ToString());
                // return empty result
                result = new ReadOnlyCollection<IWebElement>(new List<IWebElement>());
            }
            finally
            {
                this.SetImplicitTimeout(Factory.DefaultTimeoutValue);
            }
            // Return result
            return result;
        }           

        /// <summary>
        /// Waits for URL changed.
        /// </summary>
        /// <param name="url">URL.</param>
        public bool WaitForUrlChanged(string url, int timeout = 15)
        {
            IWebDriver driver = Factory.Instance;
            Console.WriteLine("WaitForUrlChanged current url '{0}'", url);
            bool fChanged = false;
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            try {
                fChanged = wait.Until((d) => {
                    Boolean b = (d.Url != url);
                    return b;
                });
            }
            finally {
                Console.WriteLine("Url before: '{0}' after: '{1}'", url, driver.Url);
            }
            return fChanged;
        }  

        /// <summary>
        /// Waits for URL changed.
        /// </summary>
        /// <param name="url">URL.</param>
        public bool WaitForUrlChanged(int timeout = 5)
        {
            IWebDriver driver = Factory.Instance;
            string url = driver.Url;
            Console.WriteLine("WaitForUrlChanged current url '{0}'", url);
            bool fChanged = false;
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            try {
                fChanged = wait.Until((d) => {
                    Boolean b = (d.Url != url);
                    return b;
                });
            }
            finally {
                Console.WriteLine("Url before: '{0}' after: '{1}'", url, driver.Url);
            }
            return fChanged;
        }  
                  
        /// <summary>
        /// Waits for URL contains.
        /// </summary>
        /// <param name="url">URL.</param>
        public void WaitForUrlContains(string url)
        {
            IWebDriver driver = Factory.Instance;
            Console.WriteLine("WaitForUrlContains '{0}'", url);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            wait.Until((d) => {
                Console.WriteLine("WaitForUrlContains: Url: '{0}' does contain: '{1}'", d.Url, url);
                Boolean b = (d.Url.ToLower().Contains(url.ToLower()));
                return b;
            });
        }  

        /// <summary>
        /// Waits for URL equals.
        /// </summary>
        /// <param name="url">URL.</param>
        public void WaitForUrlEquals(string url)
        {
            IWebDriver driver = Factory.Instance;
            Console.WriteLine("WaitForUrlEquals '{0}'", url);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            wait.Until((d) => {
                Console.WriteLine("WaitForUrlEquals: Url: '{0}' does contain: '{1}'", d.Url, url);
                Boolean b = (d.Url.ToLower().Equals(url.ToLower()));
                return b;
            });
        }  

        /// <summary>
        /// Finds the element.
        /// </summary>
        /// <returns>The element.</returns>
        /// <param name="by">By.</param>
        /// <param name="timeoutInSeconds">Timeout in seconds.</param>
        public IWebElement FindElement(By by)
        {
            IWebDriver driver = Factory.Instance;
            Console.WriteLine("FindElement by '{0}'", by.ToString());
            return driver.FindElement(by); 
        }
			
		// <summary>
		/// Finds the element and does not throw an exception.
		/// </summary>
		/// <param name="by">By.</param>
		public IWebElement FindElementNotThrow(By by)
		{
			try {
				IWebDriver driver = Factory.Instance;
				Console.WriteLine("FindElement by '{0}'", by.ToString());
				return driver.FindElement(by);
			} 
			catch (Exception) {
				return null;
			}
		}

        /// <summary>
        /// Finds the element.
        /// </summary>
        /// <returns>The element.</returns>
        /// <param name="by">By.</param>
        /// <param name="timeoutInSeconds">Timeout in seconds.</param>
        public ReadOnlyCollection<IWebElement> FindVisibleElements(By by, double timeoutInSeconds = 0.0)
        {
            IWebDriver driver = Factory.Instance;
            Console.WriteLine("FindVisibleElements by '{0}'", by.ToString());
            if (timeoutInSeconds > 0)
            {
                // Clear implicit timeout
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(0));
                try {                   
                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                    wait.Until(ExpectedConditions.ElementIsVisible(by));
                }
                finally {
                    // Restore implicit timeout
                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(Factory.DefaultTimeoutValue));
                }
                return driver.FindElements(by);
            }
            return driver.FindElements(by); 
        }

        /// <summary>
        /// Finds the elements.
        /// </summary>
        /// <returns>The elements.</returns>
        /// <param name="by">By.</param>
        /// <param name="timeoutInSeconds">Timeout in seconds.</param>
        public ReadOnlyCollection<IWebElement> FindElements(By by, double timeoutInSeconds = 0.0)
        {
            ReadOnlyCollection<IWebElement> results;
            Console.WriteLine("FindElements by '{0}'", by.ToString());
            //
            // Retry once more if timed-out
            //
            try {
                results = this._RetryFindELements(by, timeoutInSeconds);
            }
            catch (TimeoutException) {
                Console.WriteLine("FindElements timed-out, retrying again");
                results = this._RetryFindELements(by, timeoutInSeconds);
            }
            return results;
        }

        /// <summary>
        /// Find the elements.
        /// </summary>
        /// <returns>The find E lements.</returns>
        /// <param name="by">By.</param>
        /// <param name="timeoutInSeconds">Timeout in seconds.</param>
        private ReadOnlyCollection<IWebElement> _RetryFindELements(By by, double timeoutInSeconds)
        {            
            IWebDriver driver = Factory.Instance;
            if (timeoutInSeconds > 0)
            {
                // Clear implicit timeout
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(0));
                try 
                {
                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                    wait.Until((d) => {
                        Boolean b = (null != driver.FindElements(by));
                        return b;
                    });
                }
                finally {
                    // Restore implicit timeout
                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(Factory.DefaultTimeoutValue));
                }
                return driver.FindElements(by);
            }
            return driver.FindElements(by); 
        }

        /// <summary>
        /// Determines whether this instance is element in view port the specified el.
        /// </summary>
        /// <returns><c>true</c> if this instance is element in view port the specified el; otherwise, <c>false</c>.</returns>
        /// <param name="el">El.</param>
        public bool IsElementInViewPort(IWebElement el)
        {
            return Factory.IsElementInViewPort(el);
        }

		/// <summary>
		/// Waits for tests completed.
		/// </summary>
		/// <returns>The for tests completed.</returns>
		/// <param name="by">By.</param>
		/// <param name="timeout">Timeout.</param>
		public string WaitForTestsCompleted(By by, int timeout)
		{
			IWebDriver driver = Factory.Instance;
			if (timeout > 0)
			{
				var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
				wait.Until(d => d.FindElement(by).Text.Contains("Tests completed"));
				return driver.FindElement(by).Text;
			}
			return driver.FindElement(by).Text;
		}

        /// <summary>
        /// Web driver wait with message.
        /// </summary>
        /// <returns>Timeout exception message.</returns>
        /// <param name="by">By.</param>
        /// <param name="timeoutInSeconds">Timeout in seconds.</param>
        WebDriverWait WebDriverWaitWithMessage(IWebDriver driver, By by, TimeSpan timeoutInSeconds)
        {            
            var wait = new WebDriverWait(driver, timeoutInSeconds);
            wait.Message = string.Format("Waiting for '{0}'", by.ToString());
            return wait;
        }

		/// <summary>
		/// Switchs the web driver to new window.
		/// </summary>
		/// <returns><c>true</c>, if web driver to new window was switched, <c>false</c> otherwise.</returns>
		public bool SwitchWebDriverToNewWindow()
		{
			IWebDriver driver = Factory.Instance;
			bool bSwitched = false;
			//get the current window handles 
			string popupHandle = string.Empty; 
			ReadOnlyCollection<string> windowHandles = driver.WindowHandles;  
			string existingWindowHandle = driver.CurrentWindowHandle;
			foreach (string handle in windowHandles)  
			{  
				if (handle != existingWindowHandle)  
				{  
					popupHandle = handle; break;  
				}  
			}  

			//switch to new window if popupHandle is not empty (new)
			if (!String.IsNullOrWhiteSpace(popupHandle)) {
				driver.SwitchTo().Window(popupHandle); 
				bSwitched = true;
			}
			return bSwitched;
		}                  

		/// <summary>
		/// Captures the screen.
		/// </summary>
		/// <param name="fileName">File name.</param>
		public void CaptureScreen(string fileName = null)
		{
			IWebDriver driver = Factory.Instance;
			ITakesScreenshot screenShotDriver = driver as ITakesScreenshot;
			Screenshot screenShot = screenShotDriver.GetScreenshot();
            //Bitmap screenShot = this.GetEntereScreenshot();
            if (fileName != null) {
                var parentFolder = AppDomain.CurrentDomain.BaseDirectory;
                var folder = Path.Combine(parentFolder, DateTime.Now.ToString("dd-MM-yyyy"));
                if (!System.IO.Directory.Exists(folder)) {
                    Directory.CreateDirectory(folder);
                }
                fileName = string.Format("{0}/{1}.png", folder, fileName);
                Console.WriteLine("saving '{0}'", fileName);
                //screenShot.Save(fileName, ImageFormat.Png);
                screenShot.SaveAsFile(fileName, ImageFormat.Png);
            }
		}

        /// <summary>
        /// Captures the screen and put in subfolder.
        /// </summary>
        /// <param name="subfolder">Subfolder.</param>
        /// <param name="fileName">File name.</param>
        public void CaptureScreen(string subfolder, string fileName = null)
        {
            IWebDriver driver = Factory.Instance;
            ITakesScreenshot screenShotDriver = driver as ITakesScreenshot;
            Screenshot screenShot = screenShotDriver.GetScreenshot();
            //Bitmap screenShot = this.GetEntereScreenshot();
            if (subfolder != null && fileName != null) {
                var parentFolder = AppDomain.CurrentDomain.BaseDirectory;
                var folder = Path.Combine(parentFolder, DateTime.Now.ToString("dd-MM-yyyy"));
                var fullpath = folder + "/" + subfolder;
                if (!System.IO.Directory.Exists(fullpath)) {
                    Directory.CreateDirectory(fullpath);
                }
                var fullFileNamePath = string.Format("{0}/{1}.png", fullpath, fileName);
                Console.WriteLine("saving '{0}'", fullFileNamePath);
                //screenShot.Save(fileName, ImageFormat.Png);
                screenShot.SaveAsFile(fullFileNamePath, ImageFormat.Png);
            }
        }

        /// <summary>
        /// Gets the entere screenshot (Chrome Driver only get screen not entire document)
        /// </summary>
        /// <returns>The entere screenshot.</returns>
        public Bitmap GetEntereScreenshot()
        {

            Bitmap stitchedImage = null;
            IWebDriver _driver = Factory.Instance;
            try
            {
                long totalwidth1 = (long)((IJavaScriptExecutor)_driver).ExecuteScript("return document.body.offsetWidth");//documentElement.scrollWidth");

                long totalHeight1 = (long)((IJavaScriptExecutor)_driver).ExecuteScript("return  document.body.parentNode.scrollHeight");

                int totalWidth = (int)totalwidth1;
                int totalHeight = (int)totalHeight1;

                // Get the Size of the Viewport
                long viewportWidth1 = (long)((IJavaScriptExecutor)_driver).ExecuteScript("return document.body.clientWidth");//documentElement.scrollWidth");
                long viewportHeight1 = (long)((IJavaScriptExecutor)_driver).ExecuteScript("return window.innerHeight");//documentElement.scrollWidth");

                int viewportWidth = (int)viewportWidth1;
                int viewportHeight = (int)viewportHeight1;


                // Split the Screen in multiple Rectangles
                List<Rectangle> rectangles = new List<Rectangle>();
                // Loop until the Total Height is reached
                for (int i = 0; i < totalHeight; i += viewportHeight)
                {
                    int newHeight = viewportHeight;
                    // Fix if the Height of the Element is too big
                    if (i + viewportHeight > totalHeight)
                    {
                        newHeight = totalHeight - i;
                    }
                    // Loop until the Total Width is reached
                    for (int ii = 0; ii < totalWidth; ii += viewportWidth)
                    {
                        int newWidth = viewportWidth;
                        // Fix if the Width of the Element is too big
                        if (ii + viewportWidth > totalWidth)
                        {
                            newWidth = totalWidth - ii;
                        }

                        // Create and add the Rectangle
                        Rectangle currRect = new Rectangle(ii, i, newWidth, newHeight);
                        rectangles.Add(currRect);
                    }
                }

                // Build the Image
                stitchedImage = new Bitmap(totalWidth, totalHeight);
                // Get all Screenshots and stitch them together
                Rectangle previous = Rectangle.Empty;
                foreach (var rectangle in rectangles)
                {
                    // Calculate the Scrolling (if needed)
                    if (previous != Rectangle.Empty)
                    {
                        int xDiff = rectangle.Right - previous.Right;
                        int yDiff = rectangle.Bottom - previous.Bottom;
                        // Scroll
                        //selenium.RunScript(String.Format("window.scrollBy({0}, {1})", xDiff, yDiff));
                        ((IJavaScriptExecutor)_driver).ExecuteScript(String.Format("window.scrollBy({0}, {1})", xDiff, yDiff));
                        System.Threading.Thread.Sleep(200);
                    }

                    // Take Screenshot
                    var screenshot = ((ITakesScreenshot)_driver).GetScreenshot();

                    // Build an Image out of the Screenshot
                    Image screenshotImage;
                    using (MemoryStream memStream = new MemoryStream(screenshot.AsByteArray))
                    {
                        screenshotImage = Image.FromStream(memStream);
                    }

                    // Calculate the Source Rectangle
                    Rectangle sourceRectangle = new Rectangle(viewportWidth - rectangle.Width, viewportHeight - rectangle.Height, rectangle.Width, rectangle.Height);

                    // Copy the Image
                    using (Graphics g = Graphics.FromImage(stitchedImage))
                    {
                        g.DrawImage(screenshotImage, rectangle, sourceRectangle, GraphicsUnit.Pixel);
                    }

                    // Set the Previous Rectangle
                    previous = rectangle;
                }
            }
            catch (Exception)
            {
                // handle
            }
            return stitchedImage;
        }

        /// <summary>
        /// Captures image for given element
        /// </summary>
        /// <param name="element">Element.</param>
        public void CaptureScreen(IWebElement element, string filename)
        {
            IWebDriver driver = Factory.Instance;
            Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            screenshot.SaveAsFile(@"file.bmp", ImageFormat.Bmp);
            Bitmap FullScreen = new Bitmap( @"file.bmp");
            Size element_size = element.Size;
            Point element_location = element.Location;
            Rectangle rect = new Rectangle(element_location, element_size);
            Bitmap Image = FullScreen.Clone(rect, PixelFormat.Format24bppRgb);
            Image.Save(filename, ImageFormat.Bmp);
        }

        /// <summary>
        /// Scrolls to element.
        /// </summary>
        /// <param name="element">Element.</param>
        public void ScrollToElement(IWebElement element)
        {
            ((IJavaScriptExecutor)Factory.Instance).ExecuteScript("arguments[0].scrollIntoView(true);", element);
        }


        /// <summary>
        /// Executes the javascript.
        /// </summary>
        /// <param name="script">Script.</param>
        public void ExecuteJavascript(string script)
        {
            ((IJavaScriptExecutor)Factory.Instance).ExecuteScript(script);
        }

        /// <summary>
        /// Sets the element by identifier attribute.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <param name="attribute">Attribute.</param>
        /// <param name="value">Value.</param>
        public void SetElementAttribute(string selector, string attribute, string value)
        {
            IWebDriver driver = Factory.Instance;
            IJavaScriptExecutor js = ((IJavaScriptExecutor)Factory.Instance);
            js.ExecuteScript("document.querySelector('" + selector + "').setAttribute('" + attribute + "', '" + value + "')");
        }
            
        /// <summary>
        /// Sets the element attribute.
        /// </summary>
        /// <param name="elm">Elm.</param>
        /// <param name="attribute">Attribute.</param>
        /// <param name="value">Value.</param>
        public void SetElementAttribute(IWebElement elm, string attribute, string value)
        {
            IWebDriver driver = Factory.Instance;
            IJavaScriptExecutor js = ((IJavaScriptExecutor)Factory.Instance);
            js.ExecuteScript("arguments[0].setAttribute(arguments[1], arguments[2]);", elm, attribute, value);
        }

        /// <summary>
        /// Currents the scroll position.
        /// </summary>
        /// <returns>The scroll position.</returns>
        public long CurrentScrollPosition()
        {
            long pos = (long) ((IJavaScriptExecutor)Factory.Instance).ExecuteScript("return window.scrollY;");
            return pos;
        }

        /// <summary>
        /// Converts the URL to filename.
        /// </summary>
        /// <returns>The URL to filename.</returns>
        /// <param name="url">URL.</param>
        public string ConvertUrlToFilename(string href)
        {
            string filename = string.Empty;
            string encoded = 
                System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(href));           
            filename = encoded;
            return filename;
        }

        /// <summary>
        /// Generates the name from URL.
        /// </summary>
        /// <returns>The name from URL.</returns>
        /// <param name="url">URL.</param>
        public string GenerateNameFromUrl(string url)
        {
            // Replace useless chareacters with UNDERSCORE
            String uniqueName = url.Replace("://", "_").Replace(".", "_").Replace("/", "_");
            return uniqueName;
        }

        /// <summary>
        /// Performs the click.
        /// </summary>
        /// <param name="element">Element.</param>
        public void PerformClick(IWebElement element)
        {
            Actions act = new Actions(Factory.Instance);
            Console.WriteLine("PerformClick '{0}'", element.Text);
            act.Click(element);
            act.Perform();
        }
            
		/// <summary>
		/// Quit this instance/BrowserStack session.
		/// </summary>
		public void Quit()
		{
			Factory.Instance.Quit();
		}

        #region Navigation
        /// <summary>
        /// Forward this instance.
        /// </summary>
        public void Forward() 
        {
            this.IWebDriver.Navigate().Forward();
        }

        /// <summary>
        /// Back this instance.
        /// </summary>
        public void Back()
        {
            this.IWebDriver.Navigate().Back();
        }

        /// <summary>
        /// Refresh this page.
        /// </summary>
        public void Refresh()
        {
            this.IWebDriver.Navigate().Refresh();
        }
        #endregion

#if __DEBUG__
		public void DumpAllElements()
		{
			IWebDriver driver = Factory.Instance;
			string []tagNames = { "input", "button", "select", "a" };

			for (int i = 0; i < tagNames.Length; i++) {
				ReadOnlyCollection<IWebElement> arrayElements = driver.FindElements(By.TagName(tagNames[i]));            
			}
		}
#endif
	} 
}

