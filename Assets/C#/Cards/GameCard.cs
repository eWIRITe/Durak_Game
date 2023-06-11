using System.Collections;
using UnityEngine;

public class GameCard : MonoBehaviour
{
    public Table _table;

    private string _suit;
    private string _nominal;

    private bool isDragging = false;
    private Vector3 offset;

    public bool isDraggble = true;

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
    public string strimg_Suit
    {
        get
        {
            return _suit;
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
    public string str_Nnominal
    {
        get
        {
            return _nominal;
        }
    }

    public void Start()
    {
        _table = GameObject.FindGameObjectWithTag("Room").GetComponent<Table>();
    }

    public void Init(JSON.Card card)
    {
        _suit = card.suit;
        _nominal = card.nominal;
    }


    private void OnMouseDown()
    {
        if (isDraggble)
        {
            offset = transform.position - GetMouseWorldPosition();
            isDragging = true;
        }
    }

    private void OnMouseDrag()
    {
        if (isDraggble)
        {
            if (isDragging)
            {
                transform.position = GetMouseWorldPosition() + offset;
            }
        }
    }

    private void OnMouseUp()
    {
        if (isDraggble)
        {
            isDragging = false;

            if (gameObject.transform.position.y >= 1)
            {
                if(Session.role == ERole.main)
                {
                    _table.BeatCard(this);

                    Debug.Log("_table.BeatCard(this);");
                }
                else
                {
                    _table.ThrowCard(this);

                    Debug.Log("_table.ThrowCard(this);");
                }
            }

            GameObject.Find("Hands").GetComponent<CardController>().SetAllCardsPos();
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }
    public IEnumerator MoveTo(Vector3 MoveToPoint, Vector3 rotate, Vector3 scale, float Time = 1)
    {
        yield return moveTo(MoveToPoint, rotate, scale, Time);
    }
    private bool moveTo(Vector3 MoveToPoint, Vector3 rotate, Vector3 scale, float Time)
    {
        LeanTween.moveLocal(gameObject, MoveToPoint, Time);
        LeanTween.rotate(gameObject, rotate, Time);
        LeanTween.scale(gameObject, scale, Time);

        return true;
    }

}