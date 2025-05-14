using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ScalePuzzleLevel", menuName = "Scriptable Objects/ScalePuzzleLevel")]
public class ScalePuzzleLevel : ScriptableObject
{
    public List<ScaleObject> objects;
}
