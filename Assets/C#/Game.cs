using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class Game : API_controller
{
    [Header("UI")]

    [Header("Start screen")]
    public GameObject StartDarkScreen;
    public GameObject OwnerStartButton;

    [Header("Player")]
    public Image AvatarImage;
    public TMP_Text ID_Text;

    private int RoomID;
    private bool Owner = false;
    private int ID;

    private void Start()
    {
        ID = PlayerPrefs.GetInt("UserID");

        ID_Text.text = "ID: " + ToID(ID);

        StartCoroutine(base.GetPhoto(ID.ToString(), AvatarImage));

        RoomID = PlayerPrefs.GetInt("RoomID");
        StartDarkScreen.SetActive(true);

        StartCoroutine(base.GetRoomOwner(RoomID, result => 
        {
            Debug.Log(result);
            if (ID == int.Parse(result))
            {
                OwnerStartButton.SetActive(true);
                Owner = true;
            }
        }));
    }

    public void OnJoinButton()
    {
        StartCoroutine(base.RoomIsReady(RoomID));
        StartDarkScreen.SetActive(false);
    }
}
