using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;

public class API_controller : GameController
{
    string url_Users = "http://localhost/BackEnd/LobbyBackEnd.php";
    string url_Balance = "http://localhost/BackEnd/BalanceController.php";
    string url_Photos = "http://localhost/BackEnd/AvatarController.php";
    string url_Room = "http://localhost/BackEnd/Room.php";

    /////////////////////////////
    ///////////Users/////////////
    /////////////////////////////
    public IEnumerator GetUserByEmail(string email, Action<string> callback)
    {
        WWWForm form = new WWWForm();

        using (UnityWebRequest request = UnityWebRequest.Get(url_Users + "?ReqestType=GetUserByEmail&email="+email))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                callback(request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                callback(json);
            }
        }
    }
    public IEnumerator GetUserByID(int ID, Action<string> callback)
    {
        WWWForm form = new WWWForm();

        using (UnityWebRequest request = UnityWebRequest.Get(url_Users + "?ReqestType=GetUserByID&ID="+ID))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                callback(request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                callback(json);
            }
        }
    }
    public IEnumerator CreateUser(string name, string email, string phone, string password, Action<bool> callback)
    {
        WWWForm _form = new WWWForm();
        _form.AddField("ReqestType", "CreateUser");
        _form.AddField("name", name);
        _form.AddField("email", email);
        _form.AddField("phone", phone);
        _form.AddField("password", password);

        using (UnityWebRequest request = UnityWebRequest.Post(url_Users, _form))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
                callback(false);
            }

            else Debug.Log(request.downloadHandler.text); callback(true);
        }
    }

    /////////////////////////////
    ////////////Photo////////////
    /////////////////////////////
    public IEnumerator UploadPhoto(string directoryPath, int ID) 
    {
        // Load the photo file from the directory
        byte[] photoData = File.ReadAllBytes(directoryPath);

        // Create a new form to submit to the PHP script
        WWWForm form = new WWWForm();

        // Add the photo file to the form
        form.AddBinaryData("file", photoData, "userAvatar_" + ID.ToString() + ".jpeg", "image/jpeg");
        
        // Submit the form to the PHP script
        using(WWW www = new WWW(url_Photos, form))
        {
            // Wait for the response from the PHP script
            yield return www;

            // Check for errors
            if (www.error != null) {
                Debug.Log("Error uploading photo: " + www.error);
            } else {
                Debug.Log("Photo uploaded successfully.");
            }
        }
        Image Avatar = GameObject.FindGameObjectWithTag("PlayerAvatar").GetComponent<Image>();
        StartCoroutine(GetPhoto(PlayerPrefs.GetInt("UserID").ToString(), Avatar));
    }
    public IEnumerator GetPhoto(string ID, Image Avatar)
    {
        using(UnityWebRequest www = UnityWebRequestTexture.GetTexture("http://localhost/BackEnd/Avatars/userAvatar_"+ID+".jpeg"))
        {
            yield return www.SendWebRequest();
    
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else if(((DownloadHandlerTexture)www.downloadHandler).texture == null) yield return null;
            else
            {
                Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
    
                Sprite newSprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(.5f, .5f));

                Avatar.sprite = newSprite;
            }
        }
        
    }

    //////////////////////////////
    ///////////Balance////////////
    //////////////////////////////
    public IEnumerator GetBalanceOfUserByID(string ID, Action<string> callback)
    {
        WWWForm form = new WWWForm();

        using (UnityWebRequest request = UnityWebRequest.Get(url_Balance + "?ID="+ID))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                callback(request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                callback(json);
            }
        }
    }
    public IEnumerator WriteBalance(string ID, int balanceOperation, Action<bool> callback)
    {
        WWWForm _form = new WWWForm();
        _form.AddField("balanceOperation", balanceOperation);
        _form.AddField("ID", ID);

        using (UnityWebRequest request = UnityWebRequest.Post(url_Balance, _form))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
                callback(false);
            }

            else Debug.Log(request.downloadHandler.text); 
            
            callback(true);
        }
    }

    //////////////////////////////
    ////////////Room//////////////
    //////////////////////////////
    public IEnumerator GetFreeRoms(Action<string> callback)
    {
        WWWForm form = new WWWForm();

        using (UnityWebRequest request = UnityWebRequest.Get(url_Room + "?requestType=GetFreeRoms"))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                callback(request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                callback(json);
            }
        }
    }
    public IEnumerator GetRoomUsers(int RoomID, Action<string> callback)
    {
        WWWForm form = new WWWForm();

        using (UnityWebRequest request = UnityWebRequest.Get(url_Room + "?requestType=GetRoomUsers&RoomID="+RoomID.ToString()))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                callback(request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                callback(json);
            }
        }
    }
    public IEnumerator CreateRoom(int MaxPlayers, int BET, int OwnerRoomID, Action<string> callback)
    {
        WWWForm _form = new WWWForm();
        _form.AddField("requestType", "CreateRoom");
        _form.AddField("MaxPlayers", MaxPlayers);
        _form.AddField("OwnerRoomID", OwnerRoomID);
        _form.AddField("BET", BET);

        using (UnityWebRequest request = UnityWebRequest.Post(url_Room, _form))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                callback(request.error);
            }
            
            string json = request.downloadHandler.text;
            callback(json);
        }
    }
    public IEnumerator AddUser(int RoomID, int UserID, Action<bool> callback)
    {
        WWWForm _form = new WWWForm();
        _form.AddField("requestType", "AddUser");
        _form.AddField("RoomID", RoomID);
        _form.AddField("UserID", UserID);

        using (UnityWebRequest request = UnityWebRequest.Post(url_Room, _form))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                callback(false);
            }
            
            callback(true);
        }
    }
    public IEnumerator DeliteRoom(int RoomID, Action<bool> callback)
    {
        WWWForm _form = new WWWForm();
        _form.AddField("requestType", "DeliteRoom");
        _form.AddField("RoomID", RoomID);

        using (UnityWebRequest request = UnityWebRequest.Post(url_Room, _form))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                callback(false);
            }
            
            callback(true);
        }
    }
    public IEnumerator GetRoomOwner(int RoomID, Action<string> callback)
    {
        WWWForm form = new WWWForm();

        using (UnityWebRequest request = UnityWebRequest.Get(url_Room + "?requestType=GetRoomOwner&RoomID=" + RoomID.ToString()))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                callback(request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                callback(json);
            }
        }
    }
    public IEnumerator RoomIsReady(int RoomID)
    {
        WWWForm _form = new WWWForm();
        _form.AddField("requestType", "RoomIsReady");
        _form.AddField("RoomID", RoomID);

        using (UnityWebRequest request = UnityWebRequest.Post(url_Room, _form))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Room not ready");
            }

            Debug.Log("Room ready");
        }
    }
}
