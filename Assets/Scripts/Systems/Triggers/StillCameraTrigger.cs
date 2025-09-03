using UnityEngine;

public class StillCameraTrigger : Trigger
{
    [SerializeField] private GameObject cameraToActivate;
    private int playerLayer;

    // dont think these do anything?
    protected override bool OnlyOnce => true; 
    protected override bool MustBePlayer => true;

    void Start()
    {
        playerLayer = LayerMask.NameToLayer("Player");
       // cameraToActivate.SetActive(false);
    }
    
    public override bool OnEnter(Body body)
    {
        cameraToActivate.SetActive(true);
        base.OnEnter(body);

        return true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != playerLayer || cameraToActivate.activeSelf) return;

        cameraToActivate.SetActive(true);
    }

    public override void OnExit(Body body)
    {
        cameraToActivate.SetActive(false);
        base.OnExit(body);
    }
}
