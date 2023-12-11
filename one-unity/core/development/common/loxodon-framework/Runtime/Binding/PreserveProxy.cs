using System;
using Loxodon.Framework.Binding.Reflection;
using UnityEngine.Scripting;

namespace TPFive.Extended.LoxodonFramework.Binding
{
    public static class PreserveProxy
    {
        [Preserve]
        public static void Register<T, TValue>(string name, Func<T, TValue> getter, Action<T, TValue> setter)
        {
            var propertyInfo = typeof(T).GetProperty(name);
            if (propertyInfo is not null)
            {
                ProxyFactory.Default.Register(new ProxyPropertyInfo<T, TValue>(name, getter, setter));
                return;
            }

            var fieldInfo = typeof(T).GetField(name);
            if (fieldInfo is not null)
            {
                ProxyFactory.Default.Register(new ProxyFieldInfo<T, TValue>(name, getter, setter));
                return;
            }

            throw new Exception(string.Format("Not found the property or field named '{0}' in {1} type", name, typeof(T).Name));
        }
    }
}
