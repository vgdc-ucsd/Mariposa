using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using Unity.Cinemachine;
using UnityEngine;

public class TutorialRobotChaseTrigger : Trigger
{
    [SerializeField] private CinemachineCamera robotCamera;
    [SerializeField] private CinemachineBasicMultiChannelPerlin robotNoise;
    [SerializeField] private List<Robot> bots;
    [SerializeField] private GameObject leftBarrier;
    [SerializeField] private GameObject preventBacktracking;
    private CinemachineBasicMultiChannelPerlin playerNoise;

    protected override bool OnlyOnce => true;

    private const float SHAKE_AMPLITUDE = 1.0f;
    private const float SHAKE_FREQUENCY = 1.0f;

    public override bool OnEnter(Body body)
    {
        StartCoroutine(ShowRobot());
        return base.OnEnter(body);
    }

    private IEnumerator ShowRobot()
    {
        GameObject[] cameras = GameObject.FindGameObjectsWithTag("MainCamera");
        foreach (GameObject camera in cameras)
        {
            CinemachineBasicMultiChannelPerlin noise = camera.transform.GetComponent<CinemachineBasicMultiChannelPerlin>();
            if (noise != null)
            {
                playerNoise = noise;
                break;
            }
        }

        RuntimeManager.PlayOneShot(AudioEvents.SFX.earthquale_rumble_rocks);
        PlayerController.Instance.SetMovementLock(true);
        robotNoise.FrequencyGain = SHAKE_FREQUENCY;
        robotNoise.AmplitudeGain = SHAKE_AMPLITUDE;

        yield return BasicAnimations.Interpolate
        (
            null,
            (t) =>
            {
                float smooth = BasicAnimations.Smooth(t);
                playerNoise.AmplitudeGain = smooth * SHAKE_AMPLITUDE;
                playerNoise.FrequencyGain = smooth * SHAKE_FREQUENCY;
            },
            () =>
            {
                playerNoise.AmplitudeGain = SHAKE_AMPLITUDE;
                playerNoise.FrequencyGain = SHAKE_FREQUENCY;
            },
            1.0f
        );

        leftBarrier.SetActive(false);
        foreach (Robot bot in bots)
        {
            bot.gameObject.SetActive(true);
        }

        robotCamera.gameObject.SetActive(true);
        yield return new WaitForSeconds(4.0f);
        robotCamera.gameObject.SetActive(false);

        yield return BasicAnimations.Interpolate
        (
            null,
            (t) =>
            {
                float smooth = 1.0f - BasicAnimations.Smooth(t);
                playerNoise.AmplitudeGain = smooth * SHAKE_AMPLITUDE;
                playerNoise.FrequencyGain = smooth * SHAKE_FREQUENCY;
            },
            () =>
            {
                playerNoise.AmplitudeGain = 0f;
                playerNoise.FrequencyGain = 0f;
            },
            2f
        );

        foreach (Robot bot in bots)
        {
            bot.gameObject.SetActive(false);
        }
        leftBarrier.SetActive(true);

        preventBacktracking.SetActive(true);
        PlayerController.Instance.SetMovementLock(false);
        Destroy(gameObject);
    }
}
