using UnityEngine;
using UnityEngine.UI;

public class MenuController : API_controller
{
    public Image AvatarImage;
    public Text Name;
    public Text ID;

    public void Start()
    {
        //Get our player
        StartCoroutine(base.GetUserByID(PlayerPrefs.GetInt("UserID"), result => {
            base.Player = JsonUtility.FromJson<User>(result);

            StartCoroutine(base.GetPhoto(base.Player.UserID.ToString(), AvatarImage));

            ID.text = "ID: " + ToID(base.Player.UserID);
            Name.text = base.Player.Name;
        }));
    }

    public void BrowseFile(Image Avatar)
    {
        string filePath = UnityEditor.EditorUtility.OpenFilePanel("Select avatar", "", "png");

        StartCoroutine(base.UploadPhoto(filePath, base.Player.UserID));
    }
}
