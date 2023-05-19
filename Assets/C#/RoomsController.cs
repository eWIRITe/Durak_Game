using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;

public class RoomsController : API_controller
{
    private SocketNetwork _socketNetwork;

    [Header("UI")]
    public GameObject roomsListField;
    public TMP_Text MistakeText;
 
    [Header("CreateNewRoom parameters")]
    public Dropdown PlayersNumber;
    public Slider Slider;
    public Dropdown isPrivate;
    public Dropdown CardsNumber;
    public Dropdown m_typeGameDropdown;

    [Header("room window prefab")]
    public GameObject roomWindowPref;

    public List<Room> FreeRoms;

    int BET;
    int MaxPlayers;
    int CardNumber;
    ETypeGame m_typeOfGame;

    // Start is called before the first frame update
    void Start()
    {
        _socketNetwork = GameObject.FindGameObjectWithTag("SocketNetwork").GetComponent<SocketNetwork>();

        StartCoroutine(base.GetFreeRoms( result =>
        { 
            FreeRoms = JsonConvert.DeserializeObject<List<Room>>(result);

            for(int i = 0; i < FreeRoms.Count; i++)
            {
                GameObject roomWing = Instantiate(roomWindowPref);
                roomWing.transform.SetParent(roomsListField.transform);
                roomWing.transform.localScale = new Vector3(1, 1, 1);

                int ID = FreeRoms[i].RoomID;

                roomWing.transform.Find("RoomID").GetComponent<TMP_Text>().text = ToID(FreeRoms[i].RoomID);
                roomWing.transform.Find("Players").GetComponent<TMP_Text>().text = FreeRoms[i].nuberOfPlayers;
                roomWing.transform.Find("MaxPlayers").GetComponent<TMP_Text>().text = FreeRoms[i].MaxPlayers;
                roomWing.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => { JoinToRoom(ID); });
            }
        }));
    }

    public void TypeGameValueChangedHandler()
    {
        m_typeOfGame = (ETypeGame)m_typeGameDropdown.value;
    }

    public void CreateNewRoom()
    {
        string token = Session.Token;

        switch (Slider.value)
        {
            case 0:
                BET = 100;
                break;

            case 1:
                BET = 200;
                break;

            case 2:
                BET = 500;
                break;

            case 4:
                BET = 1000;
                break;

            case 5:
                BET = 10000;
                break;

            case 6:
                BET = 100000;
                break;

            case 7:
                BET = 200000;
                break;

            default:
                break;
        }
        switch (PlayersNumber.value) 
        {
            case 0:
                GotMistake("chose max players number");
                return;

            case 1:
                MaxPlayers = 2;
                break;

            case 2:
                MaxPlayers = 3;
                break;

            case 3:
                MaxPlayers = 4;
                break;

            case 4:
                MaxPlayers = 5;
                break;

            case 5:
                MaxPlayers = 6;
                break;

            default:
                break;
        }
        switch (CardsNumber.value)
        {
            case 0:
                CardNumber = 24;
                break;
            case 1:
                CardNumber = 36;
                break;
            case 2:
                CardNumber = 52;
                break;
            default:
                GotMistake("you did not choosed cards number");
                break;
        }
        bool _isPrivate = isPrivate.value == 0 ? false : true;

        _socketNetwork.EmitCreateRoom(token, _isPrivate, "", (uint)BET, (uint)CardNumber, (uint)MaxPlayers, m_typeOfGame);
        SceneManager.LoadScene(2);
    }

    public void JoinToRoom(int RoomID)
    {
        StartCoroutine(base.AddUser(RoomID, PlayerPrefs.GetInt("UserID"), result => { PlayerPrefs.SetInt("RoomID", RoomID); SceneManager.LoadScene(2); }));
    }

    public void GotMistake(string Mistake)
    {
        MistakeText.text = Mistake;
    }
}
