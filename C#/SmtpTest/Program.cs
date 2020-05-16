using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SmtpTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Test 1");
                Test1();
                Console.WriteLine("Done");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            try
            {
                Console.WriteLine("Test 2");
                Test2();
                Console.WriteLine("Done");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static void Test1()
        {
            using (var mail = new MailMessage())
            {
                mail.Subject = "Test Met SMTP and UTF8";
                mail.Priority = MailPriority.Normal;
                mail.Body = "This is a Test";
                mail.IsBodyHtml = false;
                mail.From = new MailAddress("pehu@natcheurope.com");
                mail.To.Add(new MailAddress("peçter@natcheurope.com", "Peçter"));

                using (var emailClient = new SmtpClient())
                {
                    // emailClient.DeliveryFormat = SmtpDeliveryFormat.International;
                    emailClient.Send(mail);
                }
            }
        }

        static void Test2()
        {
            using (var mail = new MailMessage())
            {
                mail.Subject = "Test Met SMTP and UTF8";
                mail.Priority = MailPriority.Normal;
                mail.Body = "This is a Test";
                mail.IsBodyHtml = false;
                mail.From = new MailAddress("pehu@natcheurope.com");
                mail.To.Add(new MailAddress("peçter@natcheurope.com", "Peçter"));

                using (var emailClient = new SmtpClient())
                {
                    emailClient.DeliveryFormat = SmtpDeliveryFormat.International;
                    emailClient.Send(mail);
                }
            }
        }
    }
}
