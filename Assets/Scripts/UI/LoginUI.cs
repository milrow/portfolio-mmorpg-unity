using UnityEngine;
using UnityEngine.UIElements;
using Google.Protobuf.Protocol;
public class LoginUI : MonoBehaviour
{
    private TextField idField;
    private TextField pwField;
    private Button loginBtn;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        idField = root.Q<TextField>("IDInput");
        pwField = root.Q<TextField>("PWInput");
        loginBtn = root.Q<Button>("LoginBtn");

        loginBtn.clicked += OnLoginClicked;
    }

    void OnLoginClicked()
    {
        string id = idField.value;
        string pw = pwField.value;

        C2S_Login packet = new C2S_Login();
        packet.LoginId = id;
        packet.Password = pw;

        NetworkManager.Instance.Send(packet, ProtocolID.IdC2SLogin);
    }

    private void OnDisable()
    {
        if(loginBtn != null)
        {
            loginBtn.clicked -= OnLoginClicked;
        }
    }

    void OnSignUpClicked()
    {

    }
}
