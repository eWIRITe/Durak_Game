using UnityEngine;

public class GameCard : MonoBehaviour
{
    private CardController m_cardController;

    private Card m_card;

    public Card Card
    {
        get { return m_card; }
    }

    public SpriteRenderer m_front;
    public SpriteRenderer m_back;

    private Vector3 m_clickOffset;

    private bool isFlipped = false;

    private bool m_isInHands = false;

    public bool IsInHands
    {
        get { return m_isInHands; }
        set { m_isInHands = value; }
    }

    private bool m_isActive = true;

    private bool m_isTrump;

    public bool IsTrump
    {
        get { return m_isTrump; }
    }

    public void ToHeap()
    {
        transform.position = m_cardController.HeapPos + (Vector2.one * Random.Range(-10f, 10f));
        transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360f));
    }

    public void ChangeSprite(Sprite front, Sprite back)
    {
        m_front.sprite = front;
        m_back.sprite = back;
    }

    public void Init(Card card, bool isTrump)
    {
        m_card = card;
        m_isTrump = isTrump;
    }

    private void OnMouseDown()
    {
        if (!m_isActive)
        {
            return;
        }

        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        m_clickOffset = transform.position - mouse;
        GetComponent<BoxCollider2D>().isTrigger = false;
    }

    private void OnMouseUp()
    {
        if (!m_isActive)
        {
            return;
        }

        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnMouseDrag()
    {
        if (!m_isActive)
        {
            return;
        }

        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mouse.x + m_clickOffset.x, mouse.y + m_clickOffset.y, transform.position.z);
    }

    public void Flip()
    {
        if (!m_isActive)
        {
            return;
        }

        if (isFlipped)
        {
            m_back.sortingOrder -= m_front.sortingOrder + 1;
        }
        else
        {
            m_back.sortingOrder += m_front.sortingOrder + 1;
        }
    }

    public void Inactive()
    {
        m_isActive = false;
    }
}