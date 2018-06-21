using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Transactions;
using System.Data.Entity.Infrastructure;

namespace DataBase.Entity.Infrastructure
{
    public class RepositoryBase<T> where T : class
    {
        private AppDbContext _DataContext;
        private IDbSet<T> _Model;
        private DatabaseType _dbType = DatabaseType.SQLServer;
        protected IProcTools _procTool;

        public IDbSet<T> Model
        {
            get { return _Model; }
            set { _Model = value; }
        }
        public RepositoryBase()
        {
            try
            {
                SetDBType(ConfigurationManager.AppSettings["DBType"]);
                _procTool = DbFactory.Instance.GetProcTools(_dbType);
                _DataContext = new AppDbContext(ConfigurationManager.AppSettings["Provider"]);
                if (_DataContext.Database.Exists())
                {
                    var objectContext = (_DataContext as IObjectContextAdapter).ObjectContext;
                    objectContext.CommandTimeout = 1000;
                    _Model = _DataContext.Set<T>();
                }
                else
                {
                    _DataContext = null;
                }
            }
            catch (Exception e) { }
        }

        protected void SetDBType(string dbTypeString)
        {
            switch (dbTypeString)
            {
                case "SQLServer":
                    _dbType = DatabaseType.SQLServer;
                    break;
                case "MySQL":
                    _dbType = DatabaseType.MySQL;
                    break;
                case "PostgreSQL":
                    _dbType = DatabaseType.PostgreSQL;
                    break;
                default:
                    _dbType = DatabaseType.SQLServer;
                    break;
            }
        }

        public virtual void Add(T entity)
        {
            _Model.Add(entity);
        }
        public virtual void Update(T entity)
        {
            _Model.Attach(entity);
            _DataContext.Entry(entity).State = EntityState.Modified;
        }
        public virtual void Delete(T entity)
        {
            _Model.Remove(entity);
        }
        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            IEnumerable<T> objects = _Model.Where<T>(where).AsEnumerable();
            foreach (T obj in objects)
                _Model.Remove(obj);
        }
        public virtual T GetById(long id)
        {
            return _Model.Find(id);
        }
        public virtual T GetById(string id)
        {
            return _Model.Find(id);
        }
        public virtual List<T> GetAll()
        {
            return _Model.ToList();
        }
        public virtual IEnumerable<T> GetMany(Expression<Func<T, bool>> where)
        { 
            return _Model.Where(where);
        }
        public virtual int Commit()
        {
            return _DataContext.SaveChanges();
        }
        public T Get(Expression<Func<T, bool>> where)
        {
            return _Model.Where(where).FirstOrDefault<T>();
        }
        /// <summary>
        /// 通过T-SQL查询
        /// </summary>
        /// <typeparam name="TResult">结果集中元素的类型</typeparam>
        /// <param name="sql">T-SQL语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>结果集</returns>
        public virtual IEnumerable<T> GetBySql(string sql, params object[] parameters)
        {
            return _DataContext.Database.SqlQuery<T>(sql, parameters);
        }
        /// <summary>
        /// 通过T-SQL查询
        /// </summary>
        /// <param name="elementType">结果集中元素的类型</param>
        /// <param name="sql">T-SQL语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>结果集</returns>
        public virtual IEnumerable GetBySql(Type elementType, string sql, params object[] parameters)
        {
            return _DataContext.Database.SqlQuery(elementType, sql, parameters);
        }
        /// <summary>
        /// 执行非查询T-SQL命令，Insert、Update、Delete等
        /// </summary>
        /// <param name="sql">T-SQL语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>sql命令影响的行数</returns>
        public virtual int ExecuteNonQuery(string sql, params object[] parameters)
        {
            return _DataContext.Database.ExecuteSqlCommand(sql, parameters);
        }
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="procedure"></param>
        /// <returns></returns>
        public virtual IEnumerable<TResult> ExecuteProc<TResult>(string procName, IStoredProcedure procedure)
        {
            if (string.IsNullOrWhiteSpace(procName)) return null;

            return _DataContext.Database.ExecuteStoredProcedure<TResult>(_procTool, procName, procedure);
        }

        #region 分页查询技术
        public virtual int GetTotalPageCount(Expression<Func<T, bool>> where)
        {
            var list = _Model.Where(where);
            return list.Count();
        }
        /// <summary>
        /// 根据条件分页获得记录
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="orderBy">排序</param>
        /// <param name="ascending">是否升序</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns>记录列表</returns>
        public virtual List<T> GetMany(Expression<Func<T, bool>> where, string orderBy, bool ascending, int pageIndex, int pageSize)
        {
            var list = _Model.Where(where);
            var parm = Expression.Parameter(typeof(T), typeof(T).Name);
            var body = Expression.Property(parm, orderBy);
            dynamic keySelector = Expression.Lambda(body, parm);
            var orderbyQueryable = ascending ? Queryable.OrderBy(list, keySelector) : Queryable.OrderByDescending(list, keySelector);
            list = list.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            return list.ToList();
        }
        #endregion
    }
}
