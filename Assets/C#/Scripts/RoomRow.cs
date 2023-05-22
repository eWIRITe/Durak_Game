using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomRow : MonoBehaviour
{
    private GameObject _self;

    private Network m_network;
    private SocketNetwork m_socketNetwork;

    [Header("RoomUI")]

    public uint _roomOwnerID;
    private uint _roomID;

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
        StartCoroutine(m_network.GetAvatar(Session.UId, sucsessed => { PlayerAvatar.sprite = Sprite.Create(sucsessed, new Rect(0, 0, sucsessed.width, sucsessed.height), Vector2.one / 2.0f); }, fail => { Debug.Log(fail); }));

        uint[] PlayersInTheRoom;
        StartCoroutine(m_network.GetRoomPlayers(roomID, _playersInTheRoom => { PlayersInTheRoom = _playersInTheRoom; }));
    }

    public void PlayerJoinToOurRoom()
    {

    }
}