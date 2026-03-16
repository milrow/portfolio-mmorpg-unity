using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Google.Protobuf;
using Google.Protobuf.Protocol;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PacketHeader
{
    ushort size;
    ushort protocolId;
}

public class PacketHandler : MonoBehaviour
{
    public static void Handle_S2C_Login(IMessage message)
    {
        S2C_Login packet = (S2C_Login)message;
        if (packet != null)
        {
            uint sessionId = packet.SessionId;
            uint userId = packet.UserId;

            //테스트를 위한거긴 한데 일단 안전조건 하나
            if (NetworkManager.Instance.userId != userId)
            {
                return;
            }

            DataManager.Instance.MyUser = new UserInfo { UserID = userId, SessionID = sessionId };

            GameObject obj = ObjectManager.Instance.Fine(sessionId);
            if (obj == null)
            {
                Vector3 spawnPos = new Vector3(0, 5, 0);
                GameObject user = ObjectManager.Instance.Spawn(sessionId, spawnPos, true);

                //user.GetComponent<PlayerController>().SetCamera();
            }

            //Debug.Log($"SessionId: {packet.SessionId}");
        }
    }

    public static void Handle_S2C_BroadcastMove(IMessage message)
    {
        S2C_BroadcastMove packet = (S2C_BroadcastMove)message;
        if (packet != null)
        {
            //Debug.LogFormat("Packet SessionID : {0}", packet.SessionId);
            //Debug.LogFormat("MyUser SessionID : {0}", DataManager.Instance.MyUser.SessionID);
            uint sessionId = packet.SessionId;
            if (sessionId == DataManager.Instance.MyUser.SessionID)
            {
                return;
            }

            GameObject obj = ObjectManager.Instance.Fine(sessionId);

            if (obj == null)
            {
                obj = ObjectManager.Instance.Spawn(sessionId, new Vector3(packet.PosX, packet.PosY, packet.PosZ), false);
            }
            
                RemotePlayer remotePlayer = obj.GetComponent<RemotePlayer>();
                if (remotePlayer != null)
                {
                    remotePlayer.SetTargetPos(new Vector3(packet.PosX, packet.PosY, packet.PosZ));
                    //Debug.Log($"MoveLog : {packet.PosX} , {packet.PosY}, {packet.PosZ}");
                }


        }
    }

    public static void Handle_S2C_BroadcastJump(IMessage message)
    {
        S2C_BroadcastJump packet = (S2C_BroadcastJump)message;
        if (packet != null)
        {
            uint sessionId = packet.SessionId;

            if (sessionId == DataManager.Instance.MyUser.SessionID)
            {
                return;
            }

            GameObject obj = ObjectManager.Instance.Fine(sessionId);
            if(obj != null)
            {
                RemotePlayer remotePlayer = obj.GetComponent<RemotePlayer>();
                if(remotePlayer != null)
                {
                    remotePlayer.OnJumpEvent();
                }
            }
        }
    }

    public static void Handle_S2C_LeaveGame(IMessage message)
    {
        S2C_LeaveGame packet = (S2C_LeaveGame)message;
        if(packet != null)
        {
            uint sessionId = packet.SessionId;
            ObjectManager.Instance.DeSpawn(sessionId);
        }
    }

}
