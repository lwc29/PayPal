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
using System.Threading.Tasks;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
namespace ZF.Repository
{
    public class UnitWork : IUnitWork
    {
        private ZFDBContext _dbContext = new ZFDBContext();

        public void RegisterNew<TEntity>(TEntity entity)
           where TEntity : class
        {
            _dbContext.Set<TEntity>().Add(entity);
        }
 
        //
        private Boolean RemoveHoldingEntityInContext<TEntity>(TEntity entity) where TEntity : class
        {
            var objContext = ((IObjectContextAdapter)_dbContext).ObjectContext;
            var objSet = objContext.CreateObjectSet<TEntity>();
            var entityKey = objContext.CreateEntityKey(objSet.EntitySet.Name, entity);

            Object foundEntity;
            var exists = objContext.TryGetObjectByKey(entityKey, out foundEntity);

            if (exists)
            {
                objContext.Detach(foundEntity);
            }

            return (exists);
        }

        public void RegisterDirty<TEntity>(TEntity entity)
            where TEntity : class
        {
            RemoveHoldingEntityInContext<TEntity>(entity);
            _dbContext.Set<TEntity>().Attach(entity);
            _dbContext.Entry<TEntity>(entity).State = EntityState.Modified;
        }

        public void RegisterDirty<TEntity>(TEntity entity, Expression<Func<TEntity>> properties)
           where TEntity : class
        {
            if (properties != null)
            {
                var propertyWithValues = properties.GetPropertyWithValue();

                if (propertyWithValues != null && propertyWithValues.Count > 0)
                {
                    RemoveHoldingEntityInContext<TEntity>(entity);

                    _dbContext.Set<TEntity>().Attach(entity);
                    var entry = _dbContext.Entry(entity);
                    foreach (var pv in propertyWithValues.Keys)
                    {
                        var property = entry.Property(pv);
                        property.IsModified = true;
                        property.CurrentValue = propertyWithValues[pv];
                    }
                }
            }
        }

        public void RegisterClean<TEntity>(TEntity entity)
            where TEntity : class
        {
            _dbContext.Entry<TEntity>(entity).State = EntityState.Unchanged;
        }

        public void RegisterDeleted<TEntity>(TEntity entity)
            where TEntity : class
        {
            RemoveHoldingEntityInContext<TEntity>(entity);
            _dbContext.Entry<TEntity>(entity).State = EntityState.Deleted;
        }

        public bool Commit()
        {
            return _dbContext.SaveChanges() > 0;
        }

        public async Task<bool> CommitAsync()
        {
            //return await _dbContext.() > 0;
            throw new NotImplementedException();
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }
    }
}
