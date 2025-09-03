using System.Collections;
using FMODUnity;
using Unity.Cinemachine;
using UnityEngine;
using System;

public class BackgroundCameraShake : MonoBehaviour
{
    private static WaitForSeconds _waitForSeconds0_2 = new WaitForSeconds(0.2f);
    [SerializeField] private float avgTimeBetweenShake;
    [SerializeField][Range(0.0f, 1.0f)] private float randomFactor;
    [SerializeField] private string soundToPlay;
    [SerializeField] private bool canMoveDuringShake;
    [SerializeField] private float shakeIntensity;
    [SerializeField] private float shakeFrequency;
    [SerializeField] private float shakeDuration;

    private GameObject shakeCamObject;
    private CinemachineCamera shakeCamClass;
    private CinemachineCameraOffset cameraOffsetComponent;

    private float currentIntervalCtr;
    private float currentIntervalDuration;

    private bool shakingCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetCounters();
        StartCoroutine(GetCamera());
    }

    void OnEnable()
    {
        if (shakeCamClass == null) return;

        shakeCamClass.Target.TrackingTarget = Player.ActivePlayer.gameObject.transform;
    }

    IEnumerator GetCamera()
    {
        for (int i = 0; i < 10; i++)
        {
            shakeCamClass = GetComponentInChildren<CinemachineCamera>(true);
            if (shakeCamClass == null)
            {
                yield return _waitForSeconds0_2;
            }
            else
            {
                shakeCamObject = shakeCamClass.gameObject;
                cameraOffsetComponent = shakeCamObject.GetComponent<CinemachineCameraOffset>();
                shakeCamClass.Target.TrackingTarget = Player.ActivePlayer.gameObject.transform;

                ResetCounters();
                shakeCamObject.SetActive(false);
                CameraManager.Instance.ResetCamera();
                yield return null;
            }
        }
        Debug.Log("correct camera presets not found; make sure camera includes cinemachine camera and camera offset components");
    }

    private void ResetCounters()
    {
        currentIntervalCtr = 0.0f;
        currentIntervalDuration = avgTimeBetweenShake * (1.0f + UnityEngine.Random.Range(-randomFactor, randomFactor));
    }

    // Update is called once per frame
    void Update()
    {
        if (shakeCamClass == null || shakingCamera) return;

        float dt = Time.deltaTime;

        currentIntervalCtr += dt;

        if (currentIntervalCtr >= currentIntervalDuration)
        {
            if (!CameraManager.Instance.IsDefaultCameraEnabled() || DialogueManager.Instance.isPlayingDialogue)
            {
                ResetCounters();
                return;
            }
            
            // check if player has regular camera implemented, or else it may cause problems
            shakingCamera = true;
            if (!canMoveDuringShake)
            {
                PlayerController.Instance.SetMovementLock(true);
            }
            if (!String.IsNullOrEmpty(soundToPlay))
            {
                Debug.Log(soundToPlay);
                RuntimeManager.PlayOneShot(soundToPlay);
            }
            // Debug.Log("shaking cam");
            StartCoroutine(ShakeCamera());
        }
    }

    IEnumerator ShakeCamera()
    {
        shakeCamObject.SetActive(true);
        CameraManager.Instance.SetActiveCamera(shakeCamObject);
        // Debug.Log("active camera set");
        int numShakes = (int)(shakeDuration * shakeFrequency);
        float timeBetweenShakes = 1.0f / shakeFrequency;
        for (int i = 0; i < numShakes; i++)
        {
            cameraOffsetComponent.Offset = new(UnityEngine.Random.Range(-shakeIntensity, shakeIntensity) * 0.1f, UnityEngine.Random.Range(-shakeIntensity, shakeIntensity) * 0.1f, 0.0f);
            yield return new WaitForSeconds(timeBetweenShakes);
        }

        // end
        shakingCamera = false;
        shakeCamObject.SetActive(false);
        CameraManager.Instance.ResetCamera();
        if (!canMoveDuringShake)
        {
            PlayerController.Instance.SetMovementLock(false);
        }
        ResetCounters();
        yield return null;
    }
}
