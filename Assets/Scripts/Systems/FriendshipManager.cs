using UnityEngine;

public class FriendshipManager : Singleton<FriendshipManager>, IDataPersistence
{
    public int Score { get; private set; }

    public void SetScore(int score) => Score = score;
    public void ChangeScore(int delta) => Score += delta;
    public bool CompareScore(int compareTo) => Score >= compareTo;

    public void LoadData(GameData data)
    {
        Score = data.friendshipScore;
    }

    public void SaveData(ref GameData data)
    {
        data.friendshipScore = Score;
    }
}
