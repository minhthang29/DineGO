using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;


namespace Core.Models
{
    public class ChatMessage
    {
        [Key]
        public int chat_id { get; set; }

        [Required]
        public int sender_id { get; set; }

        [Required]
        public int receiver_id { get; set; }

        [Required]
        [MaxLength(1000)]
        public string message { get; set; }

        public DateTime sent_at { get; set; } = DateTime.UtcNow;

        public bool is_read { get; set; } = false;
        public bool is_resowner_chat { get; set; } = false;
    }
}