
using UnityEngine;

public interface IInputListener
{
    public void SetMoveDir(Vector2 dir) { }
    public void JumpInputDown() { }
    public void AbilityInputDown() { }
    public void AbilityInputUp() { }

}