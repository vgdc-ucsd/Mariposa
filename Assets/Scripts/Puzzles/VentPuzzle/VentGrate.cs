using UnityEngine;

public class VentGrate : MonoBehaviour
{
    public void OnGrateEnable()
    {
        gameObject.SetActive(true);
    }

    public void OnGrateDisable()
    {
        gameObject.SetActive(false);
    }
}
