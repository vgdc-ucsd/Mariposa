using UnityEngine;

public static class Helper
{
    public static void SetLocalScaleX(this GameObject anObject, float x)
    { 
        float scaleY = anObject.transform.localScale.y;
        float scaleZ = anObject.transform.localScale.z;
        anObject.transform.localScale = new Vector3(x, scaleY, scaleZ);
	}

    public static void SetLocalScaleY(this GameObject anObject, float y)
    { 
        float scaleX = anObject.transform.localScale.x;
        float scaleZ = anObject.transform.localScale.z;
        anObject.transform.localScale = new Vector3(scaleX, y, scaleZ);
	}

    public static void SetLocalScaleZ(this GameObject anObject, float z)
    { 
        float scaleX = anObject.transform.localScale.x;
        float scaleY = anObject.transform.localScale.y;
        anObject.transform.localScale = new Vector3(scaleX, scaleY, z);
	}
}
