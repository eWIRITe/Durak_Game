using System.Collections;
using UnityEngine;

public class GameCard : MonoBehaviour
{
    private byte bytes;
    private string _suit;
    private string _nominal;

    public byte GetBytes
    {
        get { return bytes; }
    }

    ESuit suit;
    public ESuit Suit
    {
        get {
            switch (_suit)
            {
                case "♥":
                    suit = ESuit.HEART;
                    break;
                case "♦":
                    suit = ESuit.TILE;
                    break;
                case "♣":
                    suit = ESuit.CLOVERS;
                    break;
                case "♠":
                    suit = ESuit.PIKES;
                    break;
            }

            return suit; 
        }
    }

    ENominal nominal;
    public ENominal Nominal
    {
        get {
            switch (_nominal)
            {
                case "2 ":
                    nominal = ENominal.TWO;
                    break;
                case "3 ":
                    nominal = ENominal.THREE;
                    break;
                case "4 ":
                    nominal = ENominal.FOUR;
                    break;
                case "5 ":
                    nominal = ENominal.FIVE;
                    break;
                case "6 ":
                    nominal = ENominal.SIX;
                    break;
                case "7 ":
                    nominal = ENominal.SEVEN;
                    break;
                case "8 ":
                    nominal = ENominal.EIGHT;
                    break;
                case "9 ":
                    nominal = ENominal.NINE;
                    break;
                case "10":
                    nominal = ENominal.TEN;
                    break;
                case "В ":
                    nominal = ENominal.JACK;
                    break;
                case "Д ":
                    nominal = ENominal.QUEEN;
                    break;
                case "К ":
                    nominal = ENominal.KING;
                    break;
                case "Т ":
                    nominal = ENominal.COUNT;
                    break;
            }

            return nominal; 
        }
    }

    public void Init(JSON.Card card)
    {
        _suit = card.suit;
        _nominal = card.nominal;
    }

    public IEnumerator MoveTo(Vector3 MoveToPoint, Vector3 rotate)
    {
        yield return moveTo(MoveToPoint, rotate);
    }

    private bool moveTo(Vector3 MoveToPoint, Vector3 rotate)
    {
        LeanTween.moveLocal(gameObject, MoveToPoint, 2);
        LeanTween.rotate(gameObject, rotate, 2);
        LeanTween.scale(gameObject, new Vector3(1.6f, 1.6f, 1.6f), 2);

        return true;
    }
}