using UnityEngine;

public class FriendshipManager : Singleton<FriendshipManager>
{
    public int Score { get; private set; }

    public void SetScore(int score) => Score = score;
    public void ChangeScore(int delta) => Score += delta;
    public bool CompareScore(int compareTo) => Score >= compareTo;
}
