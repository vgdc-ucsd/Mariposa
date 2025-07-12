using UnityEngine;

public class FriendshipManager : Singleton<FriendshipManager>
{
    private const int GOOD_THRESHOLD = 7;
    private int score = 0;

    public void SetScore(int score) => this.score = score;
    public void ChangeScore(int delta) => score += delta;
    public bool CompareScore(int compareTo) => score >= compareTo;
}
