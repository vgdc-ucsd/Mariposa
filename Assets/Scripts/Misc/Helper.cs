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
}
