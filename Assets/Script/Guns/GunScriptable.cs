using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "00_DefaultGun", menuName = "ScriptableObjects/Gun")]
public class GunScriptable : ScriptableObject
{
    public int id = 0;
    public string gunName = "DefaultName";
    public float damage = 0f;
    public bool isAutomatic = true;
    public float cadence = 0.2f;
    public float reloadTime = 0f;
    public int bullets = 0;
    public int pixelLenght = 16;
    public Sprite image;
}
