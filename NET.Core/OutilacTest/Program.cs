using System;

namespace OutilacTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var bb = new BodyBuilder();
            bb.StartElement("SalesPriceRequests");
            bb.StartElement("SalesPriceRequest");
            bb.AddElementAndValue("Currency", "EUR");
            bb.AddElementAndValue("Customerno", "31099");
            bb.AddElementAndValue("Division", "STAND");
            bb.AddElementAndValue("ExternalCustomerId", "");
            bb.AddElementAndValue("Item", "80017971");
            bb.AddElementAndValue("Login", "");
            bb.AddElementAndValue("Quantity", "1");
            bb.AddElementAndValue("Unit", "STK");
            bb.EndElement();
            bb.EndElement();


        }
    }
}
