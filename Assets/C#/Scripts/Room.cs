using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public float StartUsersLine;
    public float ScreenWith = 1980;
    public float MaxUsersLise;

    public RoomRow _roomRow;

    private void Start()
    {
        ScreenWith = Screen.width;
    }

    public void StartGame(uint firstPlayerID, byte trump)
    {

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
        Debug.Log(i);

        while(i < users.Count)
        {
            float x = (float)((ScreenWith * i / users.Count) - ScreenWith * 0.5);
            float y = x > 0 ? (float)(((float)((float)((float)MaxUsersLise - (float)StartUsersLine)%1) * (float)((float)i / (float)users.Count)) * -1) : (((float)((float)((float)MaxUsersLise - (float)StartUsersLine) % 1) * (float)((float)i / (float)users.Count)));

            Vector2 coords = new Vector2(x, y);

            StartCoroutine(users[i].MoveTo(coords));

            i++;
        }
    }
}