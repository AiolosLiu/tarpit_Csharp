using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SqlInjection.Models;
using SqlInjection.Database;

using Newtonsoft.Json;
using System.Net.Mail;
using System.IO;

namespace SqlInjection.Controllers
{
    class EmailService
    {
        private String host = "";
        private int port = 0;
        private String username = "";
        private String password = "";
        public EmailService(String host, int port, String username, String password)
        {

            this.host = host;
            this.port = port;
            this.username = username;
            this.password = password;
        }

        public void sendMail(String fromAddress, String toAddress, String subject, String msg)
        {
            SmtpClient smtpClient = new SmtpClient(host, port);
            Console.WriteLine("[+] sendMail ... ... ... ...");
            try
            {
                smtpClient.Credentials = new System.Net.NetworkCredential(username, password);
                smtpClient.UseDefaultCredentials = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.EnableSsl = true;
                MailMessage mail = new MailMessage();

                mail.From = new MailAddress(fromAddress);
                mail.To.Add(new MailAddress(toAddress));
                mail.Subject = subject;
                mail.Body = msg;

                smtpClient.Send(mail);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: '{e}'");
            }
        }
    }

    public class OrderController : Controller
    {
        private readonly SchoolContext _context;
        private EmailService emailService = new EmailService("smtp.mailtrap.io", 25, "87ba3d9555fae8", "91cb4379af43ed");

        // GET: /<controller>/
        public OrderController(SchoolContext context)
        {
            _context = context;
        }


        [HttpGet("AddNewOrder")]
        public ActionResult AddNewOrder(string name)
        {
            string fromAddress = "orders@mycompany.com";
            Order customerOrder = new Order
            {
                FirstName = "Luke",
                LastName = "Guang",
                AspNetUserId = 1,
            };

            string newOrderSerial = JsonConvert.SerializeObject(customerOrder);
            Console.WriteLine("SerializeObject:" + newOrderSerial);

            _context.Orders.Add(customerOrder);
            _context.SaveChanges();

            String customerEmail = customerOrder.getEmailAddr();
            String subject = "Transactions Status of Order : " + customerOrder.Id;
            String verifyUri = fromAddress + "/order/" + customerOrder.Id;
            String message = " Your Order was successfully processed. For Order status please verify on page : " + verifyUri;
            emailService.sendMail(fromAddress, customerEmail, subject, message);

            return Content(newOrderSerial);
        }

        [HttpPost("AddNewOrder")]
        public ActionResult AddNewOrderFlaw(string reader)
        {
            Order order = JsonConvert.DeserializeObject<Order>(reader);
            return Content(order.ToString());
        }


        [HttpGet("vulns")]
        public ActionResult DataLeakage(
            string login, string password,
            string encodedPath, string entityDocument)
        {
            String res = "";
            String ACCESS_KEY_ID = "AKIA2E0A8F3B244C9986";
            String SECRET_KEY = "7CE556A3BC234CC1FF9E8A5C324C0BB70AA21B6D";

            //String txns_dir = System.getProperty("transactions_folder", "/rolling/transactions");
            String txns_dir = Path.GetDirectoryName("/rolling/transactions");

            //DocumentTarpit.getDocument(entityDocument);
            Console.WriteLine(" AWS Properties are " + ACCESS_KEY_ID + " and " + SECRET_KEY);
            Console.WriteLine(" Transactions Folder is " + txns_dir);

            res += " AWS Properties are " + ACCESS_KEY_ID + " and " + SECRET_KEY + "<br />\n";
            res += " Transactions Folder is " + txns_dir + "<br />";

            return Content(res);
        }

    }
}