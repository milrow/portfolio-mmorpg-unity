using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public abstract class Session
{
    protected Socket _socket;
    int _disconnected = 0;
    

    public abstract void OnRecv(ArraySegment<byte> buffer);
    public abstract void OnConnected(EndPoint endPoint);
    public abstract void OnDisconnected(EndPoint endPoint);

    public void Start(Socket socket)
    {
        _socket = socket;
        
    }

    public void Send(ArraySegment<byte> sendBuff)
    {
        
    }

    private void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
    {
        if(args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
        {
            OnRecv(new ArraySegment<byte>(args.Buffer, args.Offset, args.BytesTransferred));
        
            
        }
    }
}
