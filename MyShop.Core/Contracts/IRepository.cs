using System.Collections.Generic;
using MyShop.Core.Models;

namespace MyShop.Core.Contracts
{
    public interface IRepository<T> where T : BaseEntity
    {
        IEnumerable<T> Collection();
        void Commit();
        void Delete(string Id);
        T Find(string Id);
        T Insert(T t);
        T Update(T t);
    }
}