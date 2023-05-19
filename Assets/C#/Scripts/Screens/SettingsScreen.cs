using TMPro;
using UnityEngine;

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
        StartCoroutine(m_network.ChangeEmail(token, newEmail, oldEmail, ChangeEmailSuccessed, ChangeEmailFailed));
    }
}
