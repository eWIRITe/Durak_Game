namespace JSON
{
    public class ServerCreateRoom
    {
        public string token;

        public bool isPrivate;

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

        public string key;

        public uint RoomOwnerID;
    }

    public class ServerExitRoom
    {
        public string token;

        public uint rid;
    }
}
