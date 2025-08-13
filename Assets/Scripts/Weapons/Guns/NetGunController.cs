using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetGunController : NetworkBehaviour
{
    [SerializeField] private GunScriptable testGun;
    private NetGun gun;
    private Transform handTransform;

    private NetworkVariable<NetworkObjectReference> gunRef = new NetworkVariable<NetworkObjectReference>(
        default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetGun Gun => gun;

    public override void OnNetworkSpawn()
    {
        handTransform = this.transform;
        gunRef.OnValueChanged += OnGunRefChanged;

        if (IsServer)
        {
            var gunObj = Instantiate(testGun.prefab);
            gun = gunObj.GetComponent<NetGun>();
            gun.Init(OwnerClientId);

            var netObj = gun.GetComponent<NetworkObject>();
            netObj.SpawnWithOwnership(OwnerClientId);

            gunRef.Value = netObj;
        }

        StartCoroutine(AssignGunRefDelayed());
    }

    private void OnGunRefChanged(NetworkObjectReference previousValue, NetworkObjectReference newValue)
    {
        if (newValue.TryGet(out NetworkObject netObj))
            gun = netObj.GetComponent<NetGun>();
    }

    private IEnumerator AssignGunRefDelayed()
    {
        int i = 0;
        do
        {
            yield return new WaitForEndOfFrame();
            OnGunRefChanged(default, gunRef.Value);
            i++;
        } while (gunRef == default);
    }

    private void FixedUpdate()
    {
        
    }

    private void Update()
    {
        if (gun == null)
            return;

        if (IsOwner)
            Rotate();

        gun.transform.position = handTransform.position;
        gun.transform.localRotation = handTransform.rotation;

        if (!IsOwner)
            return;

        if (Input.GetKeyDown(KeyCode.Mouse0) && gun.CanShoot())
            gun.ShootServerRpc(gun.GetShotDirection());
    }

    private void Rotate()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector3 dir = (mousePos - transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}