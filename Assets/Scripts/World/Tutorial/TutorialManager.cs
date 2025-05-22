using UnityEngine;


/// <summary>
/// <code>
/// Plot points:
/// Mariposa speaks to guard, attempts to fix turnstile
/// Keycard gets stuck on other side of turnstile (needs new dialogue system to work)
/// 
/// Beebo retrieves keycard
/// Mariposa uses keycard to open turnstile
/// 
/// Mariposa reaches the platform, then POV switches to Unnamed
/// 
/// Unnamed walks until they pick up radio
/// Mariposa connects to the radio (needs new dialogue system to work)
/// 
/// Unnamed tries to exit the room, but the door is jammed
/// Unnamed and Mariposa talk about ways to fix door (needs new dialogue system to work)
/// POV switches to Mariposa
/// 
/// Mariposa remembers the charging station
/// Mariposa fixes charging station by completing blocks puzzle
/// POV switches to Unnamed after Mariposa dialogue
/// 
/// Unnamed room now has the maintenance robot
/// Interacting with the robot starts next dialogue
/// Unnamed gets code from robot, which is used to open door (needs new dialogue system to work)
/// Go to city stage 1
/// </code>
/// </summary>
public class TutorialManager : Singleton<TutorialManager>
{
    [SerializeField] InventoryItemSO unnamedRadio;

    public bool UnnamedHasRadio()
    {
        return (InventoryManager.Instance.GetItemCount(InventoryType.Unnamed, unnamedRadio) > 0);
    }


}
