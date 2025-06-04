using UnityEngine;

public class UnlockSquidEvent : MonoBehaviour
{
    [SerializeField] private GameObject squid; 

    public void UnlockSquid()
    {
        squid.SetActive(true);
        PlayerController.Instance.EnableSquid(true);
    }
}
