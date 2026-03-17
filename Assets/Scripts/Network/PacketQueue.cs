using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class PacketMessage
{
    public ushort Id;
    public byte[] Payload;
}

public class PacketQueue
{
    static PacketQueue _instance = new PacketQueue();
    static public PacketQueue Instance => _instance ?? (_instance = new PacketQueue());

    private Queue<PacketMessage> _queue = new Queue<PacketMessage>();
    private Object _lock = new Object();

    public void Push(ushort id, byte[] payload)
    {
        lock (_lock)
        {
            //Debug.LogFormat("protocolID: {0}, payload: {1}", id, payload);
            PacketMessage message = new PacketMessage();
            message.Id = id;
            message.Payload = payload;
            _queue.Enqueue(message);
        }
    }

    public PacketMessage Pop()
    {
        lock (_lock) { 
            if(_queue.Count > 0)
            {
                return _queue.Dequeue();
            }
        }
        return null;
    }
}
