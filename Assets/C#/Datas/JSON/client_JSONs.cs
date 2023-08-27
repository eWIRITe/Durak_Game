using JSON_card;
using System.Collections.Generic;
using System.Linq;

namespace JSON_client
{
    #region registration requests

    public class ClientLogin
    {
        public string token;
        public string name;
        public uint UserID;
    }

    public class Sucsessed_emailChange
    {
        public string newEmail;
    }
    #endregion

    #region basic requests

    public class MessageData
    {
        public string eventType;
        public string data;
    }

    public class Token
    {
        public string token;
        public uint RoomID;
    }

    public class Role
    {
        public uint UserID;
        public int role;
    }

    public class AvatarData
    {
        public uint UserID;
        public string avatarImage;
    }

    public class PlayedUserGames
    {
        public int games;
    }

    public class FreeRooms
    {
        public uint[] FreeRoomsID;
    }

    public class PlayersInRoom
    {
        public uint[] PlayersID;
    }

    public class ClientData
    {
        public string token;
        public int chips;
    }

    public class ClientReady
    {
        public uint first;
        public Card trump;
    }
    #endregion

    #region room enter requests 
    public class JoinRoom
    {
        public uint roomOwnerID;
        public uint RoomID;

        public int bet;
        public ETypeGame type;
        public int cards;
        public int maxPlayers;
    }

    public class PlayerJoin
    {
        public uint playerID;
    }
    public class PlayerExit
    {
        public uint playerID;
    }
    #endregion

    #region playing requests

    public class Battle
    {
        public uint UserID;
        public uint RoomID;

        public Card attacingCard;
        public Card attacedCard;
    }

    public class ClientThrow
    {
        public uint RoomID;
        public uint UserID;

        public Card card;
    }

    public class ServerBattle
    {
        public string token;

        public byte[] attacked;
        public byte[] attacking;
    }

    public class ClientDistribution
    {
        public List<byte> cards;
    }
    public class ClientGrab
    {
        public uint uid;
    }
    public class ClientFold
    {
        public uint uid;
    }
    public class playerWon
    {
        public uint UserID;
    }
    public class won
    {
        public int chips;
    }
    #endregion

    #region chat client requests

    public class GotMessage
    {
        public uint UserID;
        public string message;
    }
    #endregion

    public class Client
    {
        public uint UserID;
    }
}
