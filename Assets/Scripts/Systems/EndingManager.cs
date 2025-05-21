using UnityEngine;

public enum EndingType
{
    UNKNOWN,
    SILENT,
    NOT_SILENT,
    NOTHING_WRONG,
    FUTURE_LIE,
    FUTURE_TRUTH,
}

public class EndingManager : Singleton<EndingManager>
{
    public EndingType Ending = EndingType.UNKNOWN;

    void Start()
    {
        if (FriendshipManager.Instance.IsGoodScore())
        {
            Ending = EndingType.SILENT;
        }
        else
        {
            Ending = EndingType.NOT_SILENT;
            // TODO: play dialogue, implement music/art switching
        }
    }

    public EndingType GetEnding() => Ending;
    // TODO: delete unused overload
    public void SetEnding(EndingType ending) => Ending = ending;
    public void SetEnding(int endingID) => Ending = (EndingType)endingID;
}
