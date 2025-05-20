using UnityEngine;

public class FriendshipManager : Singleton<FriendshipManager>
{
    private const int GOOD_THRESHOLD = 7;
    private int score = 0;

    public void AddScore(int delta) => score += delta;
    public bool IsGoodScore() => score >= GOOD_THRESHOLD;
}
