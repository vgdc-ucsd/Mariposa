using UnityEngine;

public static class Helper
{
    /// <summary>
    /// Set the LocalScale.x and keep the y and z the same
    /// </summary>
    /// <param name="anObject"></param>
    /// <param name="x"></param>
    public static void SetLocalScaleX(this GameObject anObject, float x)
    { 
        float scaleY = anObject.transform.localScale.y;
        float scaleZ = anObject.transform.localScale.z;
        anObject.transform.localScale = new Vector3(x, scaleY, scaleZ);
	}

    /// <summary>
    /// Set the anObject.transform.localScale.y and keep the x and z the same
    /// </summary>
    /// <param name="anObject"></param>
    /// <param name="y"></param>
    public static void SetLocalScaleY(this GameObject anObject, float y)
    { 
        float scaleX = anObject.transform.localScale.x;
        float scaleZ = anObject.transform.localScale.z;
        anObject.transform.localScale = new Vector3(scaleX, y, scaleZ);
	}

    /// <summary>
    /// Set the anObject.transform.localScale.z and keep the x and y the same
    /// </summary>
    /// <param name="anObject"></param>
    /// <param name="z"></param>
    public static void SetLocalScaleZ(this GameObject anObject, float z)
    { 
        float scaleX = anObject.transform.localScale.x;
        float scaleY = anObject.transform.localScale.y;
        anObject.transform.localScale = new Vector3(scaleX, scaleY, z);
	}

    /// <summary>
    /// Computes the minimum separation between a point and a rect the point, where the separation is a vector 
    /// with a magnitude equal to the minimum distance between the point and an edge of the rect
    /// and a direction towards the rect
    /// </summary>
    /// <param name="point"></param>
    /// <param name="rect"></param>
    /// <returns>The minimum separation vector, or the zero vector if the point is in the rect</returns>
    public static Vector2 MinPointRectSeparation(Vector2 point, Rect rect)
    {
        if (rect.Contains(point)) return Vector2.zero;

        Vector2 separation = Vector2.zero;

        if (point.x < rect.xMin) separation.x = rect.xMin - point.x;
        else if (point.x > rect.xMax) separation.x = rect.xMax - point.x;
        if (point.y < rect.yMin) separation.y = rect.yMin - point.y;
        else if (point.y > rect.yMax) separation.y = rect.yMax - point.y;

        return separation;
    }


    /// <summary>
    /// Draws a rectangle for debugging purposes
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="color"></param>
    /// <param name="time">the time the draw rectangle with last</param>
    public static void DebugDrawRect(Rect rect, Color color, float time)
    {
        Debug.DrawLine(new Vector2(rect.xMin, rect.yMax), rect.min, color, time);
        Debug.DrawLine(new Vector2(rect.xMin, rect.yMax), rect.max, color, time);
        Debug.DrawLine(new Vector2(rect.xMax, rect.yMin), rect.min, color, time);
        Debug.DrawLine(new Vector2(rect.xMax, rect.yMin), rect.max, color, time);
    }
}
