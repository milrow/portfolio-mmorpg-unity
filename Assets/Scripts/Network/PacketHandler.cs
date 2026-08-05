using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using UnityEngine.SceneManagement;

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
        //Debug.Log(message);
        S2C_Login packet = (S2C_Login)message;
        if (packet != null)
        {
            if (packet.Result != 0)
            {
                Debug.Log("Login Failed! Result : " + packet.Result);
                GameEventBus.Publish(new LoginFailedEvent("Login Failed! Result : " + packet.Result, packet.Result));
                //todo: 로그인 실패에 따른 UI 처리

                return;
            }

            uint sessionId = packet.SessionId;
            uint accountId = packet.AccountId;

            DataManager.Instance.MyUser = new UserInfo { AccountID = accountId, SessionID = sessionId };
            Debug.LogFormat("Login Success! Result : {0}, AccountID : {1}, SessionID : {2}", packet.Result, accountId, sessionId);

            SceneManager.sceneLoaded += OnLobbySceneLoaded;

            C2S_CharacterList pk = new C2S_CharacterList();
            //pk.AccountId = DataManager.Instance.MyUser.AccountID;
            NetworkManager.Instance.Send(pk, ProtocolID.IdC2SCharacterList);

            SceneManager.LoadScene("LobbyScene");


        }
        else
        {
            Debug.Log("Packet error");
        }
    }

    private static void OnMainSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "MainScene")
        {
            return;
        }

        SceneManager.sceneLoaded -= OnMainSceneLoaded;

        uint objectId = DataManager.Instance.MyCharacter.ObjectID;

        Vector3 spawnPos = DataManager.Instance.MyCharacter.Position;
        GameObject user = ObjectManager.Instance.Spawn(objectId, spawnPos, true);
    }

    private static void OnLobbySceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "LobbyScene")
        {
            return;
        }
        SceneManager.sceneLoaded -= OnLobbySceneLoaded;
        // LobbyScene에서 필요한 초기화 작업 수행
        // 예: 캐릭터 목록 요청, UI 초기화 등

    }

    public static void Handle_S2C_BroadcastMove(IMessage message)
    {
        S2C_BroadcastMove packet = (S2C_BroadcastMove)message;
        if (packet != null)
        {
            //Debug.LogFormat("Packet SessionID : {0}", packet.SessionId);
            //Debug.LogFormat("MyUser SessionID : {0}", DataManager.Instance.MyUser.SessionID);
            uint objectId = packet.ObjectId;
            if (objectId == DataManager.Instance.MyCharacter.ObjectID)
            {
                return;
            }

            GameObject obj = ObjectManager.Instance.Fine(objectId);

            if (obj == null)
            {
                obj = ObjectManager.Instance.Spawn(objectId, new Vector3(packet.PosX, packet.PosY, packet.PosZ), false);
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
            uint objectId = packet.ObjectId;

            if (objectId == DataManager.Instance.MyCharacter.ObjectID)
            {
                return;
            }

            GameObject obj = ObjectManager.Instance.Fine(objectId);
            if (obj != null)
            {
                RemotePlayer remotePlayer = obj.GetComponent<RemotePlayer>();
                if (remotePlayer != null)
                {
                    remotePlayer.OnJumpEvent();
                }
            }
        }
    }

    public static void Handle_S2C_LeaveGame(IMessage message)
    {
        S2C_LeaveGame packet = (S2C_LeaveGame)message;
        if (packet != null)
        {
            uint sessionId = packet.SessionId;
            ObjectManager.Instance.DeSpawn(sessionId);
        }
    }


    public static void Handle_S2C_CreateAccount(IMessage message)
    {
        S2C_CreateAccount packet = (S2C_CreateAccount)message;
        if (packet != null)
        {
            uint result = packet.Result;

            Debug.Log(result);
            if (result == 1)
            {
                GameEventBus.Publish(new CreateAccountFailedEvent("아이디가 중복됩니다."));
                return;
            }

            GameEventBus.Publish(new CreateAccountSuccessedEvent());
            return;
        }

    }

    public static void Handle_S2C_CreateCharacter(IMessage message)
    {
        S2C_CreateCharacter packet = (S2C_CreateCharacter)message;
        if (packet != null)
        {
            uint result = packet.Result;

            if (result == 1)
            {
                GameEventBus.Publish(new CreateCharacterFailedEvent("캐릭터 이름이 중복됩니다."));
                return;
            }

            if (packet.Character == null)
            {
                GameEventBus.Publish(new CreateCharacterFailedEvent("서버 응답에 캐릭터 정보가 없습니다."));
                return;
            }

            GameEventBus.Publish(new CreateCharacterSuccessedEvent(packet.Character));
            return;
        }
    }

    public static void Handle_S2C_CharacterList(IMessage message)
    {
        S2C_CharacterList packet = (S2C_CharacterList)message;
        if (packet != null)
        {
            if (packet.Result != 0)
            {
                Debug.Log("캐릭터 목록 요청 실패. 재요청 합니다. result = " + packet.Result);
                C2S_CharacterList pk = new C2S_CharacterList();
                NetworkManager.Instance.Send(pk, ProtocolID.IdC2SCharacterList);
                return;
            }

            List<CharacterInfo> characterList = new List<CharacterInfo>();
            foreach (var character in packet.Characters)
            {
                CharacterInfo characterInfo = new CharacterInfo
                {
                    CharacterID = character.Id,
                    NickName = character.Name,
                    Slot = character.Slot,
                    CreatedAt = character.CreatedAt?.ToDateTime() ?? DateTime.MinValue
                };

                characterList.Add(characterInfo);

            }
            DataManager.Instance.SetCharacterList(characterList);
            GameEventBus.Publish(new CharacterListReceivedEvent(packet));

        }
    }

    public static void Handle_S2C_EnterWorld(IMessage message)
    {
        S2C_EnterWorld packet = (S2C_EnterWorld)message;
        if (packet != null)
        {
            DataManager.Instance.MyCharacter = new WorldCharacter
            {
                ObjectID = (uint)packet.ObjectId,
                CharacterID = packet.Character.Id,
                NickName = packet.Character.Name,
                Position = new Vector3
                {
                    x = packet.PosX,
                    y = packet.PosY,
                    z = packet.PosZ,
                },
                Yaw = packet.Yaw
            };
            SceneManager.sceneLoaded += OnMainSceneLoaded;
            Debug.Log(
                "EnterWorld Success! ObjectID : " + DataManager.Instance.MyCharacter.ObjectID 
                + ", CharacterID : " + DataManager.Instance.MyCharacter.CharacterID 
                + ", NickName : " + DataManager.Instance.MyCharacter.NickName);

            SceneManager.LoadScene("MainScene");
        }
    }
}