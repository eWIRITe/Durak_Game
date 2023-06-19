using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RatingScreen : BaseScreen
{
    public class RatingLine
    {
        public uint ID;
        public string name;
        public int total;
        public float win_rate;
    }
    [Header("Prefabs")]
    public GameObject m_ratingLinePrefab;
    public Transform m_content;

    private void Start()
    {
        SocketNetwork.gotRaiting += gotRaiting;
    }

    public void gotRaiting(List<RatingLine> raitingList)
    {
        for (int i = m_content.childCount - 1; i >= 0; i--)
        {
            Transform child = m_content.GetChild(i);

            Destroy(child.gameObject);
        }

        raitingList.Sort((x, y) => y.win_rate.CompareTo(x.win_rate));


        for (int i = 0; i < raitingList.Count; i++)
        {
            GameObject raitingLine = Instantiate(m_ratingLinePrefab);
            raitingLine.transform.SetParent(m_content);

            raitingLine.transform.localScale = new Vector3(1, 1, 1);

            ApplyData(raitingLine.transform, i, raitingList[i].ID, raitingList[i].name, raitingList[i].total, raitingList[i].win_rate);
        }
    }

    public void OnShow()
    {
        m_socketNetwork.getRaiting();
    }

    public void ApplyData(Transform line, int lineNumber, uint ID, string name, int total, float winRate)
    {
        line.Find("Avatar").GetComponent<AvatarScr>().UserID = ID;
        m_socketNetwork.getAvatar(ID);

        line.Find("line_number").GetComponent<TMP_Text>().text = lineNumber.ToString();
        line.Find("player_name").GetComponent<TMP_Text>().text = name;
        line.Find("played_games").GetComponent<TMP_Text>().text = total.ToString();
        line.Find("win_rate").GetComponent<TMP_Text>().text = winRate.ToString("F4");
    }
}
