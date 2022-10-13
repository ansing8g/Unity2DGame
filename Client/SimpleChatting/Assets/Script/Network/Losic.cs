using System;
public class Logic
{
    public void Enter(NetworkManager _socket, Shared.StoC.Enter _packet)
    {
        ChattingManager.Instance.AddChattingLog($"{_packet.NickName}님이 입장했습니다.");

    }

    public void Chat(NetworkManager _socket, Shared.StoC.Chat _packet)
    {
        ChattingManager.Instance.AddChattingLog($"{_packet.NickName}: {_packet.Message}");
    }

    public void Leave(NetworkManager _socket, Shared.StoC.Leave _packet)
    {
        ChattingManager.Instance.AddChattingLog($"{_packet.NickName}님이 퇴장했습니다.");
    }
}