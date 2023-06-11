using JSON;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardController: MonoBehaviour
{
    public int RotationMultiplyer;
    public int PlaceMultiplyer;

    public Room m_room;

    public GameObject m_prefabCard;
    public GameObject m_prefabBackCard;

    public List<Sprite> BaseCardsHeartsTexturies;
    public List<Sprite> BaseCardsDiamondsTexturies;
    public List<Sprite> BaseCardsClubsTexturies;
    public List<Sprite> BaseCardsSpadesTexturies;

    // storage card-gameobjects
    public Transform StartOfCards;

    public List<GameObject> PlayerCards = new List<GameObject>();

    private void Start()
    {
        m_room = GameObject.Find("Room(Clone)").GetComponent<Room>();

        SocketNetwork.GetCard += GetCard;
        SocketNetwork.DestroyCard += DestroyCard;
        SocketNetwork.userGotCard += AtherUserGotCard;
        SocketNetwork.userDestroyCard += AtherUserDestroyCard;
    }

    /////////////\\\\\\\\\\\\\
    // main cards functions \\
    /////////////\\\\\\\\\\\\\
    public void GetCard(Card cardbytes)
    {
        GameObject pref_card = Instantiate(m_prefabCard, StartOfCards.position, StartOfCards.rotation);
        pref_card.transform.localScale = StartOfCards.localScale;
        pref_card.transform.SetParent(gameObject.transform);

        GameCard cardData = pref_card.GetComponent<GameCard>();

        cardData.Init(cardbytes);

        switch (cardData.Suit)
        {
            case ESuit.CLOVERS:
                pref_card.GetComponent<SpriteRenderer>().sprite = chooseCardNumber(BaseCardsClubsTexturies, cardData.Nominal);
                break;
            case ESuit.TILE:
                pref_card.GetComponent<SpriteRenderer>().sprite = chooseCardNumber(BaseCardsDiamondsTexturies, cardData.Nominal);
                break;
            case ESuit.PIKES:
                pref_card.GetComponent<SpriteRenderer>().sprite = chooseCardNumber(BaseCardsSpadesTexturies, cardData.Nominal);
                break;
            default:
                pref_card.GetComponent<SpriteRenderer>().sprite = chooseCardNumber(BaseCardsHeartsTexturies, cardData.Nominal);
                break;
        }

        PlayerCards.Add(pref_card);

        SetAllCardsPos();
    }
    public void DestroyCard(Card cardbytes)
    {
        foreach(GameObject card in PlayerCards)
        {
            if(card.GetComponent<GameCard>().strimg_Suit == cardbytes.suit)
            {
                if(card.GetComponent<GameCard>().str_Nnominal == cardbytes.nominal)
                {
                    Destroy(card);
                    PlayerCards.Remove(card);

                    SetAllCardsPos();
                    return;
                }
            }
        }
    }

    public void AtherUserGotCard(uint UserID)
    {
        for(int i = 1; i< m_room._roomRow.roomPlayers.Count; i++)
        {
            if (m_room._roomRow.roomPlayers[i].UserID == UserID)
            {
                GameObject card = Instantiate(m_prefabBackCard);
                m_room._roomRow.roomPlayers[i].UserCards.Add(card);
            }
        }

        m_room.SetPositionsForAllUserCards();
    }

    public void AtherUserDestroyCard(uint UserID)
    {
        if (m_room != null && m_room._roomRow != null)
        {
            for (int i = 1; i < m_room._roomRow.roomPlayers.Count; i++)
            {
                if (m_room._roomRow.roomPlayers[i].UserID == UserID)
                {
                    Destroy(m_room._roomRow.roomPlayers[i].UserCards[0]);
                    m_room._roomRow.roomPlayers[i].UserCards.RemoveAt(0);
                }
            }

            m_room.SetPositionsForAllUserCards();
        }
    }

    /////////\\\\\\\\\\
    //helped function\\
    /////////\\\\\\\\\\

    public void SetAllCardsPos()
    {
        Sort();

        for (int i = 0; i < PlayerCards.Count; i++)
        {
            PlayerCards[i].gameObject.GetComponent<SpriteRenderer>().sortingOrder = i;

            Vector3 pos = new Vector3((Screen.height/ PlaceMultiplyer) *(i-((PlayerCards.Count)/2)), gameObject.transform.position.y, 0);
            Vector3 rotate = new Vector3(0, 0, (RotationMultiplyer * (i - ((PlayerCards.Count) / 2))) * -1);

            StartCoroutine(PlayerCards[i].GetComponent<GameCard>().MoveTo(pos, rotate, new Vector3(1.5f, 1.5f, 1.5f)));
        }
    }

    public Sprite chooseCardNumber(List<Sprite> Cards, ENominal nominal)
    {
        switch (nominal)
        {
            case ENominal.TWO:
                return Cards[1];
            case ENominal.THREE:
                return Cards[2];
            case ENominal.FOUR:
                return Cards[3];
            case ENominal.FIVE:
                return Cards[4];
            case ENominal.SIX:
                return Cards[5];
            case ENominal.SEVEN:
                return Cards[6];
            case ENominal.EIGHT:
                return Cards[7];
            case ENominal.NINE:
                return Cards[8];
            case ENominal.TEN:
                return Cards[9];
            case ENominal.JACK:
                return Cards[10];
            case ENominal.QUEEN:
                return Cards[11];
            case ENominal.KING:
                return Cards[12];
            default:
                return Cards[0];
        }
    }

    private void Sort()
    {
        string SortType = PlayerPrefs.GetString("SortType");
        string trumpSort = PlayerPrefs.GetString("trumpSortType");

        if (SortType == "Suit")
        {
            PlayerCards.Sort(new Suit());
        }
        else
        {
            PlayerCards.Sort(new Number());
        }


        PlayerCards.Sort((x, y) =>
        {
            if(x.GetComponent<GameCard>().Suit != y.GetComponent<GameCard>().Suit)
            {
                if (x.GetComponent<GameCard>().Suit == m_room._roomRow.Trump)
                {
                    Debug.Log("trump");
                    if (trumpSort == "toLeft")
                    {
                        Debug.Log("toLeft");
                        return -1;
                    }
                    else if (trumpSort == "toRight")
                    {
                        Debug.Log("toRight");
                        return 1;
                    }
                    else return 0;
                }
                return 0;
            }
            return 0;
        });

    }
}

public class Suit : IComparer<GameObject>
{
    public int Compare(GameObject x, GameObject y)
    {
        if (((int)x.GetComponent<GameCard>().Suit) < ((int)y.GetComponent<GameCard>().Suit))
        {
            return -1;
        }
        else if (((int)x.GetComponent<GameCard>().Suit) > ((int)y.GetComponent<GameCard>().Suit))
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}
public class Number : IComparer<GameObject>
{
    public int Compare(GameObject x, GameObject y)
    {
        if (((int)x.GetComponent<GameCard>().Nominal) < ((int)y.GetComponent<GameCard>().Nominal))
        {
            return -1;
        }
        else if (((int)x.GetComponent<GameCard>().Nominal)> ((int)y.GetComponent<GameCard>().Nominal))
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}