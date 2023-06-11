//using UnityEngine;
//using UnityEngine.UI;

//public class Tooltip : MonoBehaviour
//{
//    private static Tooltip m_instance;

//    public AnimationState m_state;

//    private Image m_background;

//    private Text m_text;

//    private TooltipMessage m_tooltipMessage;

//    private float m_alpha;

//    void Awake()
//    {
//        m_instance = this;

//        m_background = GetComponentInChildren<Image>();
//        m_text = GetComponentInChildren<Text>();

//        m_background.color = new Color(1, 1, 1, 0);
//        m_text.color = new Color(1, 1, 1, 0);
//    }

//    void Update()
//    {
//        transform.position = Input.mousePosition;

//        if (m_tooltipMessage != null && !m_tooltipMessage.gameObject.activeInHierarchy)
//        {
//            m_state = AnimationState.FADEOUT;
//            m_tooltipMessage = null;
//        }

//        if(m_state == AnimationState.FADEOUT)
//        {
//            m_alpha -= Time.deltaTime * 5.0f;

//            if(m_alpha < 0)
//            {
//                m_alpha = 0;

//                m_state = AnimationState.PAUSE;
//            }

//            m_background.color = new Color(1, 1, 1, m_alpha);
//            m_text.color = new Color(1, 1, 1, m_alpha);
//        }

//        if (m_state == AnimationState.FADEIN)
//        {
//            m_alpha += Time.deltaTime * 5.0f;

//            if (m_alpha > 0.7f)
//            {
//                m_alpha = 0.7f;

//                m_state = AnimationState.PAUSE;
//            }

//            m_background.color = new Color(1, 1, 1, m_alpha);
//            m_text.color = new Color(1, 1, 1, m_alpha);
//        }
//    }

//    public static void Show(TooltipMessage tooltipMessage)
//    {
//        m_instance.m_tooltipMessage = tooltipMessage;
//        m_instance.m_text.text = Localization.GetCurrentLocale() == "ru" ? tooltipMessage.message_ru : tooltipMessage.message_en;
//        m_instance.m_state = AnimationState.FADEIN;
//    }

//    public static void Show(string message)
//    {
//        m_instance.m_text.text = message;
//        m_instance.m_state = AnimationState.FADEIN;
//    }

//    public static void Hide()
//    {
//        m_instance.m_state = AnimationState.FADEOUT;
//    }

//    public enum AnimationState { PAUSE, FADEIN, FADEOUT }
//}
