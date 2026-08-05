using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SystemNotificationView : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button comfirmBtn;

    private Action ComfirmAction;

    private void Awake()
    {
        comfirmBtn.onClick.AddListener(() =>
        {
            Hide();

            var action = ComfirmAction;
            ComfirmAction = null;
            action?.Invoke();
        });
    }

    public void Show(string message, Action task = null)
    {
        messageText.text = message;
        gameObject.SetActive(true);
        if(task != null) ComfirmAction = task;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
