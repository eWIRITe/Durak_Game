using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AdminPabel : BaseScreen
{
    public TMP_InputField ChipsInput;

    public void GetChipsAdminClickHandler()
    {
        m_socketNetwork.admin_getChips(int.Parse(ChipsInput.text));
        m_socketNetwork.GetChips(Session.Token);
    }

}
