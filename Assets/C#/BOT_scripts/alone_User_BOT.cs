using JSON_card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static alone_Game_BOT;

public class alone_User_BOT : MonoBehaviour
{
    public static IEnumerator HandleTurn(alone_Game_BOT alone_Game_BOT, CardController _cardController, Table _table, Player _payer)
    {
        yield return new WaitForSeconds(2);

        handlBot(alone_Game_BOT, _cardController, _table, _payer);
    }

    private static void handlBot(alone_Game_BOT alone_Game_BOT, CardController _cardController, Table _table, Player _payer)
    {
        if(_payer.cards.Count <= 0)
        {
            Debug.Log("playerIsWon");
            return;
        }

        switch (_payer.user.role)
        {
            case ERole.main:
                for (int i = 0; i < _table.TableCardPairs.Count; i++)
                {
                    if (!_table.TableCardPairs[i].isFull)
                    {
                        foreach(Card playerCard in _payer.cards)
                        {
                            GameCard newGameCard = new GameCard();
                            newGameCard.Init(playerCard);

                            if (_table.isAbleToBeat(_table.TableCardPairs[i].FirstCard.GetComponent<GameCard>(), newGameCard))
                            {
                                _cardController.AtherUserDestroyCard(_payer.user.UserID);
                                _table.beatCard(_payer.user.UserID, new Card { suit = _table.TableCardPairs[i].FirstCard.GetComponent<GameCard>().strimg_Suit, nominal = _table.TableCardPairs[i].FirstCard.GetComponent<GameCard>().str_Nnominal }, playerCard);

                                _payer.cards.Remove(playerCard);

                                alone_Game_BOT.handleTurn();

                                return;
                            }
                        }
                    }
                }

                if (_table.TableCardPairs.Count > 0)
                {
                    _payer.user.status = EStatus.Grab;
                    _cardController.m_room.cl_Grab();

                    alone_Game_BOT.handleTurn();
                }

                break;

            case ERole.firstThrower:
                if (_table.TableCardPairs.Count == 0)
                {
                    Card minCard = _payer.cards[0];
                    foreach(Card _card in _payer.cards)
                    {
                        if (toCompare(_card.suit) < toCompare(minCard.suit) && toCompare(_card.nominal) < toCompare(minCard.nominal))
                        {
                            minCard = _card;
                        }
                    }
                    _cardController.AtherUserDestroyCard(_payer.user.UserID);
                    _table.placeCard(_payer.user.UserID, minCard);

                    _payer.cards.Remove(minCard);

                    alone_Game_BOT.handleTurn();

                    return;
                }
                else
                {
                    for (int i = 0; i < _payer.cards.Count; i++)
                    {
                        GameCard newCard = new GameCard();
                        newCard.Init(_payer.cards[i]);

                        if (_table.isAbleToThrow(newCard))
                        {
                            _table.placeCard(_payer.user.UserID, _payer.cards[i]);
                            _cardController.AtherUserDestroyCard(_payer.user.UserID);

                            _payer.cards.RemoveAt(i);

                            alone_Game_BOT.handleTurn();

                            return;
                        }
                    }
                }

                if (alone_Game_BOT.getMain_stat() == EStatus.Grab)
                {
                    _payer.user.status = EStatus.Pass;

                    for(int i = 0; i < alone_Game_BOT._players.Count; i++)
                    {
                        if(i == 0)
                        {
                            if(Session.role != ERole.main)
                            {
                                if (_cardController.m_room._roomRow.status != EStatus.Pass) return;
                            }
                            continue;
                        }
                        if(alone_Game_BOT._players[i].user.role != ERole.main)
                        {
                            if (alone_Game_BOT._players[i].user.status != EStatus.Pass) return;
                        }
                    }

                    _cardController.m_room.GrabCards();
                }
                else
                {
                    _payer.user.status = EStatus.Fold;

                    for (int i = 0; i < alone_Game_BOT._players.Count; i++)
                    {
                        if (i == 0)
                        {
                            if (Session.role != ERole.main)
                            {
                                if (_cardController.m_room._roomRow.status != EStatus.Fold) return;
                            }
                            continue;
                        }
                        if (alone_Game_BOT._players[i].user.role != ERole.main)
                        {
                            if (alone_Game_BOT._players[i].user.status != EStatus.Fold) return;
                        }
                    }

                    _table.foldCards();
                }

                alone_Game_BOT.handleTurn();

                break;

            case ERole.thrower:
                if(_table.TableCardPairs.Count > 0)
                {
                    for (int i = 0; i < _payer.cards.Count; i++)
                    {
                        GameCard newCard = new GameCard();
                        newCard.Init(_payer.cards[i]);

                        if (_table.isAbleToThrow(newCard))
                        {
                            _table.placeCard(_payer.user.UserID, _payer.cards[i]);
                            _cardController.AtherUserDestroyCard(_payer.user.UserID);

                            _payer.cards.RemoveAt(i);

                            alone_Game_BOT.handleTurn();

                            return;
                        }
                    }
                }

                if (alone_Game_BOT.getMain_stat() == EStatus.Grab)
                {
                    _payer.user.status = EStatus.Pass;

                    for (int i = 0; i < alone_Game_BOT._players.Count; i++)
                    {
                        if (i == 0)
                        {
                            if (Session.role != ERole.main)
                            {
                                if (_cardController.m_room._roomRow.status != EStatus.Pass) return;
                            }
                            continue;
                        }
                        if (alone_Game_BOT._players[i].user.role != ERole.main)
                        {
                            if (alone_Game_BOT._players[i].user.status != EStatus.Pass) return;
                        }
                    }

                    _cardController.m_room.GrabCards();
                }
                else
                {
                    _payer.user.status = EStatus.Fold;

                    for (int i = 0; i < alone_Game_BOT._players.Count; i++)
                    {
                        if (i == 0)
                        {
                            if (Session.role != ERole.main)
                            {
                                if (_cardController.m_room._roomRow.status != EStatus.Fold) return;
                            }
                            continue;
                        }
                        if (alone_Game_BOT._players[i].user.role != ERole.main)
                        {
                            if (alone_Game_BOT._players[i].user.status != EStatus.Fold) return;
                        }
                    }

                    _table.foldCards();
                }

                alone_Game_BOT.handleTurn();

                break;

            default:break;
        }
    }
}
