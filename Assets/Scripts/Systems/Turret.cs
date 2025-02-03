using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour
{
    public GameObject bullet;
    public GameObject laser;
    public Player player;
    public float rotationSpeed = 3f;

    [Header("Debug")] 
    [SerializeField] bool isOn;
    [SerializeField] bool isCharging;
    [SerializeField] bool isFiring;

    BoxCollider2D boxCollider; // used as sight
    Rigidbody2D body;

    public bool IsOn { get => isOn;}

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        body = GetComponent<Rigidbody2D>();
        bullet.SetActive(false);
        laser.SetActive(false);
    }

    private void Update()
    {
        if (isOn)
        {
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
    }

    private void TurnToPlayer()
    {
        Vector2 lookDir = player.transform.position - transform.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        body.rotation = angle;
    }

    /*
    // Not working, don't know why
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player in view");
        isOn = true;
    }
    */

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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up * 10f);
        if (hit)
        {
            Debug.Log("Turret hit something");
        }
        if (hit.transform.TryGetComponent<Player>(out Player player))
        {
            Debug.Log("Turret hit player");
        }

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
