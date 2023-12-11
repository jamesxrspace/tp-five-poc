using UnityEngine;

namespace TPFive.Extended.LocalConfig
{
    public sealed partial class ServiceProvider
    {
        private (bool, TValue) InternalGetT<TValue>(string k)
        {
            var result = false;
            var value = default(TValue);

            if (typeof(TValue) == typeof(string))
            {
                result = _stringValueTable.TryGetValue(k, out var v);
                value = (TValue)(object)v;
            }
            else if (typeof(TValue) == typeof(ScriptableObject))
            {
                result = _scriptableObjectValueTable.TryGetValue(k, out var v);
                value = (TValue)(object)v;
            }
            else if (typeof(TValue) == typeof(object))
            {
                result = _objectValueTable.TryGetValue(k, out var v);
                value = (TValue)v;
            }

            return (result, value);
        }

        private bool InternalSetT<TValue>(string k, TValue value)
        {
            var result = false;

            // Here, using (value is SomeType someTypeValue) makes sense because the pass in value is not null,
            // but if it is really null, the result will be false as no case is matched.
            if (value is int intValue)
            {
                _intValueTable[k] = intValue;
                result = true;
            }
            else if (value is float floatValue)
            {
                _floatValueTable[k] = floatValue;
                result = true;
            }
            else if (value is string stringValue)
            {
                _stringValueTable[k] = stringValue;
                result = true;
            }
            else if (value is ScriptableObject scriptableObjectValue)
            {
                _scriptableObjectValueTable[k] = scriptableObjectValue;
                result = true;
            }
            else if (value is object objectValue)
            {
                _objectValueTable[k] = objectValue;
                result = true;
            }

            return result;
        }

        private bool InternalRemoveT<TValue>(string k)
        {
            var result = false;

            // Can not use (value is SomeType someTypeValue) because when value is null,
            // it won't be able to determine the type.
            if (typeof(TValue) == typeof(int))
            {
                result = _intValueTable.Remove(k);
            }
            else if (typeof(TValue) == typeof(float))
            {
                result = _floatValueTable.Remove(k);
            }
            else if (typeof(TValue) == typeof(string))
            {
                result = _stringValueTable.Remove(k);
            }
            else if (typeof(TValue) == typeof(ScriptableObject))
            {
                result = _scriptableObjectValueTable.Remove(k);
            }
            else if (typeof(TValue) == typeof(object))
            {
                result = _objectValueTable.Remove(k);
            }

            return result;
        }
    }
}