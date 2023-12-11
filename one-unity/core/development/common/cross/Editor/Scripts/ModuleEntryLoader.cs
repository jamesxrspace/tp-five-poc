using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace TPFive.Cross.Editor
{
    [InitializeOnLoad]
    public class ModuleEntryLoader
    {
        static ModuleEntryLoader()
        {
            var calls = FindAllInitializerTypes();
            calls.Sort((a, b) => a.order - b.order);

            var onLoadBegin = "OnLoadBegin";
            var onLoadEnd = "OnLoadEnd";

            var someParams = new object();
            foreach (var call in calls)
            {
                var method = call.type.GetMethod(
                    onLoadBegin,
                    BindingFlags.Static | BindingFlags.NonPublic);
                method?.Invoke(null, new object[] { someParams });
            }

            foreach (var call in calls)
            {
                var method = call.type.GetMethod(
                    onLoadEnd,
                    BindingFlags.Static | BindingFlags.NonPublic);
                method?.Invoke(null, new object[] { someParams });
            }
        }

        private static List<(Type type, int order)> FindAllInitializerTypes()
        {
            var calls = new List<(Type type, int order)>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    var attributes = type.GetCustomAttributes(
                        typeof(OrderedInitializeOnLoadAttribute), false);
                    if (attributes.Length == 0)
                    {
                        continue;
                    }

                    var attribute = attributes[0] as OrderedInitializeOnLoadAttribute;
                    calls.Add((type, attribute.Order));
                }
            }

            if (calls.Exists(call =>
                    calls.Exists(call2 =>
                        call != call2 && call.order == call2.order)))
            {
                throw new InvalidOperationException(
                    $"Found duplicate order for attribute {nameof(OrderedInitializeOnLoadAttribute)}");
            }

            return calls;
        }
    }
}
