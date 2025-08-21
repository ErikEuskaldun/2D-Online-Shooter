using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class NetPlayer : NetworkBehaviour
{
    [SerializeField] float speed = 10f;

    //NetworkVariables
    NetworkVariable<bool> flip = new NetworkVariable<bool>(value: default, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> currentHP = new NetworkVariable<int>(value: 100, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<int> kills = new NetworkVariable<int>(value: 0, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<int> deaths = new NetworkVariable<int>(value: 0, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<FixedString64Bytes> username = new NetworkVariable<FixedString64Bytes>("no_name", readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Owner);

    [SerializeField] private NetGunController gunController;
    private Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private const bool EDIT_MODE = false;

    public event EventHandler<NetPlayer> OnPlayerKills;
    public event EventHandler<NetPlayer> OnPlayerDies;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            currentHP.Value = 100;

        flip.OnValueChanged += OnFlipCharacter;

        rigidbody = this.GetComponent<Rigidbody2D>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        animator = this.GetComponent<Animator>();

        if (!IsOwner)
            return;
            

        if(!EDIT_MODE)
        {
            username.Value = PlayerDataManager.Instance.Username;
            NetworkManager.Singleton.SceneManager.OnSceneEvent += (sceneEvent) =>
            {
                if (sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted && IsOwner)
                {
                    Spawn(SpawnPoints.Instance.GetRandomSpawn().position);
                }
            };
        }
        else
            Spawn(SpawnPoints.Instance.GetRandomSpawn().position);
    }

    private void OnFlipCharacter(bool previousValue, bool newValue)
    {
        spriteRenderer.flipX = newValue;
    }

    void Update()
    {
        if (!IsOwner)
            return;
        MovePlayer();
        Camera.main.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -3); //TODO: Cambiar el sistema de cámara
    }

    void MovePlayer()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 move = new Vector2(horizontal, vertical);

        if (move != Vector3.zero)
        {
            AnimateServerRpc("Runing", true);
            if (horizontal > 0)
                flip.Value = true;
            else if (horizontal < 0)
                flip.Value = false;

            move.Normalize();
            move = move * speed;
            rigidbody.linearVelocity = move;
        }
        else
        {
            AnimateServerRpc("Runing", false);
            rigidbody.linearVelocity = Vector3.zero;
        }
    }

    [ServerRpc]
    void AnimateServerRpc(string animation, bool state)
    {
        animator.SetBool(animation, state);
    }

    public void Hit(int damage, ulong playerId)
    {
        currentHP.Value -= damage;

        if (currentHP.Value <= 0)
        {
            StartCoroutine(Spawning(5));
            SetAliveStateClientRpc(false);

            NetworkManager.Singleton.ConnectedClients.TryGetValue(playerId, out NetworkClient killer);
            NetPlayer netKiller = killer.PlayerObject.GetComponent<NetPlayer>();
            netKiller.kills.Value++;
            OnPlayerKills?.Invoke(netKiller, netKiller);

            deaths.Value++;
            OnPlayerDies?.Invoke(this, this);
        }
    }

    private IEnumerator Spawning(int seconds)
    {
        var sendParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { OwnerClientId }
            }
        };
        RespawnUIClientRpc(seconds, sendParams);

        int timer = seconds;
        do
        {
            yield return new WaitForSeconds(1);
            timer--;
        } while (timer > 0);
        SetAliveStateClientRpc(true);
        
        currentHP.Value = 100;
        SpawnClientRpc(sendParams);
    }

    [ClientRpc]
    public void SpawnClientRpc(ClientRpcParams clientRpcParams = default)
    {
        Vector2 spawnPosition = SpawnPoints.Instance.GetRandomSpawn().position;
        Spawn(spawnPosition);
    }

    [ClientRpc]
    public void RespawnUIClientRpc(int seconds, ClientRpcParams clientRpcParams = default)
    {
        RespawnUI.Instance.SetActive(true);
        RespawnUI.Instance.SetCounter(seconds);
    }

    private void Spawn(Vector2 spawn)
    {
        this.transform.position = spawn;
        RespawnUI.Instance.SetActive(false);
    }

    [ClientRpc]
    void SetAliveStateClientRpc(bool isAlive)
    {
        gunController.Gun.SetLocked(!isAlive);
        var collider = GetComponent<Collider2D>();
        NetPlayerUI playerUI = GetComponent<NetPlayerUI>();

        rigidbody.simulated = isAlive;
        collider.enabled = isAlive;
        spriteRenderer.enabled = isAlive;
        playerUI.SetVisible(isAlive);
        gunController.Gun.SpriteRenderer.enabled = isAlive;
    }
}
