using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Chat : BaseScreen
{
    [Header("prefabs")]
    public GameObject messagePrefab;

    [Header("UI")]
    public GameObject Chat_obj;
    public Transform chat_container;
    public TMP_InputField chat_input;

    // Start is called before the first frame update
    void Start()
    {
        SocketNetwork.got_message += gotMessage;
    }

    private void OnDestroy()
    {
        SocketNetwork.got_message -= gotMessage;
    }

    public void sendMessage()
    {
        m_socketNetwork.Emit_sendMessage(chat_input.text);
    }

    public void gotMessage(uint ID, string message)
    {
        Chat_obj.SetActive(true);

        GameObject message_obj = Instantiate(messagePrefab, chat_container);

        message_obj.GetComponent<TMP_Text>().text = ID.ToString() + ": " + message;
    }
}
