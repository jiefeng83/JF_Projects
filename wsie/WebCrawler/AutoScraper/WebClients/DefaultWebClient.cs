using System;
using System.Net;
using AutoScraper.Interfaces;

namespace AutoScraper.WebClients {
    public class DefaultWebClient : WebClient, IWebClient {
        public DefaultWebClient() {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            ServicePointManager.DefaultConnectionLimit = Int32.MaxValue;
            ServicePointManager.Expect100Continue = false;
        }

        protected override WebRequest GetWebRequest(Uri address) {
            var request = base.GetWebRequest(address) as HttpWebRequest;

            if (request != null) {
                request.Proxy = null;
                request.Timeout = 30000;
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            }

            return request;
        }
    }
}