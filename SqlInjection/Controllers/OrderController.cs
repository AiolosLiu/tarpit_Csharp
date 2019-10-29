using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SqlInjection.Models;
using SqlInjection.Database;

using Newtonsoft.Json;

namespace SqlInjection.Controllers
{
    public class OrderController : Controller
    {
        private readonly SchoolContext _context;
        // GET: /<controller>/
        public OrderController(SchoolContext context)
        {
            _context = context;
        }


        [HttpGet("AddNewOrder")]
        public ActionResult AddNewOrder(string name)
        {
            Order customerOrder = new Order
            {
                FirstName = "Luke(Xuguang)",
                LastName = "Liu",
                AspNetUserId = 1,
            };

            string newOrderSerial = JsonConvert.SerializeObject(customerOrder);
            Console.WriteLine("SerializeObject:" + newOrderSerial);

            var conn = _context.Database.GetDbConnection();
            _context.Orders.Add(customerOrder);
            _context.SaveChanges();

            //String customerEmail = customerOrder.getEmailAddress();
            //String subject = "Transactions Status of Order : " + customerOrder.getOrderId();
            //String verifyUri = fromAddress + "/order/" + customerOrder.getOrderId();
            //String message = " Your Order was successfully processed. For Order status please verify on page : " + verifyUri;
            //emailService.sendMail(fromAddress, customerEmail, subject, message);

            return Content(newOrderSerial);
        }

        [HttpPost("AddNewOrder")]
        [HttpGet("AddNewOrderFlaw")]
        public ActionResult AddNewOrderFlaw(string reader)
        {
            Order order = JsonConvert.DeserializeObject<Order>(reader);
            return Content(order.ToString());
        }
    }

    
}