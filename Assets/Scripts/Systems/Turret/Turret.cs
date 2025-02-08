using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour
{
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
	ITurretBehaviour turretBehaviour;
	public TurretBehaviourSO turretBehaviourSO;

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
    [SerializeField] InRangeDetector rangeDetectorForAttack;
    [SerializeField] InRangeDetector rangeDetectorForBattery;
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
		/*turretBehaviour = GetComponent<ITurretBehaviour>();*/
        bodyPart.GetComponent<SpriteRenderer>().color = Color.green; // For battery test
        chargingPoint.SetActive(false);
        laser.SetActive(false);
        SetLaserMaxLength();
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
        turretBehaviourSO.Act(this);
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
			}
			else
			{ 
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

    private void SetLaserMaxLength()
    {
        float lengthX = rangeDetectorForAttack.SizeX / 2 + rangeDetectorForAttack.OffsetX;
        float lengthY = rangeDetectorForAttack.SizeY / 2 + rangeDetectorForAttack.OffsetY;
        float maxLength = Mathf.Sqrt(Mathf.Pow(lengthX, 2) + Mathf.Pow(lengthY, 2));
        laser.SetLocalScaleX(maxLength);
	}

    public void TurnToTarget()
    {
        Vector2 lookDir = rangeDetectorForAttack.Target.transform.position - transform.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.rotation = Quaternion.LerpUnclamped(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
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
            RaycastHit2D aimHit = Physics2D.Raycast(chargingPoint.transform.position, transform.TransformDirection(Vector2.up), 10f);
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
        Vector2 upward = transform.TransformDirection(Vector2.up);
        RaycastHit2D hit = Physics2D.Raycast(chargingPoint.transform.position, upward, laserOriginScale.x, hitLayer);
        laser.SetActive(true);
        if (hit)
        {
            laser.SetLocalScaleX(hit.distance);
            Debug.Log("Turret hit: " + hit.transform.name);
            if (hit.transform.TryGetComponent(out Player player))
            {
                // Do something to the player
            }
            else
            {
                // Do something to the breakable
            }
        }

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

    IEnumerator ShutdownRoutine()
    {
        Debug.Log("Shotdown Routine");
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
