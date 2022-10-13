using System;
public class Logic
{
    public void Enter(NetworkManager _socket, Shared.StoC.Enter _packet)
    {
        ChattingManager.Instance.AddChattingLog($"{_packet.NickName}���� �����߽��ϴ�.");

    }

    public void Chat(NetworkManager _socket, Shared.StoC.Chat _packet)
    {
        ChattingManager.Instance.AddChattingLog($"{_packet.NickName}: {_packet.Message}");
    }

    public void Leave(NetworkManager _socket, Shared.StoC.Leave _packet)
    {
        ChattingManager.Instance.AddChattingLog($"{_packet.NickName}���� �����߽��ϴ�.");
    }
}