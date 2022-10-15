using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
    #region 变量
    Transform playerTransform;
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

    // Start is called before the first frame update
    void Start()
    {

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
}
