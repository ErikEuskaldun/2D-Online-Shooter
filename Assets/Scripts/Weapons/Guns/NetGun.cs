using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetGun : NetworkBehaviour
{
    [Header("Gun Setings")]
    [SerializeField] protected GunScriptable gunInfo;
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected Transform projectileSpawnPoint;
    [SerializeField] private SpriteRenderer spriteRenderer;

    protected bool isLocked = false;
    //Current ammo -1 (infinite) -2 (reloading)
    public NetworkVariable<int> currentAmmo = new NetworkVariable<int>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    protected float shootCooldown;

    public SpriteRenderer SpriteRenderer => spriteRenderer;

    public void Init(ulong ownerClientID)
    {
        currentAmmo.Value = gunInfo.ammo;
    }

    public void SetUI()
    {
        GameUI.Instance.SetGun(this, gunInfo.icon);
    }

    public void SetLocked(bool locked)
    {
        isLocked = locked;
    }

    private void Update()
    {
        shootCooldown -= Time.deltaTime;

        if (!IsOwner)
            return;

        if (Input.GetKeyDown(KeyCode.R) || currentAmmo.Value == 0)
            Reload();
    }

    #region Shoot
    public bool CanShoot()
    {
        return shootCooldown <= 0f && !isLocked && currentAmmo.Value>0;
    }

    public Vector2 GetShotDirection()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return (mousePos - projectileSpawnPoint.position).normalized;
    }

    [ServerRpc]
    public virtual void ShootServerRpc(Vector2 direction)
    {
        if (shootCooldown > 0 || isLocked)
            return;

        currentAmmo.Value--;
        shootCooldown = gunInfo.cadence;
        
        //projObj.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
        ShootClientRpc(direction);
    }

    [ClientRpc]
    public virtual void ShootClientRpc(Vector2 direction)
    {
        GameObject projObj = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        Projectile projectile = projObj.GetComponent<Projectile>();

        projectile.Setup(OwnerClientId, GameDatabase.Instance.GetSpriteMaterial(IsOwner));
        projectile.Shoot(direction);
    }
    #endregion

    #region Reload
    private void Reload()
    {
        if (currentAmmo.Value == gunInfo.ammo)
            return;

        ReloadServerRpc();
    }

    [ServerRpc]
    private void ReloadServerRpc()
    {
        StartCoroutine(ReloadAsync());
    }

    private IEnumerator ReloadAsync()
    {
        currentAmmo.Value = -2; //Reloading
        yield return new WaitForSeconds(gunInfo.reloadTime);
        currentAmmo.Value = gunInfo.ammo;
    }
    #endregion
}