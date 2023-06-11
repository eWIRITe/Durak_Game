using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomRow : MonoBehaviour
{
    private GameObject _self;

    private Room _room;

    private Network m_network;
    private SocketNetwork m_socketNetwork;

    [Header("RoomDATA")]

    public uint _roomOwnerID;
    private uint _roomID;
    public ESuit Trump;

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
    public Image PlayerAvatar;

    [Header("Players")]
    public List<User> roomPlayers;

    public void Awake()
    {
        _self = gameObject;

        m_network = GameObject.FindGameObjectWithTag("Network").GetComponent<Network>();
        m_socketNetwork = GameObject.FindGameObjectWithTag("SocketNetwork").GetComponent<SocketNetwork>();
    }

    public void Init(uint roomID) 
    {
        _room = GetComponent<Room>();

        StartCoroutine(m_network.GetAvatar(Session.UId, sucsessed => { PlayerAvatar.sprite = Sprite.Create(sucsessed, new Rect(0, 0, sucsessed.width, sucsessed.height), Vector2.one / 2.0f); }, fail => { Debug.Log(fail); }));
        
        StartCoroutine(m_network.GetRoomPlayers(roomID, _playersInTheRoom => { foreach (uint UId in _playersInTheRoom){ if(UId != Session.UId) _room.NewPlayerJoin(UId); } }));

        _room.StartScreen.SetActive(true);

        if (_roomOwnerID == Session.UId)
        {
            _room.OwnerStartGameButton.SetActive(true);
            Debug.Log("We are owner");
        }
    }

    public void ExitClickHandler()
    {
        m_socketNetwork.EmitExitRoom(_roomID);

        Destroy(gameObject);
    }
}