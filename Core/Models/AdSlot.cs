using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models
{
    public class AdSlot
    {
        [Key]
        public int slot_id { get; set; }

        [Required, MaxLength(100)]
        public string slot_name { get; set; } // VD: "Banner 1", "Popup"

        [Required, MaxLength(50)]
        public int slot_type { get; set; } // banner / popup

        public bool slot_is_active { get; set; } // Slot còn dùng được không

        // 💰 Giá tiền thuê slot (VD: 500000 VND / tuần)
        public decimal slot_price { get; set; }


        // Quan hệ
        public ICollection<AdRegistration>? registrations { get; set; }
    }
}