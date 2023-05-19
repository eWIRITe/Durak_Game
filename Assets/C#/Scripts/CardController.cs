using System.Collections.Generic;
using UnityEngine;

public class CardController: MonoBehaviour
{
    // heap which contain played cards
    public Transform m_heapPosition;

    public Vector2 HeapPos
    {
        get { return m_heapPosition.position; }
    }

    public Sprite[] m_base;
    public Sprite[] m_russian;
    public Sprite[] m_erotic;
    public Sprite[] m_forest;
    public Sprite[] m_sea;
    public Sprite[] m_scary;
    public Sprite[] m_heroes;
    public Sprite[] m_fallout;
    public Sprite[] m_cars;

    public GameCard m_prefabCard;

    // storage card-gameobjects
    public Transform m_table;

    private void Start()
    {
        this.SpawnCard(new Card(ESuit.Clovers, ENominal.Four), ESuit.Clovers);
        //this.SpawnCard(new Card(ESuit.Pikes, ENominal.Ace));
        //this.SpawnCard(new Card(ESuit.Pikes, ENominal.Ten));
        Session.Style = EStyle.Russian;
        this.ChangeStyle();
    }

    private void GetSprite(Card card, out Sprite front, out Sprite back)
    {
        switch (Session.Style)
        {
            case EStyle.Base:
                front = m_base[card.Byte];
                back = m_base[0x34];
                break;
            case EStyle.Russian:
                front = m_russian[card.Byte];
                back = m_russian[0x34];
                break;
            case EStyle.Erotic:
                front = m_erotic[card.Byte];
                back = m_erotic[0x34];
                break;
            case EStyle.Forest:
                front = m_forest[card.Byte];
                back = m_forest[0x34];
                break;
            case EStyle.Sea:
                front = m_sea[card.Byte];
                back = m_sea[0x34];
                break;
            case EStyle.Scary:
                front = m_scary[card.Byte];
                back = m_scary[0x34];
                break;
            case EStyle.Heroes:
                front = m_heroes[card.Byte];
                back = m_heroes[0x34];
                break;
            case EStyle.Fallout:
                front = m_fallout[card.Byte];
                back = m_fallout[0x34];
                break;
            case EStyle.Cars:
                front = m_cars[card.Byte];
                back = m_cars[0x34];
                break;
            default:
                front = m_base[card.Byte];
                back = m_base[0x34];
                break;
        }
    }

    public void ChangeStyle()
    {
        GameObject[] cards = GameObject.FindGameObjectsWithTag("Card");

        foreach (GameObject card in cards)
        {
            GameCard gameCard = card.GetComponent<GameCard>();
            this.GetSprite(gameCard.Card, out Sprite front, out Sprite back);
            gameCard.ChangeSprite(front, back);
        }
    }

    public GameCard SpawnCard(Card card, ESuit trump)
    {
        GameCard gameCard = GameObject.Instantiate<GameCard>(m_prefabCard, m_table);
        this.GetSprite(card, out Sprite front, out Sprite back);
        gameCard.Init(card, card.Suit == trump);
        gameCard.ChangeSprite(front, back);
        return gameCard;
    }
}