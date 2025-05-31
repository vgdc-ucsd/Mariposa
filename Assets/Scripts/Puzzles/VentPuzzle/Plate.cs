using UnityEngine;
using System.Collections.Generic;

public class Plate : MonoBehaviour
{
    [SerializeField]
    private List<VentGrate> grates;

    [SerializeField]
    private List<VelocityField> fanFields;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (VentGrate _grate in grates)
        {
            _grate.OnGrateToggle();
        }

        foreach (VelocityField _field in fanFields)
        {
            _field.RecomputeFieldCollider();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        foreach (VentGrate _grate in grates)
        {
            _grate.OnGrateToggle();
        }
        
        foreach (VelocityField _field in fanFields)
        {
            _field.RecomputeFieldCollider();
        }
    }
}
