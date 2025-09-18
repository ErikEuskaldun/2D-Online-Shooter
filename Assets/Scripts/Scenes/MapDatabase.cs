using System.Collections.Generic;
using UnityEngine;

public class MapDatabase : MonoBehaviour
{
    public static MapDatabase Instance;

    [SerializeField] List<MapScriptable> mapScriptables;

    private void Awake()
    {
        Instance = this;
    }

    public List<string> GetMaps()
    {
        List<string> mapStringList = new List<string>();
        foreach (MapScriptable map in mapScriptables)
        {
            mapStringList.Add(map.displayName);
        }

        return mapStringList;
    }

    public string GetMapFromIndex(int index)
    {
        return mapScriptables[index].mapName;
    }

    public string GetDisplayName(string name)
    {
        foreach (MapScriptable map in mapScriptables)
        {
            if(map.mapName == name)
                return map.displayName;
        }
        return "test_map";
    }

    public Sprite GetPreviewImage(string name)
    {
        foreach (MapScriptable map in mapScriptables)
        {
            if (map.mapName == name)
                return map.previewImage;
        }
        return mapScriptables[0].previewImage;
    }
}
