using UnityEngine;

public class TutorialRobotNPC : BasicNPC
{
    [SerializeField] private TutorialJammedDoor jammedDoor;
    protected override string GetDialogue()
    {
        jammedDoor.jammed = false;
        return base.GetDialogue();
    }

}
