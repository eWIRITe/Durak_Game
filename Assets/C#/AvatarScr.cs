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

    private void OnDestroy()
    {
        SocketNetwork.got_avatar -= SetAvatar;
    }

    public void SetAvatar(uint ID, Sprite sprite)
    {
        if (UserID == ID)
        {
            avatarImage.sprite = sprite;
            Debug.Log("avatar setted");
        }
    }
}
