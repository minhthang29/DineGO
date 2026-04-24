using Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models.Client.Custom;
using System.Threading.Tasks;
using DineGO_Api.Repository;

namespace DineGO_Api.DAO
{
    public class CustomerPointDAO
    {
        private readonly ApplicationDbContext _context;

        public CustomerPointDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Cộng hoặc trừ điểm cho customer. Nếu chưa có ví thì tạo mới.
        /// </summary>
        public void UpdatePoints(int cusId, int changeAmount, string? description = null)
        {
            var point = _context.Set<CustomerPoint>()
                                .Include(p => p.pointHistories)
                                .FirstOrDefault(p => p.cus_id == cusId);

            if (point == null)
            {
                point = new CustomerPoint
                {
                    cus_id = cusId,
                    point_balance = 0,
                    created_date = DateTime.Now,
                    last_updated = DateTime.Now
                };
                _context.Set<CustomerPoint>().Add(point);
                _context.SaveChanges();
            }

            // ❌ Nếu trừ quá số dư thì không cho phép
            if (changeAmount < 0 && point.point_balance + changeAmount < 0)
            {
                throw new InvalidOperationException("Số điểm không đủ để thực hiện giao dịch.");
            }

            // ✅ Cập nhật số dư
            point.point_balance += changeAmount;
            point.last_updated = DateTime.Now;

            // Ghi lịch sử
            var history = new CustomerPointHistory
            {
                point_id = point.point_id,
                change_amount = changeAmount,
                balance_after = point.point_balance,
                description = description,
                created_date = DateTime.Now
            };
            _context.Set<CustomerPointHistory>().Add(history);

            _context.SaveChanges();
        }

        /// <summary>
        /// Lấy ví điểm theo cus_id
        /// </summary>
        public CustomerPoint? GetPointByCustomerId(int cusId)
        {
            return _context.Set<CustomerPoint>()
                           .Include(p => p.pointHistories)
                           .FirstOrDefault(p => p.cus_id == cusId);
        }

