public class Card
{
    private ESuit m_suit;

    public ESuit Suit
    {
        get { return m_suit; }
    }

    private ENominal m_nominal;

    public ENominal Nominal
    {
        get { return m_nominal; }
    }

    private byte m_byte;

    public byte Byte
    {
        get { return m_byte; }
    }

    public Card(ESuit suit, ENominal numinal)
    {
        m_suit = suit;
        m_nominal = numinal;
        m_byte = (byte)((int)m_suit + (int)m_nominal);
    }

    public Card(byte value)
    {
        m_suit = (ESuit)(value / ((byte)ENominal.Count));
        m_nominal = (ENominal)(value % ((byte)ENominal.Count));
        m_byte = value;
    }
}
