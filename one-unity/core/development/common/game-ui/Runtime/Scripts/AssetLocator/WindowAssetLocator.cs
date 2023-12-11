using System;
using System.Collections.Generic;
using TPFive.Extended.Addressable;
using TypeReferences;
using UnityEngine;

namespace TPFive.Game.UI
{
    [CreateAssetMenu(fileName = "WindowAssetLocator", menuName = "TPFive/UI/Window Asset Locator")]
    public sealed class WindowAssetLocator :
        AssetLocatorBase<Type, string>,
        IAddressableAssetLocator<Type>,
        ISerializationCallbackReceiver
    {
        private Dictionary<Type, string> locationDict = new ();

        [SerializeField]
        private List<Location> locations;

        public override string GetLocation(Type key)
        {
            return locationDict.TryGetValue(key, out var value) ? value : default;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            locationDict = new Dictionary<Type, string>();

            if (locations == null)
            {
                return;
            }

            locations.ForEach(x =>
            {
                if (!x.Validate())
                {
                    return;
                }

                if (!locationDict.TryAdd(x.GetWindowType(), x.GetLocation()))
                {
                    Debug.LogWarning($"There more than one location for {x}");
                }
            });
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            // Nothing to do here.
        }

        [Serializable]
        private class WindowReference : ComponentReferenceT<WindowBase>
        {
            public WindowReference(string guid)
                : base(guid)
            {
            }
        }

        [Serializable]
        private class Location
        {
            [SerializeField]
            [Inherits(typeof(WindowBase), ShowAllTypes = true)]
            private TypeReference windowType;

            [SerializeField]
            private WindowReference prefab;

            public Type GetWindowType() => windowType.Type;

            public string GetLocation() => prefab.RuntimeKey as string;

            public bool Validate()
            {
                return windowType != null &&
                    windowType.Type != null &&
                    prefab != null &&
                    prefab.RuntimeKeyIsValid();
            }
        }
    }
}
