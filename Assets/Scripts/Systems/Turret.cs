using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour
{
    public GameObject bodyPart;
    public GameObject bullet;
    public GameObject laser;
    public Player player;
    public float rotationSpeed = 3f;

    [Header("View Range")]
    [SerializeField] float sizeX;
    [SerializeField] float sizeY;
    [SerializeField] float offsetX;
    [SerializeField] float offsetY;

    [Header("Debug")] 
    [SerializeField] bool isOn;
    [SerializeField] bool isCharging;
    [SerializeField] bool isFiring;
    [SerializeField] bool hasBattery;

    public bool IsOn { get => isOn;}


    private void Start()
    {
        player = FindAnyObjectByType<Player>();

        hasBattery = true;
        bodyPart.GetComponent<SpriteRenderer>().color = Color.green;
        bullet.SetActive(false);
        laser.SetActive(false);
    }


    // Charging -> Firing
    private void Update()
    {
        if (IsPlayerInRange()) isOn = true;
        else isOn = false;
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
            hasBattery = false;
            bodyPart.GetComponent<SpriteRenderer>().color = Color.gray;
        }
    }

    private bool IsPlayerInRange()
    {
        float playerX = player.transform.position.x;
        float playerY = player.transform.position.y;
        float minX = transform.position.x - offsetX - (sizeX / 2);
        float maxX = transform.position.x + offsetX + (sizeX / 2);
        float minY = transform.position.y - offsetY - (sizeX / 2);
        float maxY = transform.position.y + offsetY + (sizeY / 2);

        bool isPlayerInRange = (playerX > minX && playerX < maxX) && (playerY > minY && playerY < maxY);
        return isPlayerInRange;
	}


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
        bullet.transform.localScale = Vector3.zero;
        bullet.SetActive(true);
        Vector3 laserOriginScale = laser.transform.localScale;
        while (bullet.transform.localScale.magnitude < 1f)
        {
            Debug.DrawRay(transform.position, transform.up * 10f, Color.red);
            bullet.transform.localScale += Vector3.one * 0.2f * Time.deltaTime;
            yield return null;
		}
        bullet.SetActive(false);

        // Firing
        isFiring = true;
        laser.SetActive(true);
        Vector2 upward = transform.TransformDirection(Vector2.up);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, upward, 10f);
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
    
    private void OnDrawGizmos()
    {
        if (IsPlayerInRange()) Gizmos.color = Color.red;
        Vector3 center = new Vector3(transform.position.x + offsetX, transform.position.y + offsetY, 0);
        Vector3 size = new Vector3(sizeX, sizeY, 0);
        Gizmos.DrawWireCube(center, size);
    }
}
