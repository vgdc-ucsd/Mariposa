using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour
{

    public enum TurretType
    {
        Projectile,
        Laser,
    }

    public bool IsOn { get => isOn; set => isOn = value;}
	public bool IsCharging {get => isCharging; set => isCharging = value;}
	public bool IsFiring  {get => isFiring; set => isFiring = value;}
	public bool IsCoolingDown {get => isCoolingDown; set => isCoolingDown = value;}
    public bool HasBattery { get => hasBattery; set => hasBattery = value;}
    public bool CanRemoveBattery { get => canRemoveBattery; set => canRemoveBattery = value;}

    // These three are for testing, need to use anim in the future
    public GameObject bodyPart;
    public GameObject chargingPoint;
    public GameObject laser;
    public TurretType type;
	ITurretBehaviour turretBehaviour;

    // -------- private variables --------
    [Header("Turret Attribute")]
    [SerializeField] GameObject target;
    [SerializeField] LayerMask hitLayer;
    [SerializeField][Range(0.1f, 10)] float rotationSpeed;
    [SerializeField][Range(0.1f, 10)] float chargeTime;
    [SerializeField][Range(0.1f, 10)] float laserShrinkTime;
    [SerializeField][Range(0.1f, 10)] float chargePointSize;
    [SerializeField][Range(0.1f, 10)] float fireTimeInterval;

    [Header("Children Range Detector")]
    [SerializeField] GameObject attackRangeDetectorObject;
    [SerializeField] GameObject batteryRangeDetectorObject;
    private IRangeDetector rangeDetectorForAttack;
    private IRangeDetector rangeDetectorForBattery;
    
    [SerializeField] private BatteryItem batteryItem;
    // -------- private variables --------


    [Header("Debug")] 
    [SerializeField] bool isOn;
    [SerializeField] bool isCharging;
    [SerializeField] bool isCoolingDown;
    [SerializeField] bool isFiring;
    [SerializeField] bool hasBattery;
    [SerializeField] bool canRemoveBattery;

    // -------- IEnumerator --------
    IEnumerator chargingCO; // Have this so that we can stop the charging when take off the battery

    private void Start()
    {
        turretBehaviour = GetComponent<ITurretBehaviour>();
        rangeDetectorForAttack = attackRangeDetectorObject.GetComponent<IRangeDetector>();
        rangeDetectorForBattery = batteryRangeDetectorObject.GetComponent<IRangeDetector>();
        chargingPoint.SetActive(false);
        laser.SetActive(false);
        hasBattery = true;

        // setup rangeDetector's target to the player
        rangeDetectorForAttack.SetTarget(target);
        rangeDetectorForBattery.SetTarget(target);
    }


    // Charging -> Firing
    private void Update()
    {
        isOn = rangeDetectorForAttack.IsTargetInRange();
        canRemoveBattery = rangeDetectorForBattery.IsTargetInRange();
        turretBehaviour.Act(this);

        if (!hasBattery && chargingCO != null)
        {
            StopCoroutine(chargingCO);
            StartCoroutine(ShutdownRoutine());
            chargingCO = null;
        }

        // Remove Battery
        if (canRemoveBattery && Input.GetKeyDown(KeyCode.K))
        {
            if (hasBattery)
            {
                hasBattery = false;
                bodyPart.GetComponent<SpriteRenderer>().color = Color.gray;
                InventoryManager.Instance.GetInventory().AddItem(batteryItem);
            }
            else
            {
                InventoryManager.Instance.GetInventory().TryConsumeItem(batteryItem);
                hasBattery = true;
                bodyPart.GetComponent<SpriteRenderer>().color = Color.green;
            }
        }
    }

    public void RemoveBattery()
    {
        hasBattery = false;
	}



    // ---------- private functions ----------

    public void TurnToTarget()
    {
        Vector2 lookDir = target.transform.position - transform.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.rotation = Quaternion.LerpUnclamped(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public RaycastHit2D GetHit()
    {
        Vector3 laserOriginScale = laser.transform.localScale;
        Vector2 direction = transform.TransformDirection(Vector2.up);
        float maxLength = rangeDetectorForAttack.GetMaxLength(direction);
        RaycastHit2D hit = Physics2D.Raycast(chargingPoint.transform.position, direction, maxLength, hitLayer);

        return hit;
    }

    public bool IsLookingAtPlayer()
    {
        RaycastHit2D hit = GetHit();
        return  hit && hit.transform.TryGetComponent(out Player player);
    }

    public void StartFireRoutine()
    {
        StartCoroutine(FireRoutine());
    }

	public void StartChargingRoutine()
	{
        chargingCO = ChargingRoutine();
		StartCoroutine(chargingCO);
    }

    IEnumerator ChargingRoutine()
    {
        // Charging
        // TODO: Play charning anim
        isCharging = true;
        chargingPoint.transform.localScale = Vector3.zero;
        chargingPoint.SetActive(true);

        float chargePointGrowingRate = chargePointSize / chargeTime;
        while (chargingPoint.transform.localScale.magnitude < chargePointSize)
        {
            Vector2 aimDirection = transform.TransformDirection(Vector2.up);
            RaycastHit2D aimHit = Physics2D.Raycast(chargingPoint.transform.position, aimDirection, rangeDetectorForAttack.GetMaxLength(aimDirection));
            if (aimHit) Debug.DrawLine(chargingPoint.transform.position, aimHit.transform.position, Color.red);
            chargingPoint.transform.localScale += chargePointGrowingRate * Time.deltaTime * Vector3.one;

            yield return null;
		}
        chargingPoint.SetActive(false);
        isCharging = false;

        StartCoroutine(FireRoutine());
	}

    IEnumerator FireRoutine()
    { 
        // Firing
        isFiring = true;
        Vector3 laserOriginScale = laser.transform.localScale;
        RaycastHit2D hit = GetHit();
        laser.SetActive(true);

        float laserLength = rangeDetectorForAttack.GetMaxLength(transform.TransformDirection(Vector2.up));

        if (hit)
        {
            laserLength = Vector2.Distance(chargingPoint.transform.position, hit.point);

            if (hit.transform.TryGetComponent(out Player player))
            {
                // Do something to the player
                StartCoroutine(player.Die());
            }
            else if (hit.transform.TryGetComponent(out BreakablePlatform platform))
            {
                // Do something to the BreakablePlatform (only object in game affected by turrets)
                platform.BeenShot();
            }
        }
        
        laser.transform.localScale = new Vector3(
            laserLength,
            laser.transform.localScale.y,
            laser.transform.localScale.z
        );

        if (type == TurretType.Projectile)
        {
            // Laser shrinking
            float laserScaleY = laser.transform.localScale.y;
            float laserShrinkingRate = laserScaleY / laserShrinkTime;
            while (laserScaleY > 0)
            {
                laserScaleY -= laserShrinkingRate * Time.deltaTime;
                laser.SetLocalScaleY(laserScaleY);
                yield return null;
            }
            laser.SetActive(false);
            laser.transform.localScale = laserOriginScale;
            isFiring = false;

            isCoolingDown = true;
            yield return new WaitForSeconds(fireTimeInterval);
            isCoolingDown = false;
        }
	}

    IEnumerator ShutdownRoutine()
    {
        float chargePointShrinkingRate = (chargePointSize / chargeTime) * 2;
        while (chargingPoint.transform.localScale.x > 0)
        {
            chargingPoint.transform.localScale -= chargePointShrinkingRate * Time.deltaTime * Vector3.one;
            yield return null;
		}
        chargingPoint.SetActive(false);
        isCharging = false;
	}
}