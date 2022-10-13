
namespace ChattingServer
{
    public class Logic
    {
        public void Enter(ClientSocket _socket, Shared.CtoS.Enter _packet)
        {
            _socket.NickName = _packet.NickName;

            TestServer.Instance.SendAllClient(new Shared.StoC.Enter() { NickName = _packet.NickName });
        }

        public void Chat(ClientSocket _socket, Shared.CtoS.Chat _packet)
        {
            TestServer.Instance.SendAllClient(new Shared.StoC.Chat() { NickName = _socket.NickName, Message = _packet.Message });
        }
    }
}
