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

    }

    private void OnCollisionExit2D(Collision2D card)
    {
        m_candidate = card.gameObject.GetComponent<GameCard>();
    }
}
