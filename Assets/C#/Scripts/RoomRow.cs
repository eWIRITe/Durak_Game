using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomRow : MonoBehaviour
{
    private GameObject _self;

    private Room _room;

<<<<<<<< Updated upstream:Assets/C#/Scripts/RoomRow.cs
    private Network m_network;
========
    public GameUIs GameUI;

>>>>>>>> Stashed changes:Assets/C#/Room/RoomRow.cs
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
        _self = gameObject;

<<<<<<<< Updated upstream:Assets/C#/Scripts/RoomRow.cs
        m_network = GameObject.FindGameObjectWithTag("Network").GetComponent<Network>();
========
        GameUI = GetComponent<GameUIs>();
>>>>>>>> Stashed changes:Assets/C#/Room/RoomRow.cs
        m_socketNetwork = GameObject.FindGameObjectWithTag("SocketNetwork").GetComponent<SocketNetwork>();
    }

    public void Init(uint roomID) 
    {
        _room = GetComponent<Room>();
<<<<<<<< Updated upstream:Assets/C#/Scripts/RoomRow.cs

        StartCoroutine(m_network.GetAvatar(Session.UId, sucsessed => { PlayerAvatar.sprite = Sprite.Create(sucsessed, new Rect(0, 0, sucsessed.width, sucsessed.height), Vector2.one / 2.0f); }, fail => { Debug.Log(fail); }));
========
>>>>>>>> Stashed changes:Assets/C#/Room/RoomRow.cs
        
        StartCoroutine(m_network.GetRoomPlayers(roomID, _playersInTheRoom => { foreach (uint UId in _playersInTheRoom){ if(UId != Session.UId) _room.NewPlayerJoin(UId); } }));

        _room.StartScreen.SetActive(true);

        if (_roomOwnerID == Session.UId)
        {
            _room.OwnerStartGameButton.SetActive(true);
            Debug.Log("We are owner");
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