using System;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public float Cooficent;
    public float ScreenWith = 1980;

    public GameObject StartScreen;
    public GameObject OwnerStartGameButton;

    public RoomRow _roomRow;

    private SocketNetwork m_socketNetwork;

    private void Start()
    {
        ScreenWith = Screen.width;

        m_socketNetwork = GameObject.FindGameObjectWithTag("SocketNetwork").GetComponent<SocketNetwork>();
    }

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

    public void PlayersTurn(uint turnPlayerID)
    {

    }

    ///////\\\\\\\
    //NewPlayers\\
    //..........\\
    public GameObject NewPlayer;
    public Transform NewPlayerSpawnPoint;

    public void NewPlayerJoin(uint UId)
    {
        User _user = Instantiate(NewPlayer, NewPlayerSpawnPoint.position, NewPlayerSpawnPoint.rotation).GetComponent<User>();

        _user.gameObject.transform.localScale = NewPlayerSpawnPoint.localScale;

        _user.transform.SetParent(gameObject.transform);

        _roomRow.roomPlayers.Add(_user);

        _user.Initi(UId);

        SetPositionsForAllUsers(_roomRow.roomPlayers);
    }

    public void DeletePlayer(uint UId)
    {
        Debug.Log("DeletePlayer");

        for(int i = 0; i < _roomRow.roomPlayers.Count; i++)
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

    public void Beat(Card attaced, Card attaking)
    {
        
    }
    public void Attack(Card attacCard)
    {

    }
    public void Grab(uint UId)
    {

    }
    public void SpawnBattlefieldCard(Card battlefieldCard)
    {

    }
    public void Distribution(Card[] cardsList)
    {

    }
    public void Fold(uint folderID)
    {

    }

    private void SetPositionsForAllUsers(List<User> users)
    {
        int i = 1;

        while(i < users.Count)
        {
            float x = (float)((ScreenWith * i / users.Count) - ScreenWith * 0.5);
            float y = (float)( Math.Abs(x) / Cooficent) * -1;

            Debug.Log(y);

            Vector3 coords = new Vector3(x, y, 0);

            StartCoroutine(users[i].MoveTo(coords));

            i++;
        }

        Debug.Log("SetPositionsForAllUsers");
    }
}