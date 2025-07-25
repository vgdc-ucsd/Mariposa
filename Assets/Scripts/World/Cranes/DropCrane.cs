using Unity.VisualScripting;
using UnityEngine;

public class DropCrane : Crane
{
    [SerializeField] protected Rigidbody2D loadRB;
    [SerializeField] private GameObject preemptiveCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // loadRB = platform.gameObject.GetComponent<Rigidbody2D>();
        if (loadRB == null)
        {
            Debug.LogWarning("Warning: Attached crane load does not have a rigidbody");
        }   
    }

    public override void TriggerCrane()
    {
        Debug.Log("dropcrane triggered");
        if (loadRB == null) return;

        preemptiveCollider.SetActive(false);
        loadRB.simulated = true;
        lineRenderer.enabled = false;
        Debug.Log("Dropped");
    }

    public override void ReturnCrane()
    {
        return; 
    }
}
