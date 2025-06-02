using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class AutoscrollCamera : CameraController
{
    private CinemachineSplineDolly dolly;

    [SerializeField] private float movementStartDelay = 2.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dolly = GetComponent<CinemachineSplineDolly>();

        ResetCamera();
    }

    private IEnumerator StartMoving()
    {
        dolly.AutomaticDolly.Enabled = false;

        yield return new WaitForSeconds(movementStartDelay);

        dolly.AutomaticDolly.Enabled = true;
    }

    private void OnEnable()
    {
        Player.OnDeath += ResetCamera;
    }

    private void OnDisable()
    {
        Player.OnDeath -= ResetCamera;
    }

    public void ResetCamera()
    {
        dolly.CameraPosition = 0;

        StartCoroutine(StartMoving());
    }
}
