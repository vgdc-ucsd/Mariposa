using UnityEngine;

public class VentActivation : Interactable
{
    [SerializeField] private SpriteRenderer[] buildingTiles;
    [SerializeField] private Color col;
    [Space(10)]
    [SerializeField] private GameObject ventPuzzle;

    public override void OnInteract(IControllable controllable)
    {
        foreach (SpriteRenderer sr in buildingTiles) {
            sr.color = col;
        }
        ventPuzzle.SetActive(true);
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //ventPuzzle.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
