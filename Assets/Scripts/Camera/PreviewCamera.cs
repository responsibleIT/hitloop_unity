using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewCamera : MonoBehaviour
{
    [SerializeField] private float distance;
    [SerializeField] private float heightOffset;
    [SerializeField] private float lookAngle;

    internal RenderTexture renderTexture;
    internal int widthRT, heightRT;

    [SerializeField] private new Camera camera;

    private Transform focusTransform;

    private int layerMask;

    internal void Initialize()
    {
        RefreshRenderTexture(true);

        layerMask = LayerMask.NameToLayer("Preview");
        camera.cullingMask = 1 << layerMask;
    }

    private void Update()
    {
        if (focusTransform == null)
            return;

        if (renderTexture.width != widthRT || renderTexture.height != heightRT)
            RefreshRenderTexture();
    }

    internal void Activate(Transform focusTransform)
    {
        if (focusTransform != null)
            Deactivate();

        this.focusTransform = focusTransform;

        GameObjectUtils.ChangeLayerRecursively(focusTransform, layerMask);

        focusTransform.gameObject.layer = layerMask;

        transform.position = focusTransform.position + focusTransform.forward * -distance + new Vector3(0, heightOffset, 0);
        transform.rotation = Quaternion.Euler(new Vector3(lookAngle, 0));

        RefreshRenderTexture();

        gameObject.SetActive(true);
    }

    internal void Deactivate()
    {
        focusTransform = null;

        gameObject.SetActive(false);
    }

    internal void RefreshRenderTexture(bool forced = false)
    {
        if (forced || ((renderTexture.width != widthRT || renderTexture.height != heightRT) && (widthRT != 0 && heightRT != 0)))
        {
            renderTexture = new RenderTexture(Mathf.Max(1, widthRT), Mathf.Max(1, heightRT), 24, RenderTextureFormat.ARGB32);
            renderTexture.filterMode = FilterMode.Bilinear;
        }

        camera.targetTexture = renderTexture;
    }
}
