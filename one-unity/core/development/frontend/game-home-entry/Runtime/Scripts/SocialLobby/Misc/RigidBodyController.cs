using System;
using UnityEngine;

namespace TPFive.Home.Entry.SocialLobby
{
    public class RigidBodyController : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody rigidbody;

        [Min(1.0f)]
        [SerializeField]
        private float maxLinearVelocity;

        protected void Start()
        {
            ChangeMaxLinearVelocity();
        }

        private void ChangeMaxLinearVelocity()
        {
            Debug.Log($"{nameof(ChangeMaxLinearVelocity)} from {rigidbody.maxLinearVelocity} to {maxLinearVelocity}");
            rigidbody.maxLinearVelocity = maxLinearVelocity;
        }
    }
}