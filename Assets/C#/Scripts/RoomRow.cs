using System.Collections.Generic;
using UnityEngine;

public class RoomRow : MonoBehaviour
{
    [Header("RoomUI")]

    public GameObject GamePrefab;

    public static uint RoomID;

    public List<uint> roomPlayingUsersID = new List<uint>();

    public bool isGameStarted;

    [Header("Player Image")]
    public GameObject PlayerImagePrefab;
    private List<GameObject> playerImages;
    private Dictionary<uint, List<Card>> PlayersCardsDictionary;

    public void PlayerJoinToOurRoom()
    {

    }
}