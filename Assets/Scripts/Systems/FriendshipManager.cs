using UnityEngine;

public class FriendshipManager : Singleton<FriendshipManager>
{
    private const int GOOD_THRESHOLD = 7;
    private int score = 0;

    public void ChangeScore(int delta) => score += delta;
    // TODO: delete unused comparison
    public bool IsGoodScore() => score >= GOOD_THRESHOLD;
    public bool CompareScore(int compareTo) => score >= compareTo;
}
