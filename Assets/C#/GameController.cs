using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour
{
    public string ToID(int ID)
    {
        string strID = ID.ToString();
        if(strID.Length < 8){
            while(true){
                strID = "0" + strID;
                if(strID.Length == 8){
                    break;
                }
            }
        }

        return strID;
    }

    //////////////////////
    //chacking functions//
    //<<<<<<<<<>>>>>>>>>//
    public bool isEmail(string email)
    {
        string NeedSymbol = "@";
        int countOfNeedSymbols = 1;

        for(int i = 0; i < email.Length; i++){
            if(email[i] == NeedSymbol[0]){
                countOfNeedSymbols--;
            }
        }

        if(countOfNeedSymbols == 0) return true;
        return false;
    }

    
    /////////////////////////////
    /////////User_class//////////
    /////////////////////////////
    public class User
    {
        public int UserID;
        public string Name;
        public string Email;
        public string Phone;
        public string Password;
    }
}
