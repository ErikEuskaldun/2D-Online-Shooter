using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetPlayerUI : NetworkBehaviour
{
    [Header("HP")]
    [SerializeField] Transform hpBar;
    [SerializeField] SpriteRenderer hpSegment;
    [SerializeField] List<Sprite> hpSegmentList;
    bool isVisible;
    bool isDead;
    void Start()
    {
        this.GetComponent<NetPlayer>().currentHP.OnValueChanged += NetPlayer_HpChanged;
        if(IsOwner)
            SetVisible(true);
        else
            SetVisible(false);
    }

    private void Update()
    {
        CheckVisibility();
    }

    void CheckVisibility()
    {
        var playerTransform = NetworkManager.Singleton.LocalClient.PlayerObject.transform;

        if (playerTransform == null)
            return;

        Vector2 origin = playerTransform.position;
        Vector2 direction = (Vector2)transform.position - origin;
        float distance = direction.magnitude;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction.normalized, distance, LayerMask.GetMask("Wall"));

        bool visible = hit.collider == null; //Si no ha HITeado con nada es visible
        if(visible != isVisible)
            SetVisible(visible);

        Debug.DrawRay(origin, direction.normalized * distance, visible ? Color.blue : Color.red);
    }


    private void NetPlayer_HpChanged(int previousValue, int newValue)
    {
        float scale = newValue / 100f;
        int activeSegment = Mathf.CeilToInt(scale / 0.125f); // 8 = full, 0 = empty
        Debug.Log(newValue + "/" + activeSegment);
        hpSegment.sprite = hpSegmentList[activeSegment];

        isDead = newValue == 0;
    }

    public void SetVisible(bool visible)
    {
        if (isDead)
            visible = false;
        isVisible = visible;
        hpBar.GetComponent<SpriteRenderer>().enabled = visible;
        hpSegment.enabled = visible;
    }

    [ClientRpc]
    public void SetHitmarkerClientRpc(ClientRpcParams clientRpcParams = default)
    {
        CursorManager.Instance.Hit();
    }
}
