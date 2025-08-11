using UnityEngine;

[CreateAssetMenu(fileName = "DefaultGun", menuName = "Scriptable Objects/New Gun")]
public class GunScriptable : ScriptableObject
{
    public int id = 0;
    public string gunName = "Default Gun";
    public float cadence = 0.2f;
    public float reloadTime = 1f;
    public int ammo = 9;
    public Sprite sprite;
    public GameObject prefab;
}
