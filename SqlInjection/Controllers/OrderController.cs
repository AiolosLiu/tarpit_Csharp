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

            /*
            try
            {

                // FLAW: Insecure cryptographic algorithm (DES) 
                // CWE: 327 Use of Broken or Risky Cryptographic Algorithm 
                Cipher des = Cipher.getInstance("DES");
                SecretKey key = KeyGenerator.getInstance("DES").generateKey();
                des.init(Cipher.ENCRYPT_MODE, key);

                var conn = _context.Database.GetDbConnection();
                String sql = "SELECT * FROM USER WHERE LOGIN = '" + login + "' AND PASSWORD = '" + password + "'";
                resultSet = _context.Database.ExecuteSqlCommand(sql);

                if (resultSet.next())
                {

                    login = resultSet.getString("login");
                    password = resultSet.getString("password");

                    User user = new User(login,
                        resultSet.getString("fname"),
                        resultSet.getString("lname"),
                        resultSet.getString("passportnum"),
                        resultSet.getString("address1"),
                        resultSet.getString("address2"),
                        resultSet.getString("zipCode"));

                    String creditInfo = resultSet.getString("userCreditCardInfo");
                    byte[] cc_enc_str = des.doFinal(creditInfo.getBytes());

                    Cookie cookie = new Cookie("login", login);
                    cookie.setMaxAge(864000);
                    cookie.setPath("/");
                    response.addCookie(cookie);

                    request.setAttribute("user", user.toString());
                    request.setAttribute("login", login);

                    LOGGER.info(" User " + user + " successfully logged in ");
                    LOGGER.info(" User " + user + " credit info is " + cc_enc_str);

                    getServletContext().getRequestDispatcher("/dashboard.jsp").forward(request, response);

                }
                else
                {
                    request.setAttribute("login", login);
                    request.setAttribute("password", password);
                    request.setAttribute("keepOnline", keepOnline);
                    request.setAttribute("message", "Failed to Sign in. Please verify credentials");

                    LOGGER.info(" UserId " + login + " failed to logged in ");

                    getServletContext().getRequestDispatcher("/signIn.jsp").forward(request, response);
                }
            }
            catch (Exception e)
            {
            }
            */
            return Content(res);
        }

    }
}