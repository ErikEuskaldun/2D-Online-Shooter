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
    void Start()
    {
        this.GetComponent<NetPlayer>().currentHP.OnValueChanged += NetPlayer_HpChanged;
    }

    private void NetPlayer_HpChanged(int previousValue, int newValue)
    {
        float scale = newValue / 100f;
        int activeSegment = Mathf.CeilToInt(scale / 0.125f); // 8 = full, 0 = empty
        Debug.Log(newValue + "/" + activeSegment);
        hpSegment.sprite = hpSegmentList[activeSegment];
    }

    public void SetVisible(bool visible)
    {
        hpBar.GetComponent<SpriteRenderer>().enabled = visible;
        hpSegment.enabled = visible;
    }

    [ClientRpc]
    public void SetHitmarkerClientRpc(ClientRpcParams clientRpcParams = default)
    {
        CursorManager.Instance.Hit();
    }
}
