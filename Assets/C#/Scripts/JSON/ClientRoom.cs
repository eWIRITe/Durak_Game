namespace JSON
{
    public class Room
    {
        public uint RoomID;
    }

    public class ClientCreateRoom
    {
        public uint uid; // owner user id

        public uint rid; // room id

        public uint players;

        public uint maxPlayers;

        public uint cards;

        public int type;

        public uint bet;
    }

    public class ClientJoinRoom
    {
        public uint uid; // owner user id

        public uint pid; // place id

        public uint rid; // room id

        public uint players;

        public uint maxPlayers;

        public uint cards;

        public ETypeGame type;

        public uint bet;
    }

    public class ClientExitRoom
    {
        public uint uid; // owner user id

        public uint pid; // place id

        public uint rid; // room id

        public uint players;

        public uint maxPlayers;

        public uint cards;

        public ETypeGame type;

        public uint bet;
    }
}
