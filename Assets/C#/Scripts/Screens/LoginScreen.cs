using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginScreen : BaseScreen
{
    public InputField m_name;
    public InputField m_password;

    public Toggle m_remember;

    [Header("message")]
    public TMP_Text Message;

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

    public void LoginSuccessed(string token)
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

        PlayerPrefs.SetString("token", token);

        Session.Token = token;
        Session.Name = m_name.text;
        StartCoroutine(m_network.GetPlayerId(token, ID => { Session.UId = ID; }, FailedID => { Debug.Log(FailedID); }));

        m_screenDirector.ActiveScreen(EScreens.MenuScreen);

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
            Message.text = "Incorrect data";
            return;
        }

        StartCoroutine(m_network.Login(m_name.text, m_password.text, LoginSuccessed, LoginFailed));
    }
}
