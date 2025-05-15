using System.Collections;
using NUnit.Framework.Internal.Commands;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using FMODUnity;

public class RobotMovement : FreeBody
{
    private Vector2 moveDir = Vector2.zero;
    private bool onWalkableSlope = false;
    private Vector2 slopeDir = Vector2.zero;

    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    [SerializeField] private float movementSpeed;

  

    // Useful for when their dependent values are changed during runtime
    private void InitDerivedConsts()
    {
    }

    protected override void Awake()
    {
        base.Awake();

        InitDerivedConsts();

    }

    protected override void Update()
    {
        base.Update();
        /*
        if (Input.GetKeyDown(KeyCode.V)) // TODO make this an input action
        {
            // Consume a battery if available
            if (InventoryManager.Instance.GetItemCount(InventoryType.Mariposa, BatteryItem.Instance) > 0)
            {
                InventoryManager.Instance.DeleteItem(InventoryType.Mariposa, BatteryItem.Instance);
                StartCoroutine(DashRoutine());
            }
        }
        */
    }

    private void OnValidate()
    {
        if (Application.isPlaying) InitDerivedConsts();
    }

    protected override void FixedUpdate()
    {
        Move();
        base.FixedUpdate();
    }

    public void SetMoveDir(Vector2 dir)
    {
        moveDir = dir;
    }

    private void Move()
    {
        int dir = Mathf.RoundToInt(moveDir.x);


        // Check which acceleration parameter to use
        float accelerationParam = (dir * Velocity.x > 0)
            ? this.acceleration
            : deceleration;
        // Compute the difference between the max velocity and the current velocity
        float deltaV = dir * movementSpeed - Velocity.x;
        // If the acceleration parameter would cause the player to exceed their maximum speed,
        // use enough acceleration to reach maximum speed. Otherwise, use the acceleration parameter
        float acceleration = Mathf.Min(Mathf.Abs(deltaV / fdt), accelerationParam);

        Vector2 axis = onWalkableSlope ? slopeDir : Vector2.right;

        // Apply the acceleration
        Velocity += acceleration * fdt * Mathf.Sign(deltaV) * axis;
    }




}