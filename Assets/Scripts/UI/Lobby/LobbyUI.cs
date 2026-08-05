using UnityEngine;
using UnityEngine.UI;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine.SceneManagement;
using ProtocolCharacter = Google.Protobuf.Protocol.Character;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button GameStartBtn;
    [SerializeField] private TMP_InputField NameInput;
    [SerializeField] private GameObject CharacterSelectPanel;

    private CharacterSlot[] characterSlots;
    private CharacterSlot selectedCharacterSlot = null;
    private uint selectedCharacterIndex = 0;

    private void Awake()
    {
        characterSlots = CharacterSelectPanel.GetComponentsInChildren<CharacterSlot>();
        foreach (var slot in characterSlots)
        {

            slot.OnClickSlotBtn += HandleSlotClicked;
        }
    }

    private void Start()
    {
        //characterSlots = CharacterSelectPanel.GetComponentsInChildren<CharacterSlot>();
        //Debug.Log("Character Slots Count: " + characterSlots.Length);

        GameStartBtn.onClick.AddListener(OnClickStartGame);
    }


    private void OnEnable()
    {
        GameEventBus.Subscribe<CharacterListReceivedEvent>(OnCharacterListRecieved);
        GameEventBus.Subscribe<CreateCharacterSuccessedEvent>(OnCreateCharacterSuccessed);
        GameEventBus.Subscribe<CreateCharacterFailedEvent>(OnCreateCharacterFailed);
    }

    private void OnDisable()
    {
        GameEventBus.Unsubscribe<CharacterListReceivedEvent>(OnCharacterListRecieved);
        GameEventBus.Unsubscribe<CreateCharacterSuccessedEvent>(OnCreateCharacterSuccessed);
        GameEventBus.Unsubscribe<CreateCharacterFailedEvent>(OnCreateCharacterFailed);
    }

    private void OnCreateCharacterSuccessed(CreateCharacterSuccessedEvent e)
    {
        ProtocolCharacter character = e.Character;
        if (character == null)
        {
            Debug.LogWarning("Create character succeeded, but the response did not include character data.");
            return;
        }

        CharacterSlot slot = FindCharacterSlot(character.Slot);
        if (slot == null)
        {
            Debug.LogWarning($"Create character succeeded for slot {character.Slot}, but no matching UI slot was found.");
            return;
        }

        SystemNotificationService.Instance.Show(
            "캐릭터 생성 성공!",
            NotificationType.ALERT
            );

        slot.SetCharacterInfo(ToCharacterInfo(character));
    }

    private void OnCreateCharacterFailed(CreateCharacterFailedEvent e)
    {
        SystemNotificationService.Instance.Show(
            "캐릭터 생성 실패: " + e.Message,
            NotificationType.ERROR
            );
    }

    public void CreateCharacter()
    {
        if (selectedCharacterSlot == null)
        {
            SystemNotificationService.Instance.Show(
                "캐릭터를 생성할 슬롯을 선택해주세요.",
                NotificationType.WARNING
                );
            return;
        }

        C2S_CreateCharacter packet = new C2S_CreateCharacter();
        //packet.AccountId = DataManager.Instance.MyUser.AccountID;
        packet.SlotId = selectedCharacterSlot.slotIndex;
        packet.Name = NameInput.text;
        NetworkManager.Instance.Send(packet, ProtocolID.IdC2SCreateCharacter);

    }

    private void OnCharacterListRecieved(CharacterListReceivedEvent e)
    {
        if (e.CharacterList == null)
        {
            Debug.LogWarning("Character list event did not include a character list.");
            return;
        }

        for (int i = 0; i < characterSlots.Length; i++)
        {
            characterSlots[i].ClearCharacterInfo();
        }

        foreach (ProtocolCharacter character in e.CharacterList.Characters)
        {
            CharacterSlot slot = FindCharacterSlot(character.Slot);
            if (slot == null)
            {
                Debug.LogWarning($"Character list included slot {character.Slot}, but no matching UI slot was found.");
                continue;
            }

            slot.SetCharacterInfo(ToCharacterInfo(character));
        }
    }

    private void OnClickStartGame()
    {
        if (selectedCharacterSlot == null || selectedCharacterSlot.GetCharacterInfo() == null)
        {
            SystemNotificationService.Instance.Show(
                "시작할 캐릭터를 선택해주세요.",
                NotificationType.WARNING
                );
            return;
        }

        //SceneManager.LoadScene("MainScene");
        C2S_EnterWorld packet = new C2S_EnterWorld();
        packet.CharacterId = (uint)selectedCharacterSlot.GetCharacterInfo().CharacterID;
        NetworkManager.Instance.Send(packet, ProtocolID.IdC2SEnterWorld);
    }

    private void HandleSlotClicked(CharacterSlot slot)
    {
        if (selectedCharacterSlot != null)
            selectedCharacterSlot.SetSelected(false);

        selectedCharacterSlot = slot;
        selectedCharacterIndex = slot.slotIndex;

        selectedCharacterSlot.SetSelected(true);

        Debug.Log($"Selected Character Slot Index: {selectedCharacterIndex}");
    }

    private CharacterSlot FindCharacterSlot(uint slotIndex)
    {
        foreach (CharacterSlot slot in characterSlots)
        {
            if (slot.slotIndex == slotIndex)
                return slot;
        }

        return null;
    }

    private CharacterInfo ToCharacterInfo(ProtocolCharacter character)
    {
        return new CharacterInfo
        {
            CharacterID = character.Id,
            NickName = character.Name,
            Slot = character.Slot,
            CreatedAt = character.CreatedAt?.ToDateTime() ?? System.DateTime.MinValue
        };
    }
}