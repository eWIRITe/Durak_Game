using System.Collections.Generic;
using UnityEngine;

public class PlayerHands : MonoBehaviour
{
    public CardController m_cardController;

    public Room m_room;

    private List<GameCard> m_inHands = new List<GameCard>();
    private GameCard m_candidate;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (m_candidate)
            {
                m_inHands.Remove(m_candidate);
            }

            this.Sort();
        }
    }

    public void Clear()
    {
        foreach (GameCard card in m_inHands)
        {
            card.ToHeap();
            GameObject.Destroy(card.gameObject);
        }

        m_inHands.Clear();
    }

    public void TakeCard(GameCard card)
    {
        m_inHands.Add(card);
        this.Sort();
    }

    //public void Distribution(Card card)
    //{
    //    m_cardController.SpawnCard(card, m_room.Trump.Suit);
    //}

    private void Sort()
    {
        string SortType = PlayerPrefs.GetString("SortType");
        string trumpSort = PlayerPrefs.GetString("trumpSortType");

        if (SortType == "Suit")
        {
            m_inHands.Sort(new Suit());
        }
        else
        {
            m_inHands.Sort(new Number());
        }


        m_inHands.Sort((x, y) =>
        {
            if (x.Card.Suit != y.Card.Suit)
            {
                if (trumpSort == "toLeft")
                {
                    if (x.Card.Suit == m_room._roomRow.Trump)
                        return -1;
                    if (y.Card.Suit == m_room._roomRow.Trump)
                        return 1;
                }
                else
                {
                    if (x.Card.Suit == m_room._roomRow.Trump)
                        return 1;
                    if (y.Card.Suit == m_room._roomRow.Trump)
                        return -1;
                }
            }

            return 0;
        });

    }

    private void OnCollisionExit2D(Collision2D card)
    {
        m_candidate = card.gameObject.GetComponent<GameCard>();
    }
}

public class Suit : IComparer<GameCard>
{
    public int Compare(GameCard x, GameCard y)
    {
        if (x.Card.Suit < y.Card.Suit)
        {
            return -1;
        }
        else if (x.Card.Suit > y.Card.Suit)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}
public class Number : IComparer<GameCard>
{
    public int Compare(GameCard x, GameCard y)
    {
        if (x.Card.Nominal < y.Card.Nominal)
        {
            return -1;
        }
        else if (x.Card.Nominal > y.Card.Nominal)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}