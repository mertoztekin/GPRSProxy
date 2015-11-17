using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;

namespace TalabatIfoodGPRSProxy
{
    public static class TalabatIfoodGPRSProxy
    {
        /// <summary>
        /// Url to be used to rewrite
        /// </summary>
        private static string OriginUrl
        {
            get
            {
                if (WebConfigurationManager.AppSettings["iFoodGPRSOriginUrl"] != null)
                {
                    return WebConfigurationManager.AppSettings["iFoodGPRSOriginUrl"];
                }
                return "http://messaging.ifood.jo/MessagingWebServiceTest/GprsPrinter/";
            }
        } 


        private static  ILogger _logger=null;
        public static ILogger Logger
        {
            get { return _logger; }
            set
            {
                _logger = value;
                iFoodGprsDeviceRepository.Logger = _logger;
            }
        }

        /// <summary>
        /// Main method
        /// Checks if terminal ifood if yes then send requests to ifood servers and Rewrite the response and halts.
        /// </summary>
        public static void ExecuteProxy()
        {
            try
            {
                var gprsRequest = new GprsRequest(HttpContext.Current.Request);
                if (IsTerminalIdIfood(gprsRequest))
                {
                    LoggingHelper.Debug(Logger, "Request is iFood : " + gprsRequest);

                    var webResponse = MakeProxyRequest(gprsRequest);

                    FinalizeCurrentRequest(webResponse);
                }
                else
                {
                    LoggingHelper.Debug(Logger, "Request is not iFood : " + gprsRequest);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Cannot execute proxy. " + ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// Finalizes current request with response taken from ifood servers
        /// </summary>
        /// <param name="webResponse"></param>
        private static void FinalizeCurrentRequest(HttpWebResponse webResponse)
        {
            var currentResponse = HttpContext.Current.Response;
            currentResponse.Clear();
            foreach (var headerKey in webResponse.Headers.AllKeys)
            {
                currentResponse.Headers.Add(headerKey, webResponse.Headers[headerKey]);
            }
            currentResponse.ContentType = webResponse.ContentType;
            currentResponse.StatusCode = int.Parse(webResponse.StatusCode.ToString("D"));
            webResponse.GetResponseStream().CopyTo(currentResponse.OutputStream);

            LoggingHelper.Debug(Logger, "Request Finalized with total length:"+webResponse.ContentLength);
            currentResponse.End();
        }

        /// <summary>
        /// Generates request to ifood servers
        /// </summary>
        /// <param name="gprsRequest"></param>
        /// <returns></returns>
        private static HttpWebResponse MakeProxyRequest(GprsRequest gprsRequest)
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(RewriteUrl(gprsRequest.Url));
            request.UserAgent = "Talabat Proxy";
            var webResponse = (HttpWebResponse) request.GetResponse();
            return webResponse;
        }

        /// <summary>
        /// Checks original request and changes with new url
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private static string RewriteUrl(Uri uri)
        {
            if (uri.AbsoluteUri.ToLower().Contains("order.aspx"))
            {
                return OriginUrl+"Order.aspx"+ uri.Query;
            }
            return OriginUrl + "Callback.aspx" + uri.Query;
        }

        /// <summary>
        /// Checks if request is done by ifood device
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static bool IsTerminalIdIfood(GprsRequest request)
        {
            if (null == request || request.TerminalId <= 0)
            {
                return false;
            }
            return iFoodGprsDeviceRepository.CheckIsIfood(request.TerminalId);
        }



    }
}
