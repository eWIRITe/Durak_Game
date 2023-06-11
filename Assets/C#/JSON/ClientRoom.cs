namespace JSON
{
    public class Room
    {
        public uint RoomID;
        public uint[] FreeRoomsID;
    }

    public class PlayersInRoom
    {
        public uint RoomID;
        public uint[] PlayersID;
    }

    public class ClientCreateRoom
    {
        public uint uid;

        public uint rid;

        public uint players;

        public uint maxPlayers;

        public uint cards;

        public int type;

        public uint bet;
    }

    public class ClientJoinRoom
    {
        public uint JoinUserID; 

        public uint RoomID;
<<<<<<< HEAD:Assets/C#/JSON/ClientRoom.cs
=======

        public uint players;

        public uint maxPlayers;

        public uint cards;

        public ETypeGame type;

        public uint bet;

        public uint RoomOwnerID;
<<<<<<< HEAD:Assets/C#/JSON/ClientRoom.cs
>>>>>>> parent of 1408e7d (finish):Assets/C#/Scripts/JSON/ClientRoom.cs
=======
>>>>>>> parent of 1408e7d (finish):Assets/C#/Scripts/JSON/ClientRoom.cs
    }

    public class ClientExitRoom
    {
        public uint ExitUserID;
    }
}
