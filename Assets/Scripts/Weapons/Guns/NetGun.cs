using Unity.Netcode;
using UnityEngine;

public class NetGun : NetworkBehaviour
{
    [SerializeField] private GunScriptable gunInfo;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;

    private float shootCooldown;

    public void Init(ulong ownerId)
    {
        // si quieres guardar el ownerId para el projectile, etc.
    }

    private void Update()
    {
        if (!IsClient)
            return;
        shootCooldown -= Time.deltaTime;
    }

    public bool CanShoot()
    {
        return shootCooldown <= 0f;
    }

    public Vector2 GetShotDirection()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return (mousePos - projectileSpawnPoint.position).normalized;
    }

    [ServerRpc]
    public void ShootServerRpc(Vector2 direction)
    {
        if (shootCooldown > 0)
            return;

        shootCooldown = gunInfo.cadence;

        var projObj = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        var projectile = projObj.GetComponent<Projectile>();
        projectile.SetOwner(OwnerClientId);
        projectile.Shoot(direction);
        projObj.GetComponent<NetworkObject>().Spawn();
    }
}