using UnityEngine;

public class Plate : MonoBehaviour
{
    [SerializeField]
    private VentGrate _grate;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _grate.OnGrateDisable();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _grate.OnGrateEnable();
    }
}
