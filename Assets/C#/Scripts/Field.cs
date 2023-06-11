using UnityEngine;

public class Field : MonoBehaviour
{
    private GameCard m_top;

    public GameCard Top
    {
        get { return m_top; }
    }

    private GameCard m_bottom;

    public GameCard Bottom
    {
        get { return m_bottom; }
    }

    private GameCard m_candidate;

    private Battlefield m_battlefield;

    void Start()
    {
        m_battlefield = transform.parent.GetComponent<Battlefield>();
    }

    void Update()
    {
        if (m_bottom && m_top)
        {
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (m_candidate)
            {
                if (m_bottom)
                {
                    m_top = m_candidate;
                    m_candidate.transform.position = transform.position + Vector3.down * 50f;
                }
                else
                {
                    m_bottom = m_candidate;
                    m_candidate.transform.position = transform.position;
                    m_battlefield.NextGhost();
                }

                m_candidate = null;
            }
        }
    }

    public void Reset()
    {
        m_candidate = null;
        m_bottom = null;
        m_top = null;
    }

    public void AddCard(GameCard card)
    {
        m_candidate = card;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        this.AddCard(collision.gameObject.GetComponent<GameCard>());
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
    }
}
