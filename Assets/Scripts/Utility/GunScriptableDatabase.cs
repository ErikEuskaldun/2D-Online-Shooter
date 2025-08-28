using System.Collections.Generic;
using UnityEngine;

public class GunScriptableDatabase : MonoBehaviour
{
    public static GunScriptableDatabase Instance;

    [SerializeField] List<GunScriptable> guns;


    private void Awake()
    {
        Instance = this;
    }

    public GunScriptable GetScriptable(int gunId)
    {
        return guns[gunId];
    }
}
