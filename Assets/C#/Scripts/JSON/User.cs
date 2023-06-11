using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class User : BaseScreen
{
    public uint UserID;

    private ERole Role;

    public ERole role
    {
        set { Role = value; PrintMessage("role: " + value.ToString()); }
        get { return Role; }
    }

    public AvatarScr Avatar;
    public TMP_Text UId;

<<<<<<< Updated upstream:Assets/C#/Scripts/JSON/User.cs
    public List<Card> UserCards;
=======
    public TMP_Text MessageText;

    public List<GameObject> UserCards;
>>>>>>> Stashed changes:Assets/C#/User.cs

    public void Initi(uint ID)
    {
        UserID = ID;
        UId.text = "ID: " + ID.ToString();

<<<<<<< Updated upstream:Assets/C#/Scripts/JSON/User.cs
        StartCoroutine(m_network.GetAvatar(UserID, sucsessed => { Avatar.sprite = Sprite.Create(sucsessed, new Rect(0, 0, sucsessed.width, sucsessed.height), Vector2.one / 2.0f); }, fail => { Debug.Log(fail); }));
=======
        Avatar.UserID = UserID;
        m_socketNetwork.getAvatar(UserID);
    }

    public void PrintMessage(string message)
    {
        MessageText.text = message.ToString();
>>>>>>> Stashed changes:Assets/C#/User.cs
    }

    public IEnumerator MoveTo(Vector3 MoveToPoint)
    {
        yield return moveTo(MoveToPoint);
    }
    private bool moveTo(Vector3 MoveToPoint)
    {
        Debug.Log(MoveToPoint);

        LeanTween.moveLocal(gameObject, MoveToPoint, 2);
        LeanTween.scale(gameObject, new Vector3(1, 1, 1), 2);

        return true;
    }
}
