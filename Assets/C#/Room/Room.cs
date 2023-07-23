using JSON_card;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Table;

public class Room : MonoBehaviour
{
    public Sprite cardBack;

    public float Cooficent;
    public float ScreenWith = 1980;

    public float PlaceMultiplyer;
    public float RotationMultiplyer;

    public GameObject StartScreen;
    public GameObject OwnerStartGameButton;
    public GameObject PlayerCard;

    public CardController _cardController;

    public Table _table;
    public RoomRow _roomRow;

    private SocketNetwork m_socketNetwork;

    [Header("coloda")]
    public Transform Coloda;
    public Transform TrumpCardPos;

    public GameObject card;
    public GameObject ColodaObject;

    [Header("alone game bots")]
    public GameObject alone_Game_BOT;

    private void Start()
    {
        ScreenWith = Screen.width;

        m_socketNetwork = GameObject.FindGameObjectWithTag("SocketNetwork").GetComponent<SocketNetwork>();
        m_socketNetwork.GetAllRoomPlayersID();
        SocketNetwork.ready += OnReady;

        SocketNetwork.cl_grab += cl_Grab;
        SocketNetwork.playerGrab += GrabCards;
        Session.roleChanged += ((ERole role) => { _roomRow.status = EStatus.Null; });

        GameObject.Find("UI").GetComponent<Canvas>().worldCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

        _roomRow = GetComponent<RoomRow>();
    }

    private void OnDestroy()
    {
        SocketNetwork.ready -= OnReady;
        SocketNetwork.cl_grab -= cl_Grab;
        SocketNetwork.playerGrab -= GrabCards;
        Session.roleChanged -= ((ERole role) => { _roomRow.status = EStatus.Null; });

    }

    ///////\\\\\\
    /// ready \\\
    ///////\\\\\\
    public void StartGame()
    {
        m_socketNetwork.EmitReady(_roomRow.RoomID);
    }

    public void startGameAlone()
    {
        _roomRow.isAlone = true;

        alone_Game_BOT game_BOT = Instantiate(alone_Game_BOT, gameObject.transform).GetComponent<alone_Game_BOT>();

        game_BOT.Init(this, _roomRow, _table);

        OnReady(game_BOT._trump);
    }

    public void OnReady(Card trump)
    {
        StartScreen.SetActive(false);

        _roomRow.isGameStarted = true;

        Instantiate(ColodaObject, Coloda.transform);

        GameObject pref_card = Instantiate(card, TrumpCardPos.transform);
        pref_card.transform.localScale = TrumpCardPos.localScale;
        pref_card.transform.SetParent(gameObject.transform);

        GameCard cardData = pref_card.GetComponent<GameCard>();

        cardData.Init(trump);

        switch (cardData.Suit)
        {
            case ESuit.CLOVERS:
                pref_card.GetComponent<SpriteRenderer>().sprite = _cardController.chooseCardNumber(_cardController.cards_texturies_Clubs, cardData.Nominal);
                break;
            case ESuit.TILE:
                pref_card.GetComponent<SpriteRenderer>().sprite = _cardController.chooseCardNumber(_cardController.cards_texturies_Diamonds, cardData.Nominal);
                break;
            case ESuit.PIKES:
                pref_card.GetComponent<SpriteRenderer>().sprite = _cardController.chooseCardNumber(_cardController.cards_texturies_Spades, cardData.Nominal);
                break;
            default:
                pref_card.GetComponent<SpriteRenderer>().sprite = _cardController.chooseCardNumber(_cardController.cards_texturies_Hearts, cardData.Nominal);
                break;
        }

        _roomRow.Trump = cardData.Suit;
    }

    ///////\\\\\\\
    //NewPlayers\\
    ///////\\\\\\\
    public GameObject NewPlayer;
    public Transform NewPlayerSpawnPoint;

    /////////////\\\\\\\\\\\\
    /// players functions \\\
    /////////////\\\\\\\\\\\\
    public User NewPlayerJoin(uint UId = 0)
    {
        User _user = Instantiate(NewPlayer, NewPlayerSpawnPoint.position, NewPlayerSpawnPoint.rotation).GetComponent<User>();

        _user.gameObject.transform.localScale = NewPlayerSpawnPoint.localScale;

        _user.transform.SetParent(GameObject.Find("UI").transform);

        _roomRow.roomPlayers.Add(_user);

        _user.Init(UId);

        SetPositionsForAllUsers(_roomRow.roomPlayers);

        return _user;
    }
    public void DeletePlayer(uint UId)
    {

        for (int i = 1; i < _roomRow.roomPlayers.Count; i++)
        {
            if ((int)_roomRow.roomPlayers[i].UserID == (int)UId)
            {
                Destroy(_roomRow.roomPlayers[i].gameObject);
                _roomRow.roomPlayers.RemoveAt(i);
            }      
        } 

        SetPositionsForAllUsers(_roomRow.roomPlayers);
    }

