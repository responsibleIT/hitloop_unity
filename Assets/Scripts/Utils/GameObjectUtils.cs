using System;
using System.Reflection;
using UnityEngine;

public class GameObjectUtils : MonoBehaviour
{
    internal static void ChangeTagRecursively(Transform transform, string tag)
    {
        if (transform == null)
            return;

        // tag the GO
        transform.gameObject.tag = tag;

        // tag all of its kids
        if (transform.childCount > 0)
            foreach (Transform t in transform)
                ChangeTagRecursively(t, tag);
    }

    internal static void ChangeLayerRecursively(Transform transform, string layer)
    {
        ChangeLayerRecursively(transform, LayerMask.NameToLayer(layer));
    }

    internal static void ChangeLayerRecursively(Transform transform, int layerMask)
    {
        if (transform == null)
            return;

        // tag the GO
        transform.gameObject.layer = layerMask;

        // tag all of its kids
        if (transform.childCount > 0)
            foreach (Transform t in transform)
                ChangeLayerRecursively(t, layerMask);
    }

    internal static Vector3 GetCenterOfObject(Transform transform)
    {
        return GetBounds(transform).center;
    }

    internal static Bounds GetBounds(Transform transform, bool discardUnderground = false)
    {
        // else just return the average pos of all children 
        Bounds bounds = GetRendererBounds(transform.gameObject);

        if (bounds.extents.x == 0)
        {
            bounds = new Bounds(transform.position, Vector3.zero);
        }

        foreach (Transform child in transform)
        {
            if (child.position.y < transform.position.y)
                continue;

            bounds.Encapsulate(GetBounds(child, discardUnderground));
        }

        return bounds;
    }

    private static Bounds GetRendererBounds(GameObject gameObject)
    {
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        Renderer renderer = gameObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds;
        }
        return bounds;
    }

    internal static T FindInParents<T>(GameObject go) where T : Component
    {
        if (go == null) return null;

        var comp = go.GetComponent<T>();

        if (comp != null) return comp;

        var t = go.transform.parent;

        while (t != null && comp == null)
        {
            comp = t.gameObject.GetComponent<T>();
            t = t.parent;
        }

        return comp;
    }

    internal static T GetCopyOf<T>(Component comp, T other) where T : Component
    {
        Type type = comp.GetType();
        if (type != other.GetType()) return null; // type mis-match
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
        PropertyInfo[] pinfos = type.GetProperties(flags);
        foreach (var pinfo in pinfos)
        {
            if (pinfo.CanWrite)
            {
                try
                {
                    pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                }
                catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
            }
        }
        FieldInfo[] finfos = type.GetFields(flags);
        foreach (var finfo in finfos)
        {
            finfo.SetValue(comp, finfo.GetValue(other));
        }
        return comp as T;
    }
}
