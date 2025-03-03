using Unity.VisualScripting;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public InteractionTrigger DefaultInteractionTrigger;
    public override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        //Debug.Log(Player.ActivePlayer.Movement.InsideTriggers.ToCommaSeparatedString());
        //if (Player.ActivePlayer.Ability is BeeControlAbility b)
        //{
        //    Debug.Log(b.BeeRef.Movement.InsideTriggers.ToCommaSeparatedString());
        //}
        
    }
}
