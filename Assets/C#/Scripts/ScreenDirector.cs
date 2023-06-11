using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScreenDirector : BaseScreen
{
    public TMP_Text ID_text;
    public AvatarScr avatar;

    public GameObject StartScreen;
    public GameObject SignInScreen;
    public GameObject LoginScreenl;
    public GameObject MenuScreen;
    public GameObject SettingsScreen;
    public GameObject PolicyScreen;
    public GameObject ShopScreen;
    public GameObject RatingScreen;
    public GameObject CollectionsScreen;
    public GameObject RewardsScreen;
    public GameObject SkinsScreen;

    public GameObject AdminPanel;

    List<GameObject> screens = new List<GameObject>();

    void Start()
    {
        screens.Add(StartScreen);
        screens.Add(SignInScreen);
        screens.Add(LoginScreenl);
        screens.Add(MenuScreen);
        screens.Add(SettingsScreen);
        screens.Add(PolicyScreen);
        screens.Add(ShopScreen);
        screens.Add(RatingScreen);
        screens.Add(CollectionsScreen);
        screens.Add(RewardsScreen);
        screens.Add(SkinsScreen);

        string token = PlayerPrefs.GetString("token");
        if (string.IsNullOrEmpty(token))
        {
            if (PlayerPrefs.GetInt("remember") == 1)
            {
                ActiveScreen(EScreens.MenuScreen);
                return;
            }
        }
        ActiveScreen(EScreens.StartScreen);

        Session.changeUId += UpdateID;
    }

    public void ActiveScreen(EScreens _screenToActive = EScreens.MenuScreen)
    {
        foreach (GameObject _screen in screens)
        {
            _screen.SetActive(false);
        }

        switch (_screenToActive)
        {
            case EScreens.StartScreen:
                StartScreen.SetActive(true);
                break;

            case EScreens.MenuScreen:
                MenuScreen.SetActive(true);
                MenuScreen.GetComponent<MenuScreen>().OnShow();
                break;

            case EScreens.LoginScreen:
                LoginScreenl.SetActive(true);
                break;

            case EScreens.SignInScreen_NameAvatar:
                SignInScreen.SetActive(true);
                break;

            case EScreens.PolicyScreen:
                PolicyScreen.SetActive(true);
                break;

            case EScreens.CollectionsScreen:
                CollectionsScreen.SetActive(true);
                break;

            case EScreens.RatingScreen:
                RatingScreen.SetActive(true);
                break;

            case EScreens.ShopScreen:
                ShopScreen.SetActive(true);
                break;

            case EScreens.SkinsScreen:
                SkinsScreen.SetActive(true);
                break;

            case EScreens.SettingsScreen:
                SettingsScreen.SetActive(true);
                break;

            case EScreens.RewardsScreen:
                RewardsScreen.SetActive(true);
                break;

            default:
                break;
        }
    }

    public void UpdateID(uint ID)
    {
        avatar.UserID = ID;

        if (ID != 0) ID_text.text = "ID: " + ID.ToString();
    }

    public void activeAdminPanel()
    {
        AdminPanel.SetActive(true);
    }

    public void GameStarted()
    {
        foreach (GameObject _screen in screens)
        {
            _screen.SetActive(false);
        }
    }
}
