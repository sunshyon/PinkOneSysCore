using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class Repository<T>:IRepository<T> where T:class,new()
    {
        private PinkOneMngSysContext _dbContext = null;
        public Repository(PinkOneMngSysContext dbc)
        {
            _dbContext = dbc;
        }
        public async void AddEntity(T entity)
        {
             await _dbContext.Set<T>().AddAsync(entity);
        }
        public void UpdateEntity(T entity)
        {
             _dbContext.Set<T>().Update(entity);
        }

        public void DeleteEntity(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }

        public async Task<long> CountEntityAsync(Expression<Func<T, bool>> whereLambda)
        {
            return await _dbContext.Set<T>().Where(whereLambda).LongCountAsync();
        }

        /// <summary>
        /// 同步查询
        /// </summary>
        /// <param name="whereLambda"></param>
        public IQueryable<T> GetEntities(Expression<Func<T, bool>> whereLambda)
        {
            //_dbContext.Set<T>().ToAsyncEnumerable().w.Where(whereLambda)
            return _dbContext.Set<T>().Where(whereLambda);
        }
        /// <summary>
        /// 异步查询
        /// </summary>
        /// <param name="whereLambda"></param>
        public async Task<List<T>> GetEntitiesAsync(Expression<Func<T, bool>> whereLambda)
        {
            var res= await _dbContext.Set<T>().AsNoTracking().Where(whereLambda).ToListAsync();
            return res;
        }
        /// <summary>
        /// 用于分页查询升序
        /// </summary>
        public async Task<List<T>> GetEntitiesForPageOrderByAsync(Expression<Func<T, bool>> whereLambda, Expression<Func<T, object>> orderLambda, int skip,int take)
        {
            return await _dbContext.Set<T>().Where(whereLambda).OrderBy(orderLambda).Skip(skip).Take(take).ToListAsync();
            
        }
        /// <summary>
        /// 分页降序
        /// </summary>
        public async Task<List<T>> GetEntitiesForPageOrderByDescAsync(Expression<Func<T, bool>> whereLambda, Expression<Func<T,object>> orderLambda, int skip, int take)
        {
            return await _dbContext.Set<T>().Where(whereLambda).OrderByDescending(orderLambda).Skip(skip).Take(take).ToListAsync();
        }
    }
}
