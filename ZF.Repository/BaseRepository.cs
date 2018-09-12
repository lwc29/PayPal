using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ZF.Repository.Interface;
using EntityFramework.Extensions;
using Infrastructure;
namespace ZF.Repository
{
    public class BaseRepository<T> : IRepository<T> where T : Domain.Entity
    {
        protected ZFDBContext context;
        internal DbSet<T> dbSet;

        public BaseRepository()
        {
            context = new ZFDBContext();
            this.dbSet = this.context.Set<T>();
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">新增的实体</param>
        /// <param name="bSubmit">是否提交</param>
        /// <returns>成功返回true，失败返回false</returns>
        public bool Insert(T entity, bool bSubmit = false)
        {
            dbSet.Add(entity);

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
        public bool Insert(T[] entities, bool bSubmit = false)
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
        public bool Update(T entity, bool bSubmit = false)
        {
            dbSet.Attach(entity);
            context.Entry(entity).State = EntityState.Modified;

            if (bSubmit)
            {
                return context.SaveChanges() > 0;
            }
            return true;
        }

        public virtual void Update(T entity, Expression<Func<T>> properties, bool bSubmit = false)
        {
            UpdateWithProperties(entity, properties, bSubmit);
        }

        protected virtual void Update(T entity, Expression<Func<object>> properties, bool bSubmit = false)
        {
            UpdateWithProperties(entity, properties, bSubmit);
        }

        private void UpdateWithProperties(T entity, LambdaExpression properties, bool bSubmit = false)
        {
            if (properties == null)
            {
                Update(entity, bSubmit);
                return;
            }
            var propertyWithValues = properties.GetPropertyWithValue();

            if (propertyWithValues != null && propertyWithValues.Count > 0)
            {
                dbSet.Attach(entity);
                var entry = context.Entry(entity);
                foreach (var pv in propertyWithValues.Keys)
                {
                    var property = entry.Property(pv);
                    property.IsModified = true;
                    property.CurrentValue = propertyWithValues[pv];
                }
                if (bSubmit)
                    context.SaveChanges();
            }
        }

        /// <summary>
        /// 实现按需要只更新部分更新
        /// <para>如：Update(u =>u.Id==1,u =>new User{Name="ok"});</para>
        /// </summary>
        /// <param name="where">The where.</param>
        /// <param name="entity">The entity.</param>
        public void Update(Expression<Func<T, bool>> where, Expression<Func<T, T>> entity)
        {
            context.Set<T>().Where(where).Update(entity);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="Id">实体ID</param>
        /// <param name="bSubmit">是否提交</param>
        /// <returns>成功返回true，失败返回false</returns>
        public bool Delete(int Id, bool bSubmit = false)
        {
            T entityToDelete = dbSet.Find(Id);
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
        private void Delete(T entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        public virtual void Delete(Expression<Func<T, bool>> exp)
        {
            context.Set<T>().Where(exp).Delete();
        }

        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="Id">实体ID</param>
        /// <returns>返回对应实体</returns>
        public T Get(int Id)
        {
            return dbSet.Find(Id);
        }

        public IEnumerable<T> GetAll()
        {
            return dbSet.ToList();
        }


        //查询
        public IEnumerable<T> GetWhere(Func<T, bool> wherelambda)
        {
            return dbSet.Where<T>(wherelambda).ToList();
        }


        //分页
        public IEnumerable<T> GetPagers(int pageIndex, int pageSize, out int total,
            Func<T, bool> whereLambda, Action<IOrderable<T>> orderExpression)
        {
            var tempData = dbSet.Where(whereLambda);

            total = tempData.Count();

            //排序获取当前页的数据
            tempData = DataOrder(pageIndex, pageSize, tempData, orderExpression);

            return tempData.ToList();
        }

        private IEnumerable<T> DataOrder(int pageIndex, int pageSize, IEnumerable<T> datasource,
            Action<IOrderable<T>> orderExpression)
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

    }
}
