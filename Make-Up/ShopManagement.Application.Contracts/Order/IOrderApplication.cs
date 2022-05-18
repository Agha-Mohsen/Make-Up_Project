using System.Collections.Generic;

namespace ShopManagement.Application.Contracts.Order
{
    public interface IOrderApplication
    {
        void Cancel(long id);
        long PlaceOrder(Cart cart);
        double GetAmountBy(long id);
        string PaymentSucceeded(long orderId, long refId);
        public List<OrderItemViewModel> GetItems(long orderId);
        public List<OrderViewModel> Search(OrderSearchModel searchModel);
    }
}