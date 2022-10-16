using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
    #region 变量
    Transform playerTransform;
    Animator Animator;
    Transform cameraTransform;
    CharacterController characterController;


    //姿态变量阈值
    float crouchThreshold = 0f;
    float standThreshold = 1f;
    float midairThreshold = 2f;
    //速度
    float crouchSpeed = 1.5f;
    float walkSpeed = 2.5f;
    float runSpeed = 5.5f;
    //移动输入
    Vector2 moveInput;
    //接收操作
    bool isRunning;
    bool isCrouch;
    bool isJumping;
    bool isAiming;
    //状态机哈希值
    int postureHash;
    int moveSpeedHash;
    int turnSpeedHash;
    //实际移动方向
    Vector3 playerMovement = Vector3.zero;
    #endregion

    //保存姿态
    public enum PlayerPosture
    {
        Crouch,
        Stand,
        Midair
    };
    public PlayerPosture playerPosture = PlayerPosture.Stand;

    //保存运动状态
    public enum LocomotionState
    {
        Idle,
        Walk,
        Run
    };
    public LocomotionState locomotionState = LocomotionState.Idle;

    public enum ArmState
    {
        Normal,
        Aim
    };
    public ArmState armState = ArmState.Normal;

    // Start is called before the first frame update
    void Start()
    {
        //一坨引用
        playerTransform = transform;
        Animator = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;
        characterController = GetComponent<CharacterController>();

        postureHash = Animator.StringToHash("玩家姿态");
        moveSpeedHash = Animator.StringToHash("移动速度");
        turnSpeedHash = Animator.StringToHash("转弯速度");
        //隐藏鼠标
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateInputDirection();
        SwitchPlayerState();
        SetupAnimator();
    }
    #region 输入部分
    public void GetMoveInput(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }
    public void GetRunInput(InputAction.CallbackContext ctx)
    {
        isRunning = ctx.ReadValueAsButton();
    }
    public void GetCrouchInput(InputAction.CallbackContext ctx)
    {
        isCrouch = ctx.ReadValueAsButton();
    }
    public void GetAimInput(InputAction.CallbackContext ctx)
    {
        isAiming = ctx.ReadValueAsButton();
    }
    #endregion

    //切换玩家状态
    void SwitchPlayerState()
    {
        //下蹲
        if (isCrouch)
        {
            playerPosture = PlayerPosture.Crouch;
        }
        else
        {
            playerPosture = PlayerPosture.Stand;
        }
        //待机动画
        if (moveInput.magnitude == 0)
        {
            locomotionState = LocomotionState.Idle;
        }
        //奔跑
        else if (!isRunning)
        {
            locomotionState = LocomotionState.Walk;
        }
        else
        {
            locomotionState = LocomotionState.Run;
        }
        //瞄准
        if (isAiming)
        {
            armState = ArmState.Aim;
        }
        else
        {
            armState = ArmState.Normal;
        }
    }

    //计算方向
    void CalculateInputDirection()
    {
        //获取相机前方在水平平面上的投影并做归一化
        Vector3 canForwardProjection = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
        //用输入的y分量乘以投影的方向，输入的x分量乘以相机右方的方向；再把他俩的和赋给当前变量，就能得到相对相机的输入
        playerMovement = canForwardProjection * moveInput.y + cameraTransform.right * moveInput.x;
        //把向量转换到玩家本地的坐标系
        playerMovement = playerTransform.InverseTransformVector(playerMovement);
    }

    //根据运动状态调整移动速度
    void SetupAnimator()
    {
        if(playerPosture == PlayerPosture.Stand)
        {
            Animator.SetFloat(postureHash, standThreshold, 0.1f, Time.deltaTime);
            switch(locomotionState)
            {
                case LocomotionState.Idle:
                    Animator.SetFloat(moveSpeedHash, 0, 0.1f, Time.deltaTime);
                    break;
                case LocomotionState.Walk:
                    Animator.SetFloat(moveSpeedHash, playerMovement.magnitude * walkSpeed, 0.1f, Time.deltaTime);
                    break;
                case LocomotionState.Run:
                    Animator.SetFloat(moveSpeedHash, playerMovement.magnitude * runSpeed,0.1f,Time.deltaTime);
                    break;

            }
        }
        else if(playerPosture == PlayerPosture.Crouch)
        {
            Animator.SetFloat(postureHash, crouchThreshold, 0.1f, Time.deltaTime);
            switch (locomotionState)
            {
                case LocomotionState.Idle:
                    Animator.SetFloat(moveSpeedHash, 0, 0.1f, Time.deltaTime);
                    break;
                default:
                    Animator.SetFloat(moveSpeedHash, playerMovement.magnitude * crouchSpeed, 0.1f, Time.deltaTime);
                    break;
            }
        }

        if(armState == ArmState.Normal)
        {
            float rad = Mathf.Atan2(playerMovement.x, playerMovement.z);
            Animator.SetFloat(turnSpeedHash, rad, 0.1f, Time.deltaTime);
            playerTransform.Rotate(0, rad * 200 * Time.deltaTime, 0f);
        }
    }

    private void OnAnimatorMove()
    {
        characterController.Move(Animator.deltaPosition);
    }
}
