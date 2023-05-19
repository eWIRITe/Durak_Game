using UnityEngine;
using UnityEngine.UI;

public class SigninScreen : BaseScreen
{
    public InputField m_phone;
    public InputField m_email;
    public InputField m_password;

    public void SigninSuccessed()
    {
        m_screenDirector.SetScreen(EScreens.LoginScreen);
        //string token = PlayerPrefs.GetString("token");
        //StartCoroutine(m_durakNetwork.GetPlayerId(token, m_durakNetwork.GetPlayerIdSuccessed, m_durakNetwork.GetPlayerIdFailed));
    }

    public void SigninFailed(string resp)
    {
        Debug.LogError($"SigninFailed:\n\t{resp}");
    }

    public void SigninClickHandler()
    {
        StartCoroutine(m_network.Signin(m_phone.text, m_email.text, m_password.text, SigninSuccessed, SigninFailed));
    }
}
