using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LoginScreen : BaseScreen
{
    public InputField m_name;
    public InputField m_password;

    public Toggle m_remember;

    private new void Start()
    {
        base.Start();
    }

    public override void SetActiveHandler(bool active)
    {
        if (active == true)
        {
            if (PlayerPrefs.HasKey("remember"))
            {
                m_name.text = PlayerPrefs.GetString("name");
                m_password.text = PlayerPrefs.GetString("password");
                m_remember.isOn = PlayerPrefs.GetInt("remember") > 0;
            }
        }
    }

    public void RestoreClickHandler()
    {
        Debug.LogWarning("Player login error");
    }

    private void LoginSuccessed(string token)
    {
        Debug.Log($"My token is {token}");

        if (m_remember.isOn)
        {
            PlayerPrefs.SetInt("remember", 1);
            PlayerPrefs.SetString("name", m_name.text);
            PlayerPrefs.SetString("password", m_password.text);
        }
        else
        {
            PlayerPrefs.DeleteKey("remember");
            PlayerPrefs.DeleteKey("name");
            PlayerPrefs.DeleteKey("password");
        }

        Session.Token = token;
        Session.Name = m_name.text;

        m_screenDirector.SetScreen(EScreens.MenuScreen);

        ScreenReset();
    }

    private void LoginFailed(string resp)
    {
        Debug.LogError($"LoginFailed:\n\t{resp}");

        ScreenReset();
    }

    private void ScreenReset()
    {
        m_name.text = string.Empty;
        m_password.text = string.Empty;
        m_remember.isOn = false;
    }

    public void LoginClickHandler()
    {
        if (string.IsNullOrEmpty(m_name.text) || string.IsNullOrEmpty(m_password.text))
        {
            return;
        }

        StartCoroutine(m_network.Login(m_name.text, m_password.text, LoginSuccessed, LoginFailed));
    }
}
