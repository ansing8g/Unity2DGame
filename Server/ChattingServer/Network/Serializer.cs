﻿using System;
using System.Text;

using Newtonsoft.Json;

namespace Network
{
    public interface ISerializer
    {
        public bool ToByte<PacketIndex>(PacketBase<PacketIndex> _packet_object, out byte[]? _packet_data);
        public bool ToPacketBase<PacketIndex>(byte[] _packet_data, out PacketBase<PacketIndex>? _packet);
        public bool ToPacket<PacketIndex>(byte[] _packet_data, Type _packet_type, out PacketBase<PacketIndex>? _packet);
    }

    public class JsonSerializer : ISerializer
    {
        public bool ToByte<PacketIndex>(PacketBase<PacketIndex> _packet_object, out byte[]? _packet_data)
        {
            try
            {
                string json_data = JsonConvert.SerializeObject(_packet_object);
                _packet_data = Encoding.UTF8.GetBytes(json_data);
                if(_packet_data == null)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                _packet_data = null;
                return false;
            }

            return true;
        }

        public bool ToPacketBase<PacketIndex>(byte[] _packet_data, out PacketBase<PacketIndex>? _packet)
        {
            _packet = null;

            try
            {
                string json_data = Encoding.UTF8.GetString(_packet_data);
                _packet = JsonConvert.DeserializeObject<PacketBase<PacketIndex>>(json_data);
                if(_packet == null)
                {
                    return false;
                }
            }
            catch(Exception)
            {
                _packet = null;
                return false;
            }

            return true;
        }

        public bool ToPacket<PacketIndex>(byte[] _packet_data, Type _packet_type, out PacketBase<PacketIndex>? _packet)
        {
            _packet = null;

            try
            {
                string json_data = Encoding.UTF8.GetString(_packet_data);
                object? packet_obj = JsonConvert.DeserializeObject(json_data, _packet_type);
                if(packet_obj == null ||
                    packet_obj is PacketBase<PacketIndex> == false)
                {
                    return false;
                }

                _packet = packet_obj as PacketBase<PacketIndex>;
            }
            catch(Exception)
            {
                _packet = default;
                return false;
            }

            return true;
        }
    }
}
