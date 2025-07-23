using UnityEngine;
using System.Collections;
using Microsoft.Unity.VisualStudio.Editor;
using System;
using UnityEditor.EditorTools;

public class DowntownTurret : MonoBehaviour
{

    public enum TurretType
    {
        Projectile,
        Laser,
    }

    public bool IsOn;

    public bool HasBattery;
    public bool IsTargetInRange;

    public bool isFocusing;
    public bool isOnCooldown;

    // These three are for testing, need to use anim in the future
    [SerializeField] private GameObject turretHead;
    [SerializeField] private GameObject turretCenter;
    [SerializeField] private GameObject turretBase;
    [SerializeField] private GameObject chargingPoint;
    [SerializeField] private GameObject projectile;
    [SerializeField] private TurretType type;
    [SerializeField] private GameObject playerTargetObj;

    // -------- private variables --------
    [Header("Turret Attribute")]
    private LayerMask playerLayer;
    private LayerMask playerAndEnvLayer;
    [SerializeField][Range(0.0f, 1000.0f)] private float rotationSpeed;
    [SerializeField][Range(0.1f, 10.0f)] private float attackCooldownDuration;
    private float attackCooldownCounter;
    [SerializeField][Range(0.1f, 10)] private float projectileFocusDuration;
    private float projectileFocusCounter;
    [SerializeField][Range(0.1f, 10)] float chargePointSize;
    private float chargeFocusRate;

    [Header("Range Detector")]
    [SerializeField][Range(0.1f, 10.0f)] float rangeRadius;

    [Tooltip("Determines how precise turret is. The greater the number, the farther the player can be before the turret starts to charge. Number is in degrees.")]
    [SerializeField][Range(0.0f, 90.0f)] float precision;

    [Header("Sprites")]
    [SerializeField] private Image activeSprite;
    [SerializeField] private Image deactivatedSprite;

    private void Start()
    {
        IsOn = true;
        ResetState();

        playerLayer = LayerMask.GetMask("Player");
        playerAndEnvLayer = LayerMask.GetMask("Player", "Barrier");

        playerTargetObj = Array.Find(GameObject.FindGameObjectsWithTag("Player"), player => player.name == "Unnamed Player");
        if (playerTargetObj == null)
        {
            Debug.LogWarning("Target player not assigned on this turret");
        }
    }


    // handles all game states; firing, charging, etc.
    private void Update()
    {
        float dt = Time.deltaTime;
        // handle cooldowns
        if (isOnCooldown)
        {
            HandleCooldowns(dt);
        }

        // check if on
        if (!IsOn || isOnCooldown || playerTargetObj == null) return;

        // check if target is in range
        if (CheckForTargetInRange())
        {
            IsTargetInRange = true;
        }
        else
        {
            // if moves out of range, reset stuff
            if (IsTargetInRange)
            {
                IsTargetInRange = false;
                ResetState();
            }
            return;
        }

        // if made it this far, target is in range

        // 0.0174533f = pi / 180.0f;
        Vector3 playerDirectionVector = (playerTargetObj.transform.position - chargingPoint.transform.position).normalized;
        Vector3 currentFaceDirection = new((float)Math.Cos(turretHead.transform.eulerAngles.z * 0.0174533f), (float)Math.Sin(turretHead.transform.eulerAngles.z * 0.0174533f));
        // Debug.DrawRay(chargingPoint.transform.position, currentFaceDirection, Color.yellow, 0.5f);

        float angleBetweenTurretAndPlayer = Vector2.SignedAngle(currentFaceDirection, playerDirectionVector);

        // lock on to target
        isFocusing = true;
        TurnToTarget(dt, angleBetweenTurretAndPlayer);

        // if locked on (enough), charge projectile
        if (Math.Abs(angleBetweenTurretAndPlayer) <= precision)
        {
            ChargeProjectile(dt);
        }
        else
        {
            projectileFocusCounter = 0.0f;
            chargingPoint.transform.localScale = new(0.0f, 0.0f, 1.0f);
            isFocusing = false;
        }

        // if charged up, fire
        if (projectileFocusCounter >= projectileFocusDuration)
        {
            FireProjectile(dt, (Vector2)currentFaceDirection);
            ResetState();
            isOnCooldown = true;
        }
    }

