using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using JSON;

public class MenuScreen : BaseScreen
{
    public Text m_name;
    
    public Text m_chips;

    public GameObject m_roomPrefab;

    public Transform m_content;

    public Slider m_betSlider;
    public Dropdown m_cardsDropdown;
    public Dropdown m_typeGameDropdown;
    public Dropdown m_maxPlayersDropdown;
    public Dropdown m_isPrivateDropdown;

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

    private new void Start()
    {
        base.Start();

        m_socketNetwork = GameObject.FindGameObjectWithTag("SocketNetwork").GetComponent<SocketNetwork>();
        BetValueChangedHandler();
        CardsValueChangedHandler();
        TypeGameValueChangedHandler();
        MaxPlayersValueChangedHandler();
        IsPrivateValueChangedHandler();
    }

    public void BetValueChangedHandler()
    {
        try
        {
            uint value = (uint)Mathf.RoundToInt(m_betSlider.value);
            m_betText.text = m_betValues[value].ToString();
            m_bet = m_betValues[value];
            this.Filter();

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
            this.Filter();
        }
        catch (Exception)
        {
            m_numberOfCards = 0;
        }

    }

    public void TypeGameValueChangedHandler()
    {
        m_typeOfGame = (ETypeGame)m_typeGameDropdown.value;
        this.Filter();
    }

    public void MaxPlayersValueChangedHandler()
    {
        try
        {
            m_maxPlayers = UInt32.Parse(m_maxPlayersDropdown.options[m_maxPlayersDropdown.value].text);
            this.Filter();
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

    private void LogoutSuccessed()
    {
        Session.Token = string.Empty;
        m_screenDirector.SetScreen(EScreens.StartScreen);
    }

    private void LogoutFailed(string resp)
    {
        Debug.LogError($"LogoutFailed:\n\t{resp}");
        Session.Token = string.Empty;
        m_screenDirector.SetScreen(EScreens.StartScreen);
    }

    public void ExitClickHandler()
    {
        StartCoroutine(m_network.Logout(Session.Token, LogoutSuccessed, LogoutFailed));
    }
    
    public void AddChipsClickHandler(){ }

    public void ExchangeChipsClickHandler() { }


    public void RatingClickHandler()
    {
        m_screenDirector.SetScreen(EScreens.RatingScreen);
    }

    public void StoreClickHandler() { }

    public void CollectionsClickHandler()
    {
        m_screenDirector.SetScreen(EScreens.CollectionsScreen);
    }

    public void AwardsClickHandler() { }

    public void SettingsClickHandler()
    {
        m_screenDirector.SetScreen(EScreens.SettingsScreen);
    }

    private void GetUIdSuccessed(uint uid)
    {
        Session.UId = uid;
    }

    private void GetUIdFailed(string resp)
    {
        Debug.LogError($"GetUIdFailed:\n\t{resp}");
    }

    public void OnShow()
    {
        StartCoroutine(m_network.GetChips(Session.Token, GetChipsSuccessed, GetChipsFailed));
        StartCoroutine(m_network.GetPlayerId(Session.Token, GetUIdSuccessed, GetUIdFailed));
        StartCoroutine(m_network.GetPlayerName(Session.Token, Session.UId, GetPlayerNameSuccessed, GetPlayerNameFailed));
        m_name.text = Session.Name;
    }

    private void GetChipsSuccessed(uint chips)
    {
        m_chips.text = chips.ToString();
    }

    private void GetChipsFailed(string resp)
    {
        Debug.LogError($"GetChipsFailed:\n\t{resp}");
    }

    private void GetPlayerNameSuccessed(string name)
    {
        m_name.text = name;
    }

    private void GetPlayerNameFailed(string resp)
    {
        Debug.LogError($"GetPlayernameFailed:\n\t{resp}");
    }

    public void PlayClickHandler()
    {
        string token = Session.Token;

        if (m_bet == 0 || m_numberOfCards == 0 || m_maxPlayers == 0)
        {
            Debug.Log("create room mistake");
            return;
        }

        Debug.Log("room was created");
        m_socketNetwork.EmitCreateRoom(token, m_isPrivate, "", m_bet, m_numberOfCards, m_maxPlayers, m_typeOfGame);
    }

    public RoomRow CreateRoom(JSON.ClientCreateRoom json)
    {
        Debug.Log("CreateRoom function was");
        return this.AddRoom($"Room ¹{json.rid}", json.rid, json.cards, json.bet, json.type, json.players, json.maxPlayers);
    }

    public RoomRow JoinRoom(JSON.ClientJoinRoom json)
    {
        RoomRow room;

        if (m_rooms.ContainsKey(json.rid.ToString()))
        {
            room = ((RoomRow)m_rooms[json.rid.ToString()]);
            room.IncPlayers();
        }
        else
        {
            room = this.AddRoom($"Room ¹{json.rid}", json.rid, json.cards, json.bet, json.type, json.players, json.maxPlayers);
        }

        return room;
    }

    // if room is deleted - returns null
    public RoomRow ExitRoom(JSON.ClientExitRoom json)
    {
        RoomRow room;

        if (m_rooms.ContainsKey(json.rid.ToString()))
        {
            room = ((RoomRow)m_rooms[json.rid.ToString()]);
            room.DecPlayers();
        }
        else
        {
            room = this.AddRoom($"Room ¹{json.rid}", json.rid, json.cards, json.bet, json.type, json.players, json.maxPlayers);
        }

        if (room.NumberOfPlayers == 0)
        {
            this.RemoveRoom(json.rid);
            m_rooms.Remove(json.rid.ToString());
            return null;
        }

        return room;
    }

    private RoomRow AddRoom(string name, uint rid, uint cards, uint bet, ETypeGame type, uint players, uint maxPlayers)
    {
        RoomRow room = Instantiate(m_roomPrefab, m_content).GetComponent<RoomRow>();

        room.GetComponent<Button>().onClick.AddListener(room.RoomClickHandler);
        room.Init(name, rid, bet, type, cards, maxPlayers);

        m_rooms[rid.ToString()] = room;

        if (this.IsRoomFallUnderFilter(room))
        {
            room.gameObject.SetActive(false);
        }

        return room;
    }

    private void RemoveRoom(uint rid)
    {
        GameObject.Destroy((GameObject)m_rooms[rid.ToString()]);
    }

    // satisfies the filter ?
    private bool IsRoomFallUnderFilter(RoomRow room)
    {
        return !(m_bet == room.Bet && m_numberOfCards == room.NumberOfCards && room.TypeGame == m_typeOfGame && room.MaxNumberOfPlayers == m_maxPlayers);
    }

    private void Filter()
    {
        foreach (DictionaryEntry entry in m_rooms)
        {
            RoomRow row = (RoomRow)entry.Value;
            if (IsRoomFallUnderFilter(row))
            {
                row.gameObject.SetActive(false);
            }
            else
            {
                row.gameObject.SetActive(true);
            }
        }
    }
}
