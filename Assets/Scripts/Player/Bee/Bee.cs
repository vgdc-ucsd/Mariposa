using UnityEngine;

public class Bee : MonoBehaviour
{
    public BeeMovement Movement;

    private void Awake()
    {
        Movement = GetComponent<BeeMovement>();
    }
}