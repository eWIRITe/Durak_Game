using JSON_card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Table;

// table 
// responseble for folding, beatind/throwing cards on the table

public class Table : BaseScreen
{
    public CardController _cardController;
    public GameUIs _gameUI;
    public RoomRow _roomRow;
    public Room _room;

    public Transform FoldPlace;

    public List<CardPair> TableCardPairs = new List<CardPair>();

    private void Start()
    {
        _cardController = GameObject.FindGameObjectWithTag("Hands").GetComponent<CardController>();
        _gameUI = GetComponent<GameUIs>();
        _roomRow = GetComponent<RoomRow>();
        _room = GetComponent<Room>();

        SocketNetwork.placeCard += placeCard;
        SocketNetwork.beatCard += beatCard;
        SocketNetwork.FoldAllCards += foldCards;
    }

    // server BEAT/THROW pre-emit functions
    public void ThrowCard(GameCard card)
    {
        if (Session.role != ERole.main)
        {
            if (_roomRow.status == EStatus.Fold || _roomRow.status == EStatus.Pass)
            {
                return;
            }
            if (isAbleToThrow(card))
            {
                if (!_roomRow.isAlone) m_socketNetwork.EmitThrow(new Card { suit = card.strimg_Suit, nominal = card.str_Nnominal });
                else
                {
                    placeCard(Session.UId, new Card { suit = card.strimg_Suit, nominal = card.str_Nnominal });
                    _cardController.DestroyCard(new Card { suit = card.strimg_Suit, nominal = card.str_Nnominal });

                    _room.alone_Game_BOT.GetComponent<alone_Game_BOT>().handleTurn();
                }
            }
        }
    }
    public void BeatCard(GameCard card, GameCard beatingCard)
    {
        if (Session.role == ERole.main)
        {
            if (_roomRow.status == EStatus.Grab)
            {
                return;
            }
            if (isAbleToBeat(card, beatingCard))
            {
                if (!_roomRow.isAlone) m_socketNetwork.EmitBeat(new Card { suit = beatingCard.strimg_Suit, nominal = beatingCard.str_Nnominal }, new Card { suit = card.strimg_Suit, nominal = card.str_Nnominal });
                else
                {
                    beatCard(Session.UId, new Card { suit = beatingCard.strimg_Suit, nominal = beatingCard.str_Nnominal }, new Card { suit = card.strimg_Suit, nominal = card.str_Nnominal });
                    _cardController.DestroyCard(new Card { suit = beatingCard.strimg_Suit, nominal = beatingCard.str_Nnominal });

                    _room.alone_Game_BOT.GetComponent<alone_Game_BOT>().handleTurn();
                }
            }
        }
    }

