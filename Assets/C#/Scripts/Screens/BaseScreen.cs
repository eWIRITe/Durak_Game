using Org.BouncyCastle.Asn1.BC;
using UnityEngine;

public class BaseScreen : MonoBehaviour
{
    protected ScreenDirector m_screenDirector;
    protected Network m_network;

    protected void Start()
    {
        //m_screenDirector = GameObject.FindGameObjectWithTag("ScreenDirector").GetComponent<ScreenDirector>();
        m_network = GameObject.FindGameObjectWithTag("Network").GetComponent<Network>();
    }

    public virtual void SetActiveHandler(bool active)
    { }
}
