using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Models.Client.Custom
{
    public class CustomPostViewModel
    {
        public Customer Customer { get; set;}
        public List<Customer> Customers { get; set; }
        public List<Post> Posts { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Like> Likes { get; set; }
    }
}
