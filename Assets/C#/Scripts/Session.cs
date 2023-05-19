using UnityEngine;

public static class Session
{
    private static string m_token;
    public static string Token
    {
        get { return m_token; }
        set { m_token = value; }
    }

    private static string m_name;
    public static string Name
    {
        get { return m_name; }
        set { m_name = value; }
    }

    private static uint m_id;
    public static uint UId
    {
        get { return m_id; }
        set { m_id = value; }
    }

    private static EStyle m_style = EStyle.Base;
    public static EStyle Style
    {
        get { return m_style; }
        set { m_style = value; }
    }

    private static ESortGeneral m_sortGeneral;

    public static ESortGeneral SortGeneral
    {
        get { return m_sortGeneral; }
        set { m_sortGeneral = value; }
    }

    private static ESortTrump m_sortTrump;

    public static ESortTrump SortTrump
    {
        get { return m_sortTrump; }
        set { m_sortTrump = value; }
    }

    // room
    private static uint m_rid;
    public static uint Rid
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

    private static uint m_players;
    public static uint Players
    {
        get { return m_players; }
        set { m_players = value; }
    }

    private static uint m_maxPlayers;
    public static uint MaxPlayers
    {
        get { return m_maxPlayers; }
        set { m_maxPlayers = value; }
    }
}
