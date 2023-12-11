using System.Collections.Generic;
using UnityEngine;

namespace TPFive.Extended.LocalConfig
{
    using GameConfig = TPFive.Game.Config;

    public sealed partial class ServiceProvider
    {
#if UNITY_INCLUDE_TESTS
        public IReadOnlyDictionary<string, int> IntValueTable => _intValueTable;

        public IReadOnlyDictionary<string, float> FloatValueTable => _floatValueTable;

        public IReadOnlyDictionary<string, string> StringValueTable => _stringValueTable;

        public IReadOnlyDictionary<string, ScriptableObject> ScriptableObjectValueTable => _scriptableObjectValueTable;

        public IReadOnlyDictionary<string, object> ObjectValueTable => _objectValueTable;
#endif
    }
}
