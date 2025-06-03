using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class GunLogic : NetworkBehaviour
{
    Camera mainCam;
    [SerializeField] Transform gunRig;
    public ActualGunLogic actualGun;
    NetworkGameAction serverAction;
    public int actualBullets = 0;
    public bool facingX = false;
    [SerializeField] SpriteRenderer gunSprite;
    [SerializeField] PlayerInfo playerInfo;
    [SerializeField] GunVFX gunVFX;

    NetworkManagerUI netUi;

    private void Awake()
    {
        netUi = FindObjectOfType<NetworkManagerUI>();
    }
    void Start()
    {
        mainCam = Camera.main;
        actualGun = GetComponent<ActualGunLogic>();
        serverAction = FindObjectOfType<NetworkGameAction>();
    }

    void Update()
    {
        GunFacing();
        if (!IsOwner)
            return;
        AngleRotation();
        RaycastFunction();
        Reload();
    }

    public void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R) && !reloading)
            StartCoroutine(Reloading());
    }
    private void RaycastFunction()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 100f;
        mousePos = mainCam.ScreenToWorldPoint(mousePos);
        Debug.DrawRay(transform.position, mousePos - transform.position,Color.blue);

        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, mousePos - transform.position);


        Shoot(hit);
    }

    private float timeBetwenBullets = 100f;
    private void Shoot(RaycastHit2D[] hit)
    {
        if (!playerInfo.GetComponent<PlayerNetwork>().IsControllable)
            return;

        //CHECK
        bool shoot = false;
        if (actualGun.gunInfo.isAutomatic && Input.GetMouseButton(0) && !reloading)
            shoot = true;
        else if (!actualGun.gunInfo.isAutomatic && Input.GetMouseButtonDown(0) && !reloading)
            shoot = true;

        if (timeBetwenBullets < actualGun.gunInfo.cadence)
        {
            timeBetwenBullets += Time.deltaTime;
            shoot = false;
        }

        if (shoot == false)
        {
            gunVFX.Shoot(false);
            return;
        }

        //SHOOT
        timeBetwenBullets = 0f;

        ReduceBullets();
        gunVFX.Shoot(true);

        string ammoTxt = "Ammo [" + actualGun.gunInfo.gunName + "]:" + actualBullets + "/" + actualGun.gunInfo.bullets;
        if(!reloading) netUi.GunUI(ammoTxt);

        //HIT
        if (hit.Length == 1)
            return;
        if (hit[1].collider != null)
        {
            string enemyTag = default;
            if (playerInfo.team.Value == 0)
                enemyTag = TeamType.Red.ToString();
            else enemyTag = TeamType.Blue.ToString();
            if (hit[1].collider.tag == enemyTag)
            {
                PlayerInfo playerHited = hit[1].collider.GetComponent<PlayerInfo>();
                float damage = actualGun.gunInfo.damage;
                serverAction.HitServerRpc(playerHited.username.Value, damage);
            }
        }
    }

    [SerializeField] public Vector3 mouseOnScreen;
    [SerializeField] public Vector3 positionOnScreen;
    private void AngleRotation()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = mainCam.ScreenToWorldPoint(mousePosition);

        Vector2 direction = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);

        transform.up = direction;
    }

    private void GunFacing()
    {
        if(this.transform.localRotation.eulerAngles.z < 180)
        {
            gunSprite.flipY = true;
            facingX = false;
        }
        else
        {
            gunSprite.flipY = false;
            facingX = true;
        }
    }

    public void ChangeGun(int gunType)
    {
        GunScriptable gun = actualGun.ChangeGun(gunType);
        gunVFX.RepositionEffect(gun);

        if (!IsOwner)
            return;
        actualBullets = actualGun.gunInfo.bullets;
        ReloadInfo();
    }

    public void ReloadInfo()
    {
        string ammoTxt = "Ammo [" + actualGun.gunInfo.gunName + "]:" + actualBullets + "/" + actualGun.gunInfo.bullets;
        netUi.GunUI(ammoTxt);
    }
    private void ReduceBullets()
    {
        actualBullets--;
        if (actualBullets == 0)
            StartCoroutine(Reloading());
    }

    private bool reloading = false;
    private IEnumerator Reloading()
    {
        reloading = true;

        netUi.GunUI("Reloading...");
        yield return new WaitForSeconds(actualGun.gunInfo.reloadTime);
        actualBullets = actualGun.gunInfo.bullets;
        ReloadInfo();

        reloading = false;
    }
}