    //client BEAT/THROW handle functions
    public void placeCard(uint UserID, Card card)
    {
        if (Session.role == ERole.main)
        {
            if(TableCardPairs.Count == 0)
            {
                _gameUI.showGrabButton();
            }
        }

        GameObject pref_card = Instantiate(_cardController.m_prefabCard);
        pref_card.transform.localScale = _cardController.StartOfCards.localScale;
        pref_card.transform.SetParent(gameObject.transform);
        pref_card.tag = "tableBeatingCard";

        GameCard cardData = pref_card.GetComponent<GameCard>();

        cardData.Init(card);
        cardData.isDraggble = false;

        switch (cardData.Suit)
        {
            case ESuit.CLOVERS:
                pref_card.GetComponent<SpriteRenderer>().sprite = _cardController.chooseCardNumber(_cardController.BaseCardsClubsTexturies, cardData.Nominal);
                break;
            case ESuit.TILE:
                pref_card.GetComponent<SpriteRenderer>().sprite = _cardController.chooseCardNumber(_cardController.BaseCardsDiamondsTexturies, cardData.Nominal);
                break;
            case ESuit.PIKES:
                pref_card.GetComponent<SpriteRenderer>().sprite = _cardController.chooseCardNumber(_cardController.BaseCardsSpadesTexturies, cardData.Nominal);
                break;
            default:
                pref_card.GetComponent<SpriteRenderer>().sprite = _cardController.chooseCardNumber(_cardController.BaseCardsHeartsTexturies, cardData.Nominal);
                break;
        }

        CardPair cardPair = new CardPair();
        cardPair.FirstCard = pref_card;

        TableCardPairs.Add(cardPair);

        SortCardPairs();
        SetAllTableCardsPos();

        if (Session.role == ERole.main)
        {
            _gameUI.showGrabButton();
            return;
        }
    }
    public void beatCard(uint UserID, Card beatCard, Card beatingCard)
    {
        GameObject pref_card = Instantiate(_cardController.m_prefabCard);
        pref_card.transform.localScale = _cardController.StartOfCards.localScale;
        pref_card.transform.SetParent(gameObject.transform);
        pref_card.tag = "tableNotBeatingCard";

        GameCard cardData = pref_card.GetComponent<GameCard>();

        cardData.Init(beatingCard);
        cardData.isDraggble = false;

        switch (cardData.Suit)
        {
            case ESuit.CLOVERS:
                pref_card.GetComponent<SpriteRenderer>().sprite = _cardController.chooseCardNumber(_cardController.BaseCardsClubsTexturies, cardData.Nominal);
                break;
            case ESuit.TILE:
                pref_card.GetComponent<SpriteRenderer>().sprite = _cardController.chooseCardNumber(_cardController.BaseCardsDiamondsTexturies, cardData.Nominal);
                break;
            case ESuit.PIKES:
                pref_card.GetComponent<SpriteRenderer>().sprite = _cardController.chooseCardNumber(_cardController.BaseCardsSpadesTexturies, cardData.Nominal);
                break;
            default:
                pref_card.GetComponent<SpriteRenderer>().sprite = _cardController.chooseCardNumber(_cardController.BaseCardsHeartsTexturies, cardData.Nominal);
                break;
        }

        for (int i = 0; i < TableCardPairs.Count; i++)
        {
            if(TableCardPairs[i].FirstCard.GetComponent<GameCard>().strimg_Suit == beatCard.suit && TableCardPairs[i].FirstCard.GetComponent<GameCard>().str_Nnominal == beatCard.nominal)
            {
                TableCardPairs[i].FirstCard.tag = "tableNotBeatingCard";
                TableCardPairs[i].SecondCard = pref_card;
                TableCardPairs[i].isFull = true;
            }
        }

        SortCardPairs();
        SetAllTableCardsPos();

        if (Session.role == ERole.main)
        {
            if (TableCardPairs.Count > 0)
            {
                foreach (CardPair pair in TableCardPairs)
                {
                    if (!pair.isFull)
                    {
                        _gameUI.showGrabButton();
                        return;
                    }
                }
                _gameUI.hideGrabButton();
            }
        }

        if (Session.role == ERole.firstThrower || Session.role == ERole.thrower)
        {
            foreach (CardPair cardPair in TableCardPairs)
            {
                if (!cardPair.isFull)
                {
                    _gameUI.hideFoldButton();
                    return;
                }
            }
            _gameUI.showFoldButton();
        }
    }

    public void foldCards()
    {
        for (int i = 0; i < TableCardPairs.Count; i++)
        {
            TableCardPairs[i].FirstCard.tag = "tableNotBeatingCard";
            if (TableCardPairs[i].SecondCard != null) TableCardPairs[i].SecondCard.tag = "tableNotBeatingCard";

            StartCoroutine(TableCardPairs[i].FirstCard.GetComponent<GameCard>().MoveTo(new Vector3(FoldPlace.position.x, (float)((float)(FoldPlace.position.y) - (float)(i / 10)), FoldPlace.position.z), new Vector3(0, 0, 0), TableCardPairs[i].FirstCard.transform.localScale, 1));
            if (TableCardPairs[i].SecondCard != null) StartCoroutine(TableCardPairs[i].SecondCard.GetComponent<GameCard>().MoveTo(new Vector3(FoldPlace.position.x, (float)((float)(FoldPlace.position.y) - (float)(i / 15)), FoldPlace.position.z), new Vector3(0, 0, 0), TableCardPairs[i].SecondCard.transform.localScale, 1));
        }
        TableCardPairs = new List<CardPair>();
    }

