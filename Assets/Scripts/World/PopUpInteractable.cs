using UnityEngine;

public class PopUpInteractable : Interactable
{
    [SerializeField] private GameObject canvas; // Reference to the pop-up UI
    private bool isPlayerNearby = false;

    protected override void SetProximity()
    {
        Proximity = 2.0f; // TEMP VALUE: For now, Player must be within 2 units to interact
    }

    protected override void Start()
    {
        base.Start();
        if (canvas != null)
        {
            canvas.SetActive(false); // Hide pop-up by default
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (canvas != null)
            {
                canvas.SetActive(true); // Show pop-up
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (canvas != null)
            {
                canvas.SetActive(false); // Hide pop-up
            }
        }
    }

    public override void OnInteract()
    {
        if (isPlayerNearby)
        {
            Debug.Log("Interacted with the pop-up!");
            // CHANGE this to fill in the triggered event that should occur
        }
    }
}
