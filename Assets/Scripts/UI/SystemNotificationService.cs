using System;
using UnityEngine;

public enum NotificationType
{
    NONE = 0,
    ERROR,
    WARNING,
    ALERT,
}

public class SystemNotificationService : MonoBehaviour
{
    public static SystemNotificationService Instance { get; private set; }
    
    [SerializeField] private SystemNotificationView viewPrefab;
    private SystemNotificationView view;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        view = Instantiate(viewPrefab, transform);
        
        view.Hide();
    }

    public void Show(string message, NotificationType type, Action task = null)
    {
        switch (type)
        {
            case NotificationType.NONE:
                break;
            case NotificationType.ERROR:
                view.Show("Error: " + message, task);
                break;
            case NotificationType.WARNING:
                view.Show("Warning" + message, task);
                break;
            case NotificationType.ALERT:
                view.Show("Warning" + message, task);
                break;
        }
    }
}
