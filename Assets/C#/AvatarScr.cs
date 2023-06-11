using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarScr : MonoBehaviour
{
    public uint UserID;
    public Image avatarImage;

    private void Start()
    {
        SocketNetwork.got_avatar += SetAvatar;
        avatarImage = GetComponent<Image>();
    }

    public void SetAvatar(uint ID, Sprite sprite)
    {
        Debug.Log("public void SetAvatar(uint ID, Sprite sprite)");

        Debug.Log("script UserID: " + UserID.ToString());
        Debug.Log("event ID: " + ID.ToString());

        if (UserID == ID)
        {
            avatarImage.sprite = sprite;
            Debug.Log("avatar setted");
        }
    }
}
