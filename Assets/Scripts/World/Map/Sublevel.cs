using UnityEngine;

public class Sublevel : MonoBehaviour
{
    [SerializeField]
    private CharID _subLevelCharacter;
    public CharID SublevelCharacter { 
        get => _subLevelCharacter; 
    }

    public void Load()
    {
        gameObject.SetActive(true);
        enabled = true;
    }

    public void Unload()
    {
        gameObject.SetActive(false);
        enabled = false;
    }
}
