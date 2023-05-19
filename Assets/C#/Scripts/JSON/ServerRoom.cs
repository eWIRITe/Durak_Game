namespace JSON
{
    public class ServerCreateRoom
    {
        public string token;

        public bool isPrivate;

        public string key;

        public uint bet;

        public uint cards;

        public ETypeGame type;

        public uint maxPlayers;
    }

    public class ServerJoinRoom
    {
        public string token;

        public uint rid;

        public string key;
    }

    public class ServerExitRoom
    {
        public string token;

        public uint rid;
    }
}
