using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
/**
    Scene-Based Rooms: If isPrefabRoom is false, the room is handled as a separate Unity scene, using SceneManager.LoadSceneAsync and UnloadSceneAsync.
    Prefab-Based Rooms: If isPrefabRoom is true, it enables/disables all GameObjects in the roomContainer.
    Loading Screen: If a loadingScreen GameObject is assigned, it will show while the room is being loaded.

    TDLR:
        If you have a separate Scene for each room, it loads/unloads it.
        If you have a Prefab for the room, it enables/disables all objects in it.
        A loading screen can appear while switching rooms.
        It works with both prefab-based rooms and scene-based rooms.
**/

public class Room : MonoBehaviour
{
    public string sceneName; // Name of the scene if using multi-scene setup
    public bool isPrefabRoom = false; // Determines if this room is a prefab (reusable object) or a separate scene
    public GameObject roomContainer; // The parent object holding all elements of the room (for prefab rooms), If the room is a Prefab, this is the main object that holds all the smaller objects.
    public GameObject loadingScreen; // Optional loading screen

    // Getting All Objects in a Prefab Room
    /**
        Runs when the room is first created.
        If this room is a Prefab, it loops through every object inside roomContainer and saves them into roomObjects.
        This lets us easily enable/disable everything in the room later.
    **/
    void Awake()
    {
        if (roomContainer != null)
        {
            foreach (Transform child in roomContainer.transform)
            {
                //roomObjects.Add(child.gameObject);
            }
        }
    }

    // Turns on the room
    /**
        If we have a loadingScreen, turn it on.
        If the room is a Scene, start loading it.
        If it's a Prefab, just enable everything.
    **/
    public void Load()
    {
        if (loadingScreen) loadingScreen.SetActive(true);

        if (!isPrefabRoom)
        {
            //StartCoroutine(LoadScene());
        }
        else
        {
            EnableRoom();
        }
    }

    // Turn off the room
    /**
        If it's a Scene, start unloading it.
        If it's a Prefab, just disable everything.
    **/
    public void Unload()
    {
        if (!isPrefabRoom)
        {
            //StartCoroutine(UnloadScene());
        }
        else
        {
            DisableRoom();
        }
    }

    // Turns on all objects in a Prefab
    /**
        If the room is a Prefab, it turns on roomContainer (the whole room).
        Turns on every object inside roomObjects.
    **/
    private void EnableRoom()
    {
        if (roomContainer) roomContainer.SetActive(true);
        if (loadingScreen) loadingScreen.SetActive(false);
    }

    // Turns off all objects in a Prefab
    /**
        Turns off every object in roomObjects.
        Turns off roomContainer, making the entire room disappear.
    **/
    private void DisableRoom()
    {
        if (roomContainer) roomContainer.SetActive(false);
    }
}
