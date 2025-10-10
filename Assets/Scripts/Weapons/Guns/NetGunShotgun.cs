using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetGunShotgun : NetGun
{
    //[Header("Shotgun Setings")]

    public override void Press()
    {
        if (currentAmmo.Value == -2)
            return;
        pressTime = 0;
    }

    public override void Release()
    {
        //Pellets 3-6
        //Spread 45-22.5
        if(!CanShoot() || pressTime == -1f) 
            return;

        Vector2 direction = GetShotDirection();

        switch (pressTime)
        {
            case < 0.35f:
                ShootShotgunServerRpc(direction, 3, 45f, 5f);
                break;
            case < 0.7f:
                ShootShotgunServerRpc(direction, 4, 37.5f, 6.66f);
                break;
            case < 1.05f:
                ShootShotgunServerRpc(direction, 5, 30f, 8.33f);
                break;
            case < 1.4f:
            default:
                ShootShotgunServerRpc(direction, 6, 22.5f, 10f);
                break;
        }

        base.Release();
    }

    [ServerRpc]
    protected void ShootShotgunServerRpc(Vector2 direction, int pellets, float spreadAngle, float distance)
    {
        Debug.Log(pellets + " " + spreadAngle);
        if (shootCooldown > 0 || isLocked)
            return;

        currentAmmo.Value--;
        shootCooldown = gunInfo.cadence;

        Vector2[] directions = new Vector2[pellets];
        for (int i = 0; i < pellets; i++)
        {
            float angleOffset = Random.Range(-spreadAngle / 2, spreadAngle / 2);
            Quaternion rotation = Quaternion.Euler(0, 0, angleOffset);

            Vector2 bulletDirection = rotation * direction;
            directions[i] = bulletDirection;
        }

        ShootPelletsClientRpc(directions, distance);
    }

    [ClientRpc]
    public void ShootPelletsClientRpc(Vector2[] directions, float distance)
    {
        foreach (Vector2 direction in directions)
        {
            GameObject projObj = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
            Projectile projectile = projObj.GetComponent<Projectile>();

            projectile.Setup(gunInfo.id, OwnerClientId, GameDatabase.Instance.GetSpriteMaterial(IsOwner));
            projectile.Shoot(direction);
            projectile.GetComponent<ProjectileDieOnDistance>().SetDistance(distance);
        }
    }
}
