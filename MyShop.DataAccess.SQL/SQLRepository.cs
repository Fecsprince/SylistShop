using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyShop.Core.Models;
using MyShop.Core.Contracts;
using System.Data.Entity;

namespace MyShop.DataAccess.SQL
{
    public class SQLRepository<T> : IRepository<T> where T : BaseEntity
    {

        internal DataContext context;
        internal DbSet<T> Dbset;

        public SQLRepository(DataContext dbContext)
        {
            this.context = dbContext;
            this.Dbset = context.Set<T>();
        }


        public IEnumerable<T> Collection()
        {
            return Dbset;
        }

        public void Commit()
        {
            context.SaveChanges();
        }

        public void Delete(string Id)
        {
            var t = Find(Id);
            if (context.Entry(t).State == EntityState.Detached)
            {
                Dbset.Attach(t);

                Dbset.Remove(t);
            }
        }

        public T Find(string Id)
        {
          return  Dbset.Find(Id);
        }

        public void Insert(T t)
        {
            Dbset.Add(t);
        }

        public void Update(T t)
        {
            Dbset.Attach(t);
            context.Entry(t).State = EntityState.Modified;
        }
    }
}
