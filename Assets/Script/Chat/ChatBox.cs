using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatBox : MonoBehaviour
{
    [SerializeField] TMP_Text tmpText;

    public void SetText(string text)
    {
        this.tmpText.text = text;
    }
}
