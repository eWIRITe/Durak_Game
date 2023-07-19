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

    public GameObject user_bot_prefab;

    public List<Card> B_room_Deck =  new List<Card>();
    public List<Card> B_room_FoldDeck = new List<Card>();
    public List<Card> B_room_BattleField = new List<Card>();

    public List<Player> _players = new List<Player>();

    string[] suits = { "♥", "♦", "♣", "♠" };
    string[] nominals = { "2 ", "3 ", "4 ", "5 ", "6 ", "7 ", "8 ", "9 ", "10", "В ", "Д ", "К ", "Т " };

    public void Init(Room Room, RoomRow RoomRow)
    {
        B_room = Room;
        B_roomRow = RoomRow;

        init_Deck();

        _trump = B_room_Deck[0];

        Player main_player = new Player();

        main_player._user = B_roomRow.roomPlayers[0];
        main_player.bot_index = 0;

        _players.Add(main_player);

        for (int i = 1; i < B_roomRow.maxPlayers_number; i++)
        {
            Debug.Log("new player: " + i.ToString());

            alone_User_BOT _bot = Instantiate(user_bot_prefab, gameObject.transform).GetComponent<alone_User_BOT>();
            User _user = B_room.NewPlayerJoin();

            Player _player = new Player();

            _player.bot = _bot;
            _player._user = _user;
            _player.bot_index = i;

            _players.Add(_player);
        }

        _players[0]._user.role = ERole.main;

        give_roles();

        Session.role = ERole.main;
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

    }

    public void give_roles()
    {
        for(int i = 0; i < _players.Count; i++)
        {
            Debug.Log(_players[i]._user.role);
            switch (_players[i]._user.role)
            {
                case ERole.main:
                    _players[i]._user.role = ERole.thrower;
                    _players[i + 1]._user.role = ERole.main;
                    _players[i + 2]._user.role = ERole.firstThrower;
                    break;
                default:
                    break;
            }
        }
    }

    public void distribCards(Player _player)
    {
        _player.Cards.Add(B_room_Deck[0]);
        B_room_Deck.RemoveAt(0);

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
}

public class Player
{
    public alone_User_BOT bot;

    public User _user;
    public List<Card> Cards;

    public int bot_index;
}