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

    private Uri m_url;
    private SocketIOUnity m_socket;

    // the room the player is in
    private Room m_room;
    
    public Room Room
    {
        set { m_room = value; }
    }

    // private List<Ro>
    void Start()
    {
        m_url = new Uri("ws://127.0.0.1:5000");
        m_socket = new SocketIOUnity(m_url, new SocketIOOptions {
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });

        m_socket.unityThreadScope = UnityThreadScope.Update;
        m_socket.JsonSerializer = new NewtonsoftJsonSerializer();

        m_socket.OnUnityThread("cl_createRoom", response =>
        {
            Debug.Log("fdvbx");

            //var json = response.GetValue<JSON.ClientCreateRoom>();

            //m_menuScreen.CreateRoom(json);

            //// my create
            //if (json.uid == Session.UId)
            //{
            //    this.EmitJoinRoom((int)json.rid);

            //    //string sceneName = "GameMatch";
            //    //SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            //}
        });

        m_socket.OnUnityThread("cl_joinRoom", response =>
        {
            var json = response.GetValue<JSON.ClientJoinRoom>();

            m_menuScreen.JoinRoom(json);

            // my join
            if (json.uid == Session.UId)
            {
                this.EmitJoinRoom((int)json.rid);

                string sceneName = "GameMatch";
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);

                Session.Rid = json.rid;
                Session.Bet = json.bet;
                Session.Players = json.players;
                Session.MaxPlayers = json.maxPlayers;
            }
            else
            // join in my room
            if (json.rid == Session.Rid)
            {
                Session.Players++;
                m_room.AddPlayer(json.uid, json.pid);
            }
        });

        m_socket.OnUnityThread("cl_exitRoom", response =>
        {
            var json = response.GetValue<JSON.ClientExitRoom>();

            m_menuScreen.ExitRoom(json);

            // my exit
            if (json.uid == Session.UId)
            {
                // SceneManager.LoadScene("GameMatch", LoadSceneMode.Additive);
                SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            }
            else
            // exit from my room
            if (json.rid == Session.Rid)
            {
                Session.Players--;
                m_room.RemovePlayer(json.uid, json.pid);
            }
        });

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

        m_socket.OnUnityThread("cl_start", response =>
        {
            var json = response.GetValue<JSON.ClientStart>();
            m_room.StartGame(json.first, json.trump);
        });

        m_socket.OnUnityThread("cl_finish", response =>
        {
        });

        m_socket.OnUnityThread("cl_turn", response =>
        {
            var json = response.GetValue<JSON.ClientTurn>();
            m_room.PlayersTurn(json.uid);
        });

        ///// reserved socketio events
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
        ////

        m_socket.Connect();
    }

    void OnApplicationQuit()
    {
        m_socket.Disconnect();
    }

    public void EmitCreateRoom(string token, bool isPrivate, string key, uint bet, uint cards, uint maxPlayers, ETypeGame type)
    {
        m_socket.Emit("srv_createRoom", new JSON.ServerCreateRoom() { token = token, isPrivate = isPrivate, key = key, bet = bet, cards = cards, type = type, maxPlayers = maxPlayers });
    }

    public void EmitJoinRoom(int rid, string key = "")
    {
        m_socket.Emit("srv_joinRoom", new JSON.ServerJoinRoom() { token = Session.Token, rid = Convert.ToUInt32(rid), key = key });
    }

    public void EmitExitRoom(uint rid)
    {
        m_socket.Emit("srv_exitRoom", new JSON.ServerExitRoom() { token = Session.Token, rid = rid });
    }

    public void EmitFold()
    {
        m_socket.Emit("srv_fold", new JSON.Token() { token = Session.Token });
    }

    public void EmitReady()
    {
        m_socket.Emit("srv_ready", new JSON.Token() { token = Session.Token });
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
}
