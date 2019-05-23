using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class UnitOfWork:IUnitOfWork,IDisposable
    {
        private PinkOneMngSysContext _dbContext;
        //private IDictionary<Type, object> repositories;

        public UnitOfWork()
        {
            //这里传入连接配置，解决了：由于EF（OnConfiguring）未配置结束，而调用dbContext产生的bug （必须再配合ajax同步执行）
            var dco = new DbContextOptionsBuilder<PinkOneMngSysContext>().UseSqlServer("Server=212.64.49.60;Database=PinkOneMngSys;user id=admin;password=Pinkone_2019;MultipleActiveResultSets=true;").Options;
#if RELEASE
            dco = new DbContextOptionsBuilder<PinkOneMngSysContext>().UseSqlServer("Server=.;Database=PinkOneMngSys;user id=admin;password=Pinkone_2019;MultipleActiveResultSets=true;").Options;
#endif
            _dbContext = new PinkOneMngSysContext(dco);
            //repositories = new Dictionary<Type, object>();
        }
        public IRepository<T> Repository<T>() where T : class, new()
        {
            //if (repositories.Keys.Contains(typeof(T)) == true)
            //{
            //    var rpo = repositories[typeof(T)] as IRepository<T>;
            //    return repositories[typeof(T)] as IRepository<T>;
            //}
            IRepository<T> objRepo = new Repository<T>(_dbContext);
            //repositories.Add(typeof(T), objRepo);
            return objRepo;
        }
        public int Commit()
        {
            return _dbContext.SaveChanges();
        }
        public async Task<int> CommitAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
        public void Dispose()
        {
            if (_dbContext != null)
                _dbContext.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 针对数据库直接执行查询语句
        /// </summary>
        public IQueryable<T> ExecuteSqlQuery<T>(string sql)where T:class
        {
            if (_dbContext != null)
            {
                return _dbContext.Set<T>().FromSql(sql);
            }
            return null;
        }
        /// <summary>
        /// 针对数据库直接执行sql语句
        /// </summary>
        public int ExecuteSqlCmd(string sql, params object[] parameters)
        {
            if (_dbContext != null)
            {
                return _dbContext.Database.ExecuteSqlCommand(sql, parameters);
            }
            return 0;
        }
    }
}
