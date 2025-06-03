using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] float speed = 3f;
    LoginInfo loginInfo;
    GameInfo gameInfo;
    Camera cam;
    public GunLogic gunLogic;
    [SerializeField] SpriteRenderer playerSprite, gunSprite;
    private bool isControllable = true;

    public bool IsControllable => isControllable;

    private void OnConnectedToServer()
    {
        Debug.Log("Connected");
    }

    public void SetControllable(bool isControllable)
    {
        if (!IsOwner)
            return;
        this.isControllable = isControllable;
    }

    public void ChangeInfo(string name)
    {
        if (!IsOwner)
            return;
    }

    public override void OnNetworkSpawn()
    {
        loginInfo = FindObjectOfType<LoginInfo>();
        GetComponent<PlayerInfo>().SetInfo(loginInfo.username, loginInfo.team);
        
        base.OnNetworkSpawn();
        
        gameInfo = FindObjectOfType<GameInfo>();
        FindObjectOfType<NetworkManagerUI>().HideUI();
        gameInfo.ReloadGameInfoLocal();

        if(IsLocalPlayer)
        {
            cam = Camera.main;
            FindObjectOfType<LoginInfo>().SetLocalPlayer(this);
        }
    }

    public override void OnNetworkDespawn()
    {
        gameInfo.ReloadGameInfoServerRpc();
        base.OnNetworkDespawn();
    }

    void Update()
    {
        if (!IsOwner) 
            return;
        MovePlayer();
        if (IsLocalPlayer)
            cam.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -3);
    }

    void MovePlayer()
    {
        if (!isControllable)
            return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 move = new Vector2(horizontal, vertical);
        if (!isControllable)
            move = Vector3.zero;

        bool flipFacing = default;
        if (move != Vector3.zero)
        {
            GetComponent<Animator>().SetBool("Runing", true);
            if (horizontal != 0)
            {
                if (horizontal > 0)
                    flipFacing = true;
                else
                    flipFacing = false;
            }
            else flipFacing = gunLogic.facingX;
        }
        else
        {
            GetComponent<Animator>().SetBool("Runing", false);
            flipFacing = gunLogic.facingX;
        }
        if (GetComponent<PlayerInfo>().flipX.Value != flipFacing)
            GetComponent<PlayerInfo>().flipX.Value = flipFacing;

        move.Normalize();
        move = move * speed * Time.deltaTime;
        transform.position += move;
    }

    public void ChangeTeam(int team)
    {
        Color teamColor = default;
        if (team == 0)
            teamColor = Color.blue;
        else if (team == 1)
            teamColor = Color.red;

        TeamType teamType = (TeamType)team;
        this.tag = teamType.ToString();

        playerSprite.color = teamColor;
        gunSprite.color = teamColor;
    }
}
