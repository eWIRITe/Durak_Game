using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using JSON;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;
using SFB;
using System.IO;
using TMPro;

public class MenuScreen : BaseScreen
{
    [Header("User data")]
    public Text m_name;
    public Text m_chips;
    public Transform m_content;
    public Image Avatar;

    [Header("Create new room UI")]
    public Slider m_betSlider;
    public Dropdown m_cardsDropdown;
    public Dropdown m_typeGameDropdown;
    public Dropdown m_maxPlayersDropdown;
    public Dropdown m_isPrivateDropdown;

    [Header("free rooms")]
    public VerticalLayoutGroup _listOfFreeRooms;
    public GameObject FreeRoomPanel;

    private uint m_bet;
    private uint m_numberOfCards;
    private ETypeGame m_typeOfGame;
    private uint m_maxPlayers;
    private bool m_isPrivate;
    //private string m_key;

    public Text m_betText;

    public uint[] m_betValues;

    private Hashtable m_rooms = new Hashtable();

    private SocketNetwork m_socketNetwork;

    private void Start()
    {
        m_socketNetwork = GameObject.FindGameObjectWithTag("SocketNetwork").GetComponent<SocketNetwork>();
        BetValueChangedHandler();
        CardsValueChangedHandler();
        TypeGameValueChangedHandler();
        MaxPlayersValueChangedHandler();
        IsPrivateValueChangedHandler();

        Session.changeChips += GetChipsSuccessed;
        SocketNetwork._roomCreateEvent += reloadRooms;
    }

    public void OnShow()
    {
        StartCoroutine(m_network.GetChips(Session.Token, GetChipsSuccessed, GetChipsFailed));
        StartCoroutine(m_network.GetPlayerId(Session.Token, GetUIdSuccessed, GetUIdFailed));
        StartCoroutine(m_network.GetAvatar(Session.UId, sucsessed => { Avatar.sprite = Sprite.Create(sucsessed, new Rect(0, 0, sucsessed.width, sucsessed.height), Vector2.one / 2.0f); }, fail => { Debug.Log(fail); }));

        reloadRooms();

        Debug.Log("ID: " + Session.UId.ToString());
        m_name.text = Session.Name;
    }

    public void reloadRooms()
    {
        for (int i = _listOfFreeRooms.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(_listOfFreeRooms.transform.GetChild(i).gameObject);
        }

        StartCoroutine(m_network.GetFreeRooms(sucsessed =>
        {
            foreach (uint RoomID in sucsessed)
            {
                GameObject _freeRoomPanel = Instantiate(FreeRoomPanel);
                _freeRoomPanel.transform.SetParent(_listOfFreeRooms.transform);

                _freeRoomPanel.transform.localScale = new Vector2(1, 1);

                _freeRoomPanel.transform.Find("RoomID").GetComponent<TMP_Text>().text = "ID: " + RoomID.ToString();
                _freeRoomPanel.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => { m_socketNetwork.EmitJoinRoom(RoomID); });
            }
        }));
    }

    //////Value changing functions\\\\\\\\
    //////////////////////////////////////
    //==================================\\
    public void BetValueChangedHandler()
    {
        try
        {
            uint value = (uint)Mathf.RoundToInt(m_betSlider.value);
            m_betText.text = m_betValues[value].ToString();
            m_bet = m_betValues[value];
            //this.Filter();

        }
        catch (Exception)
        {
            m_bet = 0;
        }
    }
    public void CardsValueChangedHandler()
    {
        try
        {
            string selected = m_cardsDropdown.options[m_cardsDropdown.value].text;
            m_numberOfCards = UInt32.Parse(selected.Split(" ")[0]); // XX <-- cards
            //this.Filter();
        }
        catch (Exception)
        {
            m_numberOfCards = 0;
        }

    }
    public void TypeGameValueChangedHandler()
    {
        m_typeOfGame = (ETypeGame)m_typeGameDropdown.value;
        //this.Filter();
    }
    public void MaxPlayersValueChangedHandler()
    {
        try
        {
            m_maxPlayers = UInt32.Parse(m_maxPlayersDropdown.options[m_maxPlayersDropdown.value].text);
            //this.Filter();
        }
        catch (Exception)
        {
            m_maxPlayers = 0;
        }
    }
    public void IsPrivateValueChangedHandler()
    {
        m_isPrivate = m_isPrivateDropdown.value != 0;
    }


    ////////API funcions\\\\\\\\\\
    //??????????????????????????\\

    //LogOut
    private void LogoutSuccessed()
    {
        Session.Token = string.Empty;
        m_screenDirector.ActiveScreen(EScreens.StartScreen);
    }
    private void LogoutFailed(string resp)
    {
        Debug.LogError($"LogoutFailed:\n\t{resp}");
        Session.Token = string.Empty;
        m_screenDirector.ActiveScreen(EScreens.StartScreen);
    }
    public void ExitClickHandler()
    {
        StartCoroutine(m_network.Logout(Session.Token, LogoutSuccessed, LogoutFailed));
    }

    //Get ID
    private void GetUIdSuccessed(uint uid)
    {
        Session.UId = uid;
        StartCoroutine(m_network.GetPlayerName(Session.Token, Session.UId, GetPlayerNameSuccessed, GetPlayerNameFailed));
    }
    private void GetUIdFailed(string resp)
    {
        Debug.LogError($"GetUIdFailed:\n\t{resp}");
    }

    //Get chips
    public void GetChipsSuccessed(uint chips)
    {
        if (chips != 0) m_chips.text = chips.ToString();
        else m_chips.text = "You dont have any chips";
    }
    private void GetChipsFailed(string resp)
    {
        Debug.LogError($"GetChipsFailed:\n\t{resp}");
        m_chips.text = "Cant get your chips";
    }

    //Get Player Name
    private void GetPlayerNameSuccessed(string name)
    {
        m_name.text = name;
    }
    private void GetPlayerNameFailed(string resp)
    {
        Debug.LogError($"GetPlayernameFailed:\n\t{resp}");
        m_name.text = "Cant get your name";
    }


    //set avatar
    public void SetAvatarClickHandler()
    {
        OnClick();
    }

