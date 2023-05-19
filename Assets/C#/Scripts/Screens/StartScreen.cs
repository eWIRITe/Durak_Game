using UnityEngine;

public class StartScreen : BaseScreen
{
    private new void Start()
    {
        base.Start();
    }

    public void LoginClickHandler()
    {
        m_screenDirector.SetScreen(EScreens.LoginScreen);
    }

    public void SigninClickHandler()
    {
        m_screenDirector.SetScreen(EScreens.SignInScreen_NameAvatar);
    }
}
