using UnityEditor.Overlays;
using UnityEngine;

public class SaveTestingKeyCounter : MonoBehaviour, IDataPersistence
{
    private int keyCount = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey("up"))
        {
            this.keyCount++;
            Debug.Log(this.keyCount);
        }
    }
    public void SaveData(ref GameData gameData)
    {
        gameData.TEST_keyStrokeCount = this.keyCount;
    } 
    
    public void LoadData(GameData gameData)
    {
        this.keyCount = gameData.TEST_keyStrokeCount;
    }
}
