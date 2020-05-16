namespace MyCurl
{
    using System;

    public class Program
    {
        static string url = "http://eshop.cebeo.be:29501/webservice/netpriceh?customercode=229274&products=169506,100|4045121,1|4147177,1|125990,10";

        static string baseAddress = "http://eshop.cebeo.be:29501";
        static string path = "/webservice/netpriceh";
        static string content = "customercode=229274&products=169506,100|4045121,1|4147177,1|125990,10";

        static void Main(string[] args)
        {
            Console.WriteLine("Url: {0}", url);

            TryHttpClient();
            //TryWebRequest();
            // TryWebClient();

            Console.WriteLine("Press <enter>");
            Console.ReadLine();
        }

        static void TryWebClient()
        {
            string output = Curl.DoGetWebClient(url);
            Console.WriteLine("[OUTPUT]");
            Console.WriteLine(output);
            Console.WriteLine("[/OUTPUT]");
        }

        static void TryHttpClient()
        {
            string output = Curl.RunCode(baseAddress, path, content).GetAwaiter().GetResult();
            Console.WriteLine("[OUTPUT]");
            Console.WriteLine(output);
            Console.WriteLine("[/OUTPUT]");
        }

        static void TryWebRequest()
        {
            Curl curl = new Curl();
            string redrectTo = null;
            string output = curl.DoGet(url, string.Empty, ref redrectTo);

            Console.WriteLine("[OUTPUT]");
            Console.WriteLine(output);
            Console.WriteLine("[/OUTPUT]");
        }
    }
}
