using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public Text filePathText;
    public Button browseButton;

    void Start()
    {
        browseButton.onClick.AddListener(BrowseFile);
    }

    void BrowseFile()
    {
        string filePath = UnityEditor.EditorUtility.OpenFilePanel("Select avatar", "", "jpg");
        filePathText.text = filePath;
    }
}
