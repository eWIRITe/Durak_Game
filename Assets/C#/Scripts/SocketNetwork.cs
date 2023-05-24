using System.Collections.Generic;
using System;
using UnityEngine;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine.UI;
using System.Collections;

using static SocketIOUnity;
using UnityEngine.SceneManagement;
using JSON;

public class SocketNetwork : MonoBehaviour
{ 
    public MenuScreen m_menuScreen;

    public CardController card;

    private ScreenDirector _scrDirector;

    private Uri m_url;
    private SocketIOUnity m_socket;

    public Room m_room;
    public RoomRow m_roomRow;
    public GameObject RoomPrefab;

    public int _type;

    protected Network m_network;

    //room was created event
    public delegate void RoomCreateEvent();
    public static event RoomCreateEvent _roomCreateEvent;

    // private List<Ro>
    void Start()
    {
        //Server settings\\
        /////////\\\\\\\\\\
        m_url = new Uri("ws://127.0.0.1:5000");
        //socket settings\\
        m_socket = new SocketIOUnity(m_url, new SocketIOOptions {
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });
        m_socket.unityThreadScope = UnityThreadScope.Update;
        m_socket.JsonSerializer = new NewtonsoftJsonSerializer();

        m_network = GameObject.FindGameObjectWithTag("Network").GetComponent<Network>();

        //Game start server resposses\\
        ///////////////=\\\\\\\\\\\\\\\

        //we are entering to the room
        m_socket.OnUnityThread("cl_enterInTheRoom", response =>
        {
            var json = response.GetValue<JSON.ClientJoinRoom>();

            Debug.Log(json.RoomID);

            GameObject Room = Instantiate(RoomPrefab);

            m_room = Room.GetComponent<Room>();
            m_roomRow = Room.GetComponent<RoomRow>();

            m_roomRow.RoomOwner = json.roomOwner;
            m_roomRow.RoomID = json.RoomID;

            Session.RoomID = json.RoomID;

            Debug.Log("cl_enterInTheRoom: " + json);
        });

        //we are entering to the room as owner
        m_socket.OnUnityThread("cl_enterInTheRoomAsOwner", response =>
        {
            var json = response.GetValue<JSON.ClientJoinRoom>();

            Debug.Log(json.RoomID);

            GameObject Room = Instantiate(RoomPrefab);

            m_room = Room.GetComponent<Room>();
            m_roomRow = Room.GetComponent<RoomRow>();

            m_roomRow.RoomOwner = json.roomOwner;
            m_roomRow.RoomID = json.RoomID;

            Session.RoomID = json.RoomID;

            Debug.Log("cl_enterInTheRoomAsOwner: " + json);
        });

        //when some ather one joinning in the room
        m_socket.OnUnityThread("cl_joinRoom", response =>
        {
            var json = response.GetValue<JSON.ClientJoinRoom>();

            m_room.NewPlayerJoin(json.uid);

            Debug.Log("cl_joinRoom: " + response);
        });

        //Someone exit from the room
        m_socket.OnUnityThread("cl_exitRoom", response =>
        {
            var json = response.GetValue<JSON.ClientExitRoom>();

            m_room.DeletePlayer(json.uid);

            Debug.Log("cl_exitRoom: " + response);
        });

        m_socket.OnUnityThread("cl_RoomWasCreated", response => 
        {
            var json = response.GetValue<JSON.ServerCreateRoom>();

            _roomCreateEvent?.Invoke();
        });

        //////////\\\\\\\\\\\
        //playing functions\\
        //////////\\\\\\\\\\\
        m_socket.OnUnityThread("cl_battle", response =>
        {
            var json = response.GetValue<JSON.ClientBattle>();

            if (json.attacked.Length > 0)
            {
                for (int i = 0; i < json.attacked.Length; ++i)
                {
                    m_room.Beat(new Card(json.attacked[i]), new Card(json.attacking[i]));
                }
            }
            else
            {
                foreach (byte card in json.attacking)
                {
                    m_room.Attack(new Card(card));
                }
            }
        });

        m_socket.OnUnityThread("cl_transfer", response =>
        {
            var json = response.GetValue<JSON.ClientTransfer>();
            m_room.SpawnBattlefieldCard(new Card(json.card));
        });

        m_socket.OnUnityThread("cl_grab", response =>
        {
            var json = response.GetValue<JSON.ClientGrab>();
            m_room.Grab(json.uid);
        });

        m_socket.OnUnityThread("cl_pass", response =>
        {
        });

        m_socket.OnUnityThread("cl_fold", response =>
        {
            var json = response.GetValue<JSON.ClientFold>();
            m_room.Fold(json.uid);
        });

        m_socket.OnUnityThread("cl_ready", response =>
        {
            m_room.OnReady();
        });

        m_socket.OnUnityThread("cl_distribution", response =>
        {
            var json = response.GetValue<JSON.ClientDistribution>();

            List<Card> cards = new List<Card>();

            foreach (byte card in json.cards)
            {
                cards.Add(new Card(card));
            }

            m_room.Distribution(cards.ToArray());
        });

        m_socket.OnUnityThread("cl_finish", response =>
        {
        });

        m_socket.OnUnityThread("cl_turn", response =>
        {
            var json = response.GetValue<JSON.ClientTurn>();
            m_room.PlayersTurn(json.uid);
        });

        m_socket.OnUnityThread("cl_isAdmin", response =>
        {
            _scrDirector = GameObject.FindGameObjectWithTag("ScreenDirector").GetComponent<ScreenDirector>();

            _scrDirector.activeAdminPanel();
        });

        /////////////////\\\\\\\\\\\\\\\\\\\
        ///// reserved socketio events \\\\\
        ///++++++++++++++++++++++++++++++\\\
        m_socket.OnConnected += (sender, e) =>
        {
            Debug.Log("socket.OnConnected");
        };
        m_socket.OnPing += (sender, e) =>
        {
            Debug.Log("Ping");
        };
        m_socket.OnPong += (sender, e) =>
        {
            Debug.Log("Pong: " + e.TotalMilliseconds);
        };
        m_socket.OnDisconnected += (sender, e) =>
        {
            Debug.Log("disconnect: " + e);
        };
        m_socket.OnReconnectAttempt += (sender, e) =>
        {
            Debug.Log($"{DateTime.Now} Reconnecting: attempt = {e}");
        };


        //connect to server\\
        //////////=\\\\\\\\\\
        m_socket.Connect();
    }

