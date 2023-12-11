using System.Collections.Generic;
using UnityEngine;

public sealed class CameraFacingBillboard : MonoBehaviour
{
    [SerializeField]
    private bool isLimitRotateOnY = false;
    [SerializeField]
    private bool enableToFixSize = false;
    [SerializeField]
    private List<Transform> excludeObjects;

    [SerializeField]
    [Range(0f, 5f)]
    private float fixedSize = .005f;

    [SerializeField]
    private Camera relativeCamera;

    private Transform cachedTransform;
    private Transform cameraTransform;

    private List<Vector3> scaleOfExcludeObjects;
    private float cacheDistance;

    public void LimitRotateOnY(bool isOn)
    {
        this.isLimitRotateOnY = isOn;
    }

    public void SetRelativeCamera(Camera camera)
    {
        relativeCamera = camera;
    }

    private void Awake()
    {
        cachedTransform = transform;
    }

    private void OnDestroy()
    {
        cameraTransform = cachedTransform = null;
    }

    private void OnEnable()
    {
        if (relativeCamera == null)
        {
            relativeCamera = Camera.main;
        }

        if (relativeCamera != null)
        {
            cameraTransform = relativeCamera.transform;
        }

        if (excludeObjects != null)
        {
            scaleOfExcludeObjects = new List<Vector3>(excludeObjects.Count);
            for (int i = 0; i < excludeObjects.Count; ++i)
            {
                scaleOfExcludeObjects.Add(excludeObjects[i].lossyScale);
            }
        }
    }

    // Orient the camera after all movement is completed this frame to avoid jittering
    private void LateUpdate()
    {
        if (cameraTransform == null)
        {
            return;
        }

        if (isLimitRotateOnY)
        {
            // Only Rotate on Y Axis
            var pos = new Vector3(cameraTransform.position.x, cachedTransform.position.y, cameraTransform.position.z);
            cachedTransform.LookAt((2 * cachedTransform.position) - pos);
        }
        else
        {
            var pos = cachedTransform.position + (cameraTransform.rotation * Vector3.forward);
            cachedTransform.LookAt(pos, worldUp: cameraTransform.rotation * Vector3.up);
        }

        if (enableToFixSize)
        {
            var distance = (relativeCamera.transform.position - cachedTransform.position).magnitude;
            if (cacheDistance == distance)
            {
                return;
            }

            cacheDistance = distance;
            var size = distance * fixedSize * relativeCamera.fieldOfView;
            var sizeReciprocal = 1 / size;
            cachedTransform.localScale = Vector3.one * size;

            if (excludeObjects != null && scaleOfExcludeObjects != null)
            {
                for (int i = 0; i < excludeObjects.Count; i++)
                {
                    if (excludeObjects[i] == null)
                    {
                        continue;
                    }

                    excludeObjects[i].transform.localScale = sizeReciprocal * scaleOfExcludeObjects[i];
                }
            }
        }
    }
}