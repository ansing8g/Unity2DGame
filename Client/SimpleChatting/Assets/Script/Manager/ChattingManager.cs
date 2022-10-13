using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChattingManager : MonoSingleton<ChattingManager>
{
    public static ChattingManager chattingManager;
    public NickName nickName;
    public EnterChat enterChat;
    public UnityEngine.UI.GridLayoutGroup chattingLog;
    public ChattingData chattingData;

    public void ReadyEnterChat()
    {
        nickName.gameObject.SetActive(false);
        enterChat.gameObject.SetActive(true);
        chattingLog.gameObject.SetActive(true);
    }

    public void AddChattingLog(string _chat)
    {
        ChattingData chatData = Instantiate(chattingData,chattingLog.transform);   

        chatData.SetText(_chat);
    }
}
