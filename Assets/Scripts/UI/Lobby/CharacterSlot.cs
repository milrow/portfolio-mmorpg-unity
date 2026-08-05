using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CharacterSlot : MonoBehaviour
{
    [SerializeField] Button slotBtn;
    [SerializeField] Button createCharacterBtn;
    [SerializeField] public uint slotIndex;
    [SerializeField] public TMP_Text slotIdxText;
    [SerializeField] TMP_Text nameText = null;
    [SerializeField] TMP_Text createdAtText = null;

    private bool selected = false;
    private CharacterInfo characterInfo;

    public event Action<CharacterSlot> OnClickSlotBtn;

    void Start()
    {
        createCharacterBtn.onClick.AddListener(OnClickCreateCharacter);
        slotBtn.onClick.AddListener(OnClickSlot);
    }

    

    public void SetSelected(bool isSelected)
    {
        selected = isSelected;
        // Update the visual state of the slot based on selection
        // For example, change the background color or highlight the slot
    }

    public CharacterInfo GetCharacterInfo()
    {
        return characterInfo;
    }
    public void SetCharacterInfo(CharacterInfo info)
    {
        characterInfo = info;
        if (info == null)
        {
            ClearCharacterInfo();
            return;
        }

        nameText.text = info.NickName;
        createdAtText.text = info.CreatedAt == DateTime.MinValue ? "" : info.CreatedAt.ToString("yyyy-MM-dd HH:mm");
        createCharacterBtn.gameObject.SetActive(false);
        //slotIdxText.text =  "Slot" + info.Slot.ToString();
    }


    public void ClearCharacterInfo()
    {
        characterInfo = null;
        nameText.text = "";
        createdAtText.text = "";
        createCharacterBtn.gameObject.SetActive(true);
    }

    public void OnClickSlot()
    {
        OnClickSlotBtn?.Invoke(this);
    }

    private void OnClickCreateCharacter()
    {
        RequestCreateCharacter();
    }

    private void OnClickDeleteCharacter()
    {
        
    }

    private void RequestCreateCharacter()
    {
        C2S_CreateCharacter packet = new C2S_CreateCharacter();
        packet.SlotId = slotIndex;
        packet.Name = "NewCharacter"; // You can modify this to get the name from user input if needed
        
        NetworkManager.Instance.Send(packet, ProtocolID.IdC2SCreateCharacter);
    }
}
