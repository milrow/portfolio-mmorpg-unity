using UnityEngine;
using System;
using System.Net.Sockets;
using Google.Protobuf;
using UnityEditor.Networking.PlayerConnection;
using System.Collections;
using Google.Protobuf.Protocol;


public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    public uint accountId;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private Socket _socket;
    private RecvBuffer _recvBuffer = new RecvBuffer(4096);


    private void Start()
    {
        ConnectToServer();
    }

    private void Update()
    {
        while (true)
        {
            PacketMessage packet = PacketQueue.Instance.Pop();
            if (packet == null) break;

            HandlePacket(packet.Id, packet.Payload);
        }
    }

    void HandlePacket(ProtocolID id, byte[] payload)
    {
        try
        {
            switch (id)
            {
                case ProtocolID.IdS2CCreateAccount:
                    {
                        S2C_CreateAccount pkt = S2C_CreateAccount.Parser.ParseFrom(payload);
                        PacketHandler.Handle_S2C_CreateAccount(pkt);
                    }
                    break;
                case ProtocolID.IdS2CBroadcastMove:
                    {
                        S2C_BroadcastMove pkt = S2C_BroadcastMove.Parser.ParseFrom(payload);
                        PacketHandler.Handle_S2C_BroadcastMove(pkt);
                    }
                    break;
                case ProtocolID.IdS2CLogin:
                    {
                        S2C_Login pkt = S2C_Login.Parser.ParseFrom(payload);
                        PacketHandler.Handle_S2C_Login(pkt);
                    }
                    break;
                case ProtocolID.IdS2CBroadcastJump:
                    {
                        S2C_BroadcastJump pkt = S2C_BroadcastJump.Parser.ParseFrom(payload);
                        PacketHandler.Handle_S2C_BroadcastJump(pkt);
                    }
                    break;
                case ProtocolID.IdS2CLeaveGame:
                    {
                        S2C_LeaveGame pkt = S2C_LeaveGame.Parser.ParseFrom(payload);
                        PacketHandler.Handle_S2C_LeaveGame(pkt);
                    }
                    break;
                case ProtocolID.IdS2CCharacterList:
                    {
                        S2C_CharacterList pkt = S2C_CharacterList.Parser.ParseFrom(payload);
                        PacketHandler.Handle_S2C_CharacterList(pkt);
                    }
                    break;
                case ProtocolID.IdS2CCreateCharacter:
                    {
                        S2C_CreateCharacter pkt = S2C_CreateCharacter.Parser.ParseFrom(payload);
                        PacketHandler.Handle_S2C_CreateCharacter(pkt);
                    }
                    break;
                    case ProtocolID.IdS2CEnterWorld:
                    {
                        S2C_EnterWorld pkt = S2C_EnterWorld.Parser.ParseFrom(payload);
                        PacketHandler.Handle_S2C_EnterWorld(pkt);
                    }
                    break;
                default:
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }


    }

    #region Network Recieving
    void ConnectToServer()
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _socket.BeginConnect("127.0.0.1", 9000, OnConnect, null);
    }

    private void OnConnect(IAsyncResult result)
    {
        _socket.EndConnect(result);
        Debug.Log("Conneted");

        RegistRecieve();
    }

    private void RegistRecieve()
    {
        _socket.BeginReceive(
            _recvBuffer.writeSegment.Array,
            _recvBuffer.writeSegment.Offset,
            _recvBuffer.FreeSize,
            SocketFlags.None,
            OnRecieve,
            null
            );
    }

    void OnRecieve(IAsyncResult result)
    {
        try
        {
            int byteRead = _socket.EndReceive(result);
            if (byteRead <= 0) return;

            if (_recvBuffer.OnWrite(byteRead) == false) return;

            while (true)
            {
                if (_recvBuffer.DataSize < 4) break;

                var segment = _recvBuffer.readSegment;
                int offset = segment.Offset;
                byte[] buffer = segment.Array;

                ushort packetSize = BitConverter.ToUInt16(buffer, offset);
                ushort protocolId = BitConverter.ToUInt16(buffer, offset + 2);

                if (_recvBuffer.DataSize < packetSize) break;

                byte[] payload = new byte[packetSize - 4];
                Array.Copy(_recvBuffer.readSegment.Array, offset + 4, payload, 0, packetSize - 4);

                PacketQueue.Instance.Push(protocolId, payload);
                //Debug.Log($"protocolId: {protocolId}, size: {packetSize}, offset: { offset}");

                //Debug.Log($"[before Read] Previous DataSize: {_recvBuffer.DataSize}");
                _recvBuffer.OnRead(packetSize);
                //Debug.Log($"[After Read] Remaining DataSize: {_recvBuffer.DataSize}");
            }
            _recvBuffer.Clean();
            RegistRecieve();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public void Send(IMessage packet, ProtocolID _protocolId)
    {
        ushort protocolId = (ushort)_protocolId;
        ushort dataSize = (ushort)packet.CalculateSize();
        ushort packetSize = (ushort)(dataSize + 4);

        byte[] sendBuffer = new byte[packetSize];

        Array.Copy(BitConverter.GetBytes(packetSize), 0, sendBuffer, 0, 2);
        Array.Copy(BitConverter.GetBytes(protocolId), 0, sendBuffer, 2, 2);

        packet.WriteTo(new ArraySegment<byte>(sendBuffer, 4, dataSize));
        //Debug.Log(_socket);
        //Debug.Log(sendBuffer);
        _socket.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, OnSend, _socket);
    }

    void OnSend(IAsyncResult result)
    {
        try
        {
            Socket socket = (Socket)result.AsyncState;
            int bytesSent = socket.EndSend(result);
            //Debug.Log($"서버로 {bytesSent} 바이트 전송 완료");
        }
        catch (Exception ex)
        {
            Debug.Log($"OnSend error {ex.Message}");
            throw;
        }
    }
    #endregion

    private void OnApplicationQuit()
    {
        Disconnect();
    }

    private void Disconnect()
    {
        C2S_LeaveGame packet = new C2S_LeaveGame();
        packet.SessionId = DataManager.Instance.MyUser.SessionID;
        Instance.Send(packet, ProtocolID.IdC2SLeaveGame);
    }

    public bool isConnected = false;
}
