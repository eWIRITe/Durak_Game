using JSON_card;
using JSON_client;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class alone_Game_BOT : MonoBehaviour
{
    public Card _trump;

    public Room B_room;
    public RoomRow B_roomRow;
    public Table B_table;

    public List<Card> B_room_Deck =  new List<Card>();
    public List<Card> B_room_FoldDeck = new List<Card>();
    public List<Card> B_room_BattleField = new List<Card>();

    public List<Player> _players = new List<Player>();

    static string[] suits = { "♥", "♦", "♣", "♠" };
    static string[] nominals = { "2 ", "3 ", "4 ", "5 ", "6 ", "7 ", "8 ", "9 ", "10", "В ", "Д ", "К ", "Т " };

    public void Init(Room Room, RoomRow RoomRow, Table table)
    {
        B_room = Room;
        B_roomRow = RoomRow; 
        B_table = table;

        Table.beatCard_event += handleTurn;
        Table.throwCard_event += handleTurn;

        Room.foldEvent += FoldHandler;
        Room.grabEvent += GrabHandler;
        Room.passEvent += PassHandler;

        Room.grabbing += (() => setAllDefaultStatus(LastMove.grabbing));
        Table.folding += (() => setAllDefaultStatus());

        init_Deck();

        _trump = B_room_Deck[B_room_Deck.Count - 1];

        for (int i = 1; i < B_roomRow.maxPlayers_number; i++)
        {
            Debug.Log("new player: " + i.ToString());

            User _user = B_room.NewPlayerJoin();

            Player _player = new Player();

            _player.user = _user;

            _players.Add(_player);
        }

        giveCards();

        _players[0].user.role = ERole.main;

        give_roles();

        handleTurn();
    }

    private void OnDestroy()
    {
        Table.beatCard_event -= handleTurn;
        Table.throwCard_event -= handleTurn;

        Room.foldEvent -= FoldHandler;
        Room.grabEvent -= GrabHandler;
        Room.passEvent -= PassHandler;

        Room.grabbing -= (() => setAllDefaultStatus());
        Table.folding -= (() => setAllDefaultStatus());
    }

    public void handleTurn()
    {
        StartCoroutine(HT());
    }
    public IEnumerator HT()
    {
        yield return new WaitForSeconds(1);
        
        Debug.Log(_players.Count);
        for (int i = 0; i < _players.Count; i++)
        {
            Debug.Log("handle turn: " + i.ToString());
            handlBot(B_room._cardController, B_table, _players[i]);
        }
    }

    public void init_Deck()
    {
        int maxCards_num = 0;

        switch (B_roomRow.maxCards_number)
        {
            case (24):
                maxCards_num = 8;
                break;
            case (36):
                maxCards_num = 5;
                break;
            case (52):
                maxCards_num = 0;
                break;
        }

        for (int i = nominals.Length - 1; i >= 0; i--)
        {
            if(maxCards_num <= i)
            {
                for (int j = 0; j < suits.Length; j++)
                {
                    Card card = new Card { suit = suits[j], nominal = nominals[i] };

                    B_room_Deck.Add(card);
                }
            }
        }

        Shuffle(B_room_Deck);
    }

    public void giveCards()
    {
        if(B_room_Deck.Count == 0) 
        {
            B_room.OnTrumpIsDone();
            return;
        }

        else if (B_room_Deck.Count == 1) 
        {
            int minCards = Math.Min(B_room_Deck.Count, 6 - B_room._cardController.PlayerCards.Count);

            for (int i = 0; i < minCards; i++)
            {
                B_room._cardController.GetCard(B_room_Deck[0]);
                B_room_Deck.RemoveAt(0);
            }
            B_room.OnColodaEmpty(); 
        }

        else
        {
            int min_cards = Math.Min(B_room_Deck.Count, 6 - B_room._cardController.PlayerCards.Count);

            for (int i = 0; i < min_cards; i++)
            {
                B_room._cardController.GetCard(B_room_Deck[0]);
                B_room_Deck.RemoveAt(0);
            }
        }
              
        for(int i = 0; i < _players.Count; i++)
        {
            distribCards(_players[i]);
        }
    }

    public void give_roles(LastMove lastMove = LastMove.folding)
    {
        List<ERole> roles = new List<ERole>();

        for (int i = 0; i < _players.Count; i++)
        {
            roles.Add(_players[i].user.role);
        }
        roles.Add(Session.role);

        for (int i = 0; i < roles.Count; i++)
        {
            if (roles[i] == ERole.main)
            {
                if(lastMove == LastMove.grabbing)
                {
                    roles[i] = ERole.main;
                    roles[i + 1 % roles.Count] = ERole.firstThrower;
                }
                else if(lastMove == LastMove.folding)
                {
                    roles[i] = ERole.firstThrower;
                    roles[(i + 1) % roles.Count] = ERole.main;
                }
            }
        }

        for (int i = 0; i < roles.Count; i++)
        {
            if(i == roles.Count - 1)
            {
                Session.role = roles[i];    
            }
            else
            {
                _players[i].user.role = roles[i];
            }
        }
    }

    public void distribCards(Player _player)
    {
        switch (B_room_Deck.Count)
        {
            case 0:
                B_room.OnColodaEmpty();
                break;

            case 1:
                int minCards = Math.Min(B_room_Deck.Count, 6 - _player.cards.Count);

                for (int i = 0; i < minCards; i++)
                {
                    B_room._cardController.AtherUserGotCard(_player.user.UserID);

                    _player.cards.Add(B_room_Deck[0]);
                    B_room_Deck.RemoveAt(0);
                }
                B_room.OnTrumpIsDone();
                break;

            default:
                int min_cards = Math.Min(B_room_Deck.Count, 6 - _player.cards.Count);

                for (int i = 0; i < min_cards; i++)
                {
                    B_room._cardController.AtherUserGotCard(_player.user.UserID);

                    _player.cards.Add(B_room_Deck[0]);
                    B_room_Deck.RemoveAt(0);
                }
                break;
        }
    }

    #region help functions
    public EStatus getMain_stat()
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if(_players[i].user.role == ERole.main)
            {
                return _players[i].user.status;
            }
        }
        return EStatus.Null;
    }

    public void setAllDefaultStatus(LastMove lastMove = LastMove.folding)
    {
        Debug.Log("set all default");

        for(int i = 0; i < _players.Count; i++)
        {
            if(i == 0)
            {
                B_roomRow.status = EStatus.Null;
            }
            else
            {
                _players[i].user.status = EStatus.Null;
            }
        }
        give_roles(lastMove);
        giveCards();
    }

    public static void Shuffle(List<Card> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }

    public static int toCompare(string needSymbol)
    {
        for (int i = 0; i < suits.Length; i++) 
        {
            if (suits[i] == needSymbol)
            {
                return i;
            }
        }
        for (int i = 0; i < nominals.Length; i++)
        {
            if (nominals[i] == needSymbol)
            {
                return i;
            }
        }
        return 0;
    }
    #endregion

    public void PassHandler()
    {
        if (getMain_stat() == EStatus.Grab)
        {
            B_room._cardController.m_room._roomRow.status = EStatus.Grab;

            for (int i = 0; i < _players.Count; i++)
            {
                if (_players[i].user.role != ERole.main)
                {
                    if (_players[i].user.status != EStatus.Pass) return;
                }
            }

            Debug.Log("grabCards");
            B_room._cardController.m_room.GrabCards();
        }
    }

    public void FoldHandler()
    {
        B_room._cardController.m_room._roomRow.status = EStatus.Fold;

        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].user.role != ERole.main)
            {
                if (_players[i].user.status != EStatus.Fold) return;
            }
        }

        B_table.foldCards();
    }

    public void GrabHandler()
    {

    }

    public void handle_main(Player _player)
    {
        Debug.Log("case ERole.main:");
        for (int i = 0; i < B_table.TableCardPairs.Count; i++)
        {
            if (!B_table.TableCardPairs[i].isFull)
            {
                foreach (Card playerCard in _player.cards)
                {
                    GameCard newGameCard = new GameCard();
                    newGameCard.Init(playerCard);

                    if (B_table.isAbleToBeat(newGameCard, B_table.TableCardPairs[i].FirstCard.GetComponent<GameCard>()))
                    {
                        B_room._cardController.AtherUserDestroyCard(_player.user.UserID);
                        B_table.beatCard(_player.user.UserID, new Card { suit = B_table.TableCardPairs[i].FirstCard.GetComponent<GameCard>().strimg_Suit, nominal =B_table.TableCardPairs[i].FirstCard.GetComponent<GameCard>().str_Nnominal }, playerCard);

                        _player.cards.Remove(playerCard);

                        handleTurn();

                        return;
                    }
                }
            }
        }
        for (int i = 0; i < B_table.TableCardPairs.Count; i++)
        {
            if (!B_table.TableCardPairs[i].isFull)
            {
                _player.user.status = EStatus.Grab;
                B_room._cardController.m_room.cl_Grab();

                handleTurn();
            }
        }
    }

    public void handle_firstThrower(Player _player)
    {
        Debug.Log("case ERole.firstThrower:");
        if (B_table.TableCardPairs.Count == 0)
        {
            Card minCard = _player.cards[0];
            foreach (Card _card in _player.cards)
            {
                if (toCompare(_card.suit) < toCompare(minCard.suit) && toCompare(_card.nominal) < toCompare(minCard.nominal))
                {
                    minCard = _card;
                }
            }
            B_room._cardController.AtherUserDestroyCard(_player.user.UserID);
            B_table.placeCard(_player.user.UserID, minCard);

            _player.cards.Remove(minCard);

            handleTurn();

            return;
        }
        else
        {
            for (int i = 0; i < _player.cards.Count; i++)
            {
                GameCard newCard = new GameCard();
                newCard.Init(_player.cards[i]);

                if (B_table.isAbleToThrow(newCard))
                {
                    B_table.placeCard(_player.user.UserID, _player.cards[i]);
                    B_room._cardController.AtherUserDestroyCard(_player.user.UserID);

                    _player.cards.RemoveAt(i);

                    handleTurn();

                    return;
                }
            }
        }
        if (getMain_stat() == EStatus.Grab)
        {
            _player.user.status = EStatus.Pass;

            for (int i = 0; i < _players.Count; i++)
            {
                if (i == 0)
                {
                    if (Session.role != ERole.main)
                    {
                        if (B_room._cardController.m_room._roomRow.status != EStatus.Pass) return;
                    }
                    continue;
                }
                if (_players[i].user.role != ERole.main)
                {
                    if (_players[i].user.status != EStatus.Pass) return;
                }
            }

            B_room._cardController.m_room.GrabCards();
        }

        else
        {
            _player.user.status = EStatus.Fold;

            for (int i = 0; i < _players.Count; i++)
            {
                if (i == 0)
                {
                    if (Session.role != ERole.main)
                    {
                        if (B_room._cardController.m_room._roomRow.status != EStatus.Fold) return;
                    }
                    continue;
                }
                if (_players[i].user.role != ERole.main)
                {
                    if (_players[i].user.status != EStatus.Fold) return;
                }
            }

            B_table.foldCards();
        }
    }

    public void handle_thrower(Player _player)
    {
        Debug.Log("case ERole.thrower:");
        if (B_table.TableCardPairs.Count > 0)
        {
            for (int i = 0; i < _player.cards.Count; i++)
            {
                GameCard newCard = new GameCard();
                newCard.Init(_player.cards[i]);

                if (B_table.isAbleToThrow(newCard))
                {
                    B_table.placeCard(_player.user.UserID, _player.cards[i]);
                    B_room._cardController.AtherUserDestroyCard(_player.user.UserID);

                    _player.cards.RemoveAt(i);

                    handleTurn();

                    return;
                }
            }
        }

        if (getMain_stat() == EStatus.Grab)
        {
            _player.user.status = EStatus.Pass;

            for (int i = 0; i < _players.Count; i++)
            {
                if (i == 0)
                {
                    if (Session.role != ERole.main)
                    {
                        if (B_room._cardController.m_room._roomRow.status != EStatus.Pass) return;
                    }
                    continue;
                }
                if (_players[i].user.role != ERole.main)
                {
                    if (_players[i].user.status != EStatus.Pass) return;
                }
            }

            B_room._cardController.m_room.GrabCards();
        }
        else
        {
            _player.user.status = EStatus.Fold;

            for (int i = 0; i < _players.Count; i++)
            {
                if (i == 0)
                {
                    if (Session.role != ERole.main)
                    {
                        if (B_room._cardController.m_room._roomRow.status != EStatus.Fold) return;
                    }
                    continue;
                }
                if (_players[i].user.role != ERole.main)
                {
                    if (_players[i].user.status != EStatus.Fold) return;
                }
            }

            B_table.foldCards();
        }

        handleTurn();
    }

    public void handlBot(CardController _cardController, Table _table, Player _player)
    {
        if (_player.cards.Count <= 0)
        {
            Debug.Log("playerIsWon");
            return;
        }

        switch (_player.user.role)
        {
            case ERole.main:
                handle_main(_player);

                break;

            case ERole.firstThrower:
                handle_firstThrower(_player);

                break;

            case ERole.thrower:
                handle_thrower(_player);

                break;

            default:
                break;
        }
    }
}

public class Player
{
    public User user;
    public List<Card> cards = new List<Card>();
}