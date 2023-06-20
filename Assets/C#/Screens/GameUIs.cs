using JSON;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIs : BaseScreen
{
    //help scr room
    public RoomRow _roomRow;
    public Room _room;

    [Header("Fold timer")]
    public Image BeatTimerLine;
    public TMP_Text BeatTimerText;
    private float BeatTimer;
    public float BeatTimeToSet;

    [Header("Grab Timer")]
    public Image TakeTimerLine;
    public TMP_Text TakeTimerText;
    private float TakeTimer;
    public float TakeTimeToSet;

    [Header("Pass Timer")]
    public Image PassTimerLine;
    public TMP_Text PassTimerText;
    private float PassTimer;
    public float PassTimeToSet;

    [Header("Buttons")]
    public GameObject PassButton;
    public GameObject GrabButton;
    public GameObject FoldButton;

    [Header("role")]
    public GameObject roleObj;
    public TMP_Text roleText;

    [Header("chat")]
    public GameObject chatObject;

    // event
    public delegate void loseTime();
    public static event loseTime lose;

    private void Start()
    {
        _roomRow = GetComponent<RoomRow>();
        _room = GetComponent<Room>();

        Session.roleChanged += onRoleChange;
    }

    public void hideGrabButton()
    {
        GrabButton.SetActive(false);
        TakeTimer = 0;
    }
    public void showGrabButton()
    {
        hidePassButton();
        hideFoldButton();

        GrabButton.SetActive(true);
        TakeTimer = TakeTimeToSet;
    }

    public void hidePassButton()
    {
        PassButton.SetActive(false);
        PassTimer = 0;
    }
    public void showPassButton()
    {
        hideGrabButton();
        hideFoldButton();

        PassButton.SetActive(true);
        PassTimer = PassTimeToSet;
    }

    public void hideFoldButton()
    {
        FoldButton.SetActive(false);
        BeatTimer = 0;
    }
    public void showFoldButton()
    {
        hidePassButton();
        hideGrabButton();

        FoldButton.SetActive(true);
        BeatTimer = BeatTimeToSet;
    }

    private void FixedUpdate()
    {
        // Take timer
        if(TakeTimer > 0)
        {
            TakeTimer -= Time.deltaTime;

            TakeTimerLine.fillAmount = TakeTimer / TakeTimeToSet;

            TakeTimerText.text = "Can you beat any card? : " + Mathf.Round(TakeTimer);

            if (TakeTimer <= 0)
            {
                GrabButton.SetActive(false);
                _room.Fold();
            }
        }
        // Pass timer
        if (PassTimer > 0)
        {
            PassTimer -= Time.deltaTime;

            PassTimerLine.fillAmount = PassTimer / PassTimeToSet;

            PassTimerText.text = "Do you have anything to throw? : " + Mathf.Round(PassTimer);

            if (PassTimer <= 0)
            {
                PassButton.SetActive(false);
                _room.Pass();
            }
        }
        // Beat timer
        if (BeatTimer > 0)
        {
            BeatTimer -= Time.deltaTime;

            BeatTimerLine.fillAmount = BeatTimer / BeatTimeToSet;

            BeatTimerText.text = "Do you have anything to throw? : " + Mathf.Round(BeatTimer);

            if (BeatTimer <= 0)
            {
                FoldButton.SetActive(false);
                _room.Grab();
            }
        }
    }

    private void onRoleChange(ERole role)
    {
        roleObj.SetActive(true);

        string message = "";

        switch (role)
        {
            case ERole.main:
                message = "You are need to struggle.";
                break;
            case ERole.thrower:
                message = "you need to throw";
                break;
            case ERole.firstThrower:
                message = "You beginning, throw!";
                break;
        }

        roleText.text = message;
    }

    public void chatButton()
    {
        chatObject.SetActive(!chatObject.activeSelf);
    }
}
