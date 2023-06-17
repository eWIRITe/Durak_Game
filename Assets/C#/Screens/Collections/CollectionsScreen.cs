using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionsScreen : MonoBehaviour
{
    public GameObject standart_style;
    public GameObject Russian_style;
    public GameObject Fallout_style;
    public GameObject nature_tropicks_style;
    public GameObject herous_style;
    public GameObject cars_style;
    public GameObject horror_style;
    public GameObject erotick_style;

    List<GameObject> collections_list = new List<GameObject>();

    private void Start()
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

            }
        }
    }
}
