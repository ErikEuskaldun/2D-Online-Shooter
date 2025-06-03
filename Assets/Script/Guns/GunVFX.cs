using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunVFX : MonoBehaviour
{
    [SerializeField] Animator shootAnimation;
    [SerializeField] GameObject ShootVFX;
    public void Shoot(bool isShooting)
    {
        shootAnimation.SetBool("Shooting", isShooting);
    }

    public void RepositionEffect(GunScriptable gun)
    {
        float positionX = (gun.pixelLenght/100f)-0.08f;
        ShootVFX.transform.localPosition = new Vector3(positionX, 0.005f);
    }
}
