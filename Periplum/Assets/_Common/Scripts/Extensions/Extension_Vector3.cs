using UnityEngine;


public static class Extension_Vector3
{
	public static Vector3 FlatXY(this Vector3 vector) => new Vector3(vector.x, vector.y, 0);
	public static Vector3 FlatXZ(this Vector3 vector) => new Vector3(vector.x, 0, vector.z);

	public static Vector2 XY(this Vector3 vector) => new Vector2(vector.x, vector.y);
	public static Vector2 XZ(this Vector3 vector) => new Vector2(vector.x, vector.z);

    public static Vector3 Divide(this Vector3 self, Vector3 vector3)
    {
        return new Vector3(
                self.x / vector3.x,
                self.y / vector3.y,
                self.z / vector3.z);
    }

    public static bool IsOnLine(this Vector3 self, Vector3 start, Vector3 end, float colinearTolerance = 0.00001f)
    {
        bool lAreColinear = self.x * (start.y - end.y) + start.x * (end.y - self.y) + end.x * (self.y - start.y) < colinearTolerance;
        bool lIsBetween = Mathf.Min(start.x, end.x) <= self.x &&
                          self.x <= Mathf.Max(start.x, end.x) &&
                          Mathf.Min(start.y, end.y) <= self.y &&
                          self.y <= Mathf.Max(start.y, end.y);

        return lAreColinear && lIsBetween;
    }
}