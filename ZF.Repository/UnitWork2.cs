using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ZF.Repository.Interface;
using Infrastructure;
using ZF.Repository.Domain;
using EntityFramework.Extensions;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Validation;
namespace ZF.Repository
{
    public class UnitWork2 : IUnitWork2
    {
        private ZFDBContext context = new ZFDBContext();
 
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">新增的实体</param>
        /// <param name="bSubmit">是否提交</param>
        /// <returns>成功返回true，失败返回false</returns>
        public bool Insert<T>(T entity, bool bSubmit = false) where T : Entity
        {
            context.Set<T>().Add(entity);

            if (bSubmit)
            {
                return context.SaveChanges() > 0;
            }
            return true;
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="entities">The entities.</param>
        public bool Insert<T>(T[] entities, bool bSubmit = false) where T : Entity
        {
            //foreach (var entity in entities)
            //{
            //    entity.Id = Guid.NewGuid().ToString();
            //}
            context.Set<T>().AddRange(entities);
            if (bSubmit)
            {
                return context.SaveChanges() > 0;
            }
            return true;
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="entity">要更新的实体</param>
        /// <param name="bSubmit">是否提交</param>
        /// <returns>成功返回true，失败返回false</returns>
        public bool Update<T>(T entity, bool bSubmit = false) where T : class
        {
            context.Set<T>().Attach(entity);
            context.Entry(entity).State = EntityState.Modified;

            if (bSubmit)
            {
                return context.SaveChanges() > 0;
            }
            return true;
        }

        /// <summary>
        /// 实现按需要只更新部分更新
        /// <para>如：Update(u =>u.Id==1,u =>new User{Name="ok"});</para>
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="entity">The entity.</param>
        public void Update<T>(Expression<Func<T, bool>> where, Expression<Func<T, T>> entity) where T : class
        {
            context.Set<T>().Where(where).Update(entity);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="Id">实体ID</param>
        /// <param name="bSubmit">是否提交</param>
        /// <returns>成功返回true，失败返回false</returns>
        public bool Delete<T>(int Id, bool bSubmit = false) where T : class
        {
            T entityToDelete = context.Set<T>().Find(Id);
            Delete(entityToDelete);

            if (bSubmit)
            {
                return context.SaveChanges() > 0;
            }
            return true;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entityToDelete">要删除的实体</param>
        private void Delete<T>(T entityToDelete) 
            where T : class
        {
            if (context.Entry<T>(entityToDelete).State == EntityState.Detached)
            {
                context.Set<T>().Attach(entityToDelete);
            }
            context.Set<T>().Remove(entityToDelete);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        public virtual void Delete<T>(Expression<Func<T, bool>> exp)
            where T : class
        {
            context.Set<T>().Where(exp).Delete();
        }

        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="Id">实体ID</param>
        /// <returns>返回对应实体</returns>
        public T Get<T>(int Id)
            where T : class
        {
            return context.Set<T>().Find(Id);
        }

        public IEnumerable<T> GetAll<T>()
            where T : class
        {
            return context.Set<T>().ToList();
        }


        //查询
        public IEnumerable<T> GetWhere<T>(Func<T, bool> wherelambda)
             where T : class
        {
            return context.Set<T>().Where<T>(wherelambda).ToList();
        }


        //分页
        public IEnumerable<T> GetPagers<T>(int pageIndex, int pageSize, out int total,
            Func<T, bool> whereLambda, Action<IOrderable<T>> orderExpression)
             where T : class
        {
            var tempData = context.Set<T>().Where(whereLambda);

            total = tempData.Count();

            //排序获取当前页的数据
            tempData = DataOrder(pageIndex, pageSize, tempData, orderExpression);

            return tempData.ToList();
        }

        private IEnumerable<T> DataOrder<T>(int pageIndex, int pageSize, IEnumerable<T> datasource,
            Action<IOrderable<T>> orderExpression)
             where T : class
        {
            var orders = new Orderable<T>(datasource);

            if (orderExpression != null)
                orderExpression(orders);

            //排序获取当前页的数据
            datasource = orders.Queryable.Skip<T>(pageSize * (pageIndex - 1)).Take(pageSize);

            return datasource;
        }

        public int ExecuteSql(string sql, params object[] parameters)
        {
            return this.context.Database.ExecuteSqlCommand(sql, parameters);
        }

        public IEnumerable<T> ExecuteQuery<T>(string sql, params object[] parameters)
        {
            return this.context.Database.SqlQuery<T>(sql, parameters).ToList();
        }

        public DataSet ExecuteQueryDataTable(string sql, params object[] parameters)
        {
            var ds = new DataSet();
            using (var con = new SqlConnection(this.context.Database.Connection.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, con);
                foreach (var parameter in parameters)
                {
                    cmd.Parameters.Add(parameter as SqlParameter);
                }

                SqlDataAdapter sqlDataReader = new SqlDataAdapter(cmd);
                sqlDataReader.Fill(ds, "tb");
                return ds;
            }
        }


        public int Save()
        {
            try
            {
                return context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                throw new Exception(e.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage);
            }
        }
        //public void RegisterNew<TEntity>(TEntity entity)
        //    where TEntity : class
        //{
        //    _dbContext.Set<TEntity>().Add(entity);
        //}

        //public void RegisterDirty<TEntity>(TEntity entity)
        //    where TEntity : class
        //{
        //    _dbContext.Entry<TEntity>(entity).State = EntityState.Modified;
        //}

        //public void RegisterDirty<TEntity>(TEntity entity, Expression<Func<TEntity>> properties)
        //   where TEntity : class
        //{
        //    if (properties != null)
        //    {
        //        var propertyWithValues = properties.GetPropertyWithValue();

        //        if (propertyWithValues != null && propertyWithValues.Count > 0)
        //        {
        //            _dbContext.Set<TEntity>().Attach(entity);
        //            var entry = _dbContext.Entry(entity);
        //            foreach (var pv in propertyWithValues.Keys)
        //            {
        //                var property = entry.Property(pv);
        //                property.IsModified = true;
        //                property.CurrentValue = propertyWithValues[pv];
        //            } 
        //        }
        //    } 
        //}

        //public void RegisterClean<TEntity>(TEntity entity)
        //    where TEntity : class
        //{
        //    _dbContext.Entry<TEntity>(entity).State = EntityState.Unchanged;
        //}

        //public void RegisterDeleted<TEntity>(TEntity entity)
        //    where TEntity : class
        //{
        //    _dbContext.Entry<TEntity>(entity).State=EntityState.Deleted;
        //}

        //public bool Commit()
        //{
        //    return _dbContext.SaveChanges() > 0;
        //}

        //public void Rollback()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
