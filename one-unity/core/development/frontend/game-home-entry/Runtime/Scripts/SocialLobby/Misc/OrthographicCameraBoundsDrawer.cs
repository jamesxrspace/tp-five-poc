using UnityEngine;
using UnityEngine.Serialization;

namespace TPFive.Home.Entry.SocialLobby
{
    public class OrthographicCameraBoundsDrawer : MonoBehaviour
    {
        [FormerlySerializedAs("orthoGraphicCamera")]
        [FormerlySerializedAs("camera")]
        [SerializeField]
        private Camera orthographicCamera;

        protected void OnValidate()
        {
            if (orthographicCamera == null)
            {
                orthographicCamera = Camera.main;
            }
        }

        protected void OnDrawGizmos()
        {
            if (orthographicCamera == null || !orthographicCamera.orthographic)
            {
                return;
            }

            var height = orthographicCamera.orthographicSize * 2.0f;
            var aspect = orthographicCamera.aspect;
            var width = height * aspect;
            var nearClipPlane = orthographicCamera.nearClipPlane;
            var depth = orthographicCamera.farClipPlane - nearClipPlane;

            Gizmos.color = Color.cyan;
            var previousMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;
            Vector3 size = new Vector3(
                width,
                height,
                depth);
            Gizmos.DrawWireCube(new Vector3(0, 0, (size.z / 2) + nearClipPlane), size);
            Gizmos.matrix = previousMatrix;
        }
    }
}