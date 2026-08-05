using UnityEngine;
using Google.Protobuf.Protocol;

public interface IGameEvent { }

public readonly struct CreateAccountSuccessedEvent : IGameEvent { }


public readonly struct CreateAccountFailedEvent : IGameEvent 
{
    public readonly string Message;

    public CreateAccountFailedEvent(string message)
    {
        Message = message;
    }
}

public readonly struct LoginSuccessedEvent : IGameEvent { }

public readonly struct LoginFailedEvent : IGameEvent 
{ 
    public readonly string Message;
    public readonly uint Result;

    public LoginFailedEvent(string message, uint result)
    {
        Message = message;
        Result = result;
    }
}

public readonly struct CreateCharacterSuccessedEvent : IGameEvent 
{
    public readonly Google.Protobuf.Protocol.Character Character;
    public CreateCharacterSuccessedEvent(Google.Protobuf.Protocol.Character character)
    {
        Character = character;
    }
}

public readonly struct CreateCharacterFailedEvent : IGameEvent
{
    public readonly string Message;
    public CreateCharacterFailedEvent(string message)
    {
        Message = message;
    }
}

public readonly struct CharacterListReceivedEvent : IGameEvent
{
    public readonly S2C_CharacterList CharacterList;
    public CharacterListReceivedEvent(S2C_CharacterList characterList)
    {
        CharacterList = characterList;
    }
}
