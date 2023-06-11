using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class User : BaseScreen
{
    public uint UserID;

    public AvatarScr Avatar;
    public TMP_Text UId;

    public ERole role;

    public List<GameObject> UserCards;

    public TMP_Text MassegeText;

    public void Initi(uint ID)
    {
        UserID = ID;
        UId.text = "ID: " + ID.ToString();

        //StartCoroutine(m_network.GetAvatar(UserID, sucsessed => { Avatar.sprite = Sprite.Create(sucsessed, new Rect(0, 0, sucsessed.width, sucsessed.height), Vector2.one / 2.0f); }, fail => { Debug.Log(fail); }));
    }

    public void PrintMessage(string massege)
    {
        MassegeText.text = massege;
    }

    public IEnumerator MoveTo(Vector2 MoveToPoint)
    {
        yield return moveTo(MoveToPoint);
    }
    private bool moveTo(Vector2 MoveToPoint)
    {
        LeanTween.moveLocal(gameObject, MoveToPoint, 2);
        LeanTween.scale(gameObject, new Vector3(1, 1, 1), 2);

        return true;
    }
}
