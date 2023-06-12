using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json;
using System.Collections.Generic;
using JSON;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.IO;
using System;


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

    //Card events
    public delegate void CardEvent(Card card);
    public static event CardEvent GetCard;
    public static event CardEvent DestroyCard;

    public delegate void RoomEvents(Card trumpCard);
    public static event RoomEvents ready;

    public delegate void placeCardEvent(uint UID, Card card);
    public static event placeCardEvent placeCard;

    public delegate void BeatCardEvent(uint UID, Card beat, Card beating);
    public static event BeatCardEvent beatCard;

    public delegate void FoldCardEvent();
    public static event FoldCardEvent FoldAllCards;

    public delegate void atherUsersCards(uint uid);
    public static event atherUsersCards userGotCard;
    public static event atherUsersCards userDestroyCard;

    public delegate void GrabEvent();
    public static event GrabEvent playerGrab;

    public delegate void cl_foldEvent(uint UserID);
    public static event cl_foldEvent cl_fold;

    public delegate void cl_passEvent(uint UserID);
    public static event cl_passEvent cl_pass;

    public delegate void cl_grabEvent();
    public static event cl_grabEvent cl_grab;
    public static event cl_grabEvent grab;

    public delegate void gotAvagar(uint UserID, Sprite avatar);
    public static event gotAvagar got_avatar;

    // Turn
    public delegate void gamesEvents(uint UserID);
    public static event gamesEvents newTurn;

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
    ///////////\\\\\\\\\\
    // Server messages \\
    void HandleMessageFromServer(string message)
    {
        JSON.MessageData data = JsonConvert.DeserializeObject<JSON.MessageData>(message);

        Debug.Log(data.data.ToString());

        switch (data.eventType)
        {

            //////////////////////////
            // room message handler //
            //////////////////////////

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
                    var enterRoomOwnerData = JsonConvert.DeserializeObject<JSON.ServerJoinRoom>(data.data);

                    Session.RoomID = enterRoomOwnerData.RoomID;

                    GameObject Room = Instantiate(RoomPrefab);

                    m_room = Room.GetComponent<Room>();
                    m_roomRow = Room.GetComponent<RoomRow>();

                    m_roomRow.RoomOwner = enterRoomOwnerData.roomOwner;

                    m_roomRow.GameType = (ETypeGame)enterRoomOwnerData.type;

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

            ///////////////////////////
            // enter message handler //
            ///////////////////////////
            case "sucsessedLogin":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var loginData = JsonConvert.DeserializeObject<JSON.ClientLogin>(data.data);
                    loginSucsessed?.Invoke(loginData.token);
                });
                break;

            case "sucsessedSignIn":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    SignInSucsessed?.Invoke();
                });
                break;

            case "cl_getId":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    UId?.Invoke(uint.Parse(data.data));
                });
                break;

            case "FreeRooms":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var freeRoomsID = JsonConvert.DeserializeObject<JSON.FreeRooms>(data.data);
                    roomChange?.Invoke(freeRoomsID.FreeRoomsID);
                });
                break;

            case "roomPlayersID":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var playersID = JsonConvert.DeserializeObject<JSON.PlayersInRoom>(data.data);
                    changePlayers?.Invoke(playersID.PlayersID);
                });
                break;

            case "Chips":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var chips = JsonConvert.DeserializeObject<JSON.ClientData>(data.data);
                    gotChips?.Invoke(chips.chips);
                });
                break;

            case "ready":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var ready_data = JsonConvert.DeserializeObject<JSON.ClientReady>(data.data);

                    ready?.Invoke(ready_data.trump);
                });
                break;

            /////////////////////////////
            // playing message handler //
            /////////////////////////////

            case "cl_getImage":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    Debug.Log("cl_getImage");
                    Debug.Log(data.data);

                    AvatarData _data = JsonConvert.DeserializeObject<JSON.AvatarData>(data.data);

                    byte[] imageBytes = System.Convert.FromBase64String(_data.avatarImage);

                    // Create a new Texture2D and load the image bytes
                    Texture2D texture = new Texture2D(1, 1);
                    texture.LoadImage(imageBytes);

                    // Create a Sprite using the loaded Texture2D
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

                    got_avatar?.Invoke(_data.UserID, sprite);
                });
                break;

            case "GetCard":
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

            case "cl_gotCard":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var user = JsonConvert.DeserializeObject<JSON.Client>(data.data);
                    userGotCard?.Invoke(user.UserID);
                });
                break;

            case "cl_destroyCard":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var user = JsonConvert.DeserializeObject<JSON.Client>(data.data);
                    userDestroyCard?.Invoke(user.UserID);
                });
                break;

            case "DestroyCard":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var _data = JsonConvert.DeserializeObject<JSON._Card>(data.data);
                    DestroyCard?.Invoke(_data.card);
                });
                break;

            case "cl_role":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    Role[] role = JsonConvert.DeserializeObject<JSON.Role[]>(data.data);

                    foreach(Role _role in role)
                    {
                        if (_role.UserID == Session.UId)
                        {
                            Session.role = (ERole)_role.role;

                            if(Session.role == ERole.firstThrower || Session.role == ERole.thrower)
                            {
                                m_roomRow.GameUI.showFoldButton();
                            }

                            m_roomRow.Grabed = false;
                            m_roomRow.Folded = false;
                            m_roomRow.Passed = false;
                        }
                        else
                        {
                            for(int i = 1; i < m_roomRow.roomPlayers.Count; i++)
                            {
                                if(m_roomRow.roomPlayers[i].UserID == _role.UserID)
                                {
                                    m_roomRow.roomPlayers[i].role = (ERole)_role.role;
                                }
                            }
                        }
                    }
                });
                break;

            case "cl_ThrowedCard":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var _data = JsonConvert.DeserializeObject<JSON._Card>(data.data);

                    placeCard?.Invoke(_data.UId, _data.card);
                });
                break;

            case "cl_BeatCard":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var _data = JsonConvert.DeserializeObject<JSON.Battle>(data.data);

                    beatCard?.Invoke(_data.UserID, _data.attacedCard, _data.attacingCard);
                });
                break;

            case "grab":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    playerGrab?.Invoke();
                    Debug.Log("It was a playerGrab event");
                });
                break;

            case "cl_grab":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    cl_grab?.Invoke();
                    Debug.Log("It was a cl_grab event");
                });
                break;

            case "cl_playerFold":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var _data = JsonConvert.DeserializeObject<JSON.Client>(data.data);

                    cl_fold?.Invoke(_data.UserID);
                });
                break;

            case "cl_pass":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var _data = JsonConvert.DeserializeObject<JSON.Client>(data.data);

                    cl_pass?.Invoke(_data.UserID);

                    Debug.Log("It was a cl_pass event");
                });
                break;

            case "cl_fold":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    FoldAllCards?.Invoke();
                });
                break;

            case "error":
                error?.Invoke(data.data);
                break;

            default:
                Debug.Log("Unknown event type: " + data.eventType);
                break;
        }
    }

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


    ////////////\\\\\\\\\\\\\
    // room emit functions \\

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

    public void GetAllRoomPlayersID()
    {
        var exitRoomData = new JSON.Room()
        {
            RoomID = Session.RoomID
        };

        SendMessageToServer("get_RoomPlayers", exitRoomData);
    }

    ///////////\\\\\\\\\\\\
    // Playing functions \\

    public void EmitThrow(Card throw_Card)
    {
        var data = new JSON.ClientThrow()
        {
            card = throw_Card,
            RoomID = Session.RoomID,
            UserID = Session.UId
        };

        SendMessageToServer("srv_Throw", data);
    }

    public void EmitBeat(Card beat, Card beating)
    {
        var data = new JSON.Battle()
        {
            attacedCard = beat,
            attacingCard = beating,
            RoomID = Session.RoomID,
            UserID = Session.UId
        };

        SendMessageToServer("srv_battle", data);
    }

    public void EmitReady(uint RoomID)
    {
        var readyData = new JSON.ServerJoinRoom()
        {
            RoomID = RoomID
        };

        SendMessageToServer("srv_ready", readyData);
    }

    public void EmitFold()
    {
        var foldData = new JSON.Token()
        {
            token = Session.Token
        };

        SendMessageToServer("srv_fold", foldData);
    }
    public void EmitPass()
    {
        var tokenData = new JSON.Token()
        {
            RoomID = Session.RoomID,
            token = Session.Token
        };

        SendMessageToServer("srv_pass", tokenData);
    }
    public void EmitGrab()
    {
        var tokenData = new JSON.Token()
        {
            RoomID = Session.RoomID,

            token = Session.Token
        };

        SendMessageToServer("srv_grab", tokenData);
    }

    ////////////\\\\\\\\\\\\\\
    // enter emit functions \\
    public void Login(string _name, string _password)
    {
        var userData = new JSON.ClientLogin()
        {
            name = _name,
            password = _password
        };

        SendMessageToServer("Login", userData);
    }

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

    ////////////\\\\\\\\\\\\
    // get emit functions \\

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

    public void getAvatar(uint ID)
    {
        var requestData = new JSON.Client()
        {
            UserID = ID
        };

        SendMessageToServer("getAvatar", requestData);
    }

    public void setAvatar(string imagePath)
    {
        byte[] imageBytes = File.ReadAllBytes(imagePath);
        string base64Image = Convert.ToBase64String(imageBytes);

        var avatarData = new JSON.AvatarData()
        {
            UserID = Session.UId,
            avatarImage = base64Image
        };
        
        SendMessageToServer("setAvatar", avatarData);
        Debug.Log("avatar is setted: " + avatarData.ToString());
    }
}
