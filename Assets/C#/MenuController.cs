using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;

public class MenuController : API_controller
{
    public Image AvatarImage;
    public Text Name;
    public Text ID;

    private User _player;

    public void Start()
    {
        //Get our player
        StartCoroutine(base.GetUserByID(PlayerPrefs.GetInt("UserID"), result => {
            _player = JsonUtility.FromJson<User>(result);

            StartCoroutine(base.GetPhoto(_player.UserID.ToString(), AvatarImage));

            ID.text = "ID: " + ToID(_player.UserID);
            Name.text = _player.Name;
        }));
    }

    public void BrowseFile(Image Avatar)
    {
        string filePath = UnityEditor.EditorUtility.OpenFilePanel("Select avatar", "", "png");

        StartCoroutine(base.UploadPhoto(filePath, _player.UserID));
    }
}
