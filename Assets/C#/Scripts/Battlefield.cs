using System.Collections.Generic;
using UnityEngine;

public class Battlefield : MonoBehaviour
{
    public Field[] m_fields;

    public Room m_room;

    private uint m_usedFields = 0;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public GameCard[] GetAllCards()
    {
        List<GameCard> cards = new List<GameCard>();

        foreach (Field field in m_fields)
        {
            cards.Add(field.Top);
            cards.Add(field.Bottom);
        }

        cards.RemoveAll(item => item == null);
        return cards.ToArray();
    }

    public void Attack(GameCard card)
    {
        Field field = m_fields[m_usedFields];
        field.AddCard(card);
        this.NextGhost();
    }

    public void Beat(Card attacked, GameCard attacking)
    {
        foreach (Field field in m_fields)
        {
            if (field.Bottom.Card.Byte == attacked.Byte)
            {
                field.AddCard(attacking);
                break;
            }
        }
    }

    public void Clear()
    {
        this.ResetGhost();

        foreach (Field field in m_fields)
        {
            field.Reset();
        }
    }

    public void ResetGhost()
    {
        m_usedFields = 0;

        foreach (Field field in m_fields)
        {
            field.gameObject.SetActive(false);
        }

        m_fields[m_usedFields].gameObject.SetActive(true);
    }

    public void NextGhost()
    {
        m_fields[m_usedFields++].gameObject.SetActive(false);
        m_fields[m_usedFields].gameObject.SetActive(true);
    }
}
