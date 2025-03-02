using Unity.VisualScripting;
using UnityEngine;

/*
Add this script as a component to an item to make the item's effect and visibility in game disable upon contact with player.
Enables the item to be "picked up"
Currently the item is not added to any inventory but solely sets the object's active state to false
*/
public class ItemPickup : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Makes the object disappear and no longer have effects in game if the player collides with it
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name == Player.ActivePlayer.gameObject.name)
        {
            Debug.Log(gameObject.name + " collected by Player");
            gameObject.SetActive(false);

        }
    }
}
