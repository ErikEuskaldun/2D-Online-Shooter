using System.Collections.Generic;
using UnityEngine;

public class WeaponDatabase : MonoBehaviour
{
    [SerializeField] List<Sprite> weaponSprites;

    public static WeaponDatabase Instance;

    private void Awake()
    {
        Instance = this;
    }

    public Sprite GetWeaponSprite(int index)
    {
        return weaponSprites[index];
    }
}
