using UnityEngine;

[CreateAssetMenu(fileName = "DefaultMap", menuName = "Scriptable Objects/New Map")]
public class MapScriptable : ScriptableObject
{
    public int id = 0;
    public string mapName = "map_default";
    public string displayName = "Default";
    public Sprite previewImage;
}
