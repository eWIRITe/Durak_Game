using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionnDATA : MonoBehaviour
{
    [Header("images for activation/disactivation")]
    public Sprite ActiveSprite;
    public Sprite notActiveSprite;

    [Header("style DATA")]
    public int NeedGames;
    public string style_name;
    public bool available = false;

    public void setStyle()
    {
        if (available)
        {
            GameObject.FindGameObjectWithTag("CollectionScreen").GetComponent<CollectionsScreen>().setAllNotActive();

            GetComponent<Image>().sprite = ActiveSprite;

            PlayerPrefs.SetString("Style", style_name);
        }
    }
}
