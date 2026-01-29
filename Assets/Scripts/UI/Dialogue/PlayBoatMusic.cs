using UnityEngine;
using System.Collections;
public class PlayBoatMusic : DialogueEvent
{
    public override void Trigger()
    {
        MusicManager.Instance.ChangeMusic(AudioEvents.Music.S2Pier_unnamed_boat, 2.0f);
        Debug.Log(this + " got triggered");
    }
}
