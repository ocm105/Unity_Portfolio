using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMoveControl : MonoBehaviour
{
    #region 점프
    [SerializeField] float playerJumpValue = 5f;
    [SerializeField] float playerGravity = 7f;
    private Vector3 jumpPostion;
    public bool IsJump { get; set; }
    #endregion

    #region 이동
    private Vector3 originPosition; // 초기 위치
    private Vector3 movePostion;
    private Vector3 lookPosition;
    private float winX, winZ;   // 윈도우 컨트롤 X,Z 값
    private float mobX, mobZ;   // 모바일 컨트롤 X,Z 값
    private float moveValue = 0f;   // 조이스틱 이동 값
    private float playerSpeed = 0f; // 기본 스피드
    public void SetPlayerSpeed(float speed) { playerSpeed = speed; }
    private Joystick joystick;
    public void SetJoystick(Joystick joystick) { this.joystick = joystick; }
    #endregion

    private Camera mainCam;
    private PlayerInfo playerInfo;
    private CharacterController characterController;

    private void Awake()
    {
        playerInfo = this.GetComponent<PlayerInfo>();
        mainCam = playerInfo._mainCamera;
        characterController = this.GetComponent<CharacterController>();
    }

    private void Init()
    {
        this.transform.position = originPosition + Vector3.up;
        jumpPostion.y = 0f;
        characterController.velocity.Set(0f, 0f, 0f);

        playerInfo._playerAniControl.AnimationChanger(PlayerAniState.Default);
        playerInfo._playerAniControl.SetMoveValue(0f);
    }

    public void Jump()
    {
        if (IsJump == false)
        {
            IsJump = true;
            playerInfo._playerAniControl.AnimationChanger(PlayerAniState.Jump);
            jumpPostion.y = playerJumpValue;
        }
    }

    private void FixedUpdate()
    {
        if (joystick == null)
            return;

        switch (playerInfo._playerAniControl.playerAniState)
        {
            case PlayerAniState.Default:
            case PlayerAniState.Jump:
                if (this.transform.position.y < -5f)
                {
                    Init();
                    return;
                }

                if (Input.GetButtonDown("Jump"))
                {
                    Jump();
                }

                mobX = Mathf.Abs(joystick.Horizontal) >= 0.05 ? joystick.Horizontal : 0;
                mobZ = Mathf.Abs(joystick.Vertical) >= 0.05 ? joystick.Vertical : 0;

                winX = Input.GetAxis("Horizontal");
                winZ = Input.GetAxis("Vertical");


                switch (playerInfo.cinemachineControl.viewType)
                {
                    case CameraViewType.FPSView:
                        movePostion = this.transform.right * mobX + this.transform.forward * mobZ;
#if UNITY_EDITOR_WIN
                        movePostion = this.transform.right * winX + this.transform.forward * winZ;
#endif
                        moveValue = Mathf.Clamp01(Mathf.Abs(movePostion.x) + Mathf.Abs(movePostion.z));

                        characterController.Move(movePostion * playerSpeed * Time.fixedDeltaTime);
                        break;

                    case CameraViewType.QuarterView:
                    case CameraViewType.ShoulderView:
                        movePostion.x = mobX;
                        movePostion.z = mobZ;
#if UNITY_EDITOR_WIN
                        movePostion.x += winX;
                        movePostion.z += winZ;
#endif
                        moveValue = Mathf.Clamp01(Mathf.Abs(movePostion.x) + Mathf.Abs(movePostion.z));

                        if (moveValue > 0)
                        {
                            lookPosition = Quaternion.LookRotation(movePostion).eulerAngles;
                            this.transform.rotation = Quaternion.Euler(Vector3.up * (lookPosition.y + mainCam.transform.eulerAngles.y)).normalized;
                        }
                        CollisionFlags flags = characterController.Move(((this.transform.forward * playerSpeed * moveValue) + jumpPostion) * Time.fixedDeltaTime);

                        if ((flags & CollisionFlags.Below) != 0)
                        {
                            if (IsJump)
                            {
                                jumpPostion.y = 0f;
                                playerInfo._playerAniControl.AnimationChanger(PlayerAniState.Default);
                                IsJump = false;
                            }
                        }
                        else
                        {
                            jumpPostion.y -= playerGravity * Time.fixedDeltaTime;
                        }
                        break;
                }

                playerInfo._playerAniControl.SetMoveValue(moveValue);
                break;
            default:
                break;
        }
    }
}
