using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatFunctions : MonoBehaviour
{
    [SerializeField] TMP_InputField ifChatTextArea;

    public void SelectTextArea()
    {
        FindObjectOfType<LoginInfo>().localPlayer.SetControllable(false);
    }

    public void DeselectTextArea()
    {
        FindObjectOfType<LoginInfo>().localPlayer.SetControllable(true);
    }
}