    public void SetPositionsForAllUserCards()
    {
        for (int j = 1; j < _roomRow.roomPlayers.Count; j++)
        {
            Vector3 playerPos = _roomRow.roomPlayers[j].gameObject.transform.position;
            playerPos.y -= 1;

            for (int i = 0; i < _roomRow.roomPlayers[j].UserCards.Count; i++)
            {
                _roomRow.roomPlayers[j].UserCards[i].transform.SetParent(_roomRow.roomPlayers[j].gameObject.transform);

                _roomRow.roomPlayers[j].UserCards[i].gameObject.GetComponent<SpriteRenderer>().sortingOrder = i;

                Vector3 pos = new Vector3((Screen.height / PlaceMultiplyer) * (i - ((_roomRow.roomPlayers[j].UserCards.Count) / 2)), gameObject.transform.position.y - 1.2f, 0);
                Vector3 rotate = new Vector3(0, 0, 0);

                _roomRow.roomPlayers[j].UserCards[i].transform.localScale = new Vector3(20,20,20);

                StartCoroutine(MoveCard(_roomRow.roomPlayers[j].UserCards[i], pos, rotate));
            }
        }
    }
    public void SetPositionsForAllUsers(List<User> users)
    {
        int i = 1;

        while (i < users.Count)
        {
            float x = (float)((ScreenWith * i / users.Count) - ScreenWith * 0.5);
            float y = (float)(Math.Abs(x) / Cooficent) * -1;

            Debug.Log(y);

            Vector3 coords = new Vector3(x, y, 0);

            StartCoroutine(users[i].MoveTo(coords));

            i++;
        }

        Debug.Log("SetPositionsForAllUsers");
    }

    public void cl_Grab()
    {
        if(Session.role == ERole.firstThrower || Session.role == ERole.thrower)
        {
            _roomRow.GameUI.showPassButton();
        }

        for(int i = 1; i < _roomRow.roomPlayers.Count; i++)
        {
            if(_roomRow.roomPlayers[i].role == ERole.main)
            {
                _roomRow.roomPlayers[i].PrintMessage("i am taking!");
            }
        }
    }

    public void GrabCards()
    {
        if(Session.role == ERole.main)
        {
            foreach(CardPair _card in GetComponent<Table>().TableCardPairs)
            {
                Card first_card = new Card { nominal = _card.FirstCard.GetComponent<GameCard>().str_Nnominal, suit = _card.FirstCard.GetComponent<GameCard>().strimg_Suit };
                _cardController.GetCard(first_card);

                if (_card.isFull)
                {
                    Card second_card = new Card { nominal = _card.SecondCard.GetComponent<GameCard>().str_Nnominal, suit = _card.SecondCard.GetComponent<GameCard>().strimg_Suit };
                    _cardController.GetCard(second_card);
                }
                
                Destroy(_card.FirstCard);
                if (_card.isFull) Destroy(_card.SecondCard);
            }

            GetComponent<Table>().TableCardPairs = new List<CardPair>();

            _cardController.SetAllCardsPos();
        }

        else
        {
            for (int i = 1; i < _roomRow.roomPlayers.Count; i++)
            {
                if(_roomRow.roomPlayers[i].role == ERole.main)
                {
                    foreach(CardPair _card in GetComponent<Table>().TableCardPairs)
                    {
                        _cardController.AtherUserGotCard(_roomRow.roomPlayers[i].UserID);
                        if(_card.isFull) _cardController.AtherUserGotCard(_roomRow.roomPlayers[i].UserID);

                        Destroy(_card.FirstCard);
                        if (_card.isFull) Destroy(_card.SecondCard);
                    }

                    GetComponent<Table>().TableCardPairs = new List<CardPair>();
                }
            }

            GetComponent<Table>().SetAllTableCardsPos();
        }
    }

    public void Fold()
    {
        GetComponent<GameUIs>().hideFoldButton();

        _roomRow.status = EStatus.Fold;

        if (!_roomRow.isAlone) m_socketNetwork.EmitFold();
    }

    public void Pass()
    {
        GetComponent<GameUIs>().hidePassButton();

        _roomRow.status = EStatus.Pass;

        if (!_roomRow.isAlone)  m_socketNetwork.EmitPass();
    }

    public void Grab()
    {
        GetComponent<GameUIs>().hideGrabButton();

        _roomRow.status = EStatus.Grab;

        if (!_roomRow.isAlone)  m_socketNetwork.EmitGrab();
    }

    //////////////\\\\\\\\\\\\\
    /// lean twin functions \\\
    //////////////\\\\\\\\\\\\\
    public IEnumerator MoveCard(GameObject card, Vector3 newCardPos, Vector3 rotate)
    {
        LeanTween.moveLocal(card, newCardPos, 2);
        LeanTween.rotate(card, rotate, 2);
        yield return null;
    }
}

