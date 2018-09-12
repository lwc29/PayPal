using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZF.Repository.Domain;
using ZF.Repository.Interface;

namespace ZF.App
{
   public class BaseApp<T> where T : Entity
    {
        /// <summary>
        /// 用于事务操作
        /// </summary>
        /// <value>The unit work.</value>
        public IUnitWork UnitWork { get; set; }
        /// <summary>
        /// 用于普通的数据库操作
        /// </summary>
        /// <value>The repository.</value>
        public IRepository<T> Repository { get; set; }

        //常用方法
        public bool Delete(int Id) {
           return Repository.Delete(Id,true);
        }
        public T Get(int Id) {
            return Repository.Get(Id);
        }
        public bool Insert(T entity) {
            return Repository.Insert(entity, true);
        }
       /// <summary>
       ///  暂时不用
       /// </summary>
       /// <param name="entity"></param>
       /// <returns></returns>
        public bool Update(T entity)
        {
            var config = Get(entity.Id);
            if(config == null)
                throw new Exception("该记录不存在");
            return Repository.Update(entity,true);
        }
 
    }
}
