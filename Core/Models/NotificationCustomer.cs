using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models
{
    public class NotificationCustomer
    {
        [Key]
        public int noti_customer_id { get; set; }

        public int noti_id { get; set; }
        public int cus_id { get; set; }
        public int? order_id { get; set; }
        public bool noti_customer_is_read { get; set; } = false;
        public DateTime? read_date { get; set; }
        public DateTime noti_customer_send_date { get; set; }

        public Notification? notification { get; set; }
        public Customer? customer { get; set; }
    }
}