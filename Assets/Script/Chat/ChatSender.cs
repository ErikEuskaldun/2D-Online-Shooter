using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChatSender : MonoBehaviour
{
    [SerializeField] TMP_InputField ifChatText;
    [SerializeField] ChatCreator chatCreator;

    public void SendMessage()
    {
        chatCreator.InstantiateChatBox(ifChatText.text);
        ifChatText.text = "";
        //GameObject.FindGameObjectWithTag("NullUI").GetComponent<TMP_InputField>().Select();
    }

    public void NewMessageOrdenator()
    {
        StartCoroutine(MessagerOrdenatorWait());
    }

    public IEnumerator MessagerOrdenatorWait()
    {
        yield return new WaitForSeconds(0.05f);
        this.GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
    }
}
