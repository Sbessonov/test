using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<String> Get()
        {
            return "value";
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        //POST api/values
       [HttpPost]
        public Message Post([FromBody] Message post)
        {
            TestContext context = new TestContext();
            Customer customer = new Customer();
            /*
             *  вернул identity(Customer, Message)
             *  изменил Message(норм название столбцов) 
             */
            customer = FindCostomer(post.Customer.Email, post.Customer.Phone);
            if ( customer == null )
            {
                customer = new Customer();
                customer = context.Customers.Add(post.Customer).Entity;
                context.SaveChanges();
            }
            post.ClientID = customer.ID;

            Subject subject = FindSubject(post.Subject.Title);
            if (subject == null)
            {
                subject = new Subject();
                post.Subject.ID = FindMaxSubjectID() + 1;
                subject.ID = post.Subject.ID;
                subject = context.Subjects.Add(post.Subject).Entity;
                context.SaveChanges();
            }
            post.SubjectID = subject.ID;

            post.Customer = null;
            post.Subject = null;
            context.Messages.Add(post);
            context.SaveChanges();
            post.Customer = customer;
            post.Subject = subject;
            post.Customer.CustomerMessages = null;
            post.Subject.CustomerMessages = null;

            return post;
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        /// <summary>
        /// Возвращает объект темы сообщения, который соответсвует
        /// заданному заголовку темы и null в противном случае.
        /// </summary>
        private Subject FindSubject(string title)
        {
            TestContext context = new TestContext();
            Subject subject = new Subject();
            subject = context.Subjects.Where(x => x.Title.Equals(title)).FirstOrDefault();

            return subject;
        }

        private int FindMaxSubjectID()
        {
            TestContext context = new TestContext();
            int maxID = context.Subjects.Max(x => x.ID);

            return maxID;
        }

        /// <summary>
        /// Возвращает ID клиента, для которого совпали
        /// заданные поля email и phone и -1 в противном случае.
        /// </summary>
        private Customer  FindCostomer(string email, string phone)
        {
            TestContext context = new TestContext();
            Customer customer = new Customer();
            customer = context.Customers.Where(x => x.Email.Equals(email)).Where(x => x.Phone.Equals(phone)).FirstOrDefault();
            
            return customer;
        }

        private Message FindLastMessage()
        {
            Message message = new Message();
            TestContext context = new TestContext();
            int maxID = context.Messages.Max(x => x.ID);
            message = context.Messages.Where(x => x.ID == maxID).FirstOrDefault();

            return message;
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
