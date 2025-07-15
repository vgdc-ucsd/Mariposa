using UnityEngine;

public class BigRobotLevelCompleteTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            BigRobotLevel.Instance.CompleteLevel();
    }
}
