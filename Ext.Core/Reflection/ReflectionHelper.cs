using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ext.Core.Reflection
{
    #region Nested classes

    public delegate object ObjectActivator(params object[] args);

    internal class TypeDescriptor
    {
        public TypeDescriptor(Type type)
        {
            Type = type;
        }

        public Type Type { get; set; }
        public List<PropertyInfo> Properties { get; set; }
        public List<MethodInfo> Methods { get; set; }
        public ObjectActivator Activator { get; set; }

        public void InitProperties()
        {
            var props = this.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            this.Properties = props.ToList();
        }

        public void InitMethods()
        {
            var methods = this.Type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            this.Methods = methods.ToList();
        }

        public void InitActivator()
        {
            var constructor = this.Type.GetConstructor(Type.EmptyTypes);
            var activator = ReflectionHelper.GetNewActivator(constructor);
            this.Activator = activator;
        }
    }

    #endregion

    public class ReflectionHelper
    {
        private static readonly IDictionary<Type, TypeDescriptor> cache = new Dictionary<Type, TypeDescriptor>();
        private static readonly object Lock = new object();

        public delegate object ObjectCloner(object obj);
        private static ObjectCloner _objectCloner = null;

        #region Get from cache

        private static ObjectActivator GetActivator(ConstructorInfo ctor)
        {
            var type = ctor.DeclaringType;

            return GetFromCacheGeneric(type, t => t.Activator, td => td.InitActivator());
        }

        public static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return GetFromCacheGeneric(type, t => t.Properties, td => td.InitProperties());
        }

        public static IEnumerable<MethodInfo> GetMethods(Type type)
        {
            return GetFromCacheGeneric(type, t => t.Methods, td => td.InitMethods());
        }

        internal static T GetFromCacheGeneric<T>(Type type, Func<TypeDescriptor, T> get, Action<TypeDescriptor> set) where T : class
        {
            var typeDesctiptor = GetTypeDescriptorAddIfMissing(type);

            if (get(typeDesctiptor) == null)
            {
                lock (Lock)
                {
                    if (get(typeDesctiptor) == null)
                    {
                        set.Invoke(typeDesctiptor);
                    }
                }
            }
            return get(typeDesctiptor);
        }

        private static TypeDescriptor GetTypeDescriptorAddIfMissing(Type type)
        {
            if (!cache.ContainsKey(type))
            {
                lock (Lock)
                {
                    var typeDescriptor = new TypeDescriptor(type);
                    cache.Add(type, typeDescriptor);
                    return typeDescriptor;
                }
            }
            return cache[type];
        }

        #endregion

        public static object CreateNew(Type type)
        {
            var constructor = type.GetConstructor(Type.EmptyTypes);

            Check.NotNull(constructor, "Object to clone should have parameterless constructor");

            var activator = GetActivator(constructor);

            var newObject = activator();

            return newObject;
        }

        internal static ObjectActivator GetNewActivator(ConstructorInfo ctor)
        {
            Type type = ctor.DeclaringType;
            ParameterInfo[] paramsInfo = ctor.GetParameters();

            //create a single param of type object[]
            ParameterExpression param =
                Expression.Parameter(typeof(object[]), "args");

            Expression[] argsExp =
                new Expression[paramsInfo.Length];

            //pick each arg from the params array 
            //and create a typed expression of them
            for (int i = 0; i < paramsInfo.Length; i++)
            {
                Expression index = Expression.Constant(i);
                Type paramType = paramsInfo[i].ParameterType;

                Expression paramAccessorExp =
                    Expression.ArrayIndex(param, index);

                Expression paramCastExp =
                    Expression.Convert(paramAccessorExp, paramType);

                argsExp[i] = paramCastExp;
            }

            //make a NewExpression that calls the
            //ctor with the args we just created
            NewExpression newExp = Expression.New(ctor, argsExp);

            //create a lambda with the New
            //Expression as body and our param object[] as arg
            LambdaExpression lambda =
                Expression.Lambda(typeof(ObjectActivator), newExp, param);

            //compile it
            ObjectActivator compiled = (ObjectActivator)lambda.Compile();
            return compiled;
        }

        public static object CloneObject(object obj)
        {
            return GetCloner().Invoke(obj);
        }

        private static ObjectCloner GetCloner()
        {
            if (_objectCloner == null)
            {
                var cloneParam = Expression.Parameter(typeof(object));
                var memberwiseClone = typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
                var cloneExp = Expression.Call(cloneParam, memberwiseClone);
                LambdaExpression lambda = Expression.Lambda(typeof(ObjectCloner), cloneExp, cloneParam);
                _objectCloner = (ObjectCloner)lambda.Compile();
            }
            return _objectCloner;
        }

        //TODO cache
        public static Type GetEnumerableType(Type type)
        {
            var result = (Type)null;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                result = type.GetGenericArguments()[0];
            }

            foreach (Type intType in type.GetInterfaces())
            {
                if (intType.IsGenericType && intType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    result = intType.GetGenericArguments()[0];
                }
            }

            return result;
        }

        //TODO cache
        public static IList CreateGenericList(Type listType)
        {
            var genericListType = typeof(List<>);
            var concreteType = genericListType.MakeGenericType(listType);
            var newList = CreateNew(concreteType);
            return newList as IList;
        }

        public static bool IsSimpleType(Type type)
        {
            if (type.IsValueType || type == typeof(string))
            {
                return true;
            }
            return false;
        }
    }
}
