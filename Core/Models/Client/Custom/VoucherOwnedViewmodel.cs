using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.Client.Custom
{
    public class VoucherOwnedViewmodel
    {
        public int VoucherId { get; set; }
        public string VoucherCode { get; set; }
        public decimal VoucherDiscount { get; set; }
        public DateTime VoucherStartDate { get; set; }
        public DateTime VoucherEndDate { get; set; }
        public decimal? VoucherCapAmount { get; set; }
        public int Quantity { get; set; }

        // thêm các flag từ bảng Voucher
        public bool VoucherIsActive { get; set; }
        public bool VoucherIsDeleted { get; set; }

        // nếu cần phân biệt % và VND
        public int VoucherType { get; set; }

        // 👇 Danh sách bạn bè để có thể tặng
        public List<FriendItem> Friends { get; set; }
    }

    public class FriendItem
    {
        public int CusId { get; set; }
        public string CusName { get; set; }
        public string? CusImage { get; set; }
    }
}