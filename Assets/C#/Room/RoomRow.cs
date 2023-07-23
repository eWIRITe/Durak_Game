using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomRow : BaseScreen
{
    public bool isAlone = false;

    private Room _room;

    public GameUIs GameUI;

    [Header("RoomDATA")]

    public uint _roomOwnerID;
    private uint _roomID;
    public ESuit Trump;
    public ETypeGame GameType;
    public int maxCards_number;
    public int maxPlayers_number;
    public int Bet;

    public uint RoomOwner
    {
        get { return _roomOwnerID; }
        set { _roomOwnerID = value; roomPlayers.Add(new User { UserID = value }); }
    }
    public uint RoomID
    {
        get { return _roomID; }
        set { _roomID = value; Init(value); }
    }

    public bool isGameStarted;

    [Header("Player Image")]
    public AvatarScr PlayerAvatar;

    [Header("Players")]
    public List<User> roomPlayers;

    public EStatus status;

    [Header("bet's")]
    public TMP_Text Users_Bet;
    public TMP_Text Rooms_Bet;

    public void Start()
    {
        GameUI = GetComponent<GameUIs>();
        SocketNetwork.changePlayers += UpdateRoomPlayers;
    }

    private void OnDestroy()
    {
        SocketNetwork.changePlayers -= UpdateRoomPlayers;
    }

    public void Init(uint roomID) 
    {
        _room = GetComponent<Room>();

        _room.StartScreen.SetActive(true);

        if (_roomOwnerID == Session.UId)
        {
            _room.OwnerStartGameButton.SetActive(true);
        }

        PlayerAvatar.UserID = Session.UId;
        m_socketNetwork.getAvatar(Session.UId);

        Users_Bet.text = Session.Chips.ToString();
        Rooms_Bet.text = Bet.ToString();
    }

    private void UpdateRoomPlayers(uint[] PlayersID)
    {
        List<uint> usersID = new List<uint>();
        foreach (User _user in roomPlayers)
        {
            usersID.Add(_user.UserID);
        }

        foreach (uint ID in PlayersID)
        {
            if (ID == Session.UId)
            {
                break;
            }
            else
            {
                _room.NewPlayerJoin(ID);
            }
        }
    }

    public void ExitClickHandler()
    {
        m_socketNetwork.EmitExitRoom(_roomID);

        Destroy(gameObject);

        m_screenDirector.ActiveScreen();
    }
}