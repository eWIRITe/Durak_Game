using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OldRoom : MonoBehaviour
{
    public Player m_prefabPlayer;

    public Transform[] m_spawnPoint2Players;
    public Transform[] m_spawnPoint3Players;
    public Transform[] m_spawnPoint4Players;
    public Transform[] m_spawnPoint5Players;
    public Transform[] m_spawnPoint6Players;

    public Transform m_mySpawnPoint;

    public CardController m_cardController;

    public Button m_ready;

    public Text m_chips;

    public Text m_bank;

    public PlayerHands m_playerHands;

    public Battlefield m_battlefield;

    private SocketNetwork m_socketNetwork;

    private Network m_network;

    private uint m_first;

    private Card m_trump;

    public Card Trump
    {
        get { return m_trump; }
    }

    private Player m_turn;

    private List<Player> m_players = new List<Player>();

    private List<GameCard> m_attacked = new List<GameCard>();
    private List<GameCard> m_attacking = new List<GameCard>();

    private void Start()
    {
        m_socketNetwork = GameObject.FindGameObjectWithTag("SocketNetwork").GetComponent<SocketNetwork>();
        m_network = GameObject.FindGameObjectWithTag("Network").GetComponent<Network>();

        //m_socketNetwork.Room = this;

        this.AddPlayer(Session.UId, 10);

        StartCoroutine(m_network.GetChips(Session.Token, GetChipsSuccessed, GetChipsFailed));
    }

    private void Update()
    {

    }

    private void GetChipsSuccessed(uint chips)
    {
        m_chips.text = chips.ToString();
    }

    private void GetChipsFailed(string resp)
    {
        Debug.LogError($"GetChipsFailed:\n\t{resp}");
    }

    public void ReadyClickHandler()
    {
        m_ready.enabled = false;

        m_socketNetwork.EmitReady();
    }

    public void TurnClickHandler()
    {

    }

    public void RulesClickHandler()
    {

    }

    public void FoldClickHandler()
    {
        m_playerHands.Clear();

        m_socketNetwork.EmitFold();
    }

    private Player GetPlayer(uint uid)
    {
        foreach (Player player in m_players)
        {
            if (player.Uid == uid)
            {
                return player;
            }
        }

        return null;
    }

    public void Grab(uint uid)
    {
        if (uid == Session.UId)
        {
            GameCard[] cards = m_battlefield.GetAllCards();
            foreach (GameCard card in cards)
            {
                m_playerHands.TakeCard(card);
            }
        }

        m_battlefield.Clear();
    }

    public void SpawnBattlefieldCard(Card card)
    {
        GameCard gameCard = m_cardController.SpawnCard(card, m_trump.Suit);
        m_battlefield.Attack(gameCard);
    }

    public void Attack(Card card)
    {
        GameCard gameCard = m_cardController.SpawnCard(card, m_trump.Suit);
        m_battlefield.Attack(gameCard);
    }

    public void Beat(Card attacked, Card attacking)
    {
        GameCard gameCard = m_cardController.SpawnCard(attacking, m_trump.Suit);
        m_battlefield.Beat(attacked, gameCard);
    }

    public void Fold(uint uid)
    {
        if (uid == Session.UId)
        {
            m_playerHands.Clear();
        }
        else
        {

        }
    }

    public void StartGame(uint first, byte trump)
    {
        m_first = first;
        m_trump = new Card(trump);
        m_ready.gameObject.SetActive(false);
        m_bank.text = (Session.Bet * Session.Players).ToString();

        this.PlayersTurn(m_first);
        m_battlefield.gameObject.SetActive(true);
    }

    public void FinishGame()
    {
        // show result table

        m_ready.enabled = false;
        m_ready.gameObject.SetActive(true);
        m_battlefield.gameObject.SetActive(false);
    }

    public void AddPlayer(uint uid, uint pid)
    {
        Transform parent;

        switch (Session.MaxPlayers)
        {
            case 2:
                parent = m_spawnPoint2Players[pid];
                break;

            case 3:
                parent = m_spawnPoint3Players[pid];
                break;

            case 4:
                parent = m_spawnPoint4Players[pid];
                break;

            case 5:
                parent = m_spawnPoint5Players[pid];
                break;

            case 6:
                parent = m_spawnPoint6Players[pid];
                break;

            default:
                parent = m_spawnPoint2Players[pid];
                break;
        }

        if (uid == Session.UId)
        {
            parent = m_mySpawnPoint;
        }

        Player player = GameObject.Instantiate<Player>(m_prefabPlayer, parent);

        player.Init(uid, pid);

        m_players.Insert(Convert.ToInt32(pid), player);
    }

    public void RemovePlayer(uint uid, uint pid)
    {
        Player player_ = null;

        foreach (Player player in m_players)
        {
            if (player.Uid == uid)
            {
                player_ = player;
                break;
            }
        }

        if (player_ != null)
        {
            m_players.Remove(player_);

            GameObject.Destroy(player_);
        }
    }

    //public bool Attack(GameCard card)
    //{
    //    return m_turn.Uid == Session.UId;
    //}

    //public bool Beat(GameCard attacked, GameCard attacking)
    //{
    //    if ((attacked.Card.Suit == attacking.Card.Suit && attacked.Card.Nominal < attacking.Card.Nominal) || 
    //        attacking.Card.Suit == m_trump.Suit)
    //    {

    //        return true;
    //    }

    //    return false;
    //}

    public void ToHeap()
    {
        GameCard[] cards = m_battlefield.GetAllCards();

        foreach (GameCard card in cards)
        {
            card.ToHeap();
        }

        m_battlefield.Clear();
    }

    public void Distribution(Card[] cards)
    {
        foreach (Card card in cards)
        {
            GameCard gameCard = m_cardController.SpawnCard(card, m_trump.Suit);
            m_playerHands.TakeCard(gameCard);
        }
    }

    public void PlayersTurn(uint uid)
    {
        GameCard[] cards = m_battlefield.GetAllCards();

        foreach (GameCard card in cards)
        {
            card.ToHeap();
        }

        m_battlefield.Clear();

        Player player = this.GetPlayer(uid);
        player.StartTimer(m_socketNetwork.EmitWhatsup);

        m_turn.StopTimer();
        m_turn = player;
    }
}
