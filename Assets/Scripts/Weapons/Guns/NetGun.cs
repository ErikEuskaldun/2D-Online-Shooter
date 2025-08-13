using Unity.Netcode;
using UnityEngine;

public class NetGun : NetworkBehaviour
{
    [SerializeField] private GunScriptable gunInfo;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool isLocked = false;

    private float shootCooldown;

    public SpriteRenderer SpriteRenderer => spriteRenderer;

    public void Init(ulong ownerId)
    {
        // si quieres guardar el ownerId para el projectile, etc.
    }

    public void SetLocked(bool locked)
    {
        isLocked = locked;
    }

    private void Update()
    {
        if (!IsClient)
            return;
        shootCooldown -= Time.deltaTime;
    }

    public bool CanShoot()
    {
        return shootCooldown <= 0f && !isLocked;
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
        if (shootCooldown > 0 || isLocked)
            return;

        shootCooldown = gunInfo.cadence;
        
        //projObj.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
        ShootClientRpc(direction);
    }

    [ClientRpc]
    public void ShootClientRpc(Vector2 direction)
    {
        var projObj = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        var projectile = projObj.GetComponent<Projectile>();
        projectile.SetOwner(OwnerClientId);
        projectile.Shoot(direction);
    }
}