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

    [Header("Message")]
    public GameObject MessageScreen;
    public TMP_Text MessageText;

    private void Start()
    {
        SocketNetwork.loginSucsessed += LoginSuccessed;
        SocketNetwork.UId += UpdateUID;
        SocketNetwork.error += PrintMaessage;
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
        }

        Session.Token = token;
        Session.Name = m_name.text;

        m_socketNetwork.GetUserID(token);

        MainThreadDispatcher.RunOnMainThread(() =>
        {
            m_screenDirector.ActiveScreen(EScreens.MenuScreen);
        });
    }

    private void UpdateUID(uint UId)
    {
        MainThreadDispatcher.RunOnMainThread(() =>
        {
            Session.UId = UId;
        });
        
    }

    private void LoginFailed(string resp)
    {
        Message.text = resp.ToString();

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

        m_socketNetwork.Login(m_name.text, m_password.text);
    }

    public void PrintMaessage(string Message)
    {
        MainThreadDispatcher.RunOnMainThread(() =>
        {
            MessageText.text = Message;
            LeanTween.scale(MessageScreen, new Vector3(1, 1, 1), 2).setOnComplete(finishMessage);
        });
    }
    public void finishMessage()
    {
        MainThreadDispatcher.RunOnMainThread(() =>
        {
            LeanTween.scale(MessageScreen, new Vector3(0, 0, 0), 1).setOnComplete(finishMessage);
        });
    }
}
