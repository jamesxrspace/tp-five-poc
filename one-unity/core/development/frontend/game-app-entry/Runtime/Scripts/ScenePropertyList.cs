using System.Collections.Generic;
using UnityEngine;

namespace TPFive.Game.App.Entry
{
    using TPFive.Game;

    [CreateAssetMenu(fileName = "ScenePropertyList", menuName = "TPFive/App/ScenePropertyList")]
    public class ScenePropertyList : ScriptableObject
    {
        [SerializeField]
        private List<SceneProperty> sceneProperties;

        public List<SceneProperty> SceneProperties => sceneProperties;
    }
}
