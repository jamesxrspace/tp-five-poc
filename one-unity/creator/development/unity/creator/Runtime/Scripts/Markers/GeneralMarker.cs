using System.Collections.Generic;
using UnityEngine;

namespace TPFive.Creator
{
    // [CustomStyle("GeneralMarker")]
    public sealed class GeneralMarker : MarkerBase
    {
        [SerializeField]
        private List<int> intParams;

        [SerializeField]
        private List<float> floatParams;

        [SerializeField]
        private List<string> stringParams;

        [SerializeField]
        private List<GameObject> gameObjectParams;

        [SerializeField]
        private List<ScriptableObject> scriptableObjectParams;

        public List<int> IntParams => intParams;

        public List<float> FloatParams => floatParams;

        public List<GameObject> GameObjectParams => gameObjectParams;

        public List<string> StringParams => stringParams;

        public List<ScriptableObject> ScriptableObjectParams => scriptableObjectParams;

#pragma warning disable SA1300
        public PropertyName id => $"{typeof(GeneralMarker)}";
#pragma warning restore SA1300
    }
}