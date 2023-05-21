using System;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Image m_avatar;

    public Text m_name;

    public GameObject m_shadow;

    public Text m_timer;

    public Image m_progress;

    private uint m_uid;

    public uint Uid
    {
        get { return m_uid; }
    }

    private uint m_pid;

    public uint Pid
    {
        get { return m_pid; }
    }

    public float m_timeToStep = 30.5f;

    private float m_t = 0f;

    private Action m_dingCallback;

    private bool m_started;

    private void Start()
    {
    }

    private void Update()
    {
        if (m_t > 0)
        {
            m_t -= Time.deltaTime;

            m_progress.rectTransform.offsetMax += new Vector2((170f / m_timeToStep), 2);

            m_timer.text = Convert.ToInt32(m_t).ToString();
        }
        else if (m_started)
        {
            this.StopTimer();

            m_dingCallback();
        }
    }

    public void StopTimer()
    {
        m_started = false;

        m_t = 0;

        m_shadow.SetActive(false);

        m_timer.gameObject.SetActive(false);
    }

    public void StartTimer(Action ding)
    {
        m_started = true;

        m_t = m_timeToStep;

        m_progress.rectTransform.offsetMax = new Vector2(0, 2);

        m_timer.gameObject.SetActive(false);

        m_shadow.SetActive(true);

        m_dingCallback = ding;
    }

    public void Init(uint uid, uint pid)
    {
        m_uid = uid;
        m_pid = pid;

        Network net = GameObject.FindGameObjectWithTag("Network").GetComponent<Network>();

        net.GetAvatar(Session.Token, m_uid, GetAvatarSuccessed, GetAvatarFailed);
        net.GetPlayerName(Session.Token, m_uid, GetPlayerNameSuccessed, GetPlayerNameFailed);
    }

    private void GetAvatarSuccessed(Texture2D avatar)
    {
        m_avatar.sprite = Sprite.Create(avatar, new Rect(0, 0, avatar.width, avatar.height), Vector2.one / 2.0f);
    }

    private void GetAvatarFailed(string resp)
    {
        Debug.LogError($"GetChipsFailed:\n\t{resp}");
    }

    private void GetPlayerNameSuccessed(string name)
    {
        m_name.text = name;
    }

    private void GetPlayerNameFailed(string resp)
    {
        Debug.LogError($"GetChipsFailed:\n\t{resp}");
    }
}
