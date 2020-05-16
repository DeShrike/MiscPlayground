namespace MyCurl
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;

    public class Curl
    {
        #region WebClient
        private static System.IO.Stream GetResponseStream(string url)
        {
            using (var wclient = new WebClient())
            {
                return wclient.OpenRead(url);
            }
        }

        public static string DoGetWebClient(string url)
        {
            string output = "NULL";
            using (var wclient = new WebClient())
            {
                byte[] arr = wclient.DownloadData(url);
                output = System.Text.Encoding.UTF8.GetString(arr);
            }

            return output;
        }

        #endregion

        #region HttpClient

        static HttpClient client = new HttpClient();

        public static async Task<string> GetAsync(string baseAddress, string path, string content)
        {
            var u = new Uri(baseAddress + path + "?" + content);
            ////HttpResponseMessage response = await client.GetAsync(baseAddress+path);
            HttpResponseMessage response = await client.GetAsync(u);
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                return result;
            }

            return response.StatusCode.ToString();
        }

        public static async Task<string> PostAsync(string baseAddress, string path, string content)
        {
            var u = new Uri(baseAddress + path);
            ////HttpResponseMessage response = await client.GetAsync(baseAddress+path);
            HttpResponseMessage response = await client.PostAsync(u, new StringContent(content));
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                return result;
            }

            return response.StatusCode.ToString();
        }


        public static async Task<string> RunCode(string baseAddress, string path, string content)
        {
            //client.BaseAddress = new Uri(baseAddress);
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                string output = await GetAsync(baseAddress, path, content);
                ////string output = await PostAsync(baseAddress, path, content);
                return output;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return "NULL";
        }

        #endregion

        #region HttpWebRequest

        public string DoGet(string url, string body, ref string location)
        {
            bool ret = false;
            location = string.Empty;

            Console.WriteLine(string.Empty);
            Console.WriteLine("*****************************");
            Console.WriteLine("DoGet: {0} {1}", url, body);

            var urlx = url.Replace("|", "%7c");
            Uri uri = new Uri(urlx);
            if (uri.UserEscaped)
            {
                Console.WriteLine("UserEscaped");
            }
            // Uri uri = new Uri(Uri.EscapeUriString(url));

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri + body);
            webRequest.Method = WebRequestMethods.Http.Get;
            this.SetRequestParameters(webRequest, url);

            //webRequest.ContentType = "application/x-www-form-urlencoded";

            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(this.ValidateRemoteCertificate);
            string output = "";
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)webRequest.GetResponse();
                string cookies = response.GetResponseHeader("Set-Cookie");
                StreamReader reader = new StreamReader(response.GetResponseStream());
                output = reader.ReadToEnd();
                response.Close();

                // this.SessionId = this.ExtractCookie(cookies, "JSESSIONID");
                this.ProcessResponse(response, url);
                //this.ExtractAllCookies(cookies, uri);
                location = response.GetResponseHeader("Location");

                Console.WriteLine(string.Empty);
                Console.WriteLine("Redirect to: {0}", location);

                ret = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                output = null;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }

            return output;
        }

        private bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            Console.WriteLine("ValidateRemoteCertificate {0} Errors: {1}", certificate.Subject, policyErrors.ToString());
            return true;
        }

        private void SetRequestParameters(HttpWebRequest webRequest, string url)
        {
            // this.AddCookies(webRequest, this.theCookies, url);ICI
            //CookieHelper.AddCookies(webRequest, url);

            // webRequest.ClientCertificates.Add(this.certificate);
            
            webRequest.Accept = "text/html, image/jpeg, image/gif, image/pjpeg, application/x-ms-application, application/xaml+xml, application/x-ms-xbap, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
            ////webRequest.KeepAlive = true;
            webRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/5.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0C; .NET4.0E)";
            ////webRequest.Headers.Add("Accept-Language", "nl-BE");
            webRequest.SendChunked = false;
            webRequest.ReadWriteTimeout = 60000;
            webRequest.Timeout = 60000;
            webRequest.ContentType = "text/html; charset=utf-8";
            ////webRequest.ProtocolVersion = HttpVersion.Version11;
            webRequest.AllowAutoRedirect = false;
            ////webRequest.ConnectionGroupName = "A";
        }

        private void ProcessResponse(HttpWebResponse response, string uri)
        {
            string cookies = response.GetResponseHeader("Set-Cookie");
            if (!string.IsNullOrEmpty(cookies))
            {
                CookieHelper.ExtractAllCookies(cookies, uri);
                // this.ExtractAllCookies(cookies, uri);ICI
            }

            Console.WriteLine("***** {0} {1}", response.StatusCode, response.StatusDescription);
            // Console.WriteLine("***** Content-Length {0}", response.ContentLength);
            // Console.WriteLine("***** Content-Type {0}", response.ContentType);

            string location = response.GetResponseHeader("Location");
            if (!string.IsNullOrEmpty(location))
            {
                Console.WriteLine("***** Location {0}", location);
            }
        }

        #endregion
    }
}
