using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RatingScreen : BaseScreen
{
    public class RatingLine
    {
        public int id;
        public string name;
        public int total;
        public float win_rate;

        public int Id
        { get { return id; } }

        public string Name
        { get { return name; } }

        public int Total
        { get { return total; } }

        public float WinRate
        { get { return win_rate; } }
    }

    public GameObject m_ratingLinePrefab;

    public Transform m_content;

    public void OnShow()
    {
        string token = PlayerPrefs.GetString("token");
        //StartCoroutine(m_network.GetRating(token, 0, 50, GetRatingSuccessed, GetRatingFailed));
    }

    public void ApplyData(Transform line, int lineNumber, string name, int total, float winRate)
    {
        //            0  -  profile photo
        line.GetChild(1).GetComponent<Text>().text = lineNumber.ToString();
        line.GetChild(2).GetComponent<Text>().text = name;
        line.GetChild(3).GetComponent<Text>().text = total.ToString();
        line.GetChild(4).GetComponent<Text>().text = winRate.ToString("F4");
    }

    public void GetRatingSuccessed(List<RatingLine> rating)
    {
        int number = 1;
        foreach (RatingLine line in rating)
        {
            this.ApplyData(GameObject.Instantiate(m_ratingLinePrefab, m_content).transform, number++, line.Name, line.Total, line.WinRate);
        }
    }

    public void GetRatingFailed(string resp)
    {
        Debug.LogError($"GetRatingFailed:\n\t{resp}");
    }
}
