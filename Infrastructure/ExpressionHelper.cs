using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;


namespace Infrastructure
{
   public static class ExpressionHelper
    {
        //public static Action<IOrderable<TEntity>> CreateOrderAsc<TEntity, TKey>(Func<TEntity, TKey> orderSource)
        //{
        //    Action<IOrderable<TEntity>> orderAction = (s) => s.Asc(orderSource);
        //    return orderAction;
        //}

        //public static Action<IOrderable<TEntity>> CreateOrderDesc<TEntity, TKey>(Func<TEntity, TKey> orderSource)
        //{
        //    Action<IOrderable<TEntity>> orderAction = (s) => s.Desc(orderSource);
        //    return orderAction;
        //}

        public static Expression<Func<T, bool>> True<T>() { return f => true; }
        public static Expression<Func<T, bool>> False<T>() { return f => false; }
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = System.Linq.Expressions.Expression.Invoke(expr2, expr1.Parameters.Cast<System.Linq.Expressions.Expression>());
            return System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(System.Linq.Expressions.Expression.Or(expr1.Body, invokedExpr), expr1.Parameters);
        }
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = System.Linq.Expressions.Expression.Invoke(expr2, expr1.Parameters.Cast<System.Linq.Expressions.Expression>());
            return System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(System.Linq.Expressions.Expression.And(expr1.Body, invokedExpr), expr1.Parameters);
        }

        public static Dictionary<string, object> GetPropertyWithValue(this LambdaExpression properties)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            MemberInitExpression param = properties.Body as MemberInitExpression;
            foreach (var item in param.Bindings)
            {
                string propertyName = item.Member.Name;
                object propertyValue;
                var memberAssignment = item as MemberAssignment;
                if (memberAssignment.Expression.NodeType == ExpressionType.Constant)
                {
                    propertyValue = (memberAssignment.Expression as ConstantExpression).Value;
                }
                else
                {
                    propertyValue = System.Linq.Expressions.Expression.Lambda(memberAssignment.Expression, null).Compile().DynamicInvoke();
                }
                dic.Add(propertyName, propertyValue);
            }
            return dic;
        }

       private static string ObjToStr(object obj){
            if(obj == null)
                return string.Empty;
            return obj.ToString();
       }

        public static Dictionary<string, string> GetProperties<T>(T t)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();

            if (t == null)
            {
                return null;
            }
            System.Reflection.PropertyInfo[] properties = t.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            if (properties.Length <= 0)
            {
                return null;
            }
            foreach (System.Reflection.PropertyInfo item in properties)
            {
                string name = item.Name;                                                  //实体类字段名称  
                string value = ObjToStr(item.GetValue(t, null));                //该字段的值  

                if (item.PropertyType.IsValueType || item.PropertyType.Name.StartsWith("String"))
                {
                    ret.Add(name, value);        //在此可转换value的类型  
                }
            }

            return ret;
        }  
    }
}
