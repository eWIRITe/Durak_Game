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

        public uint roomOwner;
    }

    public class ServerJoinRoom
    {
        public string Token;

        public uint RoomID;

<<<<<<<< Updated upstream:Assets/C#/Scripts/JSON/ServerRoom.cs
        public string key;

        public uint RoomOwnerID;
========
        public uint roomOwner;

        public int type;
>>>>>>>> Stashed changes:Assets/C#/Datas/JSON/ServerRoom.cs
    }

    public class ServerExitRoom
    {
        public string token;

        public uint rid;
    }
}
