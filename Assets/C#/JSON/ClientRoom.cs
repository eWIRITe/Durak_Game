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
        public uint uid; 

        public uint RoomID;
    }

    public class ClientExitRoom
    {
        public uint uid;
    }
}