//#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    public void OnClick() {
        UploadFile(gameObject.name, "Chose new avatar", ".png", false);
    }

    // Called from browser
    public void OnFileUpload(string url) {
        if (url.Length > 0)
        {
            StartCoroutine(m_network.UploadAvatar(Session.Token, url, sucsessed => {

                StartCoroutine(m_network.GetAvatar(Session.UId, sucsessed => { Avatar.sprite = Sprite.Create(sucsessed, new Rect(0, 0, sucsessed.width, sucsessed.height), Vector2.one / 2.0f); }, fail => { Debug.Log(fail); }));

            }, fail => { Debug.Log(fail); }));
        }
    }


    ////////Screens\\\\\\\\\
    //--------------------\\
    public void AddChipsClickHandler(){ }
    public void ExchangeChipsClickHandler() { }
    public void RatingClickHandler()
    {
        m_screenDirector.ActiveScreen(EScreens.RatingScreen);
    }
    public void StoreClickHandler() { }
    public void CollectionsClickHandler()
    {
        m_screenDirector.ActiveScreen(EScreens.CollectionsScreen);
    }
    public void AwardsClickHandler() { }
    public void SettingsClickHandler()
    {
        m_screenDirector.ActiveScreen(EScreens.SettingsScreen);
    }


    public void CreateRoomClickHandler()
    {
        string token = Session.Token;

        if (m_bet == 0 || m_numberOfCards == 0 || m_maxPlayers == 0)
        {
            Debug.Log("create room mistake");
            return;
        }

        m_socketNetwork.EmitCreateRoom(token, m_isPrivate, "", m_bet, m_numberOfCards, m_maxPlayers, m_typeOfGame);
        Debug.Log("room was created");
    }

    //______\\
    //Filter\\
    /////\\\\\
    //private bool IsRoomFallUnderFilter(RoomRow room)
    //{
    //    return !(m_bet == room.Bet && m_numberOfCards == room.NumberOfCards && room.TypeGame == m_typeOfGame && room.MaxNumberOfPlayers == m_maxPlayers);
    //}
    //private void Filter()
    //{
    //    foreach (DictionaryEntry entry in m_rooms)
    //    {
    //        RoomRow row = (RoomRow)entry.Value;
    //        if (IsRoomFallUnderFilter(row))
    //        {
    //            row.gameObject.SetActive(false);
    //        }
    //        else
    //        {
    //            row.gameObject.SetActive(true);
    //        }
    //    }
    //}
}
