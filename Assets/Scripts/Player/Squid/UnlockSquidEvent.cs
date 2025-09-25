using FMODUnity;
using UnityEngine;

public class UnlockSquidEvent : MonoBehaviour
{
    [SerializeField] private GameObject squid;
    [SerializeField] private SquidControlAbility ability;

    public void UnlockSquid()
    {
        squid.SetActive(true);
        RuntimeManager.PlayOneShot(AudioEvents.SFX.squid_activation);
        PlayerController.Instance.EnableSquid(ability);
    }
}
