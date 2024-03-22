using System;
using UnityEngine;

public class TransformUtils
{
    public static bool IsAnyDamageableObject(Transform transform)
    {
        return IsTransformMatchTags(transform, Consts.AllDamageableTags);
    }

    public static bool IsCollectible(Transform transform)
    {
        return IsTransformMatchTags(transform, Consts.ItemTags);
    }

    public static bool IsPlayerDamageableObject(Transform transform)
    {
        return IsTransformMatchTags(transform, Consts.PlayerDamageableTags);
    }

    public static Transform GetAncestorMatchTags(Transform transform, string[] tags)
    {
        while (transform != null)
        {
            if (IsTransformMatchTags(transform, tags))
            {
                return transform;
            }
            transform = transform.parent;
        }

        return null;
    }

    /// <summary>
    /// Get the ancestor of the transform with the matching tag
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tag"></param>
    /// <returns>matching ancestor or null if absent</returns>
    public static Transform GetAncestorMatchTag(Transform transform, string tag)
    {
        string[] tags = { tag };

        return GetAncestorMatchTags(transform, tags);
    }

    public static bool IsTransformMatchTags(Transform transform, string[] tags)
    {
        return Array.Exists(tags, element => transform.CompareTag(element));
    }
}

