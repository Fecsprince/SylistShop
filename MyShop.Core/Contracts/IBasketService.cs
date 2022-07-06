using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using MyShop.Core.ViewModels;

namespace MyShop.Core.Contracts
{   
   public interface IBasketService
    {
        void AddToBasket(HttpContextBase httpContext, string productId);
        void RemoveBasket(HttpContextBase httpContext, string itemId);
        List<BasketItemViewModel> GetBasketItem(HttpContextBase httpContext);
        BasketSummaryViewModel GetBasketSummary(HttpContextBase httpContext);
        void ClearBasket(HttpContextBase httpContext);
    }
}
