namespace JSON
{
    public class ServerCreateRoom
    {
        public uint RoomID;

        public string token;

        public int isPrivate;

        public string key;

        public uint bet;

        public uint cards;

        public int type;

        public uint maxPlayers;
    }

    public class ServerJoinRoom
    {
        public uint joinUserID;

        public uint RoomID;

        public uint roomOwner;
    }

    public class ServerExitRoom
    {
        public string token;

        public uint rid;
    }
}
