using System;
using System.Net;
using System.Net.Sockets;

namespace Network
{
    public class AcceptSocket
    {
        private Socket? m_acceptsocket;
        private ServerSocketEvent m_event;
        private int m_port;
        private int m_listen_count;
        private uint m_bufsize;
        private uint m_total_bufsize;
        public AcceptSocket(ServerSocketEvent _event)
        {
            m_acceptsocket = null;
            m_event = _event;

            m_port = 0;
            m_listen_count = 0;
            m_bufsize = 0;
        }

        public bool Start(int _port, int _listen_count = 1000, uint _bufsize = 1024, uint _total_bufsize = 10240)
        {
            Disconnect();

            try
            {
                m_port = _port;
                m_listen_count = _listen_count;
                m_bufsize = _bufsize;
                m_total_bufsize = _total_bufsize;

                IPEndPoint end_point = new IPEndPoint(IPAddress.Any, _port);
                m_acceptsocket = new Socket(end_point.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                m_acceptsocket.Bind(end_point);
                m_acceptsocket.Listen(_listen_count);
                m_acceptsocket.BeginAccept(AcceptCallback, null);
            }
            catch (Exception e)
            {
                if (m_event != null)
                {
                    m_event.OnError(SocketErrorType.Accept, e, null);
                }

                return false;
            }

            return true;
        }

        private void Disconnect()
        {
            if (m_acceptsocket == null)
            {
                return;
            }

            try
            {
                m_acceptsocket.Close();
                m_acceptsocket.Dispose();
                m_acceptsocket = null;
            }
            catch (Exception e)
            {
                if (m_event != null)
                {
                    m_event.OnError(SocketErrorType.Disconnect, e, null);
                }
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            Socket? socket = null;
            try
            {
                if (m_acceptsocket == null)
                {
                    return;
                }

                socket = m_acceptsocket.EndAccept(ar);
            }
            catch (Exception e)
            {
                if (m_event != null)
                {
                    m_event.OnError(SocketErrorType.Accept, e, null);
                }

                if (null != socket)
                {
                    socket.Disconnect(false);
                    socket.Close();
                }
            }
            finally
            {
                m_acceptsocket!.BeginAccept(AcceptCallback, null);
            }

            SessionSocket? sessionsocket = null;
            try
            {
                sessionsocket = new SessionSocket(socket, m_event, m_bufsize, m_total_bufsize);

                if (m_event != null)
                {
                    m_event.OnAccept(sessionsocket);
                }

                if (false == sessionsocket.Receive())
                {
                    Disconnect();
                }
            }
            catch (Exception e)
            {
                if (m_event != null)
                {
                    m_event.OnError(SocketErrorType.Accept, e, null);
                }

                if (sessionsocket != null)
                {
                    sessionsocket.Disconnect();
                }
            }
        }
    }
}
