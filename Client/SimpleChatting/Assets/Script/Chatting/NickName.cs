using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NickName : MonoBehaviour
{
    private InputField nickName_InputField;
    private Button ok_Btn;

    // Start is called before the first frame update
    void Start()
    {
        BindObject();
        BindButtonEvent();
    }

    void BindObject()
    {
        nickName_InputField = transform.GetComponentInChildren<InputField>();
        if(nickName_InputField == null)
        {
            Debug.LogError("nickName_InputField is null");
        }
        ok_Btn = transform.GetComponentInChildren<Button>();
        if(ok_Btn == null)
        {
            Debug.LogError("ok_Btn is null");
        }
    }

    void BindButtonEvent()
    {
        if(ok_Btn != null)
        {
            ok_Btn.onClick.AddListener(OnClickOKButton);
        }
    }

    void OnClickOKButton()
    {
        string nickName = nickName_InputField.text;

        if(string.IsNullOrEmpty(nickName) == true)
        {
            return;
        }

        ChattingManager.Instance.ReadyEnterChat();
        // 서버로 데이터 송신
        Shared.CtoS.Enter enterPacket = new Shared.CtoS.Enter();
        enterPacket.NickName = nickName;
        NetworkManager.Instance.Send(enterPacket);
 
    }

}
