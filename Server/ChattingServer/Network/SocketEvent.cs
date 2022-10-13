
namespace Network
{
    public enum SocketErrorType
    {
        Accept,
        Connect,
        Disconnect,
        Send,
        Receive,
    }

    public interface ServerSocketEvent
    {
        public void OnError(SocketErrorType _error_type, System.Exception _exception, SessionSocket? _sessionsocket);

        public void OnAccept(SessionSocket _sessionsocket);
        public void OnDisconnect(SessionSocket _sessionsocket);
        public void OnSend(SessionSocket _sessionsocket);
        public void OnReceive(SessionSocket _sessionsocket, byte[] _data);
    }
}
