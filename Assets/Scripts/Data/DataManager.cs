using UnityEngine;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    
    public UserInfo MyUser { get; set; }
    public WorldCharacter MyCharacter { get; set; }
    public List<CharacterInfo> CharacterList { get; set; } = new List<CharacterInfo>();

    public void SetCharacterList(List<CharacterInfo> characterList)
    {
        CharacterList = characterList;
    }

    
}
