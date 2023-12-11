using UnityEngine;

namespace TPFive.Home.Entry.SocialLobby
{
    public class SpawnPoint : MonoBehaviour
    {
        [SerializeField]
        private new MeshRenderer renderer;

        public bool EnableRenderer
        {
            get => renderer.enabled;
            set => renderer.enabled = value;
        }

        public IEntity Occupier { get; private set; }

        public void SetOccupier(IEntity entity)
        {
            Occupier = entity;
        }

        public void ResetOccupier()
        {
            Occupier = null;
        }

        public void SetOccupierLocalPosition(Transform entityLocalTransform)
        {
            if (Occupier == null)
            {
                return;
            }

            Occupier.LocalPosition = entityLocalTransform.InverseTransformPoint(transform.position);
        }
    }
}