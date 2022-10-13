using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;
using Shared;

public class NetworkManager : MonoSingleton<NetworkManager>, ClientSocketEvent
{
    private Queue<byte[]> m_receiveQueue;
    private ConnectSocket m_socket;
    private ISerializer m_serializer;
    private Dispatcher<NetworkManager, Define.PacketIndex> m_dispatcher;

    void Start()
    {
        m_receiveQueue = new Queue<byte[]>();
        m_socket = new ConnectSocket(this);
        m_serializer = new JsonSerializer();
        m_dispatcher = new Dispatcher<NetworkManager, Define.PacketIndex>();
        StartCoroutine(CR_CheckReceiveQueue());
        if (ConnectToServer() == false)
        {
            Debug.LogError("Error ConnectToServer");
        }
    }

    private bool ConnectToServer()
    {
        if (m_socket.Connect("127.0.0.1", 12121 ) == false)
        {
            return false;    
        }
        m_dispatcher.Clear();
        m_dispatcher.RegistClass(new Logic());

        return true;
    }

    IEnumerator CR_CheckReceiveQueue()
    {
        byte[] _data;
        while (true)
        {
            yield return null;
            if (m_receiveQueue.Count > 0)
            {

                lock (m_receiveQueue)
                {
                    _data = m_receiveQueue.Dequeue();
                }

                if (m_serializer.ToPacketBase(_data, out PacketBase<Define.PacketIndex>? packet_base) == false)
                {
                    continue;
                }

                else if (packet_base == null)
                {
                    continue;
                }

                else if (m_dispatcher.GetFunction(packet_base.Index, out FunctionBase<NetworkManager, Define.PacketIndex>? func_handler, out System.Type? packet_type) == false)
                {
                    continue;
                }

                else if (func_handler == null || 
                         packet_type == null)
                {
                    continue;
                }

                else if (m_serializer.ToPacket(_data, packet_type, out PacketBase<Define.PacketIndex>? packet) == false)
                {
                    continue;
                }

                else if (packet == null)
                {
                    continue;
                }

                else
                {
                    func_handler.ExecuteFunction(this, packet);
                }
            }
        }
        
    }

    #region NetworkThread
    public void OnError(SocketErrorType _error_type, System.Exception _exception, ConnectSocket? _connectsocket)
    {
        Debug.LogError($"NetworkManager-OnError : {_error_type}");
    }

    public void OnConnect(ConnectSocket _connectsocket)
    {
        Debug.Log($"NetworkManager-OnConnect");
    }
    public void OnDisconnect(ConnectSocket _connectsocket)
    {
        Debug.Log($"NetworkManager-OnDisconnect");
    }
    public void OnSend(ConnectSocket _connectsocket)
    {
        
    }
    public void OnReceive(ConnectSocket _connectsocket, byte[] _data)
    { 
        lock(m_receiveQueue)
        {
            m_receiveQueue.Enqueue(_data);
        }
    }
    #endregion

    public bool Send(PacketCommon _packet_object)
    {
        if (null == _packet_object)
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
}
