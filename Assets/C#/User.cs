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

    private ERole _role = ERole.thrower;
    public ERole role
    {
        set
        {
            Debug.Log("set role: " + value.ToString());
            _role = value;
            PrintMessage(value.ToString());
        }
        get
        {
            return _role;
        }
    }

    public List<GameObject> UserCards;

    public TMP_Text MassegeText;

    public EStatus status = EStatus.Null;

    public void Init(uint ID)
    {
        UserID = ID;
        UId.text = "BOT";

        if(ID != 0)
        {
            UId.text = "ID: " + ID.ToString();
            Avatar.UserID = ID;
            m_socketNetwork.getAvatar(ID);
        }
    }

    public void PrintMessage(string massege)
    {
        if(MassegeText != null) MassegeText.text = massege;
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
