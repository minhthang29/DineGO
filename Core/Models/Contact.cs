using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class Contact
    {
        [Key]
        public int contact_id { get; set; }

        [MaxLength(100)]
        public string contact_name { get; set; }

        [MaxLength(150)]
        public string contact_email { get; set; }

        [MaxLength(200)]
        public string contact_subject { get; set; }

        public string contact_message { get; set; }

        public int contact_status { get; set; }
        public string? contact_reply_by_admin { get; set; }

        public string? contact_reply_message { get; set; }

        public DateTime contact_created_at { get; set; }

        public DateTime? contact_replied_at { get; set; }
        public bool contact_is_deleted { get; set; }
    }
}