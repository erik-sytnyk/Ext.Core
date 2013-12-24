using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ext.Core
{
    public static class Meta
    {
        public static string Name<TSource, TProperty>(Expression<Func<TSource, TProperty>> expression)
        {
            var memberExpression = GetMemberExpression(expression);
            if (memberExpression == null)
            {
                throw new ArgumentException(string.Format("Expression '{0}' refers to a method, not a property.", expression.ToString()));
            }

            var propInfo = memberExpression.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    expression.ToString()));

            return propInfo.Name;
        }

        public static string FullName<TSource, TProperty>(TSource source, Expression<Func<TSource, TProperty>> expression)
        {
            var segments = new List<string>();

            var memberExpression = GetMemberExpression(expression);

            while (memberExpression != null)
            {
                string propertyName = memberExpression.Member.Name;

                segments.Add(propertyName);

                memberExpression = memberExpression.Expression as MemberExpression;
            }

            segments.Reverse();

            return String.Join(".", segments);
        }

        private static string FullNameShortVersion<TSource, TProperty>(TSource source, Expression<Func<TSource, TProperty>> expression)
        {
            var result = String.Join(".", expression.ToString().Split('.').Skip(1));
            return result;
        }

        private static MemberExpression GetMemberExpression<TSource, TProperty>(Expression<Func<TSource, TProperty>> expr)
        {
            var result = (MemberExpression)null;

            switch (expr.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    var ue = expr.Body as UnaryExpression;
                    result = ((ue != null) ? ue.Operand : null) as MemberExpression;
                    break;
                default:
                    result = expr.Body as MemberExpression;
                    break;
            }

            return result;
        }
    }
}
