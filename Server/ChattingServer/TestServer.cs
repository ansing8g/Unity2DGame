using System;
using System.Collections.Generic;
using System.Threading;

using Network;
using Shared;

namespace ChattingServer
{
    public class TestServer : ServerSocketEvent
    {
        private AcceptSocket m_acceptsocket;
        private ISerializer m_serializer;
        private Dispatcher<ClientSocket, Define.PacketIndex> m_dispatcher;

        private LinkedList<ClientSocket> m_llsocket;
        private ReaderWriterLockSlim m_rwlock;

        public static TestServer Instance
        {
            get
            {
                if(TestServer.m_instance == null)
                {
                    TestServer.m_instance = new TestServer();
                }

                return TestServer.m_instance;
            }
            private set
            {

            }
        }
        private static TestServer? m_instance = null;

        private TestServer()
            : base()
        {
            m_acceptsocket = new AcceptSocket(this);
            m_serializer = new JsonSerializer();
            m_dispatcher = new Dispatcher<ClientSocket, Define.PacketIndex>();

            m_llsocket = new LinkedList<ClientSocket>();
            m_rwlock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        public bool Start(int _port)
        {
            if(m_acceptsocket.Start(_port) == false)
            {
                return false;
            }

            m_dispatcher.Clear();
            m_dispatcher.RegistClass(new Logic());

            return true;
        }

        public void OnError(SocketErrorType _error_type, Exception _exception, SessionSocket? _sessionsocket)
        {
            Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}]OnError. ErrorType={_error_type.ToString()}, Msg={_exception.Message}, StackTrace={_exception.StackTrace}, Source={_exception.Source}");
        }

        public void OnAccept(SessionSocket _sessionsocket)
        {
            ClientSocket socket = new ClientSocket(_sessionsocket, m_serializer);
            _sessionsocket.StateObject = socket;

            m_rwlock.EnterWriteLock();
            socket.Node = m_llsocket.AddLast(socket);
            m_rwlock.ExitWriteLock();
        }

        public void OnDisconnect(SessionSocket _sessionsocket)
        {
            if(_sessionsocket.StateObject is ClientSocket == false)
            {
                return;
            }

            ClientSocket socket = (ClientSocket)_sessionsocket.StateObject;

            m_rwlock.EnterWriteLock();
            m_llsocket.Remove(socket);
            m_rwlock.ExitWriteLock();

            socket.Node = null;

            SendAllClient(new Shared.StoC.Leave() { NickName = socket.NickName });
        }

        public void OnSend(SessionSocket _sessionsocket)
        {

        }

        public void OnReceive(SessionSocket _sessionsocket, byte[] _data)
        {
            if (_sessionsocket.StateObject is ClientSocket == false)
            {
                return;
            }

            ClientSocket socket = (ClientSocket)_sessionsocket.StateObject;

            if (m_serializer.ToPacketBase(_data, out PacketBase<Define.PacketIndex>? packet_base) == false)
            {
                return;
            }

            if (packet_base == null)
            {
                return;
            }

            if (m_dispatcher.GetFunction(packet_base.Index, out FunctionBase<ClientSocket, Define.PacketIndex>? func_handler, out Type? packet_type) == false)
            {
                return;
            }

            if (func_handler == null ||
                packet_type == null)
            {
                return;
            }

            if(m_serializer.ToPacket(_data, packet_type, out PacketBase<Define.PacketIndex>? packet) == false)
            {
                return;
            }

            if(packet == null)
            {
                return;
            }

            func_handler.ExecuteFunction(socket, packet);
        }

        public void SendAllClient(PacketCommon _packet_object)
        {
            if (_packet_object == null)
            {
                return;
            }

            byte[]? byte_data = null;
            if (m_serializer.ToByte(_packet_object!, out byte_data) == false)
            {
                return;
            }

            if (byte_data == null)
            {
                return;
            }

            m_rwlock.EnterReadLock();
            foreach (ClientSocket socket in m_llsocket)
            {
                socket.Send(byte_data);
            }
            m_rwlock.ExitReadLock();
        }
    }

    public class ClientSocket
    {
        private SessionSocket m_socket;
        private ISerializer m_serializer;
        public string NickName { get; set; }
        public LinkedListNode<ClientSocket>? Node { get; set; }

        public ClientSocket(SessionSocket _sessionsocket, ISerializer serializer)
        {
            m_socket = _sessionsocket;
            m_serializer = serializer;

            NickName = "";
            Node = null;
        }

        public bool Send(PacketCommon _packet_object)
        {
            if (_packet_object == null)
            {
                return false;
            }

            byte[]? byte_data = null;
            if (m_serializer.ToByte(_packet_object!, out byte_data) == false)
            {
                return false;
            }

            if (byte_data == null)
            {
                return false;
            }

            return m_socket.Send(byte_data);
        }

        public bool Send(byte[] _byte_data)
        {
            return m_socket.Send(_byte_data);
        }

       
    }
}
