using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace MyCurl
{
    public static class CookieHelper
    {
        public static List<System.Net.Cookie> theCookies;

        static CookieHelper()
        {
            theCookies = new List<System.Net.Cookie>();
        }

        public static void AddCookies(HttpWebRequest req, string uri)
        {
            Uri url = new Uri(uri);
            string host = url.Host;

            string cookies = "";

            Console.WriteLine("### Adding cookies for [{0}]", uri);
            Console.WriteLine("### Host [{0}]", host);

            foreach (System.Net.Cookie k in theCookies)
            {
                // Console.WriteLine("### ?? cookie: {0}={1} D: [{2}] P: [{3}]", k.Name, k.Value.LimitLength(50), k.Domain, k.Path);
                if (host.Contains(k.Domain) && uri.Contains(k.Path))
                {
                    if (!string.IsNullOrEmpty(cookies))
                    {
                        cookies += "; ";
                    }

                    cookies += k.Name + "=" + k.Value;
                    //// Console.WriteLine("### Sending cookie: {0}={1}", k.Name, k.Value.LimitLength(50));
                }
            }

            if (string.IsNullOrEmpty(cookies) == false)
            {
                req.Headers.Add("Cookie", cookies);
            }
        }

        public static void ExtractAllCookies(string cookies, string uri)
        {
            Uri url = new Uri(uri);
            CookieCollection cc = GetAllCookiesFromHeader(cookies, url.Host);
            // nu merge

            foreach (System.Net.Cookie k in cc)
            {
                var oo = theCookies.FirstOrDefault(_ => _.Domain.Equals(k.Domain) && _.Name.Equals(k.Name) && _.Path.Equals(k.Path));
                if (oo == null)
                {
                    ////Console.WriteLine("NEW Cookie: {0} = {1} Domain {2} Path {3}", k.Name, k.Value.LimitLength(50), k.Domain, k.Path);
                    theCookies.Add(k);
                }
                else
                {
                    ////Console.WriteLine("OLD Cookie: {0} = {1} Domain {2} Path {3}", k.Name, k.Value.LimitLength(50), k.Domain, k.Path);
                    oo.Value = k.Value;
                }
            }
        }

        public static CookieCollection GetAllCookiesFromHeader(string strHeader, string strHost)
        {
            ArrayList al = new ArrayList();
            CookieCollection cc = new CookieCollection();
            if (strHeader != string.Empty)
            {
                al = ConvertCookieHeaderToArrayList(strHeader);
                cc = ConvertCookieArraysToCookieCollection(al, strHost);
            }

            return cc;
        }

        private static ArrayList ConvertCookieHeaderToArrayList(string strCookHeader)
        {
            strCookHeader = strCookHeader.Replace("\r", "");
            strCookHeader = strCookHeader.Replace("\n", "");
            string[] strCookTemp = strCookHeader.Split(',');
            ArrayList al = new ArrayList();
            int i = 0;
            int n = strCookTemp.Length;
            while (i < n)
            {
                if (strCookTemp[i].IndexOf("expires=", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    al.Add(strCookTemp[i] + "," + strCookTemp[i + 1]);
                    i = i + 1;
                }
                else
                {
                    al.Add(strCookTemp[i]);
                }

                i = i + 1;
            }

            return al;
        }

        private static CookieCollection ConvertCookieArraysToCookieCollection(ArrayList al, string strHost)
        {
            CookieCollection cc = new CookieCollection();

            int alcount = al.Count;
            string strEachCook;
            string[] strEachCookParts;
            for (int i = 0; i < alcount; i++)
            {
                strEachCook = al[i].ToString();
                strEachCookParts = strEachCook.Split(';');
                int intEachCookPartsCount = strEachCookParts.Length;
                string strCNameAndCValue = string.Empty;
                string strPNameAndPValue = string.Empty;
                string strDNameAndDValue = string.Empty;
                System.Net.Cookie cookTemp = new System.Net.Cookie();

                for (int j = 0; j < intEachCookPartsCount; j++)
                {
                    if (j == 0)
                    {
                        strCNameAndCValue = strEachCookParts[j];
                        if (strCNameAndCValue != string.Empty)
                        {
                            int firstEqual = strCNameAndCValue.IndexOf("=");
                            string firstName = strCNameAndCValue.Substring(0, firstEqual);
                            string allValue = strCNameAndCValue.Substring(firstEqual + 1, strCNameAndCValue.Length - (firstEqual + 1));
                            cookTemp.Name = firstName;
                            cookTemp.Value = allValue;
                        }

                        continue;
                    }

                    if (strEachCookParts[j].IndexOf("path", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        strPNameAndPValue = strEachCookParts[j];
                        if (strPNameAndPValue != string.Empty)
                        {
                            var nameValuePairTemp = strPNameAndPValue.Split('=');
                            if (nameValuePairTemp[1] != string.Empty)
                            {
                                cookTemp.Path = nameValuePairTemp[1].Trim();
                            }
                            else
                            {
                                cookTemp.Path = "/";
                            }
                        }

                        continue;
                    }

                    if (strEachCookParts[j].IndexOf("domain", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        strPNameAndPValue = strEachCookParts[j];
                        if (strPNameAndPValue != string.Empty)
                        {
                            var nameValuePairTemp = strPNameAndPValue.Split('=');

                            if (nameValuePairTemp[1] != string.Empty)
                            {
                                cookTemp.Domain = nameValuePairTemp[1].Trim();
                            }
                            else
                            {
                                cookTemp.Domain = strHost;
                            }
                        }

                        continue;
                    }
                }

                if (cookTemp.Path == string.Empty)
                {
                    cookTemp.Path = "/";
                }

                if (cookTemp.Domain == string.Empty)
                {
                    cookTemp.Domain = strHost;
                }

                cc.Add(cookTemp);
            }

            return cc;
        }
    }
}
