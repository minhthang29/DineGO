using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace Core.Models
{
    public class AdHistory
    {
        [Key]
        public int history_id { get; set; }

        public int ad_id { get; set; }       // tham chiếu từ AdRegistration
        public int slot_id { get; set; }
        public int res_owner_id { get; set; }

        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }

        public DateTime archived_date { get; set; } // ngày ghi log
    }
}