using System;
using UnityEngine;


[System.Serializable]
public class CharacterInfo
{
    public uint CharacterID;
    public string NickName;
    public uint Slot;
    public DateTime CreatedAt;
}

[System.Serializable]
public class WorldCharacter
{
    public uint ObjectID;
    public uint CharacterID;
    public string NickName;
    public Vector3 Position;
    public float Yaw;
}