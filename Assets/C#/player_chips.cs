using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class player_chips : MonoBehaviour
{
    public TMP_Text chips_text;

    void Start()
    {
        chips_text = gameObject.GetComponent<TMP_Text>();
        Session.changeChips += changeChips;
    }
    private void OnDestroy()
    {
        Session.changeChips -= changeChips;
    }

    public void changeChips(uint chips)
    {
        chips_text.text = chips.ToString();
    }
}
