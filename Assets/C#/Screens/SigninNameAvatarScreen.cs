using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SigninNameAvatarScreen : BaseScreen
{
    public InputField m_name;
    public InputField m_email;
    public InputField m_password;

    [Header("Message")]
    public GameObject MessageScreen;
    public TMP_Text MessageText;

    void Start()
    {
        SocketNetwork.SignInSucsessed += SigninSuccessed;
        SocketNetwork.SignInFailed += SigninFailed;
        SocketNetwork.error += PrintMaessage;
    }

    private void SigninSuccessed()
    {
        MainThreadDispatcher.RunOnMainThread(() =>
        {
            m_screenDirector.ActiveScreen(EScreens.LoginScreen);
            Debug.Log("SigninSuccessed");
        });
    }

    private void SigninFailed()
    {
        Debug.LogError("LoginFailed");
    }

    public void SigninClickHandler()
    {
        m_socketNetwork.Signin(m_name.text, m_email.text, m_password.text);
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
