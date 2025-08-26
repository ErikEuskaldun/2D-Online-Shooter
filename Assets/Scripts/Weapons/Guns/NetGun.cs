using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetGun : NetworkBehaviour
{
    [SerializeField] private GunScriptable gunInfo;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool isLocked = false;
    //Current ammo -1 (infinite) -2 (reloading)
    public NetworkVariable<int> currentAmmo = new NetworkVariable<int>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private float shootCooldown;

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
    public void ShootServerRpc(Vector2 direction)
    {
        if (shootCooldown > 0 || isLocked)
            return;

        currentAmmo.Value--;
        shootCooldown = gunInfo.cadence;
        
        //projObj.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
        ShootClientRpc(direction);
    }

    [ClientRpc]
    public void ShootClientRpc(Vector2 direction)
    {
        GameObject projObj = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        Projectile projectile = projObj.GetComponent<Projectile>();

        Debug.Log(IsOwner);
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