using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using JSON;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;
using System.IO;
using TMPro;
using UnityEditor;

public class MenuScreen : BaseScreen
{
    [Header("User data")]
    public Text m_name;
    public Text m_chips;
    public Transform m_content;

    [Header("Create new room UI")]
    public Slider m_betSlider;
    public Dropdown m_cardsDropdown;
    public Dropdown m_typeGameDropdown;
    public Dropdown m_maxPlayersDropdown;
    public Dropdown m_isPrivateDropdown;

    [Header("free rooms")]
    public VerticalLayoutGroup _listOfFreeRooms;
    public GameObject FreeRoomPanel;

    [Header("Message")]
    public GameObject MessageScreen;
    public TMP_Text MessageText;

    private uint m_bet;
    private uint m_numberOfCards;
    private ETypeGame m_typeOfGame;
    private uint m_maxPlayers;
    private int m_isPrivate;
    //private string m_key;

    public Text m_betText;

    public uint[] m_betValues;

    private Hashtable m_rooms = new Hashtable();

    public void Start()
    {
        BetValueChangedHandler();
        CardsValueChangedHandler();
        TypeGameValueChangedHandler();
        MaxPlayersValueChangedHandler();
        IsPrivateValueChangedHandler();

        SocketNetwork.roomChange += reloadRooms;
        SocketNetwork.gotChips += GetChipsSuccessed;
        SocketNetwork.error += PrintMaessage;
    }

    public void OnShow()
    {
        m_socketNetwork.GetFreeRooms();
        m_socketNetwork.GetChips(Session.Token);

        m_name.text = Session.Name;
    }

    public void reloadRooms(uint[] FreeRoomsID)
    {
        MainThreadDispatcher.RunOnMainThread(() =>
        {
            for (int i = _listOfFreeRooms.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(_listOfFreeRooms.transform.GetChild(i).gameObject);
            }

            if (FreeRoomsID.Length <= 0) return;

            foreach (uint RoomID in FreeRoomsID)
            {
                GameObject _freeRoomPanel = Instantiate(FreeRoomPanel);
                _freeRoomPanel.transform.SetParent(_listOfFreeRooms.transform);

                _freeRoomPanel.transform.localScale = new Vector3(1, 1, 1);

                _freeRoomPanel.transform.Find("RoomID").GetComponent<TMP_Text>().text = "ID: " + RoomID.ToString();
                _freeRoomPanel.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => { m_socketNetwork.EmitJoinRoom(RoomID); });
            }
        });
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
        m_isPrivate = m_isPrivateDropdown.value != 0 ? 0:1;
    }


    //Get chips
    public void GetChipsSuccessed(int chips)
    {
        MainThreadDispatcher.RunOnMainThread(() =>
        {
            Session.Chips = chips;
            if (chips != 0) m_chips.text = chips.ToString();
            else m_chips.text = "You dont have any chips";
        });
    }

    
    ////////\\\\\\\\
    ///set avatar\\\
    ////////\\\\\\\\
    public void OpenFileExplorer()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        OpenFileExplorerWebGL();
#else
        OpenFileExplorerEditor();
#endif
    }

    private void OpenFileExplorerEditor()
    {
        string[] extensions = { "png", "jpg", "jpeg" };
        string path = EditorUtility.OpenFilePanel("Select Image", "", string.Join(",", extensions));

        HandleSelectedFilePath(path);
    }

    private void OpenFileExplorerWebGL()
    {
        string jsCode = @"
            const fileInput = document.createElement('input');
            fileInput.type = 'file';
            fileInput.accept = 'image/*';
            fileInput.onchange = (event) => {
                const files = event.target.files;
                if (files && files.length > 0) {
                    const path = URL.createObjectURL(files[0]);
                    UnitySendMessage('ImagePicker', 'HandleSelectedFilePath', path);
                }
            };
            fileInput.click();
        ";

        Application.ExternalEval(jsCode);
    }

    private void HandleSelectedFilePath(string path)
    {
        m_socketNetwork.setAvatar(path);
        m_socketNetwork.getAvatar(Session.UId);
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
            return;
        }

        m_socketNetwork.EmitCreateRoom(token, m_isPrivate, "", m_bet, m_numberOfCards, m_maxPlayers, m_typeOfGame);
    }

    //////\\\\\\\
    ///Message\\\
    //////\\\\\\\
    public void PrintMaessage(string Message)
    {
        MainThreadDispatcher.RunOnMainThread(() =>
        {
            MessageText.text = Message;
            LeanTween.scale(MessageScreen, new Vector3(1, 1, 1), 2).setOnComplete(finishMessage);
        });
    }
    public void finishMessage()
    {
        MainThreadDispatcher.RunOnMainThread(() =>
        {
            LeanTween.scale(MessageScreen, new Vector3(0, 0, 0), 1).setOnComplete(finishMessage);
        });
    }
}
