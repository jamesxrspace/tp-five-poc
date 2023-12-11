using UnityEngine;

namespace TPFive.Home.Entry.SocialLobby
{
    public class EntityDataGroup : MonoBehaviour
    {
        [SerializeField]
        private int offset;

        [SerializeField]
        private int size;

        [SerializeField]
        private Transform left;

        [SerializeField]
        private Transform right;

        public int Offset => offset;

        public int Size => size;

        public Vector3 LeftWorldPosition => left.position;

        public Vector3 RightWorldPosition => right.position;

        public bool IsFetched { get; set; }
    }
}