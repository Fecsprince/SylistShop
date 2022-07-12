using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Core.Models
{
    public class StoreType : BaseEntity
    {
        public string Name { get; set; }

        public virtual ICollection<Shop> Shops { get; set; }
    }
}
