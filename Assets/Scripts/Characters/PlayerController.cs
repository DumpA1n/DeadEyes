using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.VisualScripting;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : NetworkBehaviour
{
    private CharacterController m_CharacterController;
    private Camera m_Camera;
    private Animator m_Animator;
    private Transform m_WeaponHolder;
    public float m_BaseMoveSpeed = 5f;            // 移动速度
    public float m_Acceleration = 0f;             // 加速度
    public float m_BaseJumpHeight = 0.7f;         // 跳跃高度
    public float m_BaseRotationSpeed = 2f;        // 旋转速度
    public float m_Gravity = -9.81f;              // 重力
    private float ySpeed = 0f;                    // Y轴速度

    void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_Animator = GetComponentInChildren<Animator>();
        m_Camera = GetComponentInChildren<Camera>();
        m_WeaponHolder = transform.Find("WeaponHolder");
        if (m_Animator != null)
            m_Animator.applyRootMotion = false;
        if (!isLocalPlayer) {
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return;
        
        HandleMovement();
        HandleJump();
        HandleRotation();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        ySpeed += m_Gravity * Time.deltaTime;
        moveDirection.y = ySpeed;

        if (Input.GetKey(KeyCode.LeftShift)) {
            m_Acceleration = 5.0f;
        } else {
            m_Acceleration = 0.0f;
        }

        if (m_Animator != null) {
            if (IsMoving()) {
                m_Animator.SetBool("bWalk", true);
                if (m_Acceleration != 0.0f) {
                    m_Animator.SetBool("bRun", true);
                } else {
                    m_Animator.SetBool("bRun", false);
                }
            } else {
                m_Animator.SetBool("bWalk", false);
            }
        }

        m_CharacterController.Move(moveDirection * (m_BaseMoveSpeed + m_Acceleration) * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (m_CharacterController.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ySpeed = Mathf.Sqrt(m_BaseJumpHeight * -2f * m_Gravity);
            }
        }
    }

    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // 角色左右旋转
        transform.Rotate(0f, mouseX * m_BaseRotationSpeed, 0f);

        // 摄像机上下旋转
        float rotationX = m_Camera.transform.localEulerAngles.x - mouseY * m_BaseRotationSpeed;

        if (rotationX > 180) rotationX -= 360;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        m_Camera.transform.localEulerAngles = new Vector3(rotationX, m_Camera.transform.localEulerAngles.y, 0f);
        if (m_WeaponHolder != null)
            m_WeaponHolder.localEulerAngles = m_Camera.transform.localEulerAngles;
    }

    private bool IsMoving()
    {
        Vector3 velocity = m_CharacterController.velocity;
        return velocity.magnitude > 0.1f;
    }
}