    #region checkers
    public bool isAbleToBeat(GameCard beatCard, GameCard beatingCard)
    {
        if (((int)beatCard.Nominal) > ((int)beatingCard.Nominal))
        {
            if (beatingCard.Suit != _roomRow.Trump)
            {
                return true;
            }
            else
            {
                if (beatCard.Suit == _roomRow.Trump)
                {
                    return true;
                }
            }
        }
        else
        {
            if (beatCard.Suit == _roomRow.Trump)
            {
                if (beatingCard.Suit != _roomRow.Trump)
                {
                    return true;
                }
            }
        }

        Debug.Log("return false;");

        return false;
    }
    public bool isAbleToThrow(GameCard card)
    {
        if (TableCardPairs.Count == 0)
        {
            if (Session.role == ERole.firstThrower)
            {
                return true;
            }
        }
        else
        {
            if (Session.role != ERole.main)
            {
                if (isRightCard(card)) return true;
            }
        }

        return false;
    }

    public bool isRightCard(GameCard card)
    {
        foreach (CardPair cardPair in TableCardPairs)
        {
            if (cardPair.FirstCard.GetComponent<GameCard>().str_Nnominal == card.str_Nnominal) return true;

            else if (cardPair.SecondCard != null)
            {
                if (cardPair.SecondCard.GetComponent<GameCard>().str_Nnominal == card.str_Nnominal) return true;
            }
        }
        return false;
    }
    #endregion

    #region
    Vector3 newPos;
    Vector3 newRotate = new Vector3(0, 0, 0);
    public void SetAllTableCardsPos()
    {
        for(int i = 0; i < TableCardPairs.Count; i++)
        {
            switch (i)
            {
                case 0:
                    newPos.x = -3;
                    newPos.y = -0.4f;
                    break;
                case 1:
                    newPos.x = 0;
                    newPos.y = -0.4f;
                    break;
                case 2:
                    newPos.x = 3;
                    newPos.y = -0.4f;
                    break;
                case 3:
                    newPos.x = -3;
                    newPos.y = -1.6f;
                    break;
                case 4:
                    newPos.x = 0;
                    newPos.y = -1.6f;
                    break;
                case 5:
                    newPos.x = 3;
                    newPos.y = -1.6f;
                    break;

                default:
                    newPos.x = 5;

                    newPos.y = (i-5)/ 2;
                    break;
            }

            StartCoroutine(TableCardPairs[i].FirstCard.GetComponent<GameCard>().MoveTo(newPos, newRotate, new Vector3(0.7f, 0.7f, 0.7f), 0.5f));
            newPos.y -= 0.5f;
            if (TableCardPairs[i].SecondCard != null) StartCoroutine(TableCardPairs[i].SecondCard.GetComponent<GameCard>().MoveTo(newPos, newRotate, new Vector3(0.7f, 0.7f, 0.7f), 0.5f));
        }
    }

    void SortCardPairs()
    {
        TableCardPairs.Sort(new CardPairComparer());
    }

    public class CardPair 
    {
        public GameObject FirstCard;
        public GameObject SecondCard;

        public bool isFull = false;

    }
    #endregion
}
public class CardPairComparer : IComparer<CardPair>
{
    public int Compare(CardPair x, CardPair y)
    {
        if (x.isFull && !y.isFull)
        {
            return -1; // x is considered smaller, so it will appear before y
        }
        else if (!x.isFull && y.isFull)
        {
            return 1; // x is considered larger, so it will appear after y
        }
        else
        {
            return 0; // x and y have the same isFull value, so their order remains unchanged
        }
    }
}
