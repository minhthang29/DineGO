using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Notification
    {
        [Key]
        public int noti_id { get; set; }

        [Required, MaxLength(100)]
        public string noti_title { get; set; }

        [Required]
        public string noti_content { get; set; }

        [Required, MaxLength(50)]
        public string noti_type { get; set; }

        public DateTime noti_date { get; set; }

        public DateTime? noti_schedule { get; set; }
        public string? noti_action { get; set; }

        public bool noti_is_read { get; set; }
        public bool noti_is_deleted { get; set; }

        public ICollection<NotificationCustomer>? notificationCustomers { get; set; }
    }
}