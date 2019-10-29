using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SqlInjection.Models;
using SqlInjection.Database;

using Newtonsoft.Json;
using System.Net.Mail;

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

            smtpClient.Credentials = new System.Net.NetworkCredential(username, password);
            smtpClient.UseDefaultCredentials = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;
            MailMessage mail = new MailMessage();

            //Setting From , To and CC
            mail.From = new MailAddress(fromAddress, "MyWeb Site");
            mail.To.Add(new MailAddress(toAddress));
            mail.Subject = subject;
            mail.Body = msg;

            smtpClient.Send(mail);
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

            var conn = _context.Database.GetDbConnection();
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
    }

    
}