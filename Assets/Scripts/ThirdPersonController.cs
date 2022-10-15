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

    //移动输入
    Vector2 moveInput;
    //接收操作
    bool isRunning;
    bool isCrouch;
    bool isJumping;
    bool isAiming;
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
        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {

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


}
