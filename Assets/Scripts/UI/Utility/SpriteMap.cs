using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
struct NamedSprite
{
    public string Name;
    public Sprite Icon;
};

public class SpriteMap : MonoBehaviour
{
    [SerializeField] private List<NamedSprite> namedSprites;

    private Dictionary<string, Sprite> spriteMap;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        spriteMap = new Dictionary<string, Sprite>();
        foreach (NamedSprite ns in namedSprites)
        {
            spriteMap.Add(ns.Name, ns.Icon);
        }
    }

    public Sprite GetSprite(string name)
    {
        if (!spriteMap.ContainsKey(name))
        {
            Debug.LogWarning($"The sprite \"{name}\" could not be found!");
            return null;
        }
        
        return spriteMap[name];
    }
}
