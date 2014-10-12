using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ext.Core
{
    public static class TypeExtensions
    {
        public static bool Implements<TTypeToImplemenet>(this Type type)
        {
            var typeToImplement = typeof(TTypeToImplemenet);
            return typeToImplement.IsAssignableFrom(type) && type != typeToImplement;
        }

        public static bool DerivesFrom<TTypeToDeriveFrom>(this Type type)
        {
            return type.Implements<TTypeToDeriveFrom>();
        }

        public static bool HasParameterlessConstructor(this Type type)
        {
            return type.GetConstructors().Any(c => c.GetParameters().Length == 0);
        }

        public static bool HasProperty(this Type type, string propertyName)
        {
            return type.GetProperty(propertyName) != null;
        }

        public static object GetDefault(this Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        public static bool IsDecoratedWithAttribute<TAttribute>(this MemberInfo method)
        {
            return method.CustomAttributes.Any(attribute => attribute.AttributeType == typeof(TAttribute));
        }

        public static bool IsProperty(this MethodInfo method)
        {
            var declaringType = method.DeclaringType;
            var isSetProperty = declaringType.GetProperties().FirstOrDefault(p => p.SetMethod == method) != null;
            var isGetProperty = declaringType.GetProperties().FirstOrDefault(p => p.GetMethod == method) != null;
            return isGetProperty || isSetProperty;
        }

        //TODO move somewhere else
        public static void SetProperty<TE, TP>(this TE obj, Expression<Func<TE, TP>> propertyExpression, TP propertyValue)
        {
            var propertyName = Meta.Name(propertyExpression);
            obj.SetProperty(propertyName, propertyValue);
        }

        public static void SetProperty<TE>(this TE obj, string propertyName, object propertyValue)
        {
            var property = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).SingleOrDefault(x => x.Name == propertyName);
            
            Check.NotNull(property, String.Format("Type does not have property with name = '{0}'", propertyName));

            property.SetValue(obj, propertyValue);
        }

        public static string GetPropertyName<TE, TP>(this TE obj, Expression<Func<TE, TP>> propertyExpression)
        {
            return Meta.Name(propertyExpression);
        }
    }
}
