using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;


namespace Core.Models
{
    public class Friend
    {
        [Key]
        public int friend_id { get; set; }

        [Required]
        public int customer_id { get; set; }

        [Required]
        public int friend_customer_id { get; set; }
        public bool is_resowner { get; set; } = false;

        public DateTime created_at { get; set; } = DateTime.UtcNow;

        [ForeignKey("customer_id")]
        public Customer Customer { get; set; }

        [ForeignKey("friend_customer_id")]
        public Customer FriendCustomer { get; set; }
    }
}