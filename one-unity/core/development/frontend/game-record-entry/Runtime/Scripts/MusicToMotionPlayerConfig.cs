using UnityEngine;

namespace TPFive.Game.Record.Entry
{
    [CreateAssetMenu(fileName = "Music To Motion Player Config", menuName = "TPFive/Record/MusicToMotionPlayerConfig")]
    public class MusicToMotionPlayerConfig : ScriptableObject
    {
        [SerializeField]
        private GameObject motionPlayerPrefab;
        [SerializeField]
        private string musicToMotionStreamingAssetsPath;

        [SerializeField]
        private GameObject dummyAvatarPrefab;
        [SerializeField]
        private Rect moveRange = new (0f, 0.3f, 4f, 2f);

        public GameObject MotionPlayerPrefab => motionPlayerPrefab;

        public string MusicToMotionStreamingAssetsPath => musicToMotionStreamingAssetsPath;

        public GameObject DummyAvatarPrefab => dummyAvatarPrefab;

        public Rect MoveRange => moveRange;
    }
}
