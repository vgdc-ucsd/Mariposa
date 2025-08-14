using System.Collections;
using UnityEngine;

public class DialogueCranePressurePlate : CranePressurePlate
{
    [SerializeField] private string notEnoughBatteriesDialogue;
    [SerializeField] private string enoughBatteriesDialogue;
    [SerializeField] private GameObject destroyedTerrain;
    [SerializeField] private BoxCollider2D ghostCollider;
    [SerializeField] private GameObject grappleToHide;

    [Header("Camera Settings")]
    [SerializeField] private GameObject followCraneLoadCamera;
    [SerializeField] private float dropDelay;
    [SerializeField] private float cameraTrackDuration;

    public override void NotEnoughBatteries()
    {
        DialogueManager.Instance.PlayDialogue(notEnoughBatteriesDialogue);
    }

    public override void EnoughBatteries()
    {
        destroyedTerrain.SetActive(false);
        ghostCollider.gameObject.SetActive(false);
        grappleToHide.SetActive(false);

        StartCoroutine(ShiftCameraTemp());
    }

    IEnumerator ShiftCameraTemp()
    {
        yield return new WaitForSeconds(dropDelay);
        DialogueManager.Instance.PlayDialogue(enoughBatteriesDialogue);
        CameraManager.Instance.SetActiveCamera(followCraneLoadCamera);
        yield return new WaitForSeconds(cameraTrackDuration);
        CameraManager.Instance.ResetCamera();
    }
}
