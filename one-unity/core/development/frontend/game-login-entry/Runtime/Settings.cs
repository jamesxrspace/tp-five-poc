using UnityEngine;

namespace TPFive.Game.Login.Entry
{
    [CreateAssetMenu(fileName = "LoginEntrySettings", menuName = "TPFive/Login/Settings - Entry")]
    public class Settings : ScriptableObject
    {
        [SerializeField]
        private string signUpUrl;
        [SerializeField]
        private string currentScene;
        [SerializeField]
        private string nextScene;
        [SerializeField]
        private int[] tokenRefreshIntervals;

        public string SignUpUrl => signUpUrl;

        public string CurrentScene => currentScene;

        public string NextScene => nextScene;

        public int[] TokenRefreshIntervals => tokenRefreshIntervals ?? System.Array.Empty<int>();
    }
}
