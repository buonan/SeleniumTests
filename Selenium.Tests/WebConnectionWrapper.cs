using System;
using System.IO;
using System.Linq;
using System.Configuration;
using com.gargoylesoftware.htmlunit;
using com.gargoylesoftware.htmlunit.html;
using NUnit.Framework;
using NUnit_retry;
using Selenium.PageObjects;

namespace Selenium.Tests
{
    /// <summary>
    /// Web connection wrapper.
    /// </summary>
    public class ForumsWebConnectionWrapper : HttpWebConnection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Selenium.Tests.HtmlUnitTests+WebConnectionWrapper"/> class.
        /// </summary>
        /// <param name="webclient">Webclient.</param>
        public ForumsWebConnectionWrapper(WebClient webclient) : base(webclient)
        {
        }

        /// <summary>
        /// Gets the response and modify the headers.
        /// </summary>
        /// <returns>The response.</returns>
        /// <param name="request">Request.</param>
        public override WebResponse getResponse(WebRequest request)
        {
            WebResponse response = base.getResponse(request);
            //
            // Only if Url matches
            //
            if (request.getUrl().toExternalForm().Contains("mb.aspx")) {
                string content = response.getContentAsString("UTF-8");
                java.util.ArrayList newheaders = new java.util.ArrayList();
                java.util.List headers = response.getResponseHeaders();
                java.util.Iterator it = headers.iterator();
                //
                // Remove the 'Access-Control-Allow-Origin' header
                //
                while (it.hasNext()) {
                    com.gargoylesoftware.htmlunit.util.NameValuePair o = (com.gargoylesoftware.htmlunit.util.NameValuePair)it.next();
                    if (o.getName().Equals("Access-Control-Allow-Origin")) {
                        string value = response.getResponseHeaderValue("Access-Control-Allow-Origin");
                        Console.WriteLine("Found header 'Access-Control-Allow-Origin' = \"{0}\" and stripping it from new headers for response", value);
                        continue; //headers.remove(o);
                    }
                    newheaders.add(o);
                }
                byte[] utf = System.Text.Encoding.UTF8.GetBytes(content);
                WebResponseData data = new WebResponseData(utf,
                    response.getStatusCode(), response.getStatusMessage(), newheaders);
                response = new WebResponse(data, request, response.getLoadTime());
                return response;
            }
            return response;
        }
    }

    /// <summary>
    /// Web connection wrapper.
    /// </summary>
    public class FrontWebConnectionWrapper : HttpWebConnection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Selenium.Tests.HtmlUnitTests+WebConnectionWrapper"/> class.
        /// </summary>
        /// <param name="webclient">Webclient.</param>
        public FrontWebConnectionWrapper(WebClient webclient) : base(webclient)
        {
        }

        /// <summary>
        /// Gets the response and modify the headers.
        /// </summary>
        /// <returns>The response.</returns>
        /// <param name="request">Request.</param>
        public override WebResponse getResponse(WebRequest request)
        {
            WebResponse response = base.getResponse(request);
            //
            // Only if Url matches
            //
            if (request.getUrl().toExternalForm().Contains("com")) {
                string content = response.getContentAsString("UTF-8");
                java.util.ArrayList newheaders = new java.util.ArrayList();
                java.util.List headers = response.getResponseHeaders();
                java.util.Iterator it = headers.iterator();
                //
                // Remove the 'Access-Control-Allow-Origin' header
                //
                while (it.hasNext()) {
                    com.gargoylesoftware.htmlunit.util.NameValuePair o = (com.gargoylesoftware.htmlunit.util.NameValuePair)it.next();
                    if (o.getName().Equals("Access-Control-Allow-Origin")) {
                        string value = response.getResponseHeaderValue("Access-Control-Allow-Origin");
                        Console.WriteLine("Found header 'Access-Control-Allow-Origin' = \"{0}\" and stripping it from new headers for response", value);
                        continue; //headers.remove(o);
                    }
                    newheaders.add(o);
                }
                byte[] utf = System.Text.Encoding.UTF8.GetBytes(content);
                WebResponseData data = new WebResponseData(utf,
                    response.getStatusCode(), response.getStatusMessage(), newheaders);
                response = new WebResponse(data, request, response.getLoadTime());
                return response;
            }
            return response;
        }
    }
}

