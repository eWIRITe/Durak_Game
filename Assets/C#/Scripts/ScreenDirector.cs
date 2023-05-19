using UnityEngine;

public class ScreenDirector : MonoBehaviour
{
    private EScreens m_activeScreen;

    public GameObject[] m_screens = new GameObject[(int)EScreens.Count];

    public GameObject m_top;

    void Start()
    {
        foreach (var item in m_screens)
            item.SetActive(false);

        m_top.SetActive(false);
        m_activeScreen = EScreens.StartScreen;
        this.SetActive(m_activeScreen, true);
    }

    private void SetActive(EScreens screen, bool value)
    {
        GameObject screenObject = m_screens[(int)m_activeScreen];

        screenObject.GetComponent<BaseScreen>().SetActiveHandler(value);

        screenObject.SetActive(value);
    }

    public void Back()
    {
        switch (m_activeScreen)
        {
            case EScreens.LoginScreen:
            case EScreens.SignInScreen:
            case EScreens.SignInScreen_NameAvatar:
                this.SetScreen(EScreens.StartScreen);
                break;

            case EScreens.SettingsScreen:
            case EScreens.ShopScreen:
            case EScreens.SkinsScreen:
            case EScreens.CollectionsScreen:
            case EScreens.RatingScreen:
            case EScreens.RewardsScreen:
                this.SetScreen(EScreens.MenuScreen);
                break;
            default:
                break;
        }
    }

    public void SetScreen(EScreens screen)
    {
        if (!EScreens.IsDefined(typeof(EScreens), screen))
        {
            return;
        }
        
        m_top.SetActive(false);
        this.SetActive(m_activeScreen, false);
        m_activeScreen = screen;
        this.SetActive(m_activeScreen, true);

        switch (screen)
        {
            case EScreens.SignInScreen_NameAvatar:
            case EScreens.SignInScreen:
            case EScreens.ShopScreen:
            case EScreens.RewardsScreen:
            case EScreens.SettingsScreen:
            case EScreens.SkinsScreen:
            case EScreens.LoginScreen:
                m_top.SetActive(true);
                break;

            case EScreens.MenuScreen:
                m_screens[(int)screen].GetComponent<MenuScreen>().OnShow();
                break;

            case EScreens.RatingScreen:
                m_screens[(int)screen].GetComponent<RatingScreen>().OnShow();
                m_top.SetActive(true);
                break;
        }
    }
}
