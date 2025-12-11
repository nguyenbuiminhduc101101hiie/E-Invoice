using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Security;
using System.IO;

namespace InvoiceClient
{
    public class CreateRequest
    {
        public static string webRequestV2(string pzUrl, string pzData, string accessToken, string pzMethod, string pzContentType, string proxyIP, int port, int ssl)
        {
            try
            {
                if (ssl == 1)
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                           | SecurityProtocolType.Tls11
                                                           | SecurityProtocolType.Tls12
                                                           | SecurityProtocolType.Ssl3;
                }
                var httpWebRequest = WebRequest.Create(pzUrl);                
                httpWebRequest.ContentType = pzContentType;
                httpWebRequest.Method = pzMethod;
                httpWebRequest.Headers.Add("Cookie", accessToken);
                httpWebRequest.Timeout = 30000;

                if (!string.IsNullOrEmpty(proxyIP))
                {
                    WebProxy proxy = new WebProxy(proxyIP, port);
                    httpWebRequest.Proxy = proxy;
                }
                //InitiateSSLTrust();//bypass SSL
                

                if (!string.IsNullOrEmpty(pzData))
                {
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string json = pzData;

                        streamWriter.Write(json);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }
                var httpResponse = httpWebRequest.GetResponse();
                var result = string.Empty;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
                return result;
            }
            catch (WebException ex)
            {
                string errorText = "";
                using (var errorResponse = (HttpWebResponse)ex.Response)
                {
                    using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                    {
                        errorText = reader.ReadToEnd();

                    }
                }
                return errorText;
            }
        }
        public static string webRequestgetToken(string pzUrl, string pzData, string pzMethod, string pzContentType, string proxyIP, int port, int ssl)
        {
            try
            {
                //InitiateSSLTrust();//bypass SSL
                if (ssl == 1)
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                           | SecurityProtocolType.Tls11
                                                           | SecurityProtocolType.Tls12
                                                           | SecurityProtocolType.Ssl3;
                }
                var httpWebRequest = WebRequest.Create(pzUrl);
                httpWebRequest.ContentType = pzContentType;
                httpWebRequest.Method = pzMethod;
                httpWebRequest.Timeout = 30000;

                if (!string.IsNullOrEmpty(proxyIP))
                {
                    WebProxy proxy = new WebProxy(proxyIP, port);
                    httpWebRequest.Proxy = proxy;
                }
                
                if (!string.IsNullOrEmpty(pzData))
                {
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string json = pzData;

                        streamWriter.Write(json);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }
               
                var httpResponse = httpWebRequest.GetResponse();
                var result = string.Empty;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
                return result;
            }
            catch (Exception ex)
            {
                return "NOK" + ex.Message;
            }
        }
        public static string webRequest(string pzUrl, string pzData, string pzAuthorization, string pzMethod, string pzContentType)
        {

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(pzUrl);
            httpWebRequest.ContentType = pzContentType;
            httpWebRequest.Method = pzMethod;
            httpWebRequest.Headers.Add("Authorization", "Basic " + pzAuthorization);
            httpWebRequest.Proxy = new WebProxy();//no proxy

            if (!string.IsNullOrEmpty(pzData))
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = pzData;

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }
            InitiateSSLTrust();//bypass SSL
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            var result = string.Empty;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            return result;

        }
        public static void InitiateSSLTrust()
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback =
                   new RemoteCertificateValidationCallback(
                        delegate
                        { return true; }
                    );
            }
            catch (Exception ex)
            {

            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
