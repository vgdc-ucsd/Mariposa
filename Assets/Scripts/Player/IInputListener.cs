
using UnityEngine;

public interface IInputListener
{
    public void HorzMoveTowards(int dir) { }
    public void VertMoveTowards(int dir) { }
    public void JumpInputDown() { }
    public void AbilityInputDown() { }
    public void AbilityInputUp() { }

}