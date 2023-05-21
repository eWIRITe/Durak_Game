using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AdminPabel : BaseScreen
{
    public TMP_InputField ChipsInput;
    public TMP_InputField NewAdminIDInput;

    public void GetChipsAdminClickHandler()
    {
        Debug.Log("GetChipsAdminClickHandler");
        StartCoroutine(m_network.GetChips_admin(Session.Token, int.Parse(ChipsInput.text), newChips =>
       {
           Session.Chips = newChips;
       }, error =>
       {
           Debug.Log(error);
       }));
    }
    
    public void SetNewAdminClickHandler()
    {

    }
}