        /// <summary>
        /// Lấy lịch sử điểm theo cus_id
        /// </summary>
        public List<CustomerPointHistory> GetHistoryByCustomerId(int cusId)
        {
            return _context.Set<CustomerPointHistory>()
                           .Where(h => h.customerPoint.cus_id == cusId)
                           .OrderByDescending(h => h.created_date)
                           .ToList();
        }
        public VoucherPointViewmodel.VoucherListResponse GetAvailableVouchersWithCustomerPoints(int cusId)
        {
            var customerPoint = _context.CustomerPoints.FirstOrDefault(p => p.cus_id == cusId);
            int balance = customerPoint?.point_balance ?? 0;

            var vouchers = _context.Vouchers
            .Where(v => v.voucher_is_active
             && !v.voucher_is_deleted
             && v.voucher_end_date >= DateTime.Now
             && v.voucher_stock > 0
             && v.voucher_apply_type == 0)
            .Select(v => new VoucherPointViewmodel.VoucherItem
            {
                voucher_id = v.voucher_id,
                voucher_code = v.voucher_code,
                voucher_discount = v.voucher_discount,
                voucher_start_date = v.voucher_start_date,
                voucher_end_date = v.voucher_end_date,
                voucher_stock = v.voucher_stock,
                voucher_type = v.voucher_type,
                voucher_apply_type = v.voucher_apply_type,
                required_points = v.required_points,
            })
            .ToList();


            return new VoucherPointViewmodel.VoucherListResponse
            {
                CustomerBalance = balance,
                Vouchers = vouchers
            };
        }
        public VoucherOwnedListResponse GetOwnedVouchers(int cusId)
        {
            // Lấy số dư
            var customerPoint = _context.CustomerPoints.FirstOrDefault(p => p.cus_id == cusId);
            int balance = customerPoint?.point_balance ?? 0;

            // Lấy danh sách bạn bè
            var friendIds = _context.Friends
                .Where(f => f.customer_id == cusId || f.friend_customer_id == cusId)
                .Select(f => f.customer_id == cusId ? f.friend_customer_id : f.customer_id)
                .Distinct()
                .ToList();

            var friends = _context.Customers
                .Where(c => friendIds.Contains(c.cus_id))
                .Select(c => new FriendItem
                {
                    CusId = c.cus_id,
                    CusName = c.cus_name,
                    CusImage = c.cus_image
                })
                .ToList();

            // Lấy voucher đã sở hữu
            var vouchers = _context.CustomerVouchers
                .Include(cv => cv.voucher)
                .Where(cv => cv.cus_id == cusId && cv.customer_voucher_quantity > 0)
                .Select(cv => new VoucherOwnedViewmodel
                {
                    VoucherId = cv.voucher.voucher_id,
                    VoucherCode = cv.voucher.voucher_code,
                    VoucherDiscount = cv.voucher.voucher_discount,
                    VoucherStartDate = cv.voucher.voucher_start_date,
                    VoucherEndDate = cv.voucher.voucher_end_date,
                    Quantity = cv.customer_voucher_quantity,
                    VoucherIsActive = cv.voucher.voucher_is_active,
                    VoucherIsDeleted = cv.voucher.voucher_is_deleted,
                    VoucherType = cv.voucher.voucher_type,
                    Friends = friends,
                    VoucherCapAmount = cv.voucher.voucher_cap_amount
                })
                .ToList();

            return new VoucherOwnedListResponse
            {
                CustomerBalance = balance,
                Vouchers = vouchers
            };
        }
        public void GiftVoucher(int senderCusId, int receiverCusId, int voucherId)
        {
            // ✅ Kiểm tra bạn bè
            var isFriend = _context.Friends.Any(f =>
                (f.customer_id == senderCusId && f.friend_customer_id == receiverCusId) ||
                (f.customer_id == receiverCusId && f.friend_customer_id == senderCusId));

            if (!isFriend)
                throw new InvalidOperationException("Người nhận không phải bạn bè.");

            // ✅ Kiểm tra voucher của sender
            var senderVoucher = _context.CustomerVouchers
                .FirstOrDefault(cv => cv.cus_id == senderCusId && cv.voucher_id == voucherId);

            if (senderVoucher == null || senderVoucher.customer_voucher_quantity <= 0)
                throw new InvalidOperationException("Bạn không có voucher này để tặng.");

            // Giảm số lượng của người gửi
            senderVoucher.customer_voucher_quantity -= 1;

            // ✅ Tăng số lượng cho receiver
            var receiverVoucher = _context.CustomerVouchers
                .FirstOrDefault(cv => cv.cus_id == receiverCusId && cv.voucher_id == voucherId);

            if (receiverVoucher != null)
            {
                receiverVoucher.customer_voucher_quantity += 1;
            }
            else
            {
                receiverVoucher = new CustomerVoucher
                {
                    cus_id = receiverCusId,
                    voucher_id = voucherId,
                    customer_voucher_quantity = 1
                };
                _context.CustomerVouchers.Add(receiverVoucher);
            }

            _context.SaveChanges();
            // ✅ Sau khi lưu thành công thì gửi mail
            var sender = _context.Customers.FirstOrDefault(c => c.cus_id == senderCusId);
            var receiver = _context.Customers.FirstOrDefault(c => c.cus_id == receiverCusId);
            var voucher = _context.Vouchers.FirstOrDefault(v => v.voucher_id == voucherId);

            if (receiver != null && voucher != null && sender != null)
            {
                string subject = $"Bạn vừa nhận voucher từ {sender.cus_name}";
                string body = $@"
            Xin chào {receiver.cus_name},<br/><br/>
            Bạn vừa được <b>{sender.cus_name}</b> tặng một voucher:<br/>
            <ul>
                <li>Mã voucher: <b>{voucher.voucher_code}</b></li>
                <li>Giảm: {(voucher.voucher_type == 0 ? voucher.voucher_discount + "%" : voucher.voucher_discount + "đ")}</li>
                <li>Hạn dùng: {voucher.voucher_end_date:dd/MM/yyyy}</li>
            </ul>
            Chúc bạn mua sắm vui vẻ!";

                var mailRepo = new MailSenderRepository();
                mailRepo.SendMail(receiver.cus_email, subject, () => body);

            }
        }

