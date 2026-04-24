using Core.Models;
using DineGO_Api.DAO;
using System.Collections.Generic;
using Core.Models.Client.Custom;
using System.Threading.Tasks;


namespace DineGO_Api.Repository
{
    public class CustomerPointRepository : ICustomerPointRepository
    {
        private readonly CustomerPointDAO _dao;

        public CustomerPointRepository(CustomerPointDAO dao)
        {
            _dao = dao;
        }

        public void UpdatePoints(int cusId, int changeAmount, string? description = null)
        {
            _dao.UpdatePoints(cusId, changeAmount, description);
        }

        public CustomerPoint? GetPointByCustomerId(int cusId)
        {
            return _dao.GetPointByCustomerId(cusId);
        }

        public List<CustomerPointHistory> GetHistoryByCustomerId(int cusId)
        {
            return _dao.GetHistoryByCustomerId(cusId);
        }

        public VoucherPointViewmodel.VoucherListResponse GetAvailableVouchersWithCustomerPoints(int cusId)
            => _dao.GetAvailableVouchersWithCustomerPoints(cusId);

        public VoucherOwnedListResponse GetOwnedVouchers(int cusId)
            => _dao.GetOwnedVouchers(cusId);

        public void GiftVoucher(int senderCusId, int receiverCusId, int voucherId)
            => _dao.GiftVoucher(senderCusId, receiverCusId, voucherId);

        public void RedeemVoucher(int cusId, int voucherId)
            => _dao.RedeemVoucher(cusId, voucherId);
        public void DeleteCustomerVoucher(int cusId, int voucherId)
            => _dao.DeleteCustomerVoucher(cusId, voucherId);
        public List<CustomerPointHistoryWithName> GetAllHistoriesWithCustomerName()
    => _dao.GetAllHistoriesWithCustomerName();
        public void TransferVoucherStockToCustomer(int cusId, int voucherId)
        => _dao.TransferVoucherStockToCustomer(cusId, voucherId);
        public string UseVoucher(int customerId, string voucherCode)
            => _dao.UseVoucher(customerId, voucherCode);
    }
}