    // check if target object (player) is in range
    public bool CheckForTargetInRange()
    {
        Vector3 turretPos = chargingPoint.transform.position;

        // Collider2D rangeCheck = Physics2D.OverlapCircle(turretPos, rangeRadius, playerLayer);
        // if (rangeCheck.gameObject.CompareTag("Player"))
        // {
        //     Vector3 playerDirectionVector = (rangeCheck.gameObject.transform.position - turretPos).normalized;
        //     RaycastHit2D ray = Physics2D.Raycast(turretPos, playerDirectionVector, rangeRadius, playerAndEnvLayer);
        //     if (ray.collider.gameObject.CompareTag("Player")) ;
        // }
        // return null;

        Vector3 playerDirectionVector = (playerTargetObj.transform.position - turretPos).normalized;
        RaycastHit2D ray = Physics2D.Raycast(turretPos, playerDirectionVector, rangeRadius, playerAndEnvLayer);
        if (!ray) return false;
        // Debug.DrawRay(turretPos, playerDirectionVector, Color.white, 0.5f);
        return ray.collider.gameObject.CompareTag("Player");
    }

    // reset cooldowns and lots of logical states (arbitrary because none of this is planned)
    public void ResetState()
    {
        attackCooldownCounter = 0.0f;
        projectileFocusCounter = 0.0f;

        isFocusing = false;
        isOnCooldown = false;

        chargeFocusRate = chargePointSize / projectileFocusDuration;
        chargingPoint.transform.localScale = new(0.0f, 0.0f, 1.0f);
    }

    // called when battery is removed. also shuts down the turret
    public void RemoveBattery()
    {
        HasBattery = false;
        ShutDown();
    }

    // shutdown procedure
    public void ShutDown()
    {
        IsOn = false;

        float turretHeadZRotation = turretHead.transform.eulerAngles.z;
        turretCenter.SetActive(false);
        turretHead.transform.rotation = Quaternion.identity;
        turretHead.transform.Translate(0.0f, -1.0f, 0.0f);
        turretHead.transform.Rotate(Vector3.forward, turretHeadZRotation);

        turretHead.GetComponent<SpriteRenderer>().color = Color.gray;
        turretBase.GetComponent<SpriteRenderer>().color = Color.gray;
    }

    // locks on to first target with the tag of "player"
    public void TurnToTarget(float dt, float angle)
    {
        float angularDisplacement = Math.Min(rotationSpeed * dt, Math.Abs(angle));
        if (angle < 0.0f) angularDisplacement *= -1.0f;
        turretHead.transform.Rotate(Vector3.forward, angularDisplacement);
    }

    // charges projectile if locked on
    public void ChargeProjectile(float dt)
    {
        // Debug.Log("Locked");
        projectileFocusCounter += dt;

        // focus beam with the sphere
        float chargingPointSize = chargePointSize * projectileFocusCounter / projectileFocusDuration;
        chargingPoint.transform.localScale = new(chargingPointSize, chargingPointSize, 1.0f);
    }

    // self-explanatory
    public void FireProjectile(float dt, Vector2 direction)
    {
        GameObject newProjOBj = GameObject.Instantiate(projectile, transform.position, Quaternion.identity, transform);
        if (type == TurretType.Laser)
        {
            newProjOBj.GetComponent<LaserProjectile>().Initialize(chargingPoint.transform.position, direction);
        }
        else
        {
            newProjOBj.GetComponent<BallProjectile>().Initialize(chargingPoint.transform.position, direction);
        }
        // Debug.Log("boom");
    }

    // executes when on cooldown (turret should be unable to do anything)
    public void HandleCooldowns(float dt)
    {
        attackCooldownCounter += dt;
        if (attackCooldownCounter >= attackCooldownDuration)
        {
            ResetState();
        }
    }

}