        public void RedeemVoucher(int cusId, int voucherId)
        {
            var voucher = _context.Vouchers.FirstOrDefault(v => v.voucher_id == voucherId);
            if (voucher == null || !voucher.voucher_is_active || voucher.voucher_is_deleted)
                throw new InvalidOperationException("Voucher không hợp lệ.");

            var requiredPoints = voucher.required_points ?? 0;
            if (requiredPoints <= 0)
                throw new InvalidOperationException("Voucher này không thể đổi bằng điểm.");

            var point = _context.CustomerPoints.FirstOrDefault(p => p.cus_id == cusId);
            if (point == null || point.point_balance < requiredPoints)
                throw new InvalidOperationException("Không đủ điểm để đổi voucher.");

            // ❌ Nếu voucher hết hàng
            if (voucher.voucher_stock.HasValue && voucher.voucher_stock.Value <= 0)
                throw new InvalidOperationException("Voucher đã hết số lượng.");

            // ✅ Trừ điểm
            point.point_balance -= requiredPoints;
            point.last_updated = DateTime.Now;

            // ✅ Trừ stock voucher
            if (voucher.voucher_stock.HasValue)
            {
                voucher.voucher_stock -= 1;
            }

            // ✅ Ghi lịch sử
            var history = new CustomerPointHistory
            {
                point_id = point.point_id,
                change_amount = -requiredPoints,
                balance_after = point.point_balance,
                description = $"Đổi voucher {voucher.voucher_code}",
                created_date = DateTime.Now
            };
            _context.CustomerPointHistories.Add(history);

            // ✅ Thêm hoặc update CustomerVoucher
            var customerVoucher = _context.CustomerVouchers
                .FirstOrDefault(cv => cv.cus_id == cusId && cv.voucher_id == voucherId);

            if (customerVoucher != null)
                customerVoucher.customer_voucher_quantity += 1;
            else
                _context.CustomerVouchers.Add(new CustomerVoucher
                {
                    cus_id = cusId,
                    voucher_id = voucherId,
                    customer_voucher_quantity = 1
                });

            _context.SaveChanges();
        }

        public void DeleteCustomerVoucher(int cusId, int voucherId)
        {
            var cv = _context.CustomerVouchers
                             .FirstOrDefault(x => x.cus_id == cusId && x.voucher_id == voucherId);
            if (cv == null)
                throw new InvalidOperationException("Không tìm thấy CustomerVoucher.");

            _context.CustomerVouchers.Remove(cv);
            _context.SaveChanges();
        }

        public List<CustomerPointHistoryWithName> GetAllHistoriesWithCustomerName()
        {
            var histories = _context.CustomerPointHistories
                .Include(h => h.customerPoint)
                .ThenInclude(cp => cp.customer)   // join sang Customer
                .OrderByDescending(h => h.created_date)
                .Select(h => new CustomerPointHistoryWithName
                {
                    HistoryId = h.history_id,
                    ChangeAmount = h.change_amount,
                    BalanceAfter = h.balance_after,
                    Description = h.description,
                    CreatedDate = h.created_date,
                    CustomerName = h.customerPoint.customer.cus_name
                })
                .ToList();

            return histories;
        }
        public void TransferVoucherStockToCustomer(int cusId, int voucherId)
        {
            // Lấy voucher
            var voucher = _context.Vouchers.FirstOrDefault(v => v.voucher_id == voucherId);
            if (voucher == null)
                throw new InvalidOperationException("Voucher không tồn tại.");

            if (!voucher.voucher_is_active || voucher.voucher_is_deleted)
                throw new InvalidOperationException("Voucher không hợp lệ.");

            // Nếu stock null hoặc <= 0 thì không có gì để chuyển
            if (!voucher.voucher_stock.HasValue || voucher.voucher_stock.Value <= 0)
                throw new InvalidOperationException("Voucher không có số lượng để chuyển.");

            int stockToTransfer = voucher.voucher_stock.Value;

            // Set voucher_stock = 0
            voucher.voucher_stock = 0;

            // Gán toàn bộ số lượng này cho customer
            var customerVoucher = _context.CustomerVouchers
                .FirstOrDefault(cv => cv.cus_id == cusId && cv.voucher_id == voucherId);

            if (customerVoucher != null)
            {
                customerVoucher.customer_voucher_quantity += stockToTransfer;
            }
            else
            {
                customerVoucher = new CustomerVoucher
                {
                    cus_id = cusId,
                    voucher_id = voucherId,
                    customer_voucher_quantity = stockToTransfer
                };
                _context.CustomerVouchers.Add(customerVoucher);
            }

            _context.SaveChanges();
        }
        public string UseVoucher(int customerId, string voucherCode)
        {
            var cusVoucher = _context.CustomerVouchers
                .Include(cv => cv.voucher)
                .FirstOrDefault(cv => cv.cus_id == customerId
                                   && cv.voucher.voucher_code == voucherCode);

            if (cusVoucher == null)
                throw new InvalidOperationException("Voucher không tồn tại.");
            if (cusVoucher.customer_voucher_quantity <= 0)
                throw new InvalidOperationException("Bạn không còn voucher này.");

            cusVoucher.customer_voucher_quantity -= 1;
            _context.SaveChanges();

            return $"Đã sử dụng voucher {voucherCode} thành công.";
        }

    }
}
