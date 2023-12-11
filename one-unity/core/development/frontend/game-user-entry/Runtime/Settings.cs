using UnityEngine;

namespace TPFive.Game.User.Entry
{
    [CreateAssetMenu(fileName = "UserEntrySettings", menuName = "TPFive/User/Settings - Entry")]
    public class Settings : ScriptableObject
    {
        [SerializeField]
        private string category;
        [SerializeField]
        private string currentScene;
        [SerializeField]
        private string nextScene;
        [SerializeField]
        private string editorScene;

        public string Category => category;

        public string CurrentScene => currentScene;

        public string NextScene => nextScene;

        public string EditorScene => editorScene;
    }
}
