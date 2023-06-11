using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomRow : MonoBehaviour
{
    private Room _room;

    public GameUIs GameUI;

    private SocketNetwork m_socketNetwork;

    [Header("RoomDATA")]

    public uint _roomOwnerID;
    private uint _roomID;
    public ESuit Trump;
    public ETypeGame GameType;

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

    public bool Passed;
    public bool Grabed;
    public bool Folded;

    public void Awake()
    {
        GameUI = GetComponent<GameUIs>();
        m_socketNetwork = GameObject.FindGameObjectWithTag("SocketNetwork").GetComponent<SocketNetwork>();
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
    }

    public void ExitClickHandler()
    {
        m_socketNetwork.EmitExitRoom(_roomID);

        Destroy(gameObject);
    }
}