using FMOD.Studio;
using Unity.Cinemachine;
using UnityEngine;

public class PanCameraTrigger : Trigger
{
    [Header("Camera Settings")]
    [SerializeField] private GameObject[] camerasToPan;
    [SerializeField] private float timePerCamera;
    [SerializeField] private bool canMoveDuringPan;
    private float panCtr;
    private bool isPanning;
    private int cameraIndex;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // cameraToPan.SetActive(false);
        panCtr = 0.0f;
        isPanning = false;
        cameraIndex = 0;
    }

    public override bool OnEnter(Body body)
    {
        TriggerCollider.enabled = false;
        if (camerasToPan.Length == 0) return true;

        isPanning = true;
        if (!canMoveDuringPan)
        {
            PlayerController.Instance.SetMovementLock(true);
        }
        CameraManager.Instance.SetActiveCamera(camerasToPan[0]);

        base.OnEnter(body);
        return true;
    }

    public override void OnExit(Body body)
    {
        base.OnExit(body);
    }

    void Update()
    {
        if (!isPanning) return;

        if (cameraIndex >= camerasToPan.Length)
        {
            RestoreDefaultCam();
            this.transform.parent.gameObject.SetActive(false);
        }

        panCtr += Time.deltaTime;
        if (panCtr > timePerCamera)
        {
            cameraIndex++;
            if (!camerasToPan[cameraIndex].TryGetComponent<CinemachineCamera>(out CinemachineCamera cam))
            {
                Debug.LogWarning("Trying to activate something that is not a camera; stopping process");
                RestoreDefaultCam();
            }
            CameraManager.Instance.SetActiveCamera(camerasToPan[cameraIndex]);
            panCtr = 0.0f;
        }
    }

    public void RestoreDefaultCam()
    {
        isPanning = false;
        panCtr = 0.0f;
        CameraManager.Instance.ResetCamera();
        if (!canMoveDuringPan)
        {
            PlayerController.Instance.SetMovementLock(false);
        }
    }
}
