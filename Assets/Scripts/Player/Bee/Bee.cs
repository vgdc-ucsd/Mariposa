using UnityEngine;

public class Bee : MonoBehaviour
{
    public BeeMovement Movement;
    public float MaxControlRadius;


    private void Awake()
    {
        Movement = GetComponent<BeeMovement>();
    }
}