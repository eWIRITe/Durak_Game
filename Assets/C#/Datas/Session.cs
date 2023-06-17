using UnityEngine;

public static class Session
{
    ///////\\\\\\
    // evennts \\
    ///////\\\\\\
    public delegate void chipsChangeEvent(uint chips);
    public static event chipsChangeEvent changeChips;
    
    public delegate void UIdChangeEvent(uint UId);
    public static event UIdChangeEvent changeUId;

    public delegate void roleChange(ERole role);
    public static event roleChange roleChanged;

    /////////\\\\\\\\
    // Player DATA \\
    /////////\\\\\\\\
    private static string m_token;
    public static string Token
    {
        get { return m_token; }
        set { m_token = value; }
    }

    private static uint m_id;
    public static uint UId
    {
        get { return m_id; }
        set
        {
            m_id = value;

            //set event
            changeUId?.Invoke((uint)value);
        }
    }

    private static string m_name;
    public static string Name
    {
        get { return m_name; }
        set { m_name = value; }
    }

    private static int m_chips;
    public static int Chips
    {
        get { return m_chips; }
        set { 
            m_chips = value;

            changeChips?.Invoke((uint)value);
        }
    }

    public static int played_games;
    public static int PlayedGames
    {
        get { return played_games; }
        set { played_games = value; }
    }

    ////////\\\\\\\
    // Room Data \\
    ////////\\\\\\\
    private static uint m_rid;
    public static uint RoomID
    {
        get { return m_rid; }
        set { m_rid = value; }
    }

    private static uint m_bet;
    public static uint Bet
    {
        get { return m_bet; }
        set { m_bet = value; }
    }

    private static ERole _role;
    public static ERole role
    {
        set
        {
            _role = value;
            roleChanged?.Invoke(value);
        }
        get { return _role; }
    }
}
