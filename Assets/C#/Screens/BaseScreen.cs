using UnityEngine;

public class BaseScreen : MonoBehaviour
{
    protected ScreenDirector m_screenDirector;
    protected SocketNetwork m_socketNetwork;

    protected void Awake()
    {
        m_screenDirector = GameObject.FindGameObjectWithTag("ScreenDirector").GetComponent<ScreenDirector>();
        m_socketNetwork = GameObject.FindGameObjectWithTag("SocketNetwork").GetComponent<SocketNetwork>();
    }

    public virtual void SetActiveHandler(bool active)
    { }
}
