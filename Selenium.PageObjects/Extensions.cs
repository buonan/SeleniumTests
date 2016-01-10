using System;
using System.Web;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using OpenQA.Selenium;
using RestSharp.Extensions.MonoHttp;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace Selenium.PageObjects
{
    /// <summary>
    /// Web element extensions.
    /// </summary>
    public static class WebElementExtensions
    {   
        /// <summary>
        /// Clicks the link.
        /// </summary>
        /// <param name="me">Me.</param>
        /// <param name="subDomain">Sub domain.</param>
        static void ClickLink(this IWebElement me, string subDomain = null, bool addParams = false)
        {
            //
            // We check the href on me and modify it before clicking (ie. www.google.com -> stage.google.com).
            //
            string href = me.GetAttribute("href");
            Console.WriteLine("ClickLink before '{0}'", href);
            if (subDomain != null) {                
                if (href != null) {
                    Uri uri = new Uri(href);
                    string oldSubDomain = uri.Host.Split('.')[0];
                    string[] notReplaceSubDomains = new string[] { "secure" };
                    if (!href.Contains("#") && !(oldSubDomain.Equals(subDomain)) && !notReplaceSubDomains.Contains(oldSubDomain)) {
                        Console.WriteLine("Replacing subdomain '{0}' with '{1}'", oldSubDomain, subDomain);
                        string newHref = href.Replace(oldSubDomain, subDomain);
                        if (addParams) {                            
                            newHref = HttpExtensions.AddQuery(newHref, "param", "value");
                        }
                        IWebDriver driver = Factory.Instance;
                        IJavaScriptExecutor js = ((IJavaScriptExecutor)Factory.Instance);
                        js.ExecuteScript("arguments[0].setAttribute(arguments[1], arguments[2]);", me, "href", newHref);
                    }
                }
            }
            href = me.GetAttribute("href");
            Console.WriteLine("ClickLink after '{0}'", href);
            me.Click();
        }

        /// <summary>
        /// Clicks the specified me and subDomain.
        /// </summary>
        /// <param name="me">Me.</param>
        /// <param name="subDomain">Sub domain.</param>
        public static void Clicks(this IWebElement me, string subDomain = null)
        {   
            // If not specified use default
            if (subDomain == null) {
                subDomain = Globals.SubDomain;
            }
            WebElementExtensions.ClickLink(me, subDomain);
        }

        /// <summary>
        /// Custom set text method (example how to extend Set method).
        /// </summary>
        /// <param name="me">Me.</param>
        /// <param name="text">Text.</param>
        public static void Set(this IWebElement me, string text)
        {
            me.Clear();
            me.SendKeys(text);
        }

        /// <summary>
        /// Goto a URL called by NavigateTo().
        /// </summary>
        /// <param name="url">URL.</param>
        public static void GoToUrl(this string url)
        {
            IWebDriver driver = Factory.Instance;
            driver.Navigate().GoToUrl(url);
        }

        /// <summary>
        /// Hover the specified element.
        /// </summary>
        /// <param name="me">Me.</param>
        public static void Hover(this IWebElement me)
        {
            Actions act = new Actions(Factory.Instance);
            act.MoveToElement(me);
            act.Perform();
        }       
    }

    /// <summary>
    /// Http extensions.
    /// </summary>
    public static class HttpExtensions
    {
        /// <summary>
        /// Adds the query.
        /// </summary>
        /// <returns>The query.</returns>
        /// <param name="uri">URI.</param>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        public static string AddQuery(string currentPageUrl, string paramToReplace, string newValue)
        {
            string urlWithoutQuery = currentPageUrl.IndexOf('?') >= 0 
                ? currentPageUrl.Substring(0, currentPageUrl.IndexOf('?')) 
                : currentPageUrl;

            string queryString = currentPageUrl.IndexOf('?') >= 0
                ? currentPageUrl.Substring(currentPageUrl.IndexOf('?')) 
                : null;

            var queryParamList = queryString != null 
                ? HttpUtility.ParseQueryString(queryString) 
                : HttpUtility.ParseQueryString(string.Empty);

            if (queryParamList[paramToReplace] != null)
            {
                queryParamList[paramToReplace] = newValue;
            }
            else
            {
                queryParamList.Add(paramToReplace, newValue);
            }
            string qs = ToQueryString(queryParamList);
            return String.Format("{0}?{1}", urlWithoutQuery, qs);
        }

        /// <summary>
        /// Tos the query string.
        /// </summary>
        /// <returns>The query string.</returns>
        /// <param name="nvc">Nvc.</param>
        private static string ToQueryString(NameValueCollection nvc)
        {
            var array = (from key in nvc.AllKeys
                from value in nvc.GetValues(key)
                select string.Format("{0}={1}", key, value))
                .ToArray();
            return string.Join("&", array);
        }
    }
}
