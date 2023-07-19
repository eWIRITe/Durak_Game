using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.IO;
using System;
using TMPro;
using JSON_card;
using JSON_client;
using JSON_server;

public class SocketNetwork : MonoBehaviour
{
    [Header("Debug")]
    public bool debugEnteringReqests;
    public bool debugExitingRequests;

    [Header("other")]
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
    public delegate void LoginEvent(string token, string name);
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

    // get games event
    public delegate void games(int games);
    public static event games gotGames;

    //Error event
    public delegate void Error(string error);
    public static event Error error;

    //got raiting event
    public delegate void Raiting(List<RatingScreen.RatingLine> raiting);
    public static event Raiting gotRaiting;

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

    // Turn
    public delegate void chat(uint ID, string message);
    public static event chat got_message;

    public TMP_Text server_debug_text;

    void Start()
    {
        // Server initiolization
        string url = "ws://localhost:9954";
        websocket = new WebSocket(url);

        websocket.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket connection opened, Url: " + websocket.Url.ToString());
        };
        websocket.OnMessage += (sender, e) =>
        {
            string message = e.Data;

            if (debugEnteringReqests) Debug.Log("get: " + message);

            HandleMessageFromServer(message);
        };
        websocket.OnError += (sender, e) =>
        {
            Debug.LogError("WebSocket: " + websocket.Url.ToString() + ", error: " + e.Exception + " : " + e.Message + " : " + sender);
        };
        websocket.OnClose += (sender, e) =>
        {
            Debug.LogError("WebSocket connection closed, Url: " + websocket.Url.ToString());
            Debug.LogError(e.Reason + ", code: " + e.Code + ", was clean: " + e.WasClean);
        };

