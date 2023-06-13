namespace JSON
{
    public class ServerCreateRoom
    {
        public string token;

        public int isPrivate;

        public string key;

        public uint bet;

        public uint cards;

        public int type;

        public uint maxPlayers;

        public uint roomOwner;
    }

    public class ServerJoinRoom
    {
        public uint uid;

        public string Token;

        public uint RoomID;

        public string key;

        public uint roomOwner;

        public int type;
    }

    public class ServerExitRoom
    {
        public string token;

        public uint rid;
    }
}
