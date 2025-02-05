using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour
{
    public bool IsOn { get => isOn;}
    public bool HasBattery { get => hasBattery; }

    // These three are for testing, need to use anim in the future
    public GameObject bodyPart;
    public GameObject chargingPoint;
    public GameObject laser;

    // -------- private variables --------
    [Header("Turret Attribute")]
    [SerializeField] LayerMask hitLayer;
    [SerializeField][Range(0.1f, 10)] float rotationSpeed;
    [SerializeField][Range(0.1f, 10)] float chargeTime;
    [SerializeField][Range(0.1f, 10)] float laserShrinkTime;
    [SerializeField][Range(0.1f, 10)] float chargePointSize;
    // -------- private variables --------

    [Header("Debug")] 
    [SerializeField] bool isOn;
    [SerializeField] bool isCharging;
    [SerializeField] bool isFiring;
    [SerializeField] bool hasBattery;

    // -------- Components --------
    private Player player;
    private PlayerInRangeDetect playerInRangeDetect;
    // -------- Components --------

    private void Start()
    {
        player = FindAnyObjectByType<Player>();
        playerInRangeDetect = GetComponent<PlayerInRangeDetect>();

        bodyPart.GetComponent<SpriteRenderer>().color = Color.green;
        chargingPoint.SetActive(false);
        laser.SetActive(false);
        hasBattery = true;
    }


    // Charging -> Firing
    private void Update()
    {
        isOn = playerInRangeDetect.IsPlayerInRange();
        if (hasBattery && isOn)
        {
            // Charge and fire
            if (!isCharging && Input.GetKeyDown(KeyCode.J))
            {
                isCharging = true;
                StartCoroutine(ChargingRoutine());
            }
            if (!isFiring)
            {
                TurnToPlayer();
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
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

    private void TurnToPlayer()
    {
        Vector2 lookDir = player.transform.position - transform.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.rotation = Quaternion.LerpUnclamped(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    IEnumerator ChargingRoutine()
    {
        // Charging
        // Play charning anim
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
            Debug.Log("Turret hit: " + hit.transform.name);
            if (hit.transform.TryGetComponent(out Player player))
            {
                // Do something to the player
            }
            else
            {
                laser.transform.localScale = new Vector3(hit.distance, laser.transform.localScale.y, 1);
                // Do something to the breakable
            }
        }

        // Laser shrinking
        float laserScaleX = laser.transform.localScale.x;
        float laserScaleY = laser.transform.localScale.y;
        float laserScaleZ = laser.transform.localScale.z;
        float laserShrinkingRate = laserScaleY / laserShrinkTime;
        while (laserScaleY > 0)
        {
            laserScaleY -= laserShrinkingRate * Time.deltaTime;
            laser.transform.localScale = new Vector3(laserScaleX, laserScaleY, laserScaleZ);  
            yield return null;
		}
        laser.SetActive(false);
        laser.transform.localScale = laserOriginScale;
        isFiring = false;
	}
}
