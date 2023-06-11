using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScreen : BaseScreen
{
    public TMP_InputField m_newEmail;
    public TMP_InputField m_oldEmail;

    public void ChangeEmailSuccessed()
    {

    }

    public void ChangeEmailFailed(string resp)
    {
        Debug.LogError($"LogoutFailed:\n\t{resp}");
    }

    public void ChangeEmailClickHandler()
    {
        string token = PlayerPrefs.GetString("token");
        string newEmail = m_newEmail.text;
        string oldEmail = m_oldEmail.text;
        //StartCoroutine(m_network.ChangeEmail(token, newEmail, oldEmail, ChangeEmailSuccessed, ChangeEmailFailed));
    }

    public void SortingCardsTypeChange(string sortType)
    {
        PlayerPrefs.SetString("SortType", sortType);
    }
    public void SortingTrumpsTypeChange(Dropdown dropdownObj)
    {
        string sortType = "";
        switch (dropdownObj.value)
        {
            case 1:
                sortType = "toLeft";
                break;
            case 2:
                sortType = "toRight";
                break;
            default:
                sortType = "";
                break;
        }

        PlayerPrefs.SetString("trumpSortType", sortType);
    }
}
