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

    public interface ClientSocketEvent
    {
        public void OnError(SocketErrorType _error_type, System.Exception _exception, ConnectSocket? _connectsocket);
        public void OnConnect(ConnectSocket _connectsocket);
        public void OnDisconnect(ConnectSocket _connectsocket);
        public void OnSend(ConnectSocket _connectsocket);
        public void OnReceive(ConnectSocket _connectsocket, byte[] _data);
    }
}