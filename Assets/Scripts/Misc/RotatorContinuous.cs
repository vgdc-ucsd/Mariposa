using UnityEngine;

public class RotatorContinuous : MonoBehaviour
{
    [SerializeField] private float speed;

    void Update()
    {
        transform.Rotate(speed * Time.deltaTime * Vector3.back);
    }
}