    //rooms\\
    //<<_>>\\
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

        m_socket.Emit("srv_createRoom", new JSON.ServerCreateRoom() { token = token, isPrivate = isPrivate, key = key, bet = bet, cards = cards, type = _type, maxPlayers = maxPlayers, roomOwner = Session.UId });
    }
    public void EmitJoinRoom(uint RoomID)
    {
        m_socket.Emit("srv_joinRoom", new JSON.ServerJoinRoom() { Token = Session.Token, RoomID = Convert.ToUInt32(RoomID) });
    }
    public void EmitExitRoom(uint rid)
    {
        m_socket.Emit("srv_exitRoom", new JSON.ServerExitRoom() { token = Session.Token, rid = rid });
    }

    //Playing functions\\
    //:::::::::::::::::\\
    public void EmitFold()
    {
        m_socket.Emit("srv_fold", new JSON.Token() { token = Session.Token });
    }
    public void EmitReady(uint RoomID)
    {
        m_socket.Emit("srv_ready", new JSON.Token() { token = Session.Token, RoomID = RoomID });
    }
    public void EmitPass()
    {
        m_socket.Emit("srv_pass", new JSON.Token() { token = Session.Token });
    }
    public void EmitWhatsup()
    {
        m_socket.Emit("srv_whatsup", new JSON.Token() { token = Session.Token });
    }
    public void EmitGrab()
    {
        m_socket.Emit("srv_grab", new JSON.Token() { token = Session.Token });
    }
    public void EmitTransfer(Card card)
    {
        m_socket.Emit("srv_transfer", new JSON.ServerTransfer() { token = Session.Token, card = card.Byte });
    }
    public void EmitBattle(List<Card> attacked, List<Card> attacking)
    {
        byte[] ExtractBytes(List<Card> cards)
        {
            List<byte> bytes = new List<byte>();

            foreach (Card card in cards)
            {
                bytes.Add(card.Byte);
            }

            return bytes.ToArray();
        }

        m_socket.Emit("srv_battle", new JSON.ServerBattle() { token = Session.Token, attacked = ExtractBytes(attacked), attacking = ExtractBytes(attacking) });
    }

    //Set on application quit\\
    //.......................\\
    void OnApplicationQuit()
    {
        if(m_roomRow != null) EmitExitRoom(m_roomRow.RoomID);
        m_socket.Disconnect();
    }
}
