using UnityEngine;

public static class Extension_Transform
{
    /// <summary>
    /// Call each frame for smoothly rotating to the beneath surface.
    /// Best used with a transform which is rotated by nothing else.
    /// The used transform must be the child of a parent, which may be rotated for all needs, and which will be used as a reference for directions.
    /// </summary>
    /// <param name="surfaceMask">LayerMask used for checking surface</param>
    /// <param name="smooth">When approaching 0: rotation takes a long time. When approaching 1: rotation is instantaneous</param>
    /// <returns>Returns true if a surface was found beneath transform</returns>
    public static bool SmoothRotateToSurface(this Transform transform, int surfaceMask, float smooth = 0.15f)
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f, surfaceMask))
        {
            Quaternion lTargetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.parent.rotation;

            // Apply smoothed rotation to the transform
            transform.rotation = Quaternion.Lerp(transform.rotation, lTargetRotation, smooth);

            return true;
        }

        return false;
    }
}