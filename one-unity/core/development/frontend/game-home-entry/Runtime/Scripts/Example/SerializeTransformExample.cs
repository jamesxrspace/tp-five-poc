using System;
using UnityEngine;

namespace TPFive.Home.Entry.Example
{
    internal class SerializeTransformExample : MonoBehaviour
    {
        [SerializeField]
        private Transform target;

        [SerializeField]
        private Transform applyTarget;

        [ContextMenu("Test/Serialize Transform")]
        public void SerializeTransform()
        {
            try
            {
                var json = target.ToJson();
                Debug.Log(json);
                applyTarget.FromJson(json);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}