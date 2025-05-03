using UnityEngine;

public enum BeeState
{

}

public class Bee : MonoBehaviour
{
    public static Bee Instance;
    public BeeMovement Movement;
    public float MaxControlRadius;
    public float FollowRadius = 2f;
    public bool IsControlled = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        Movement = GetComponent<BeeMovement>();
    }

    public void ToggleControl(bool toggle)
    {
        IsControlled = toggle;
        Movement.ToggleCollisions(toggle);
        if (toggle)
        {
            PlayerController.Instance.StartControlling(Movement);
        }
        else
        {
            PlayerController.Instance.StartControlling(Player.ActivePlayer.Movement);
            StartFollow();
        }
    }

    public bool CanBeeJump()
    {
        return Movement.CurrentBehavior is Follow;
    }

    public void TriggerBeeJump()
    {
        if (Movement.CurrentBehavior is not Follow) return;
        Movement.SetBehavior(new JumpAssist());
    }

    public void StartFollow()
    {
        Movement.SetBehavior(new Follow(2f, FollowRadius));
    }


}