        websocket.Connect();
    }


    
    #region  Server comunication
    void HandleMessageFromServer(string message)
    {
        JSON_client.MessageData data = JsonConvert.DeserializeObject<JSON_client.MessageData>(message);

        Debug.Log(data.ToString());

        switch (data.eventType)
        {

            //////////////////////////
            // room message handler //
            //////////////////////////

            case "cl_enterInTheRoom":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var enterRoomData = JsonConvert.DeserializeObject<JSON_client.JoinRoom>(data.data);

                    Session.RoomID = enterRoomData.RoomID;

                    GameObject Room = Instantiate(RoomPrefab);

                    m_room = Room.GetComponent<Room>();
                    m_roomRow = Room.GetComponent<RoomRow>();

                    m_roomRow.RoomOwner = enterRoomData.roomOwnerID;

                    m_roomRow.GameType = (ETypeGame)enterRoomData.type;

                    m_roomRow.maxCards_number = enterRoomData.cards;

                    m_roomRow.maxPlayers_number = enterRoomData.maxPlayers;

                    m_roomRow.Bet = enterRoomData.bet;

                    m_roomRow.RoomID = enterRoomData.RoomID;
                });
                break;

            case "cl_enterInTheRoomAsOwner":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var enterRoomOwnerData = JsonConvert.DeserializeObject<JSON_client.JoinRoom>(data.data);

                    Session.RoomID = enterRoomOwnerData.RoomID;

                    GameObject Room = Instantiate(RoomPrefab);

                    m_room = Room.GetComponent<Room>();
                    m_roomRow = Room.GetComponent<RoomRow>();

                    m_roomRow.RoomOwner = Session.UId;

                    m_roomRow.GameType = (ETypeGame)enterRoomOwnerData.type;

                    m_roomRow.maxCards_number = enterRoomOwnerData.cards;

                    m_roomRow.maxPlayers_number = enterRoomOwnerData.maxPlayers;

                    m_roomRow.Bet = enterRoomOwnerData.bet;

                    m_roomRow.RoomID = enterRoomOwnerData.RoomID;
                });
                
                break;

            case "cl_startGameAlong":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    m_room.startGameAlone();
                });
                break;

            case "cl_joinRoom":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var joinRoomData = JsonConvert.DeserializeObject<JSON_client.PlayerJoin>(data.data);
                    m_room.NewPlayerJoin(joinRoomData.playerID);
                });
                break;

            case "cl_leaveRoom":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var joinRoomData = JsonConvert.DeserializeObject<JSON_client.PlayerExit>(data.data);
                    m_room.DeletePlayer(joinRoomData.playerID);
                });
                break;

            ///////////////////////////
            // enter message handler //
            ///////////////////////////
            case "sucsessedLogin":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var loginData = JsonConvert.DeserializeObject<JSON_client.ClientLogin>(data.data);
                    loginSucsessed?.Invoke(loginData.token, loginData.name);
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
                    var freeRoomsID = JsonConvert.DeserializeObject<JSON_client.FreeRooms>(data.data);
                    roomChange?.Invoke(freeRoomsID.FreeRoomsID);
                });
                break;

            case "roomPlayersID":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var playersID = JsonConvert.DeserializeObject<JSON_client.PlayersInRoom>(data.data);
                    changePlayers?.Invoke(playersID.PlayersID);
                });
                break;

            case "Chips":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var chips = JsonConvert.DeserializeObject<JSON_client.ClientData>(data.data);
                    gotChips?.Invoke(chips.chips);
                });
                break;

            case "got_rating":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    List<RatingScreen.RatingLine> raitingLine = JsonConvert.DeserializeObject<List<RatingScreen.RatingLine>>(data.data);
                    gotRaiting?.Invoke(raitingLine);
                });
                break;

            case "gameStats":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var games = JsonConvert.DeserializeObject<JSON_client.PlayedUserGames>(data.data);
                    gotGames?.Invoke(games.games);
                });
                break;

            case "ready":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var ready_data = JsonConvert.DeserializeObject<JSON_client.ClientReady>(data.data);

                    ready?.Invoke(ready_data.trump);
                });
                break;

            /////////////////////////////
            // playing message handler //
            /////////////////////////////

            case "chat_message":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var _data = JsonConvert.DeserializeObject<JSON_client.GotMessage>(data.data);

                    got_message?.Invoke(_data.UserID, _data.message);
                });
                break;

            case "cl_getImage":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var _data = JsonConvert.DeserializeObject<JSON_client.AvatarData>(data.data);

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
                    var Card = JsonConvert.DeserializeObject<Card>(data.data);
                    GetCard?.Invoke(Card);
                });
                
                break;

            case "atherUserGotCard":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var user = JsonConvert.DeserializeObject<JSON_client.Client>(data.data);
                    userGotCard?.Invoke(user.UserID);
                });

                break;

            case "cl_gotCard":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var user = JsonConvert.DeserializeObject<JSON_client.Client>(data.data);
                    userGotCard?.Invoke(user.UserID);
                });
                break;

            case "cl_destroyCard":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var user = JsonConvert.DeserializeObject<JSON_client.Client>(data.data);
                    userDestroyCard?.Invoke(user.UserID);
                });
                break;

            case "DestroyCard":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var _data = JsonConvert.DeserializeObject<_card>(data.data);
                    DestroyCard?.Invoke(_data.card);
                });
                break;

            case "cl_role":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var role = JsonConvert.DeserializeObject<JSON_client.Role[]>(data.data);

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
                    var _data = JsonConvert.DeserializeObject<_card>(data.data);

                    placeCard?.Invoke(_data.UId, _data.card);
                });
                break;

            case "cl_BeatCard":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var _data = JsonConvert.DeserializeObject<JSON_client.Battle>(data.data);

                    beatCard?.Invoke(_data.UserID, _data.attacedCard, _data.attacingCard);
                });
                break;

            case "grab":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    playerGrab?.Invoke();
                });
                break;

            case "cl_grab":
                MainThreadDispatcher.RunOnMainThread(() => { cl_grab?.Invoke(); });
                break;

            case "cl_playerFold":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var _data = JsonConvert.DeserializeObject<JSON_client.Client>(data.data);

                    cl_fold?.Invoke(_data.UserID);
                });
                break;

            case "cl_pass":
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    var _data = JsonConvert.DeserializeObject<JSON_client.Client>(data.data);

                    cl_pass?.Invoke(_data.UserID);
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
        JSON_client.MessageData messageData = new JSON_client.MessageData()
        {
            eventType = eventType,
            data = JsonConvert.SerializeObject(data)
        };

        string json = JsonConvert.SerializeObject(messageData);
        if (websocket != null && websocket.IsAlive)
        {
            if (debugExitingRequests) Debug.Log("send: " + json);
            websocket.Send(json);
        }
    }
    #endregion

    #region room emit functions 

    public void EmitCreateRoom(string token, int isPrivate, string key, uint bet, uint cards, int maxPlayers, ETypeGame type)
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

        var createRoomData = new JSON_server.ServerCreateRoom()
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
        var joinRoomData = new JSON_server.ServerJoinRoom()
        {
            Token = Session.Token,
            key = "",
            RoomID = (uint)RoomID
        };

        SendMessageToServer("srv_joinRoom", joinRoomData);
    }

    public void EmitExitRoom(uint rid)
    {
        var exitRoomData = new JSON_server.ServerExitRoom()
        {
            token = Session.Token,
            rid = rid
        };

        SendMessageToServer("srv_exit", exitRoomData);
    }

    public void GetAllRoomPlayersID()
    {
        var exitRoomData = new JSON_server.UserData()
        {
            RoomID = Session.RoomID
        };

        SendMessageToServer("get_RoomPlayers", exitRoomData);
    }

    #endregion

    #region playing emit functions 

    public void EmitThrow(Card throw_Card)
    {
        var data = new JSON_server.Throw()
        {
            RoomID = Session.RoomID,
            UserID = Session.UId,

            card = throw_Card
        };

        SendMessageToServer("srv_Throw", data);
    }

    public void EmitBeat(Card beat, Card beating)
    {
        var data = new JSON_server.Battle()
        {
            RoomID = Session.RoomID,
            UserID = Session.UId,

            attakingCard = beating,
            attakedCard = beat
        };

        SendMessageToServer("srv_battle", data);
    }

    public void EmitReady(uint RoomID)
    {
        var readyData = new JSON_server.ServerJoinRoom()
        {
            RoomID = RoomID
        };

        SendMessageToServer("srv_ready", readyData);
    }

    public void EmitFold()
    {
        var foldData = new JSON_server.UserData()
        {
            token = Session.Token,
            RoomID = Session.RoomID
        };

        SendMessageToServer("srv_fold", foldData);
    }
    public void EmitPass()
    {
        var tokenData = new JSON_server.UserData()
        {
            RoomID = Session.RoomID,
            token = Session.Token
        };

        SendMessageToServer("srv_pass", tokenData);
    }
    public void EmitGrab()
    {
        var tokenData = new JSON_server.UserData()
        {
            RoomID = Session.RoomID,

            token = Session.Token
        };

        SendMessageToServer("srv_grab", tokenData);
    }

    #endregion

    #region registration emit functions
    public void Emit_login(string _name, string _password)
    {
        var userData = new JSON_server.ClientLogin()
        {
            name = _name,
            password = _password
        };

        SendMessageToServer("Emit_login", userData);
    }

    public void Emit_signIn(string _name, string _email, string _password)
    {
        var userData = new JSON_server.ClientSignIN()
        {
            name = _name,
            password = _password,
            email = _email
        };

        SendMessageToServer("Emit_signIn", userData);
    }

    public void Emit_changeEmail(string tocken, string email)
    {

    }
    #endregion

    ////////////\\\\\\\\\\\\
    // get emit functions \\

    public void GetUserID(string token)
    {
        var userData = new JSON_server.UserData()
        {
            token = token
        };

        SendMessageToServer("getId", userData);
    }

    public void GetFreeRooms()
    {
        SendMessageToServer("getFreeRooms", new UserData());
    }

    public void GetChips(string token)
    {
        var userData = new JSON_server.UserData()
        {
            token = token
        };

        SendMessageToServer("getChips", userData);
    }

    public void getAvatar(uint ID)
    {
        var requestData = new JSON_server.UserData()
        {
            UserID = ID
        };

        SendMessageToServer("getAvatar", requestData);
    }

    public void setAvatar(string imagePath)
    {
        byte[] imageBytes = File.ReadAllBytes(imagePath);
        string base64Image = Convert.ToBase64String(imageBytes);

        var avatarData = new JSON_server.AvatarData()
        {
            UserID = Session.UId,
            avatarImage = base64Image
        };
        
        SendMessageToServer("setAvatar", avatarData);
        Debug.Log("avatar is setted: " + avatarData.ToString());
    }

    public void get_gameStat()
    {
        var token = new JSON_server.UserData()
        {
            token = Session.Token
        };

        SendMessageToServer("get_gamesStat", token);
    }

    public void getRaiting()
    {
        var token = new JSON_server.UserData()
        {
            token = Session.Token
        };

        SendMessageToServer("get_raiting", token);
    }

    public void Emit_sendMessage(string message)
    {
        var data = new JSON_server.SendMessage()
        {
            RoomID = Session.RoomID,
            token = Session.Token, 
            message = message
        };

        SendMessageToServer("send_message", data);
    }

    private void OnApplicationQuit()
    {
        if(Session.RoomID != 0)
        {
            EmitExitRoom(Session.RoomID);
        }
    }
}
