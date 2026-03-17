using System.Collections;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Google.Protobuf.Protocol;
using UnityEngine.EventSystems;

enum PlayerActionEnum
{
    IDLE = 0,
    WALK,
    RUN,
    JUMP,
    ATTACK1, ATTACK2, ATTACK3,
}

public class PlayerController : MonoBehaviour
{
    public long sessionId = 0;

    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float rotation_speed = 2.0f;
    [SerializeField] private float jumpHeight = 2.0f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private bool shouldFaceMoveDirection;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector3 velocity;

    private Animator animator;

    private bool attacking;
    private int combo = 0;

    private PlayerActionEnum prevAction;
    private PlayerActionEnum currAction;
    private PlayerActionEnum nextAction;

    private float _lastSendTime = 0f;
    [SerializeField] private float sendInterval = 0.1f; // 0.1초마다 전송 (초당 10회)

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        cameraTransform = GameObject.Find("ThirdPersonCamera").GetComponent<Transform>();

    }

    public void SetCamera()
    {
        cameraTransform = GameObject.Find("ThirdPersonCamera").GetComponent<Transform>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!attacking)
        {
            moveInput = context.ReadValue<Vector2>();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetBool("IsGrounded", false);
            SendJumpPacket();
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //StartCoroutine(Attacking());

        }
    }

    private IEnumerator Attacking()
    {
        combo++;

        yield return new WaitForSeconds(1f);
        combo = 0;
    }

    private void Update()
    {
        if (cameraTransform == null)
        {
            return;
        }

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;
        Vector3 delta = moveDirection * speed;

        //rotation
        if (shouldFaceMoveDirection && moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
        }

        //jump
        velocity.y += gravity * Time.deltaTime;
        delta.y = velocity.y;
        controller.Move(delta * Time.deltaTime);

        HandleNetworkSync();
        UpdateAnimations();
    }

    void HandleNetworkSync()
    {
        if (moveInput.sqrMagnitude > 0 && Time.time - _lastSendTime > sendInterval)
        {
            SendMovePacket();
            _lastSendTime = Time.time;
        }
    }

    void SendMovePacket()
    {
        C2S_Move packet = new C2S_Move();
        packet.PosX = transform.position.x;
        packet.PosY = transform.position.y;
        packet.PosZ = transform.position.z;
        packet.SessionId = DataManager.Instance.MyUser.SessionID;
        NetworkManager.Instance.Send(packet, (ushort)ProtocolID.IdC2SMove);
    }

    void SendJumpPacket()
    {
        C2S_Jump packet = new C2S_Jump();
        packet.SessionId = DataManager.Instance.MyUser.SessionID;
        NetworkManager.Instance.Send(packet, (ushort)ProtocolID.IdC2SJump);
    }

    void UpdateAnimations()
    {
        float horizontalSpeed = new Vector2(moveInput.x, moveInput.y).magnitude * speed;
        animator.SetFloat("speed", horizontalSpeed);
        animator.SetBool("IsGrounded", controller.isGrounded);
        //animator.SetInteger("Attack", combo);
    }
}
