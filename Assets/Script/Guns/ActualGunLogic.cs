using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class ActualGunLogic : NetworkBehaviour
{
    [SerializeField] GunScriptable[] gunList;
    [SerializeField] SpriteRenderer gunSprite;
    public GunScriptable gunInfo;
    
    public void ChangeGun(GunType gunType)
    {
        switch (gunType)
        {
            case GunType.Null:
                gunInfo = default;
                break;
            case GunType.Triangle:
                gunInfo = gunList[0];
                break;
            case GunType.Box:
                gunInfo = gunList[0];
                break;
            default:
                gunInfo = default;
                break;
        }
        ChangeGunData();
    }

    public GunScriptable ChangeGun(int gunType)
    {
        gunInfo = gunList[gunType];
        ChangeGunData();
        return gunInfo;
    }

    private void ChangeGunData()
    {
        gunSprite.sprite = gunInfo.image;
    }

    
}
