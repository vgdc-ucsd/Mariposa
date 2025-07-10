using FMODUnity;
using UnityEngine;

public class UnlockSquidEvent : MonoBehaviour
{
    [SerializeField] private GameObject squid;

    public void UnlockSquid()
    {
        squid.SetActive(true);
        RuntimeManager.PlayOneShot(AudioEvents.SFX.squid_activation.GetPath());
        PlayerController.Instance.EnableSquid(true);
    }
}
