using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public float Cooficent;
    public float ScreenWith = 1980;

    public float PlaceMultiplyer;
    public float RotationMultiplyer;

    public GameObject StartScreen;
    public GameObject OwnerStartGameButton;
    public GameObject PlayerCard;

    public RoomRow _roomRow;

    private SocketNetwork m_socketNetwork;

    private void Start()
    {
        ScreenWith = Screen.width;

        m_socketNetwork = GameObject.FindGameObjectWithTag("SocketNetwork").GetComponent<SocketNetwork>();
        SocketNetwork.ready += OnReady;

        GameObject.Find("UI").GetComponent<Canvas>().worldCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }


    ///////\\\\\\
    /// ready \\\
    ///////\\\\\\
    public void StartGame()
    {
        Debug.Log("RoomID: " + _roomRow.RoomID);
        m_socketNetwork.EmitReady(_roomRow.RoomID);

        StartScreen.SetActive(false);
    }
    public void OnReady()
    {
        StartScreen.SetActive(false);

        _roomRow.isGameStarted = true;
        //what to do when owner pressed ready
        //...\\
    }


    ///////\\\\\\\
    //NewPlayers\\
    ///////\\\\\\\
    public GameObject NewPlayer;
    public Transform NewPlayerSpawnPoint;


    /////////////\\\\\\\\\\\\
    /// players functions \\\
    /////////////\\\\\\\\\\\\
    public void NewPlayerJoin(uint UId)
    {
        User _user = Instantiate(NewPlayer, NewPlayerSpawnPoint.position, NewPlayerSpawnPoint.rotation).GetComponent<User>();

        _user.gameObject.transform.localScale = NewPlayerSpawnPoint.localScale;

        _user.transform.SetParent(GameObject.Find("UI").transform);

        _roomRow.roomPlayers.Add(_user);

        _user.Initi(UId);

        SetPositionsForAllUsers(_roomRow.roomPlayers);
    }
    public void DeletePlayer(uint UId)
    {
        Debug.Log("DeletePlayer");

        for (int i = 0; i < _roomRow.roomPlayers.Count; i++)
        {
            Debug.Log("for");
            if ((int)_roomRow.roomPlayers[i].UserID == (int)UId)
            {
                Destroy(_roomRow.roomPlayers[i].gameObject);
                _roomRow.roomPlayers.RemoveAt(i);

                Debug.Log("remove user");
            }
        }
        Debug.Log("no one");

        SetPositionsForAllUsers(_roomRow.roomPlayers);
    }


    /////////////\\\\\\\\\\\\
    /// playing functions \\\
    /////////////\\\\\\\\\\\\
    public void Beat(GameCard attaced, GameCard attaking)
    {

    }
    public void Attack(GameCard attacCard)
    {

    }
    public void Grab(uint UId)
    {

    }
    public void SpawnBattlefieldCard(GameCard battlefieldCard)
    {

    }
    public void Distribution(GameCard[] cardsList)
    {

    }
    public void Fold(uint folderID)
    {

    }


    public void SetPositionsForAllUserCards()
    {
        for (int j = 1; j < _roomRow.roomPlayers.Count; j++)
        {
            Vector3 playerPos = _roomRow.roomPlayers[j].gameObject.transform.position;
            playerPos.y -= 1;

            for (int i = 1; i < _roomRow.roomPlayers[j].UserCards.Count; i++)
            {
                _roomRow.roomPlayers[j].UserCards[i].transform.SetParent(_roomRow.roomPlayers[j].gameObject.transform);

                _roomRow.roomPlayers[j].UserCards[i].gameObject.GetComponent<SpriteRenderer>().sortingOrder = i;

                Vector3 pos = new Vector3((Screen.height / PlaceMultiplyer) * (i - ((_roomRow.roomPlayers[j].UserCards.Count) / 2)), gameObject.transform.position.y, 0);
                Vector3 rotate = new Vector3(0, 0, (RotationMultiplyer * (i - ((_roomRow.roomPlayers[j].UserCards.Count) / 2))) * -1);

                StartCoroutine(MoveCard(_roomRow.roomPlayers[j].UserCards[i], pos, rotate));
            }
        }
    }
    public void SetPositionsForAllUsers(List<User> users)
    {
        int i = 1;

        while (i < users.Count)
        {
            float x = (float)((ScreenWith * i / users.Count) - ScreenWith * 0.5);
            float y = (float)(Math.Abs(x) / Cooficent) * -1;

            Debug.Log(y);

            Vector3 coords = new Vector3(x, y, 0);

            StartCoroutine(users[i].MoveTo(coords));

            i++;
        }

        Debug.Log("SetPositionsForAllUsers");
    }


    //////////////\\\\\\\\\\\\\
    /// lean twin functions \\\
    //////////////\\\\\\\\\\\\\
    public IEnumerator MoveCard(GameObject card, Vector3 newCardPos, Vector3 rotate)
    {
        Debug.Log(newCardPos);
        LeanTween.moveLocal(card, newCardPos, 2);
        LeanTween.rotate(card, rotate, 2);
        yield return null;
    }
}