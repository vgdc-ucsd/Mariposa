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
    [SerializeField] private SpriteRenderer beeRadius;
    [SerializeField] private Animator beeAnimations;
    private const string ACTIVE_ANIMATION = "Bee_Active";
    private const string IDLE_ANIMATION = "Bee_Idle";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        Movement = GetComponent<BeeMovement>();
    }

    public void ToggleControl(bool toggle)
    {
        /*if (toggle && DialogueManager.Instance != null && DialogueManager.Instance.IsPlayingDialogue)
        {
            return;
        }*/

        IsControlled = toggle;
        Movement.ToggleCollisions(toggle);
        if (toggle)
        {
            beeAnimations.Play(ACTIVE_ANIMATION);
            PlayerController.Instance.StartControlling(Movement);
        }
        else
        {
            beeAnimations.Play(IDLE_ANIMATION);
            beeRadius.color = Color.clear;
            PlayerController.Instance.StartControlling(Player.ActivePlayer.Movement);
            Movement.SetBehavior(new Stay());
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

    public void SetRadiusSprite(float alpha)
    {
        if (Player.ActivePlayer.Ability is BeeControlAbility)
        {
            beeRadius.color = new Color(1.0f, 1.0f, 1.0f, alpha);
        }
        else
        {
            beeRadius.color = Color.clear;            
        }
    }
}


