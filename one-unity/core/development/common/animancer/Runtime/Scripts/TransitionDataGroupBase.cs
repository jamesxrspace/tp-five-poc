using System;
using System.Collections.Generic;
using UnityEngine;

namespace TPFive.Extended.Animancer
{
    /// <summary>
    /// Group of transition data.
    /// </summary>
    /// <typeparam name="TKey">the key of you want to identify the data.</typeparam>
    /// <typeparam name="TData">the data of you want to play by specific key.</typeparam>
    public abstract class TransitionDataGroupBase<TKey, TData> :
        ScriptableObject,
        ISerializationCallbackReceiver
        where TData : TransitionDataBase
    {
        private readonly Dictionary<TKey, TData> itemDict = new Dictionary<TKey, TData>();

        [SerializeField]
        private Pair[] items;

        public bool TryGetData(TKey key, out TData data)
        {
            return itemDict.TryGetValue(key, out data);
        }

        public void OnBeforeSerialize()
        {
            // do nothing
        }

        public void OnAfterDeserialize()
        {
            itemDict.Clear();

            foreach (var item in items)
            {
                itemDict.Add(item.Key, item.Data);
            }
        }

        [Serializable]
        private struct Pair
        {
            public TKey Key;

            public TData Data;
        }
    }
}
