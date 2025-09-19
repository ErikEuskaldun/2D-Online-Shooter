using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetGunShotgun : NetGun
{
    [Header("Shotgun Setings")]
    [SerializeField] int pellets = 3;
    [SerializeField] float spreadAngle = 45f;

    [ServerRpc]
    public override void ShootServerRpc(Vector2 direction)
    {
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

        ShootPelletsClientRpc(directions);
    }

    [ClientRpc]
    public void ShootPelletsClientRpc(Vector2[] directions)
    {
        foreach (Vector2 direction in directions)
        {
            GameObject projObj = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
            Projectile projectile = projObj.GetComponent<Projectile>();

            projectile.Setup(gunInfo.id, OwnerClientId, GameDatabase.Instance.GetSpriteMaterial(IsOwner));
            projectile.Shoot(direction);
        }
    }
}
