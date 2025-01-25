using UnityEngine;



public class PlayerMovement : FreeBody
{
    public float MoveSpeed = 10;

    protected override void Update()
    {
        base.Update();

        // very temporary code until we get proper input handling set up
        if (Input.GetKey(KeyCode.D)) Move(1);
        else if (Input.GetKey(KeyCode.A)) Move(-1);
    }

    // Move by player input: 1 = right, -1 = left
    private void Move(int dir)
    {
        Velocity.x = MoveSpeed * dir;
    }

}
