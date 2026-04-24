using Core.Models;
using System.Collections.Generic;
using Core.Models.Client.Custom;
using System.Threading.Tasks;

namespace DineGO_Api.Repository
{
    public interface ICustomerPointRepository
    {
        void UpdatePoints(int cusId, int changeAmount, string? description = null);
        CustomerPoint? GetPointByCustomerId(int cusId);
        List<CustomerPointHistory> GetHistoryByCustomerId(int cusId);
        VoucherPointViewmodel.VoucherListResponse GetAvailableVouchersWithCustomerPoints(int cusId);
        VoucherOwnedListResponse GetOwnedVouchers(int cusId);
        void GiftVoucher(int senderCusId, int receiverCusId, int voucherId);
        void RedeemVoucher(int cusId, int voucherId);
        void DeleteCustomerVoucher(int cusId, int voucherId);
        List<CustomerPointHistoryWithName> GetAllHistoriesWithCustomerName();
        void TransferVoucherStockToCustomer(int cusId, int voucherId);
        string UseVoucher(int customerId, string voucherCode);

    }
}
