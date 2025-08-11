using System;
using System.Collections.Generic;
using UnityEngine;

public class NetPlayerUI : MonoBehaviour
{
    [SerializeField] List<Transform> hpSegmentList;
    void Start()
    {
        this.GetComponent<NetPlayer>().currentHP.OnValueChanged += NetPlayer_HpChanged;
    }

    private void NetPlayer_HpChanged(int previousValue, int newValue)
    {
        float scale = newValue / 100f;
        int activeSegments = Mathf.CeilToInt(scale / 0.125f); // 8 = full, 0 = empty

        for (int i = 0; i < hpSegmentList.Count; i++)
        {
            hpSegmentList[i].gameObject.SetActive(i < activeSegments);
        }
    }
}
