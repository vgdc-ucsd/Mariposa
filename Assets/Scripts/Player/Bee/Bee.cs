using UnityEngine;

public enum BeeBehavior
{
    None, Follow
}

public class Bee : MonoBehaviour
{
    public BeeMovement Movement;
    public float MaxControlRadius;
    public bool IsControlled = false;


    private void Awake()
    {
        Movement = GetComponent<BeeMovement>();
    }

    public void ToggleControl(bool toggle)
    {
        IsControlled = toggle;
        if (toggle)
        {
            PlayerController.Instance.StartControlling(Movement);
        }
        else
        {
            PlayerController.Instance.StartControlling(Player.ActivePlayer.Movement);
            Movement.SetBehavior(new Follow(transform, Player.ActivePlayer.transform));
        }
    }


}


