using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json;
using System.Collections.Generic;

public class SocketNetwork : MonoBehaviour
{
    public MenuScreen m_menuScreen;
    public CardController card;
    private ScreenDirector _scrDirector;

    private WebSocket websocket;

    public Room m_room;
    public RoomRow m_roomRow;
    public GameObject RoomPrefab;

    public int _type;

    protected Network m_network;

    // Room was created event
    public delegate void RoomCreateEvent();
    public static event RoomCreateEvent _roomCreateEvent;

    void Start()
    {
        // Server settings
        string url = "ws://127.0.0.1:5000";
        websocket = new WebSocket(url);

        websocket.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket connection opened");
        };

        websocket.OnMessage += (sender, e) =>
        {
            string message = e.Data;
            HandleMessageFromServer(message);
        };

        websocket.OnError += (sender, e) =>
        {
            Debug.LogError("WebSocket error: " + e.Message);
        };

        websocket.OnClose += (sender, e) =>
        {
            Debug.Log("WebSocket connection closed");
        };

        websocket.Connect();
    }

    private void OnDestroy()
    {
        if (websocket != null && websocket.IsAlive)
        {
            websocket.Close();
        }
    }

    private void HandleMessageFromServer(string message)
    {
        // Обработка полученного сообщения от сервера
        // Ваш код для обработки различных сообщений от сервера
        JSON.MessageData data = JsonConvert.DeserializeObject<JSON.MessageData>(message);

        switch (data.eventType)
        {
            case "cl_enterInTheRoom":
                var enterRoomData = JsonConvert.DeserializeObject<JSON.ClientJoinRoom>(data.data);
                Debug.Log("cl_enterInTheRoom: " + enterRoomData.RoomID);
                // Обработка события cl_enterInTheRoom
                break;

            case "cl_enterInTheRoomAsOwner":
                var enterRoomOwnerData = JsonConvert.DeserializeObject<JSON.ClientJoinRoom>(data.data);
                Debug.Log("cl_enterInTheRoomAsOwner: " + enterRoomOwnerData.RoomID);
                // Обработка события cl_enterInTheRoomAsOwner
                break;

            case "cl_joinRoom":
                var joinRoomData = JsonConvert.DeserializeObject<JSON.ClientJoinRoom>(data.data);
                Debug.Log("cl_joinRoom: " + joinRoomData.RoomID);
                // Обработка события cl_joinRoom
                break;

            // Другие обработчики для различных событий от сервера

            default:
                Debug.Log("Unknown event type: " + data.eventType);
                break;
        }
    }

    // Отправка сообщения на сервер
    private void SendMessageToServer(string eventType, object data)
    {
        JSON.MessageData messageData = new JSON.MessageData()
        {
            eventType = eventType,
            data = JsonConvert.SerializeObject(data)
        };

        string json = JsonConvert.SerializeObject(messageData);
        if (websocket != null && websocket.IsAlive)
        {
            websocket.Send(json);
        }
    }

    // Rooms
    public void EmitCreateRoom(string token, bool isPrivate, string key, uint bet, uint cards, uint maxPlayers, ETypeGame type)
    {
        switch (type)
        {
            case ETypeGame.Regular:
                _type = 0;
                break;

            case ETypeGame.ThrowIn:
                _type = 1;
                break;

            case ETypeGame.Transferrable:
                _type = 2;
                break;

            default:
                break;
        }

        var createRoomData = new JSON.ServerCreateRoom()
        {
            token = token,
            isPrivate = isPrivate,
            key = key,
            bet = bet,
            cards = cards,
            type = _type,
            maxPlayers = maxPlayers,
            roomOwner = Session.UId
        };

        SendMessageToServer("srv_createRoom", createRoomData);
    }

    public void EmitJoinRoom(uint RoomID)
    {
        var joinRoomData = new JSON.ServerJoinRoom()
        {
            Token = Session.Token,
            RoomID = (uint)RoomID
        };

        SendMessageToServer("srv_joinRoom", joinRoomData);
    }

    public void EmitExitRoom(uint rid)
    {
        var exitRoomData = new JSON.ServerExitRoom()
        {
            token = Session.Token,
            rid = rid
        };

        SendMessageToServer("srv_exitRoom", exitRoomData);
    }

    // Playing functions
    public void EmitFold()
    {
        var foldData = new JSON.Token()
        {
            token = Session.Token
        };

        SendMessageToServer("srv_fold", foldData);
    }

    public void EmitReady(uint RoomID)
    {
        var readyData = new JSON.ServerJoinRoom()
        {
            Token = Session.Token,
            RoomID = RoomID
        };

        SendMessageToServer("srv_ready", readyData);
    }

    public void EmitPass()
    {
        var tokenData = new JSON.Token()
        {
            token = Session.Token
        };

        SendMessageToServer("srv_pass", tokenData);
    }

    public void EmitWhatsup()
    {
        var tokenData = new JSON.Token()
        {
            token = Session.Token
        };

        SendMessageToServer("srv_whatsup", tokenData);
    }

    public void EmitGrab()
    {
        var tokenData = new JSON.Token()
        {
            token = Session.Token
        };

        SendMessageToServer("srv_grab", tokenData);
    }

    public void EmitTransfer(Card card)
    {
        var transferData = new JSON.ServerTransfer()
        {
            token = Session.Token,
            card = card.Byte
        };

        SendMessageToServer("srv_transfer", transferData);
    }

    public void EmitBattle(List<Card> attacked, List<Card> attacking)
    {
        var battleData = new JSON.ServerBattle()
        {
            token = Session.Token,
            attacked = attacked.ConvertAll(c => c.Byte).ToArray(),
            attacking = attacking.ConvertAll(c => c.Byte).ToArray()
        };

        SendMessageToServer("srv_battle", battleData);
    }
}
