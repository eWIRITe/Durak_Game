using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionsScreen : MonoBehaviour
{
    public Sprite ActiveSprite;
    public Sprite notActiveSprite;

    [Space, Header("styles")]
    public GameObject standart_style;
    public GameObject Russian_style;
    public GameObject nature_middleLine_style;
    public GameObject Fallout_style;
    public GameObject nature_tropicks_style;
    public GameObject herous_style;
    public GameObject cars_style;
    public GameObject horror_style;
    public GameObject erotick_style;

    List<GameObject> collections_list = new List<GameObject>();

    private void Awake()
    {
        if (standart_style != null) collections_list.Add(standart_style);
        if (Russian_style != null) collections_list.Add(Russian_style);
        if (Fallout_style != null) collections_list.Add(Fallout_style);
        if (nature_tropicks_style != null) collections_list.Add(nature_tropicks_style);
        if (herous_style != null) collections_list.Add(herous_style);
        if (cars_style != null) collections_list.Add(cars_style);
        if (horror_style != null) collections_list.Add(horror_style);
        if (erotick_style != null) collections_list.Add(erotick_style);
    }

    public void OnShow()
    {

        foreach (GameObject _collection in collections_list)
        {
            if(Session.PlayedGames >= _collection.GetComponent<CollectionnDATA>().NeedGames)
            {
                _collection.GetComponent<CollectionnDATA>().available = true;

                _collection.GetComponent<Button>().interactable = true;

                _collection.transform.Find("Content").Find("Top").Find("Title").gameObject.SetActive(true);
                _collection.transform.Find("Content").Find("Top").Find("Lock").gameObject.SetActive(false);
                _collection.transform.Find("Content").Find("Cost").gameObject.SetActive(false);
            }
            else
            {
                _collection.GetComponent<CollectionnDATA>().available = false;

                _collection.GetComponent<Button>().interactable = false;

                _collection.transform.Find("Content").Find("Top").Find("Title").gameObject.SetActive(false);
                _collection.transform.Find("Content").Find("Top").Find("Lock").gameObject.SetActive(true);
                _collection.transform.Find("Content").Find("Cost").gameObject.SetActive(true);
            }
        }
    }

    public void setAllNotActive()
    {
        foreach(GameObject _collection in collections_list)
        {
            _collection.GetComponent<Image>().sprite = notActiveSprite;
        }
    }
}
