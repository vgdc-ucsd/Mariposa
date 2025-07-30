using UnityEngine;

public class CameraTrigger : Trigger
{
    [SerializeField] private GameObject cameraToActivate;

    void Start()
    {
       cameraToActivate.SetActive(false);
    }
    
    public override bool OnEnter(Body body)
    {
        cameraToActivate.SetActive(true);
        base.OnEnter(body);

        return true;
    }

    public override void OnExit(Body body)
    {
        cameraToActivate.SetActive(false);
        base.OnExit(body);
    }
}
