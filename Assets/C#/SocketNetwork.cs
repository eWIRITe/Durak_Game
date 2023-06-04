using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json;
using System.Collections.Generic;
using JSON;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

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

    // Room was created event
    public delegate void RoomChangeEvent(uint[] FreeRoomsID);
    public static event RoomChangeEvent roomChange;

    // Logins events
    public delegate void LoginEvent(string message);
    public static event LoginEvent loginSucsessed;

    // SignIn events
    public delegate void SignInEvent();
    public static event SignInEvent SignInSucsessed;
    public static event SignInEvent SignInFailed;

    // get UId event
    public delegate void get_UID(uint ID);
    public static event get_UID UId;

    // get chips event
    public delegate void chips(int chips);
    public static event chips gotChips;

    //Error event
    public delegate void Error(string error);
    public static event Error error;

    //Players number change
    public delegate void Players(uint[] PlayersID);
    public static event Players changePlayers;
    public static event Players joinPlayer;

    //Room events
    public delegate void RoomEvents();
    public static event RoomEvents ready;

    //Card events
    public delegate void CardEvent(Card card);
    public static event CardEvent GetCard;
    public static event CardEvent DestroyCard;
    public delegate void atherUsersCards(uint uid);
    public static event atherUsersCards userGotCard;
    public static event atherUsersCards userDestroyCard;

    void Start()
    {
        // Server initiolization
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
            Debug.LogError("WebSocket error: " + e.Exception + " : " + e.Message + " : " + sender);
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
            if (Session.RoomID != 0) EmitExitRoom(Session.RoomID);

            websocket.Close();
        }
    }


    //////On server massege\\\\\\\
    /////////////\\\\\\\\\\\\\\\\\
    void HandleMessageFromServer(string message)
    {
        JSON.MessageData data = JsonConvert.DeserializeObject<JSON.MessageData>(message);

        switch (data.eventType)
        {
            case "cl_enterInTheRoom":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var enterRoomData = JsonConvert.DeserializeObject<JSON.ServerJoinRoom>(data.data);

                    Session.RoomID = enterRoomData.RoomID;

                    GameObject Room = Instantiate(RoomPrefab);

                    m_room = Room.GetComponent<Room>();
                    m_roomRow = Room.GetComponent<RoomRow>();

                    m_roomRow.RoomOwner = enterRoomData.roomOwner;
                    m_roomRow.RoomID = enterRoomData.RoomID;
                });
                break;

            case "cl_enterInTheRoomAsOwner":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var enterRoomOwnerData = JsonConvert.DeserializeObject<JSON.ServerCreateRoom>(data.data);

                    Session.RoomID = enterRoomOwnerData.RoomID;

                    GameObject Room = Instantiate(RoomPrefab);

                    m_room = Room.GetComponent<Room>();
                    m_roomRow = Room.GetComponent<RoomRow>();

                    m_roomRow.RoomOwner = enterRoomOwnerData.roomOwner;
                    m_roomRow.RoomID = enterRoomOwnerData.RoomID;
                });
                
                break;

            case "cl_joinRoom":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var joinRoomData = JsonConvert.DeserializeObject<JSON.ClientJoinRoom>(data.data);
                    m_room.NewPlayerJoin(joinRoomData.uid);
                });
                break;

            case "cl_leaveRoom":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var joinRoomData = JsonConvert.DeserializeObject<JSON.ClientJoinRoom>(data.data);
                    m_room.DeletePlayer(joinRoomData.uid);
                });
                break;

            case "sucsessedLogin":
                var loginData = JsonConvert.DeserializeObject<JSON.ClientLogin>(data.data);
                loginSucsessed?.Invoke(loginData.token);
                break;

            case "sucsessedSignIn":
                SignInSucsessed?.Invoke();
                break;

            case "ID":
                UId?.Invoke(uint.Parse(data.data));
                break;

            case "FreeRooms":
                var freeRoomsID = JsonConvert.DeserializeObject<JSON.Room> (data.data);
                roomChange?.Invoke(freeRoomsID.FreeRoomsID);
                break;

            case "roomPlayersID":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var playersID = JsonConvert.DeserializeObject<JSON.PlayersInRoom>(data.data);
                    changePlayers?.Invoke(playersID.PlayersID);
                });
                break;

            case "Chips":
                var chips = JsonConvert.DeserializeObject<JSON.ClientData>(data.data);
                gotChips?.Invoke(chips.chips);
                break;

            case "ready":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    ready?.Invoke();
                });
                break;

            case "playerGetCard":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    Card Card = JsonConvert.DeserializeObject<JSON.Card>(data.data);
                    GetCard?.Invoke(Card);
                });
                
                break;

            case "atherUserGotCard":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var user = JsonConvert.DeserializeObject<JSON.Client>(data.data);
                    Debug.Log(data);
                    Debug.Log(user.UserID);
                    userGotCard?.Invoke(user.UserID);
                });

                break;

            case "DestroyCard":
                break;

            case "error":
                error?.Invoke(data.data);
                break;

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
    public void EmitCreateRoom(string token, int isPrivate, string key, uint bet, uint cards, uint maxPlayers, ETypeGame type)
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

        SendMessageToServer("srv_exit", exitRoomData);
    }

    public void getAllRoomPlayersID()
    {
        var exitRoomData = new JSON.Room()
        {
            RoomID = Session.RoomID
        };

        SendMessageToServer("get_RoomPlayers", exitRoomData);
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

    //public void EmitTransfer(Card card)
    //{
    //    var transferData = new JSON.ServerTransfer()
    //    {
    //        token = Session.Token,
    //        card = card.Byte
    //    };

    //    SendMessageToServer("srv_transfer", transferData);
    //}

    //public void EmitBattle(List<Card> attacked, List<Card> attacking)
    //{
    //    var battleData = new JSON.ServerBattle()
    //    {
    //        token = Session.Token,
    //        attacked = attacked.ConvertAll(c => c.Byte).ToArray(),
    //        attacking = attacking.ConvertAll(c => c.Byte).ToArray()
    //    };

    //    SendMessageToServer("srv_battle", battleData);
    //}

    ////\\\\\
    //login\\
    ////\\\\\
    public void Login(string _name, string _password)
    {
        var userData = new JSON.ClientLogin()
        {
            name = _name,
            password = _password
        };

        SendMessageToServer("Login", userData);
    }
    //////\\\\\\
    ///SignIn\\\
    ///::::::\\\
    public void Signin(string _name, string _email, string _password)
    {
        var userData = new JSON.ClientSignIN()
        {
            name = _name,
            password = _password,
            email = _email
        };

        SendMessageToServer("Signin", userData);
    }


    //get data help functions
    public void GetUserID(string token)
    {
        var userData = new JSON.ClientLogin()
        {
            token = token
        };

        SendMessageToServer("getId", userData);
    }

    public void GetFreeRooms()
    {
        SendMessageToServer("getFreeRooms", new JSON.Room());
    }

    public void GetChips(string token)
    {
        var userData = new JSON.ClientLogin()
        {
            token = token
        };

        SendMessageToServer("getChips", userData);
    }
    public void GetRoomPlayers(uint RoomID)
    {
        SendMessageToServer("get_RoomPlayers", new JSON.PlayersInRoom() { RoomID = Session.RoomID });
    }
}
