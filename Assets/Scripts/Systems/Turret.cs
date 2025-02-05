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

    [Header("Debug")] 
    [SerializeField] bool isOn;
    [SerializeField] bool isCharging;
    [SerializeField] bool isFiring;
    [SerializeField] bool hasBattery;

    // -------- private variables --------
    [Header("Turret Attribute")]
    [SerializeField][Range(0, 10)] float rotationSpeed;
    [SerializeField][Range(1, 10)] float chargeTime;
    [SerializeField][Range(1, 10)] float targetBulletSize;
    // -------- private variables --------

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
        Vector3 laserOriginScale = laser.transform.localScale;

        float bulletGrowingRate = targetBulletSize / chargeTime;
        while (chargingPoint.transform.localScale.magnitude < targetBulletSize)
        {
            RaycastHit2D aimHit = Physics2D.Raycast(chargingPoint.transform.position, transform.TransformDirection(Vector2.up), 10f);
            if (aimHit) Debug.DrawLine(chargingPoint.transform.position, aimHit.transform.position, Color.red);
            chargingPoint.transform.localScale += bulletGrowingRate * Time.deltaTime * Vector3.one;
            yield return null;
		}
        chargingPoint.SetActive(false);


        // Firing
        isFiring = true;
        laser.SetActive(true);
        Vector2 upward = transform.TransformDirection(Vector2.up);
        RaycastHit2D hit = Physics2D.Raycast(chargingPoint.transform.position, upward, 10f);
        if (hit)
        {
            Debug.Log("Turret hit: " + hit.transform.name);
            if (hit.transform.TryGetComponent(out Player player))
            {
                // Do something to the player
            }
        }

        // Laser shrinking
        float laserScaleX = laser.transform.localScale.x;
        float laserScaleY = laser.transform.localScale.y;
        float laserScaleZ = laser.transform.localScale.z;
        while (laser.transform.localScale.x > 0f)
        {
            laserScaleX -= 0.5f * Time.deltaTime;
            laser.transform.localScale = new Vector3(laserScaleX, laserScaleY, laserScaleZ);  
            yield return null;
		}
        laser.SetActive(false);
        laser.transform.localScale = laserOriginScale;
        isCharging = false;
        isFiring = false;
	}
}
