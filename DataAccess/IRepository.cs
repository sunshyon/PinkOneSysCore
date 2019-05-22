using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public interface IRepository<T> where T : class, new()
    {
        /// <summary>
        /// 新增实体
        /// </summary>
        void AddEntity(T entity);
        /// <summary>
        /// 更新实体
        /// </summary>
        void UpdateEntity(T entity);
        //int UpdateByExpression(Expression<Func<T, bool>> filterExpression, Expression<Func<T, T>> updateExpression);
        /// <summary>
        /// 删除实体
        /// </summary>
        void DeleteEntity(T entity);
        /// <summary>
        /// 查询实体个数
        /// </summary>
        Task<long> CountEntityAsync(Expression<Func<T, bool>> whereLambda);
        /// <summary>
        /// 根据表达式获取实体集合
        /// </summary>
        //IQueryable<T> GetEntities(Expression<Func<T, bool>> whereLambda);
        Task<List<T>> GetEntitiesAsync(Expression<Func<T, bool>> whereLambda);
        Task<List<T>> GetEntitiesForPageOrderByAsync(Expression<Func<T, bool>> whereLambda, Expression<Func<T, object>> orderLambda, int skip, int take);
        Task<List<T>> GetEntitiesForPageOrderByDescAsync(Expression<Func<T, bool>> whereLambda, Expression<Func<T, object>> orderLambda, int skip, int take);
    }
}
