using JSON_server;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScreen : BaseScreen
{
    public TMP_InputField m_newEmail;
    public TMP_InputField m_oldEmail;

    [Header("Message")]
    public GameObject MessageScreen;
    public TMP_Text MessageText;

    private void Start()
    {
        SocketNetwork.Sucsessed_emailChange += ChangeEmailSuccessed;
    }

    public void ChangeEmailSuccessed(string newEmail)
    {
        PrintMaessage("Email change is successed, new email: " + newEmail);
    }

    public void ChangeEmailFailed(string resp)
    {
        Debug.LogError($"LogoutFailed:\n\t{resp}");
    }

    public void ChangeEmailClickHandler()
    {
        string newEmail = m_newEmail.text;
        string oldEmail = m_oldEmail.text;

        if (!data_validator.CheckEmail(newEmail) || !data_validator.CheckEmail(oldEmail)) return;

        m_socketNetwork.Emit_changeEmail(Session.Token, oldEmail, newEmail);
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

    #region Message
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
    #endregion
}
