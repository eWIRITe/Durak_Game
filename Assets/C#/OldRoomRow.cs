using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OldRoomRow : MonoBehaviour
{
    // ui
    public TMP_Text m_betText;
    public TMP_Text m_nameText;
    public GameObject[] m_typeOfGameObjects;
    public TMP_Text m_numberOfCardsText;
    public Image[] m_playersImages;

    public Sprite m_leavedPlayer;
    public Sprite m_joinedPlayer;
    // end

    private uint m_bet;

    public uint Bet
    {
        get { return m_bet; }
    }

    private ETypeGame m_typeOfGame;

    public ETypeGame TypeGame
    {
        get { return m_typeOfGame; }
    }

    private uint m_numberOfCards;

    public uint NumberOfCards
    {
        get { return m_numberOfCards; }
    }

    private uint m_numberOfPlayers;

    public uint NumberOfPlayers
    {
        get { return m_numberOfPlayers; }
    }

    private uint m_maxNumberOfPlayers;

    public uint MaxNumberOfPlayers
    {
        get { return m_maxNumberOfPlayers; }
    }

    private uint m_rid;

    private SocketNetwork m_socketNetwork;

    private void Start()
    {
        m_socketNetwork = GameObject.FindGameObjectWithTag("SocketNetwork").GetComponent<SocketNetwork>();

        for (int i = 0; i < m_maxNumberOfPlayers; i++)
        {
            m_playersImages[i].gameObject.SetActive(true);
        }
    }

    private void SetType(ETypeGame type)
    {
        for (int i = 0; i < (int)ETypeGame.Count; ++i)
        {
            m_typeOfGameObjects[i].SetActive(false);
        }

        m_typeOfGameObjects[(int)type].SetActive(true);
    }

    private void SetPlayers(uint numberOfPlayers)
    {
        for (int i = 0; i < m_maxNumberOfPlayers; ++i)
        {
            m_playersImages[i].sprite = m_leavedPlayer;
        }

        m_numberOfPlayers = numberOfPlayers;

        for (int i = 0; i < m_numberOfPlayers; ++i)
        {
            m_playersImages[i].sprite = m_joinedPlayer;
        }
    }

    public void IncPlayers()
    {
        if (m_numberOfPlayers >= m_maxNumberOfPlayers)
        {
            Debug.LogError("m_numberOfPlayers >= m_maxNumberOfPlayers in Room.IncPlayers");
            return;
        }

        m_playersImages[m_numberOfPlayers++].sprite = m_joinedPlayer;
    }

    public void DecPlayers()
    {
        if (m_numberOfPlayers <= 0)
        {
            Debug.LogError("m_numberOfPlayers <= 0 in Room.DecPlayers");
            return;
        }

        m_playersImages[m_numberOfPlayers--].sprite = m_leavedPlayer;
    }

    public void Init(string name, uint rid, uint bet, ETypeGame type, uint cards, uint maxPlayers)
    {
        this.SetType(type);

        m_bet = bet;
        m_typeOfGame = type;
        m_numberOfCards = cards;

        m_rid = rid;
        m_betText.text = bet.ToString();
        m_nameText.text = name.ToString();
        m_numberOfCardsText.text = cards.ToString();
        m_maxNumberOfPlayers = maxPlayers;

        this.SetPlayers(0);

        m_numberOfPlayers = 0;
    }

    public void RoomClickHandler()
    {
        m_socketNetwork.EmitJoinRoom((int)m_rid);
    }
}
