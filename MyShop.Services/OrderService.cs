using MyShop.Core.Contracts;
using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyShop.Core.ViewModels;

namespace MyShop.Services
{
    public class OrderService : IOrderService
    {
        IRepository<Order> orderContext;
        public OrderService(IRepository<Order> OrderContext)
        {
            this.orderContext = OrderContext;
        }

        public void CreateOrder(Order baseOrder, List<BasketItemViewModel> baseItem)
        {
            foreach (var item in baseItem)
            {
                OrderItem ordi = new OrderItem()
                {
                    ProductId = item.Id,
                    Image1 = item.Image1,
                    Image2 = item.Image2,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    Price = item.Price
                };
                baseOrder.OrderItems.Add(ordi);
            }
            orderContext.Insert(baseOrder);
            orderContext.Commit();
        }
    }
}
