using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Models.Client.Custom
{
    public class PostDetailsViewModel
    {
        public Post Post { get; set; }
        public Customer Customer { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Customer> CommentAuthors { get; set; }
        public List<Like> Likes { get; set; }
    }
}
