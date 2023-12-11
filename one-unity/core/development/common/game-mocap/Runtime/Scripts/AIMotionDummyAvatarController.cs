using UnityEngine;

namespace TPFive.Game.Mocap
{
    public class AIMotionDummyAvatarController : MonoBehaviour
    {
        [SerializeField]
        private GameObject _avatarModel;
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private SkinnedMeshRenderer _faceSkinnedMeshRenderer;

        public GameObject AvatarModel => _avatarModel;

        public Animator Animator => _animator;

        public SkinnedMeshRenderer FaceSkinnedMeshRenderer => _faceSkinnedMeshRenderer;
    }
}
