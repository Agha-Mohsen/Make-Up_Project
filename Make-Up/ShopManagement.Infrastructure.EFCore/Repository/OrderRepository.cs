using System.Collections.Generic;
using System.Linq;
using _0_Framework.Application;
using _0_Framework.Infrastructure;
using AccountManagement.Infrastructure.EFCore;
using ShopManagement.Application.Contracts;
using ShopManagement.Application.Contracts.Order;
using ShopManagement.Domain.OrderAgg;

namespace ShopManagement.Infrastructure.EFCore.Repository
{
    public class OrderRepository : RepositoryBase<long, Order>, IOrderRepository
    {
        private readonly AccountContext _accountContext;
        private readonly ShopContext _shopContext;

        public OrderRepository(ShopContext context, AccountContext accountContext) : base(context)
        {
            _shopContext = context;
            _accountContext = accountContext;
        }

        public double GetAmountBy(long id)
        {
            var order = _shopContext.Orders
                .Select(x => new { x.PayAmount, x.Id }).FirstOrDefault(x => x.Id == id);
            if (order == null)
                return 0;
            return order.PayAmount;
        }

        public List<OrderItemViewModel> GetItems(long orderId)
        {
            var order = _shopContext.Orders.FirstOrDefault(z => z.Id == orderId);
            var products = _shopContext.Products.Select(z => new { z.Id, z.Name }).ToList();

            if (order == null)
                return new List<OrderItemViewModel>();

            var items = order.Items.Select(z => new OrderItemViewModel
            {
                Id = z.Id,
                ProductId = z.ProductId,
                Count = z.Count,
                UnitPrice = z.UnitPrice,
                DiscountRate = z.DiscountRate,
                OrderId = z.OrderId
            }).ToList();

            foreach (var item in items)
            {
                item.Product = products.FirstOrDefault(z => z.Id == item.ProductId)?.Name;
            }

            return items;
        }

        public List<OrderViewModel> Search(OrderSearchModel searchModel)
        {
            var accounts = _accountContext.Accounts.Select(z => new { z.Id, z.Fullname }).ToList();
            var query = _shopContext.Orders.Select(z => new OrderViewModel
            {
                Id = z.Id,
                AccountId = z.AccountId,
                DiscountAmount = z.DiscountAmount,
                IsCanceled = z.IsCanceled,
                IsPaid = z.IsPaid,
                IssueTrackingNo = z.IssueTrackingNo,
                PayAmount = z.PayAmount,
                RefId = z.RefId,
                TotalAmount = z.TotalAmount,
                CreationDate = z.CreationDate.ToFarsi(),
                PaymentMethodId = z.PaymentMethod
            });

            query = query.Where(z => z.IsCanceled == searchModel.IsCanceled);
            if (searchModel.AccountId > 0)
                query = query.Where(z => z.AccountId == searchModel.AccountId);

            foreach (var order in query)
            {
                order.AccountFullName = accounts.FirstOrDefault(z => z.Id == order.AccountId)?.Fullname;
                order.PaymentMethod = PaymentMethod.GetBy(order.PaymentMethodId)?.Name;
            }

            return query.OrderByDescending(z => z.Id).ToList();
        }
    }
}