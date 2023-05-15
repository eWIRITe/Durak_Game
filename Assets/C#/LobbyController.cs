using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LobbyController : API_controller
{
    //Ui
    [Header("UI"), Space]

    //SignIn ui
    [Header("SignIn")]
    public InputField S_Name;
    public InputField S_Email;
    public InputField S_Phone;
    public InputField S_Password;
    public Toggle S_Captcha;

    //LogIn ui
    [Header("LogIn")]
    public InputField L_Email;
    public InputField L_Password;

    [Header("Message")]
    public GameObject MessageWindow;
    public TMP_Text MessageText;

    //////////////////
    //create account//
    //////////////////
    //**************//
    public void SignInButton()
    {
        StartCoroutine(base.GetUserByEmail(S_Email.text, (result) => {
            //if there is no user with same email
            if(result == "")
            {
                //create new user
                StartCoroutine(base.CreateUser(S_Name.text, S_Email.text, S_Phone.text, S_Password.text, request => {
                    //get user
                    StartCoroutine(base.GetUserByEmail(S_Email.text, result => {
                        User _user;
                        _user = JsonUtility.FromJson<User>(result);
                        SucsessfulEnter(_user); 
                    }));
                }));
            }
            //if this email already using
            else
            {
                //print massege
                PrintMessage("this email already using");

                //reset input fields
                S_Email.text = "";
                S_Password.text = "";

                return;
            }
        }));
    }

    ///////////////////
    //come to account//
    ///////////////////
    //***************//
    public void LogInButton()
    {
        StartCoroutine(base.GetUserByEmail(L_Email.text, result => {
            //if there is incorrect email
            if(result == ""){
                PrintMessage("incorrect email or password");
            }

            //if email is correct
            else{
                User _user = JsonUtility.FromJson<User>(result);
                
                //if all is correct
                if(_user.Password == L_Password.text)
                {
                    SucsessfulEnter(_user);
                }

                //if there is incorrect password
                else
                {
                    PrintMessage("incorrect email or password");
                }
            }
        }));
    }

    /////////////////
    //enter to game//
    //-------------//
    public void SucsessfulEnter(User nbnbnbn)
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("UserID", nbnbnbn.UserID);
        SceneManager.LoadScene(1);
    }

    /////////////////
    //print message//
    //*************//
    public void PrintMessage(string massege)
    {
        MessageText.text = massege;

        LeanTween.scale(MessageWindow, new Vector3(1, 1, 1), 2f).setOnComplete(FindMessageWindow);
        
    }
    void FindMessageWindow()
    {
        LeanTween.scale(MessageWindow, new Vector3(0, 0, 0), 1f);
    }
}
