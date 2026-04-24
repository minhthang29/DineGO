using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using DineGO_Api.Models.CartItemModel;


namespace DineGO_Api.Repository
{
    public interface ICartRepository
    {
        void AddFoodToCart(int cusId, int foodId, int quantity);
        List<CartItemViewModel> GetGroupedCartByCustomer(int cusId);
        void DeleteCartItem(int cartFoodId);
        bool UpdateQuantity(int cartFoodId, int quantity);
        bool UpdateIsBuy(List<int> cartFoodIds);
        List<CartFood> GetCartFoods();
        Core.Models.Client.Custom.CheckOutViewModel GetCheckOutInfo(int customerId, string selectedIds);
        bool ClearSelectedCartItems(List<int> cartFoodIds);

    }
}