using UnityEngine;



public class PlayerMovement : FreeBody
{
    public float MoveSpeed = 10;

    public static PlayerMovement Instance;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    protected virtual void Update()
    {

        // very temporary code until we get proper input handling set up
        if (Input.GetKey(KeyCode.D)) Move(1);
        else if (Input.GetKey(KeyCode.A)) Move(-1);
        else Move(0);

        if (Input.GetKeyDown(KeyCode.Space)) Jump(1);


    }

    // Move by player input: 1 = right, -1 = left
    private void Move(int dir)
    {
        if (dir == 0) Velocity.x = 0;
        if ((dir == 1 && !touchingRight) || (dir == -1 && !touchingLeft))
        Velocity.x = MoveSpeed * dir;
    }

    private void Jump(int dir)
    {
        Velocity.y = MoveSpeed * dir; 
    }

}
