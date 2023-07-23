using JSON_card;
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

    public delegate void startBots();
    public static event startBots _startBots;

    public void Init(Room Room, RoomRow RoomRow, Table table)
    {
        B_room = Room;
        B_roomRow = RoomRow; 
        B_table = table;

        init_Deck();

        _trump = B_room_Deck[B_room_Deck.Count-1];

        Player main_player = new Player();

        main_player.user = new User();

        _players.Add(main_player);

        for (int i = 1; i < B_roomRow.maxPlayers_number; i++)
        {
            Debug.Log("new player: " + i.ToString());

            User _user = B_room.NewPlayerJoin((uint)i);

            Player _player = new Player();

            _player.user = _user;

            _players.Add(_player);
        }

        giveCards();

        _players[0].user.role = ERole.main;

        give_roles();

        _startBots?.Invoke();

        handleTurn();
    }

    public void handleTurn()
    {
        for(int i = 1; i < _players.Count; i++)
        {
            StartCoroutine(alone_User_BOT.HandleTurn(this, B_room._cardController, B_table, _players[i]));
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
        int minCards = Math.Min(B_room_Deck.Count, 6 - B_room._cardController.PlayerCards.Count);

        for (int i = 0; i < minCards; i++)
        {
            B_room._cardController.GetCard(B_room_Deck[0]);
            B_room_Deck.RemoveAt(0);
        }

        for(int i = 1; i < _players.Count; i++)
        {
            distribCards(_players[i]);
        }
    }

    public void give_roles()
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].user.role == ERole.main)
            {
                setAllThrowers();

                _players[getNormIndex(_players.Count, i + 1)].user.role = ERole.main;
                _players[getNormIndex(_players.Count, i + 2)].user.role = ERole.firstThrower;

                Session.role = _players[0].user.role;

                return;
            }
        }
    }

    public void distribCards(Player _player)
    {
        int minCards = Math.Min(B_room_Deck.Count, 6 - _player.cards.Count);

        for (int i = 0; i < minCards; i++)
        {
            B_room._cardController.AtherUserGotCard(_player.user.UserID);

            _player.cards.Add(B_room_Deck[0]);
            Debug.Log("bot, " + _player.user.UserID + ", got card: " + B_room_Deck[0].nominal.ToString() + B_room_Deck[0].suit.ToString());
            B_room_Deck.RemoveAt(0);
        }

        Debug.Log("minCards, for bot, " + _player.user.UserID + ": " + minCards.ToString());
    }

    #region help functions
    public EStatus getMain_stat()
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if(_players[i].user.role == ERole.main)
            {
                if(i == 0)
                {
                    return B_roomRow.status;
                }
                else
                {
                    return _players[i].user.status;
                }
            }
        }
        return EStatus.Null;
    }

    private void setAllThrowers()
    {
        Session.role = ERole.thrower;

        foreach (Player _player in _players)
        {
            _player.user.role = ERole.thrower;
        }
    }

    public void setAllDefaultStatus()
    {
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

    public static int getNormIndex(int count, int index)
    {
        return index % count;
    }

    public static int toCompare(string needSymbol)
    {
        for (int i = 0; i < suits.Length; i++) 
        {
            if(suits[i] == needSymbol)
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
}

public class Player
{
    public User user;
    public List<Card> cards = new List<Card>();
}