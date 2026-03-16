using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;



public class RemotePlayer : MonoBehaviour
{
    private float speed = 5.0f;
    private float rotation_speed = 2.0f;
    private float jumpHeight = 2.0f;
    private float gravity = -9.8f;
    private Vector3 velocity;

    CharacterController controller;
    Vector3 targetPos;
    Quaternion targetRot;
    float lerpSpeed = 10f;
    Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetPos = transform.position;
        targetRot = transform.rotation;
        animator = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();
    }

    public void SetTargetPos(Vector3 pos)
    {
        targetPos = pos;

        //float distance = Vector3.Distance(transform.position, pos);

    }

    public void OnJumpEvent()
    {
        if(controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetBool("IsGrounded", false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (controller == null) return;

        float distance = Vector3.Distance(transform.position, targetPos);

        if(distance > 5.0f)
        {
            transform.position = targetPos;
            return;
        }

        Vector3 direction = (targetPos - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
        }

        Vector3 nextPos = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);
        Vector3 delta = nextPos - transform.position;

        


        velocity.y += gravity * Time.deltaTime;
        delta.y += velocity.y * Time.deltaTime;

        controller.Move(delta);

        animator.SetFloat("speed", distance > 0.1f ? speed : 0);

        if (controller.isGrounded && animator.GetBool("IsGrounded") == false)
        {
            animator.SetBool("IsGrounded", controller.isGrounded);
        }

        
    }

    //transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * lerpSpeed);

    //transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * lerpSpeed);

}
