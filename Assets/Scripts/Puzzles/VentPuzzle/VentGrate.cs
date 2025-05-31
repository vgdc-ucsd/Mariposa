using UnityEngine;

public class VentGrate : MonoBehaviour
{
    public void OnGrateToggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
