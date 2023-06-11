namespace JSON
{
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

    public class _Card
    {
        public uint UId;

        public Card card;
    }

    public class Card
    {
        public string suit;
        public string nominal;
    }
    public class AvatarData
    {
        public uint UserID;
        public string avatarImage;
    }
}
