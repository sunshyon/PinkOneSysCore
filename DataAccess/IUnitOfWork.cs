using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public interface IUnitOfWork
    {
        int Commit();
        Task<int> CommitAsync();
        IRepository<T> Repository<T>() where T : class, new();
        IQueryable<T> ExecuteSqlQuery<T>(string sql) where T:class;
        int ExecuteSqlCmd(string sql, params object[] parameters);
    }
}
