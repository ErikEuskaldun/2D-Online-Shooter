using System;
using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class NetPlayer : NetworkBehaviour
{
    [SerializeField] float speed = 10f;

    //NetworkVariables
    NetworkVariable<bool> flip = new NetworkVariable<bool>(value: default, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> currentHP = new NetworkVariable<int>(value: 100, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);

    private Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            currentHP.Value = 100;

        flip.OnValueChanged += OnFlipCharacter;

        rigidbody = this.GetComponent<Rigidbody2D>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        animator = this.GetComponent<Animator>();

        if(IsClient)
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

    public void Hit(int damage)
    {
        currentHP.Value -= damage;

        if (currentHP.Value <= 0)
        {
            currentHP.Value = 100;
            SpawnClientRpc();
        }
    }

    [ClientRpc]
    public void SpawnClientRpc()
    {
        Vector2 spawnPosition = SpawnPoints.Instance.GetRandomSpawn().position;
        Spawn(spawnPosition);
    }

    private void Spawn(Vector2 spawn)
    {
        this.transform.position = spawn;
    }
}
