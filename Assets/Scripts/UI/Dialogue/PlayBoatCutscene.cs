using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayBoatCutscene : DialogueEvent
{
    [SerializeField] private Transform boat;
    [SerializeField] private float speed;
    [SerializeField] private float duration;

    public override void Trigger()
    {
        StartCoroutine(MoveBoat());
    }

    IEnumerator MoveBoat()
    {
        float timer = 0;
        while (timer < duration)
        {
            yield return new WaitForEndOfFrame();
            PlayerController.Instance.SetMovementLock(true); // TODO: fix properly, improve dialogue 'end' flag
            timer += Time.deltaTime;
            PlayerController.Instance.ControlledPlayer.transform.SetParent(boat);
            boat.Translate(speed * Time.deltaTime * Vector3.right);
        }
		yield return FadeController.Instance.FadeOut();
        
        SceneManager.LoadScene("RobotStage");
    }
}
