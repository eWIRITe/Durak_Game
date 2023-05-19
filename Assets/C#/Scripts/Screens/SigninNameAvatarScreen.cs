using UnityEngine;
using UnityEngine.UI;

public class SigninNameAvatarScreen : BaseScreen
{
    public InputField m_name;
    public InputField m_email;
    public InputField m_password;

    public GameObject MenuScreen;
    public GameObject ThisScreen;

    public void SigninSuccessed()
    {
        //m_screenDirector.SetScreen(EScreens.LoginScreen);
        ThisScreen.SetActive(false);
        MenuScreen.SetActive(true);
        Debug.Log("SigninSuccessed");
    }

    public void SigninFailed(string resp)
    {
        Debug.LogError($"LoginFailed:\n\t{resp}");
    }

    public void SigninClickHandler()
    {
        StartCoroutine(m_network.Signin(m_name.text, m_email.text, m_password.text, SigninSuccessed, SigninFailed));
    }
}
