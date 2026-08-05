using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Login_UI : MonoBehaviour
{
    
    [SerializeField] private Button LoginBtn;
    [SerializeField] private Button CreateAccountBtn;
    [SerializeField] private TMP_InputField ID_Field;
    [SerializeField] private TMP_InputField PW_Field;

    [SerializeField] private GameObject CreateAccountPanel;
    [SerializeField] private TMP_InputField CA_ID_Field;
    [SerializeField] private TMP_InputField CA_PW_Field;
    [SerializeField] private TMP_InputField CA_Comfirm_PW_Field;
    [SerializeField] private Button SignupBtn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeButtons();

        
    }

   void InitializeButtons()
    {
        LoginBtn.onClick.AddListener(() =>
        {
            string id = ID_Field.text;
            string pw = PW_Field.text;

            C2S_Login packet = new C2S_Login();
            packet.LoginId = id;
            packet.Password = pw;

            NetworkManager.Instance.Send(packet, ProtocolID.IdC2SLogin);
        });

        CreateAccountBtn.onClick.AddListener(() => { CreateAccountPanel.SetActive(true); });

        SignupBtn.onClick.AddListener(() => {
        string id = CA_ID_Field.text;
        string pw = CA_PW_Field.text;
        string c_pw = CA_Comfirm_PW_Field.text;

            if (pw != c_pw)
            {
                SystemNotificationService.Instance.Show("비밀번호가 일치 하지 않습니다.", NotificationType.ERROR);
            }

            C2S_CreateAccount packet = new C2S_CreateAccount();
            packet.LoginId = id;
            packet.Password = pw;

            NetworkManager.Instance.Send(packet, ProtocolID.IdC2SCreateAccount);
        });
    }

    private void OnEnable()
    {
        GameEventBus.Subscribe<CreateAccountSuccessedEvent>(OnCreateAccountSuccessed);
        GameEventBus.Subscribe<CreateAccountFailedEvent>(OnCreateAccountFailed);
        GameEventBus.Subscribe<LoginSuccessedEvent>(OnLoginSuccessed);
        GameEventBus.Subscribe<LoginFailedEvent>(OnLoginFailed);
    }

    private void OnDisable()
    {
        GameEventBus.Unsubscribe<CreateAccountSuccessedEvent>(OnCreateAccountSuccessed);
        GameEventBus.Unsubscribe<CreateAccountFailedEvent>(OnCreateAccountFailed);
        GameEventBus.Unsubscribe<LoginSuccessedEvent>(OnLoginSuccessed);
        GameEventBus.Unsubscribe<LoginFailedEvent>(OnLoginFailed);
    }

    private void OnCreateAccountSuccessed(CreateAccountSuccessedEvent e)
    {
        SystemNotificationService.Instance.Show(
            "계정이 생성되었습니다.",
            NotificationType.ALERT,
            () => CreateAccountPanel.SetActive(false)
            );
    }

    private void OnCreateAccountFailed(CreateAccountFailedEvent e)
    {
        SystemNotificationService.Instance.Show(
            e.Message,
            NotificationType.ALERT
            );
    }

    private void OnLoginSuccessed(LoginSuccessedEvent e)
    {
        SystemNotificationService.Instance.Show(
            "로그인에 성공했습니다.",
            NotificationType.ALERT
            );
    }

    private void OnLoginFailed(LoginFailedEvent e)
    {
        string message = string.Empty;

        switch (e.Result)
        {
            case 1:
                message = "아이디가 존재하지 않습니다.";
                break;
            case 2:
                message = "비밀번호가 일치하지 않습니다.";
                break;
            default:
                message = "알 수 없는 오류가 발생했습니다.";
                break;
        }

        SystemNotificationService.Instance.Show(
            message,
            NotificationType.ALERT
            );
    }
}