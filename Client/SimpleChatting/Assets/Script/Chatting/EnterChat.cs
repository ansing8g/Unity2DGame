using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnterChat : MonoBehaviour
{
    private InputField inputField_NickName;
    private Button btn_OK;

    // Start is called before the first frame update
    void Start()
    {
        BindObject();
        BindButtonEvent();
    }

    void BindObject()
    {
        inputField_NickName = transform.GetComponentInChildren<InputField>();
        if(inputField_NickName == null)
        {
            Debug.LogError("nickName_InputField is null");
        }
        btn_OK = transform.GetComponentInChildren<Button>();
        if(btn_OK == null)
        {
            Debug.LogError("ok_Btn is null");
        }
    }

    void BindButtonEvent()
    {
        if(btn_OK != null)
        {
            btn_OK.onClick.AddListener(OnClickOKButton);
        }
    }

    void OnClickOKButton()
    {
        string chatting = inputField_NickName.text;

        if(string.IsNullOrEmpty(chatting) == true)
        {
            return;
        }

        // 서버로 데이터 송신
        Shared.CtoS.Chat enterPacket = new Shared.CtoS.Chat();
        enterPacket.Message = chatting;
        NetworkManager.Instance.Send(enterPacket);
        inputField_NickName.text = "";
    }
}
