using UnityEngine;

namespace TPFive.Game.Resource
{
    /// <summary>
    /// XRSceneObject is a MonoBehaviour, it will be attached to GameObject.
    /// It is used to save or read XRObject data after Serialize or Deserialize.
    /// </summary>
    public partial class XRSceneObject : MonoBehaviour
    {
        private XRObject _xrObject;

        public XRObject XRObject
        {
            get => _xrObject;
            set => _xrObject = value;
        }
    }
}