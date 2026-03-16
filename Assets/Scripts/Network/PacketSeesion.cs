using System;
using System.Net;
using Google.Protobuf;

public abstract class PacketSession : Session
{
    public static readonly ushort HeaderSize = 4;

    public sealed override void OnRecv(ArraySegment<byte> buffer)
    {
        int processLen = 0;

        while (true)
        {
            if (buffer.Count < HeaderSize) break;

            ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            if ((buffer.Count < dataSize)) break;

            OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));

            processLen += dataSize;

            buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
                
            
        }

        throw new NotImplementedException();
    }

    public abstract void OnRecvPacket(ArraySegment<byte> buffer);

    public sealed override void OnConnected(EndPoint endPoint)
    {
        throw new NotImplementedException();
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        throw new NotImplementedException();
    }
}
